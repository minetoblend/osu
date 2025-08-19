// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Overlays;
using osu.Game.Screens.OnlinePlay.Matchmaking;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Tests.Visual.Matchmaking
{
    public partial class TestSceneBeatmapSelectionPanel : OsuTestScene
    {
        [Cached]
        private readonly OverlayColourProvider colourProvider = new OverlayColourProvider(OverlayColourScheme.Purple);

        [Test]
        public void TestBeatmapPanel()
        {
            var beatmap = CreateAPIBeatmap();

            BeatmapSelectionPanel panel = null!;

            AddStep("add panel", () => Child = panel = new BeatmapSelectionPanel(300, 70)
            {
                Scale = new Vector2(2),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Child = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Masking = true,
                    CornerRadius = 6,
                    Child = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Color4.LightSlateGray,
                    }
                },
                Margin = new MarginPadding(20),
            });

            AddStep("add maarvin", () => panel.AddUser(new APIUser
            {
                Id = 6411631,
                Username = "Maarvin",
            }));
            AddStep("add peppy", () => panel.AddUser(new APIUser
            {
                Id = 2,
                Username = "peppy",
            }));
            AddStep("add smogipoo", () => panel.AddUser(new APIUser
            {
                Id = 1040328,
                Username = "smoogipoo",
            }));
            AddStep("remove smogipoo", () => panel.RemoveUser(new APIUser { Id = 1040328 }));
            AddStep("remove peppy", () => panel.RemoveUser(new APIUser { Id = 2 }));
            AddStep("remove maarvin", () => panel.RemoveUser(new APIUser { Id = 6411631 }));
        }

        [Test]
        public void TestPanelGrid()
        {
            AddStep("add panels", () =>
            {
                BeatmapSelectionPanel? activePanel = null;
                var user = new APIUser
                {
                    Id = 6411631,
                    Username = "Maarvin",
                };
                var grid = new FillFlowContainer
                {
                    Width = 700,
                    AutoSizeAxes = Axes.Y,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Spacing = new Vector2(20)
                };
                Child = grid;

                for (int i = 0; i < 10; i++)
                {
                    var panel = new BeatmapSelectionPanel(300, 70)
                    {
                        Child = new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Masking = true,
                            CornerRadius = 6,
                            Child = new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Colour = Color4.LightSlateGray,
                            }
                        },
                    };

                    panel.Clicked = () =>
                    {
                        if (activePanel == panel)
                            return;

                        activePanel?.RemoveUser(user);
                        panel.AddUser(user);
                        activePanel = panel;
                    };

                    grid.Add(panel);
                }
            });
        }
    }
}
