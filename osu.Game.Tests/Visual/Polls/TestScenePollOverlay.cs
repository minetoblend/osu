// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Testing;
using osu.Game.Online.API;
using osu.Game.Online.API.Requests;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Overlays;
using osu.Game.Overlays.Polls;

namespace osu.Game.Tests.Visual.Polls
{
    public partial class TestScenePollOverlay : OsuTestScene
    {
        [Cached]
        private readonly OverlayColourProvider colourProvider = new OverlayColourProvider(OverlayColourScheme.Purple);

        private DummyAPIAccess dummyAPI => (DummyAPIAccess)API;

        private PollOverlay pollOverlay = null!;

        private PollWedge pollWedge => pollOverlay.ChildrenOfType<PollWedge>().First();
        private VoteButton pollButton(int index) => pollOverlay.ChildrenOfType<VoteButton>().ElementAt(index);

        [Test]
        public void TestPollOverlay()
        {
            var getPollLock = new ManualResetEventSlim();
            var voteLock = new ManualResetEventSlim();

            AddStep("setup request handling", () =>
            {
                getPollLock.Reset();
                voteLock.Reset();

                dummyAPI.HandleRequest = request =>
                {
                    switch (request)
                    {
                        case GetActivePollRequest getPollRequest:
                            Task.Run(() =>
                            {
                                getPollLock.Wait(10000);
                                getPollRequest.TriggerSuccess(createPoll());
                            });

                            return true;

                        case PollVoteRequest voteRequest:
                            Task.Run(() =>
                            {
                                voteLock.Wait(10000);
                                voteRequest.TriggerSuccess(createPoll(voteRequest.OptionId));
                            });

                            return true;

                        default:
                            return false;
                    }
                };
            });

            AddStep("add overlay", () => Child = pollOverlay = new PollOverlay
            {
                Anchor = Anchor.BottomLeft,
                Origin = Anchor.BottomLeft,
            });
            AddStep("finish request", () => getPollLock.Set());
            AddStep("click poll wedge", () => pollWedge.TriggerClick());
            AddStep("click vote button", () => pollButton(0).TriggerClick());
            AddStep("finish request", () => voteLock.Set());
        }

        private APIPoll createPoll(long? vote = null)
        {
            var poll = new APIPoll
            {
                Topic = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
                TotalVoteCount = 210,
                Options = new[]
                {
                    new APIPollOption
                    {
                        Id = 1,
                        Name = "Foo",
                        VoteCount = 140,
                    },
                    new APIPollOption
                    {
                        Id = 2,
                        Name = "Bar",
                        VoteCount = 70
                    },
                }
            };

            if (vote != null)
            {
                poll.HasVoted = true;
                poll.TotalVoteCount += 1;

                foreach (var option in poll.Options)
                {
                    if (option.Id == vote)
                    {
                        option.HasVoted = true;
                        option.VoteCount++;
                    }
                }
            }

            return poll;
        }
    }
}
