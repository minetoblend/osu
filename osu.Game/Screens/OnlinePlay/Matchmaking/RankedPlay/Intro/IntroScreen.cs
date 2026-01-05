// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using System.Threading.Tasks;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Database;
using osu.Game.Online.API;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Online.Multiplayer;
using osu.Game.Online.Multiplayer.MatchTypes.RankedPlay;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Intro
{
    public partial class IntroScreen : RankedPlaySubScreen
    {
        public IntroScreen()
        {
            CornerPieceVisibility.Value = Visibility.Hidden;
        }

        [Resolved]
        private UserLookupCache userLookupCache { get; set; } = null!;

        [Resolved]
        private IAPIProvider api { get; set; } = null!;

        protected override void LoadComplete()
        {
            base.LoadComplete();

            loadUsers().FireAndForget();
        }

        private async Task loadUsers()
        {
            var roomState = ((RankedPlayRoomState)Client.Room!.MatchState!);

            int[] userIds = roomState.Users.Keys.ToArray();

            var users = await userLookupCache.GetUsersAsync(userIds).ConfigureAwait(false);

            var player = users.OfType<APIUser>().First(it => it.Id == api.LocalUser.Value.Id);
            var opponent = users.OfType<APIUser>().First(it => it.Id != api.LocalUser.Value.Id);

            int playerRating = roomState.Users[player.Id].Rating;
            int opponentRating = roomState.Users[opponent.Id].Rating;

            Schedule(() => PlayIntroSequence(
                new UserWithRating(player, playerRating),
                new UserWithRating(opponent, opponentRating),
                roomState.StarRating
            ));
        }

        private StarRatingSequence? starRatingAnimation;

        public void PlayIntroSequence(UserWithRating player, UserWithRating opponent, double starRating)
        {
            double delay = 0;

            var vsScreen = new VsSequence(player, opponent);

            starRatingAnimation = new StarRatingSequence();

            AddRangeInternal([vsScreen, starRatingAnimation]);

            vsScreen.Play(ref delay);

            Scheduler.AddDelayed(() => CornerPieceVisibility.Value = Visibility.Visible, delay);

            starRatingAnimation.Play(ref delay, (float)starRating);
        }

        public override void OnExiting(RankedPlaySubScreen? next)
        {
            starRatingAnimation?.PopOut();

            this.Delay(500).FadeOut();
        }
    }
}
