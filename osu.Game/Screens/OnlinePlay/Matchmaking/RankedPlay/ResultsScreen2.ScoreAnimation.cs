// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Components;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay
{
    public partial class ResultsScreen2
    {
        private partial class ScoreAnimation(int playerScore, int opponentScore, float multiplier) : CompositeDrawable
        {
            private OsuSpriteText playerScoreText = null!;
            private OsuSpriteText opponentScoreText = null!;
            private OsuSpriteText damageScoreText = null!;
            private OsuSpriteText multiplierText = null!;

            private int damage => Math.Abs(playerScore - opponentScore);
            private int damageWithMultiplier => (int)(damage * multiplier);

            [BackgroundDependencyLoader]
            private void load()
            {
                Width = 500;
                Height = 100;

                InternalChildren =
                [
                    playerScoreText = new OsuSpriteText
                    {
                        Text = FormattableString.Invariant($"{playerScore:N0}"),
                        Origin = Anchor.Centre,
                        Anchor = Anchor.Centre,
                        X = -150,
                        Font = OsuFont.GetFont(size: 48, weight: FontWeight.Bold, typeface: Typeface.TorusAlternate),
                        Colour = ColourInfo.GradientVertical(Color4.White, Color4Extensions.FromHex("C5D8D2"))
                    },
                    opponentScoreText = new OsuSpriteText
                    {
                        Text = FormattableString.Invariant($"{opponentScore:N0}"),
                        X = 150,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Font = OsuFont.GetFont(size: 48, weight: FontWeight.Bold, typeface: Typeface.TorusAlternate),
                        Colour = ColourInfo.GradientVertical(Color4.White, Color4Extensions.FromHex("C5D8D2"))
                    },
                    damageScoreText = new OsuSpriteText
                    {
                        Text = FormattableString.Invariant($"{damage:N0}"),
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Font = OsuFont.GetFont(size: 48, weight: FontWeight.Bold, typeface: Typeface.TorusAlternate),
                        Colour = ColourInfo.GradientVertical(Color4.White, Color4Extensions.FromHex("C5D8D2")),
                        Alpha = 0,
                    },
                    multiplierText = new OsuSpriteText
                    {
                        Text = FormattableString.Invariant($"{multiplier:F}x"),
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Font = OsuFont.GetFont(size: 32, weight: FontWeight.Bold, typeface: Typeface.TorusAlternate),
                        Colour = ColourInfo.GradientVertical(Color4.White, Color4Extensions.FromHex("C5D8D2")),
                        Alpha = 0,
                    }
                ];
            }

            [Resolved]
            private RankedPlayScreen rankedPlayScreen { get; set; } = null!;

            public void Play()
            {
                RankedPlayUserDisplay loosingUserDisplay;
                OsuSpriteText winningScoreText;
                OsuSpriteText loosingScoreText;

                if (playerScore > opponentScore)
                {
                    loosingUserDisplay = rankedPlayScreen.OpponentUserDisplay;
                    winningScoreText = playerScoreText;
                    loosingScoreText = opponentScoreText;
                }
                else
                {
                    loosingUserDisplay = rankedPlayScreen.PlayerUserDisplay;
                    winningScoreText = opponentScoreText;
                    loosingScoreText = playerScoreText;
                }

                damageScoreText.Position = loosingScoreText.Position;
                multiplierText.Position = damageScoreText.Position;

                double delay = 300;

                using (BeginDelayedSequence(delay))
                {
                    var originalPosition = winningScoreText.Position;

                    winningScoreText.MoveTo(loosingScoreText.Position, 700, Easing.InQuint)
                                    .Then()
                                    .MoveTo(originalPosition, 1400, Easing.OutPow10);
                }

                delay += 700;

                using (BeginDelayedSequence(delay))
                {
                    loosingScoreText.FadeOut();

                    damageScoreText.FadeIn()
                                   .ScaleTo(0.5f)
                                   .ScaleTo(1f, 600, Easing.OutElasticHalf);
                }

                delay += 1000;

                if (multiplier > 1)
                {
                    using (BeginDelayedSequence(delay))
                    {
                        const double anticipation_duration = 100;

                        damageScoreText
                            .ScaleTo(0.8f, anticipation_duration, Easing.Out)
                            .Then()
                            .Schedule(() => damageScoreText.Text = FormattableString.Invariant($"{damageWithMultiplier:N0}"))
                            .ScaleTo(1f, 600, Easing.OutElasticHalf);

                        multiplierText
                            .Delay(anticipation_duration)
                            .FadeIn(100)
                            .Then()
                            .FadeOut(500);

                        multiplierText
                            .Delay(anticipation_duration)
                            .MoveToOffset(damageScoreText.DrawSize * new Vector2(0.5f, -0.3f))
                            .RotateTo(30)
                            .ScaleTo(0.5f)
                            .ScaleTo(1, 400, Easing.OutExpo);
                    }

                    delay += 1000;
                }

                using (BeginDelayedSequence(delay))
                {
                    Schedule(() =>
                    {
                        var position = ToLocalSpace(loosingUserDisplay.HealthDisplay.ScreenSpaceDrawQuad.Centre);

                        damageScoreText.MoveTo(position - damageScoreText.AnchorPosition, 700, Easing.InQuart)
                                       .Then()
                                       .FadeOut();

                        loosingScoreText.Delay(500)
                                        .FadeIn(300);
                    });
                }

                delay += 700;

                using (BeginDelayedSequence(delay))
                {
                    Schedule(() => loosingUserDisplay.TakeDamage(damageWithMultiplier));
                }
            }
        }
    }
}
