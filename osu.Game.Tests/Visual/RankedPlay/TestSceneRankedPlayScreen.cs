// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Testing;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Online.API;
using osu.Game.Online.API.Requests;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Online.Multiplayer.MatchTypes.RankedPlay;
using osu.Game.Online.Rooms;
using osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay;
using osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Cards;
using osu.Game.Tests.Resources;
using osu.Game.Tests.Visual.Multiplayer;
using osuTK;
using osuTK.Input;
using MatchType = osu.Game.Online.Rooms.MatchType;

namespace osu.Game.Tests.Visual.RankedPlay
{
    public partial class TestSceneRankedPlayScreen : MultiplayerTestScene
    {
        private RankedPlayScreen screen = null!;

        [BackgroundDependencyLoader]
        private void load()
        {
            StepsContainer.Add(new StepColourPicker("gradient outer", Color4Extensions.FromHex("AC6D97"))
            {
                ValueChanged = colour => updateBackground(b => b.GradientOutside = colour),
            });
            StepsContainer.Add(new StepColourPicker("gradient inner", Color4Extensions.FromHex("544483"))
            {
                ValueChanged = colour => updateBackground(b => b.GradientInside = colour),
            });
            StepsContainer.Add(new StepColourPicker("dots", Color4Extensions.FromHex("D56CF6"))
            {
                ValueChanged = colour => updateBackground(b => b.DotsColour = colour),
            });
        }

        private void updateBackground(Action<RankedPlayBackground> updateFn)
        {
            var background = this.ChildrenOfType<RankedPlayBackground>().FirstOrDefault();

            if (background != null)
                updateFn(background);
        }

        public override void SetUpSteps()
        {
            base.SetUpSteps();

            AddStep("join room", () => JoinRoom(CreateDefaultRoom(MatchType.RankedPlay)));
            WaitForJoined();

            AddStep("join other user", () => MultiplayerClient.AddUser(new APIUser { Id = 2 }));

            AddStep("load screen", () => LoadScreen(screen = new RankedPlayScreen(MultiplayerClient.ClientRoom!)));
        }

        [Test]
        public void TestIntroStage()
        {
            AddStep("set round warmup phase", () => MultiplayerClient.RankedPlayChangeStage(RankedPlayStage.RoundWarmup, s => s.StarRating = 6.3f).WaitSafely());
        }

        [Test]
        public void TestDiscardCardsStage()
        {
            AddStep("set discard phase", () => MultiplayerClient.RankedPlayChangeStage(RankedPlayStage.CardDiscard).WaitSafely());

            AddWaitStep("wait", 3);

            for (int i = 0; i < 3; i++)
            {
                int i2 = i;
                AddStep($"click card {i2}", () =>
                {
                    InputManager.MoveMouseTo(this.ChildrenOfType<PlayerCardHand.PlayerHandCard>().ElementAt(i2));
                    InputManager.Click(MouseButton.Left);
                });
            }

            AddWaitStep("wait", 3);

            AddStep("click discard button", () =>
            {
                var button = screen.ChildrenOfType<ShearedButton>().Single(it => it.Text == "Discard");

                InputManager.MoveMouseTo(button);
                InputManager.Click(MouseButton.Left);
            });

            AddWaitStep("wait", 13);
            AddStep("set finish discard phase", () => MultiplayerClient.RankedPlayChangeStage(RankedPlayStage.FinishCardDiscard).WaitSafely());
        }

        [Test]
        public void TestAddRemoveCards()
        {
            AddStep("set discard phase", () => MultiplayerClient.RankedPlayChangeStage(RankedPlayStage.CardDiscard).WaitSafely());

            for (int i = 0; i < 3; i++)
                AddStep("add card", () => MultiplayerClient.RankedPlayAddCards([new RankedPlayCardItem()]).WaitSafely());

            for (int i = 0; i < 3; i++)
                AddStep("remove card", () => MultiplayerClient.RankedPlayRemoveCards(hand => [hand[0]]).WaitSafely());
        }

        [Test]
        public void TestRevealCards()
        {
            var requestHandler = new RankedPlayRequestHandler();

            AddStep("setup api handler", () => ((DummyAPIAccess)API).HandleRequest = request => requestHandler.HandleRequest(request));

            AddStep("set discard phase", () => MultiplayerClient.RankedPlayChangeStage(RankedPlayStage.CardDiscard).WaitSafely());

            var beatmapIdsWithIndex = requestHandler.Beatmaps.Values.Select((beatmap, index) => (beatmap.OnlineID, index));

            foreach (var (beatmapId, index) in beatmapIdsWithIndex)
            {
                AddStep("reveal card", () => MultiplayerClient.RankedPlayRevealCard(hand => hand[index], new MultiplayerPlaylistItem
                {
                    ID = index,
                    BeatmapID = beatmapId
                }).WaitSafely());
            }
        }

        [Test]
        public void TestPlayCardDirect()
        {
            AddStep("set play phase", () => MultiplayerClient.RankedPlayChangeStage(RankedPlayStage.CardPlay, state => state.ActiveUserId = API.LocalUser.Value.OnlineID).WaitSafely());
            AddWaitStep("wait", 3);
            AddStep("play card", () => MultiplayerClient.PlayCard(hand => hand[0]).WaitSafely());
        }

        [Test]
        public void TestDiscardCardsDirect()
        {
            AddStep("set discard phase", () => MultiplayerClient.RankedPlayChangeStage(RankedPlayStage.CardDiscard).WaitSafely());
            AddWaitStep("wait", 3);
            AddStep("discard cards", () => MultiplayerClient.DiscardCards(hand => hand.Take(3)).WaitSafely());
            AddWaitStep("wait", 13);
            AddStep("set finish discard phase", () => MultiplayerClient.RankedPlayChangeStage(RankedPlayStage.FinishCardDiscard).WaitSafely());
        }

        [Test]
        public void TestPlayStage()
        {
            AddStep("set play phase", () => MultiplayerClient.RankedPlayChangeStage(RankedPlayStage.CardPlay, state => state.ActiveUserId = API.LocalUser.Value.OnlineID).WaitSafely());
            AddUntilStep("wait until cards are present", () => this.ChildrenOfType<PlayerCardHand.PlayerHandCard>().Count() == 5);

            for (int i = 0; i < 3; i++)
            {
                int i2 = i;
                AddStep($"click card {i2}", () =>
                {
                    InputManager.MoveMouseTo(this.ChildrenOfType<PlayerCardHand.PlayerHandCard>().ElementAt(i2));
                    InputManager.Click(MouseButton.Left);
                });
            }

            AddWaitStep("wait", 3);

            AddStep("click play button", () =>
            {
                var button = screen.ChildrenOfType<ShearedButton>().Single(it => it.Text == "Play");

                InputManager.MoveMouseTo(button);
                InputManager.Click(MouseButton.Left);
            });
        }

        [Test]
        public void TestOtherPlaysCard()
        {
            AddStep("set play phase", () => MultiplayerClient.RankedPlayChangeStage(RankedPlayStage.CardPlay, state => state.ActiveUserId = 2).WaitSafely());
            AddWaitStep("wait", 5);
            AddStep("play beatmap", () => MultiplayerClient.PlayUserCard(2, hand => hand[0]).WaitSafely());
            AddStep("reveal card", () => MultiplayerClient.RankedPlayRevealUserCard(2, hand => hand[0], new MultiplayerPlaylistItem
            {
                ID = 0,
                BeatmapID = 0
            }).WaitSafely());
        }

        [Test]
        public void TestHealthChange()
        {
            AddStep("change player 1 health", () => MultiplayerClient.RankedPlayChangeUserState(MultiplayerClient.LocalUser!.UserID, state => state.Life = 250_000).WaitSafely());
            AddStep("change player 2 health", () => MultiplayerClient.RankedPlayChangeUserState(2, state => state.Life = 250_000).WaitSafely());
        }

        public class RankedPlayRequestHandler
        {
            public IReadOnlyDictionary<int, APIBeatmap> Beatmaps { get; }

            public RankedPlayRequestHandler()
            {
                using var resourceStream = TestResources.OpenResource("Requests/api-beatmaps-rankedplay.json");
                using var reader = new StreamReader(resourceStream);

                var deserialized = JsonConvert.DeserializeObject<APIBeatmap[]>(reader.ReadToEnd())!;

                Beatmaps = deserialized.ToDictionary(beatmap => beatmap.OnlineID);
            }

            public bool HandleRequest(APIRequest request)
            {
                switch (request)
                {
                    case GetBeatmapsRequest getBeatmapsRequest:
                        getBeatmapsRequest.TriggerSuccess(
                            new GetBeatmapsResponse
                            {
                                Beatmaps = getBeatmapsRequest.BeatmapIds.Select(id => Beatmaps[id]).ToList()
                            }
                        );
                        return true;

                    case GetBeatmapRequest getBeatmapRequest:
                        if (!Beatmaps.TryGetValue(getBeatmapRequest.OnlineID, out var beatmap))
                            return false;

                        getBeatmapRequest.TriggerSuccess(beatmap);
                        return true;

                    default:
                        return false;
                }
            }
        }

        public partial class StepColourPicker : CompositeDrawable
        {
            private const float scale_adjust = 0.5f;

            public Action<Colour4>? ValueChanged { get; set; }

            private readonly Bindable<Colour4> current;

            public StepColourPicker(string description, Colour4 initialColour)
            {
                current = new Bindable<Colour4>(initialColour);

                RelativeSizeAxes = Axes.X;
                AutoSizeAxes = Axes.Y;

                InternalChildren = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = FrameworkColour.GreenDark,
                    },
                    new FillFlowContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Direction = FillDirection.Vertical,
                        Children = new Drawable[]
                        {
                            new OsuSpriteText
                            {
                                Text = description,
                                Padding = new MarginPadding(5),
                                Font = FrameworkFont.Regular.With(size: 14),
                            },
                            new BasicColourPicker
                            {
                                RelativeSizeAxes = Axes.X,
                                Width = 1f / scale_adjust,
                                Scale = new Vector2(scale_adjust),
                                Current = current,
                            }
                        }
                    }
                };
            }

            protected override void LoadComplete()
            {
                base.LoadComplete();

                current.BindValueChanged(v => ValueChanged?.Invoke(v.NewValue), true);
            }
        }
    }
}
