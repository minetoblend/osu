// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Logging;
using osu.Game.Beatmaps;
using osu.Game.Database;
using osu.Game.Graphics.UserInterface;
using osu.Game.Models;
using osu.Game.Online.API;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Online.Multiplayer;
using osu.Game.Online.Rooms;
using osu.Game.Rulesets;
using osu.Game.Scoring;
using osuTK;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay
{
    public partial class ResultsScreen : RankedPlaySubScreen
    {
        [Resolved]
        private IAPIProvider api { get; set; } = null!;

        [Resolved]
        private MultiplayerClient client { get; set; } = null!;

        [Resolved]
        private BeatmapLookupCache beatmapLookupCache { get; set; } = null!;

        [Resolved]
        private ScoreManager scoreManager { get; set; } = null!;

        [Resolved]
        private RulesetStore rulesets { get; set; } = null!;

        [Resolved]
        private IBindable<RulesetInfo> globalRuleset { get; set; } = null!;

        private LoadingSpinner loadingSpinner = null!;
        private ScoreCard scoreCard = null!;

        [BackgroundDependencyLoader]
        private void load()
        {
            CornerPieceVisibility.Value = Visibility.Hidden;

            InternalChildren = new Drawable[]
            {
                loadingSpinner = new LoadingSpinner
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre
                },
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            loadingSpinner.Show();

            queryScores().FireAndForget();
        }

        private async Task queryScores()
        {
            try
            {
                if (client.Room == null)
                    return;

                Task<APIBeatmap?> beatmapTask = beatmapLookupCache.GetBeatmapAsync(client.Room.CurrentPlaylistItem.BeatmapID);
                TaskCompletionSource<List<MultiplayerScore>> scoreTask = new TaskCompletionSource<List<MultiplayerScore>>();

                var request = new IndexPlaylistScoresRequest(client.Room.RoomID, client.Room.Settings.PlaylistItemId);
                request.Success += req => scoreTask.SetResult(req.Scores);
                request.Failure += e => scoreTask.SetException(e);
                api.Queue(request);

                await Task.WhenAll(beatmapTask, scoreTask.Task).ConfigureAwait(false);

                APIBeatmap? apiBeatmap = beatmapTask.GetResultSafely();
                List<MultiplayerScore> apiScores = scoreTask.Task.GetResultSafely();

                if (apiBeatmap == null)
                    return;

                // Reference: PlaylistItemResultsScreen
                setScores(apiScores.Select(s => s.CreateScoreInfo(scoreManager, rulesets, new BeatmapInfo
                {
                    Difficulty = new BeatmapDifficulty(apiBeatmap.Difficulty),
                    Metadata =
                    {
                        Artist = apiBeatmap.Metadata.Artist,
                        Title = apiBeatmap.Metadata.Title,
                        Author = new RealmUser
                        {
                            Username = apiBeatmap.Metadata.Author.Username,
                            OnlineID = apiBeatmap.Metadata.Author.OnlineID,
                        }
                    },
                    DifficultyName = apiBeatmap.DifficultyName,
                    StarRating = apiBeatmap.StarRating,
                    Length = apiBeatmap.Length,
                    BPM = apiBeatmap.BPM
                })).ToArray());
            }
            catch (Exception e)
            {
                Logger.Error(e, "Failed to load scores for playlist item.");
                throw;
            }
            finally
            {
                Scheduler.Add(() => loadingSpinner.Hide());
            }
        }

        private void setScores(ScoreInfo[] scores) => Scheduler.Add(() =>
        {
            ScoreInfo localUserScore = scores.SingleOrDefault(s => s.UserID == api.LocalUser.Value.OnlineID) ?? new ScoreInfo
            {
                Rank = ScoreRank.F,
                Ruleset = globalRuleset.Value
            };

            ScoreInfo otherUserScore = scores.SingleOrDefault(s => s.UserID != api.LocalUser.Value.OnlineID) ?? new ScoreInfo
            {
                Rank = ScoreRank.F,
                Ruleset = globalRuleset.Value
            };

            AddInternal(scoreCard = new ScoreCard(localUserScore, otherUserScore)
            {
                Size = new Vector2(950, 600),
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                Y = 50,
            });

            scoreCard.Play();
        });

        public override void OnExiting(RankedPlaySubScreen? next)
        {
            scoreCard.Hide();

            this.Delay(400).FadeOut(200);
        }
    }
}
