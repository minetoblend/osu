// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Online.Rooms;

namespace osu.Game.Online.Matchmaking
{
    public class MatchmakingSelectionCarousel : CompositeDrawable
    {
        /// <summary>
        /// Number of items visible on either side of the current item in the carousel at any one time.
        /// </summary>
        private const int visible_extent = 3;

        /// <summary>
        /// Number of cycles of the full list of items before the final item should be presented.
        /// </summary>
        private const int cycles = 5;

        /// <summary>
        /// How long before the final item should be presented.
        /// </summary>
        private const double duration = 5000;

        private readonly MultiplayerPlaylistItem[] items;
        private readonly int finalPosition;

        private readonly Bindable<float> currentPosition = new Bindable<float>();
        private Container<MatchmakingBeatmapPanel> panels = null!;

        public MatchmakingSelectionCarousel(MultiplayerPlaylistItem[] items, MultiplayerPlaylistItem finalItem)
        {
            this.items = items;
            finalPosition = cycles * items.Length + Array.FindIndex(items, i => i.Equals(finalItem));
        }

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

            this.TransformBindableTo(currentPosition, finalPosition, duration, Easing.OutQuint);
        }

        private void updatePosition(ValueChangedEvent<float> pos)
        {
            panels.Clear();

            float itemsPos = pos.NewValue % items.Length;
            int firstVisibleIndex = (int)itemsPos - visible_extent;
            int lastVisibleIndex = (int)itemsPos + visible_extent;

            for (int i = firstVisibleIndex; i <= lastVisibleIndex; i++)
            {
                int itemIndex;

                if (i < 0)
                    itemIndex = i + items.Length;
                else if (i >= items.Length)
                    itemIndex = i - items.Length;
                else
                    itemIndex = i;

                panels.Add(new MatchmakingBeatmapPanel(items[itemIndex])
                {
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    AllowSelection = false
                });
            }
        }
    }
}
