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

        [Resolved]
        private EditorClock editorClock { get; set; } = null!;

        [Resolved]
        private Playfield playfield { get; set; } = null!;

        [Resolved]
        private IEditorChangeHandler? changeHandler { get; set; }

        private InputManager inputManager = null!;

        protected HitObjectPlacementTool(TObject hitObject)
        {
            HitObject = hitObject;
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            inputManager = GetContainingInputManager()!;
        }

        protected override void Update()
        {
            base.Update();

            updateHitObject();
        }

        private void updateHitObject()
        {
            UpdateTimeAndPosition(playfield.ScreenSpaceToGamefield(inputManager.CurrentState.Mouse.Position), editorClock.CurrentTime);

            if (State == PlacementState.Active)
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

            updateHitObject();
            editorBeatmap.Add(HitObject);
        }

        protected void EndPlacement(bool commit)
        {
            if (State == PlacementState.Finished)
                return;

            if (State == PlacementState.Idle)
                BeginPlacement();

            State = PlacementState.Finished;

            OnEndPlacement(commit);

            if (commit)
                changeHandler?.SaveState();

            RecreateTool();
        }

        protected virtual void OnEndPlacement(bool didCommit)
        {
        }

        public override void OnExit()
        {
            base.OnExit();

            switch (State)
            {
                case PlacementState.Active:
                    editorBeatmap.Remove(HitObject);
                    break;
            }
        }

        public enum PlacementState
        {
            Idle,
            Active,
            Finished,
        }
    }
}
