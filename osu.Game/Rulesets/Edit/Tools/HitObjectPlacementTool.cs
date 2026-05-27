// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Input;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.UI;
using osu.Game.Screens.Edit;
using osuTK;

namespace osu.Game.Rulesets.Edit.Tools
{
    public class HitObjectPlacementTool<TObject> : ComposeTool
        where TObject : HitObject
    {
        protected PlacementState State { get; private set; }

        protected readonly TObject HitObject;

        [Resolved]
        private EditorBeatmap editorBeatmap { get; set; } = null!;

        protected EditorBeatmap EditorBeatmap => editorBeatmap;

        [Resolved]
        private EditorClock editorClock { get; set; } = null!;

        protected EditorClock EditorClock => editorClock;

        [Resolved]
        private Playfield playfield { get; set; } = null!;

        [Resolved]
        private IEditorChangeHandler? changeHandler { get; set; }

        private InputManager inputManager = null!;

        protected virtual bool AddToBeatmapImmediately => false;

        private bool addedToBeatmap;

        protected HitObjectPlacementTool(TObject hitObject)
        {
            HitObject = hitObject;
        }

        private int changeCount;

        protected override void LoadComplete()
        {
            base.LoadComplete();

            if (changeHandler != null)
                changeHandler.OnStateChange += onStateChange;

            inputManager = GetContainingInputManager()!;

            changeHandler?.BeginChange();

            if (AddToBeatmapImmediately)
            {
                updateHitObject();
                editorBeatmap.Add(HitObject);
                addedToBeatmap = true;
            }
        }

        private void onStateChange() => changeCount++;

        protected override void Update()
        {
            base.Update();

            updateHitObject();
        }

        private void updateHitObject()
        {
            UpdateTimeAndPosition(playfield.ScreenSpaceToGamefield(inputManager.CurrentState.Mouse.Position), editorClock.CurrentTime);

            if (addedToBeatmap && State != PlacementState.Finished)
            {
                editorBeatmap.Update(HitObject);
            }
        }

        protected virtual void UpdateTimeAndPosition(Vector2 position, double time)
        {
        }

        protected void BeginPlacement()
        {
            if (State != PlacementState.Idle)
                return;

            State = PlacementState.Active;

            if (!addedToBeatmap)
            {
                updateHitObject();
                editorBeatmap.Add(HitObject);
                addedToBeatmap = true;
            }

            OnBeginPlacement();
        }

        protected void EndPlacement(bool commit)
        {
            if (State == PlacementState.Finished)
                return;

            if (State == PlacementState.Idle)
                BeginPlacement();

            State = PlacementState.Finished;

            OnEndPlacement(commit);

            changeHandler?.EndChange();

            if (!commit)
                changeHandler?.RestoreState(-changeCount);

            RecreateTool();
        }

        protected virtual void OnBeginPlacement()
        {
        }

        protected virtual void OnEndPlacement(bool didCommit)
        {
        }

        public override void OnExit()
        {
            base.OnExit();

            if (State != PlacementState.Finished && addedToBeatmap)
            {
                editorBeatmap.Remove(HitObject);

                changeHandler?.EndChange();
                changeHandler?.RestoreState(-changeCount);
            }
        }

        protected override void Dispose(bool isDisposing)
        {
            if (changeHandler != null)
                changeHandler.OnStateChange -= onStateChange;

            base.Dispose(isDisposing);
        }

        public enum PlacementState
        {
            Idle,
            Active,
            Finished,
        }
    }
}
