// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Online.Rooms;
using osuTK;

namespace osu.Game.Online.Matchmaking
{
    public class MatchmakingSelectionCarousel : CompositeDrawable
    {
        /// <summary>
        /// Number of items visible on either side of the current item in the carousel at any one time.
        /// </summary>
        private const int visible_extent = 8;

        /// <summary>
        /// Number of cycles of the full list of items before the final item should be presented.
        /// </summary>
        private const int cycles = 5;

        private readonly Bindable<float> currentPosition = new Bindable<float>();
        private Container<MatchmakingBeatmapPanel> panels = null!;
        private MultiplayerPlaylistItem[] items = [];

        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChild = panels = new FillFlowContainer<MatchmakingBeatmapPanel>
            {
                RelativeSizeAxes = Axes.Both,
                Direction = FillDirection.Vertical
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            currentPosition.BindValueChanged(updatePosition, true);
        }

        public void BeginScroll(MultiplayerPlaylistItem[] candidateItems, MultiplayerPlaylistItem item, double duration)
        {
            ClearTransforms();

            items = candidateItems;
            currentPosition.Value = 0;
            currentPosition.TriggerChange();

            if (candidateItems.Length == 0)
                return;

            int finalPosition = cycles * candidateItems.Length + Array.FindIndex(candidateItems, i => i.Equals(item));
            this.TransformBindableTo(currentPosition, finalPosition, duration, Easing.OutQuint);
        }

        private void updatePosition(ValueChangedEvent<float> pos)
        {
            panels.Clear();

            if (items.Length == 0)
                return;

            float centrePos = pos.NewValue % items.Length;
            int firstVisibleIndex = (int)centrePos - visible_extent;
            int lastVisibleIndex = (int)centrePos + visible_extent;

            for (int i = firstVisibleIndex; i <= lastVisibleIndex; i++)
            {
                int itemIndex = i;

                while (itemIndex < 0)
                    itemIndex += items.Length;

                while (itemIndex >= items.Length)
                    itemIndex -= items.Length;

                float distFromCentre = (i - centrePos) / visible_extent;

                panels.Add(new MatchmakingBeatmapPanel(items[itemIndex])
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    AllowSelection = false,
                    Scale = new Vector2(Math.Clamp(1f - Math.Abs(distFromCentre), 0f, 1))
                });
            }
        }
    }
}
