// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.Screens.Idle
{
    public partial class ProxiedParticipantList : CompositeDrawable
    {
        [Resolved]
        private ParticipantList? participantList { get; set; }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            if (participantList == null)
            {
                InternalChild = new ParticipantList
                {
                    RelativeSizeAxes = Axes.Both
                };
            }
            else
            {
                participantList.SetTarget(this);
            }
        }

        protected override void Dispose(bool isDisposing)
        {
            participantList?.SetTarget(null);

            base.Dispose(isDisposing);
        }
    }
}
