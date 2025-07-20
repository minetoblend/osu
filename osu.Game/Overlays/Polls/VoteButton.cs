// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Sample;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Graphics;
using osu.Game.Graphics.Backgrounds;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Online.API.Requests.Responses;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Overlays.Polls
{
    public partial class VoteButton : ShearedButton
    {
        private readonly Bindable<(APIPoll, APIPollOption)> model = new Bindable<(APIPoll, APIPollOption)>();

        public required (APIPoll poll, APIPollOption option) Model
        {
            get => model.Value;
            set => model.Value = value;
        }

        private readonly ProgressBar progressBar;
        private readonly OsuSpriteText text;
        private readonly OsuSpriteText percentageText;
        private readonly LoadingSpinner loadingSpinner;

        private Container outline = null!;

        public VoteButton()
            : base(300)
        {
            ButtonContent.AutoSizeAxes = Axes.None;
            ButtonContent.RelativeSizeAxes = Axes.Both;

            ButtonContent.Children = new Drawable[]
            {
                progressBar = new ProgressBar(),
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Padding = new MarginPadding { Horizontal = 20 },
                    Children = new Drawable[]
                    {
                        text = new OsuSpriteText
                        {
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Font = OsuFont.Style.Body.With(weight: FontWeight.SemiBold),
                        },
                        percentageText = new OsuSpriteText
                        {
                            Anchor = Anchor.CentreRight,
                            Origin = Anchor.CentreRight,
                            Font = OsuFont.Style.Body.With(weight: FontWeight.SemiBold),
                        },
                        loadingSpinner = new LoadingSpinner
                        {
                            Anchor = Anchor.CentreRight,
                            Origin = Anchor.CentreRight,
                            Size = new Vector2(18),
                            State = { Value = Visibility.Hidden }
                        }
                    }
                }
            };
        }

        [Resolved]
        private OverlayColourProvider colourProvider { get; set; } = null!;

        [Resolved]
        private PollManager pollManager { get; set; } = null!;

        private ISample? voteSample;

        [BackgroundDependencyLoader]
        private void load(AudioManager audio)
        {
            voteSample = audio.Samples.Get("UI/notification-done");

            Action = vote;

            Add(outline = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Masking = true,
                CornerRadius = CORNER_RADIUS,
                Child = new Box { Alpha = 0, RelativeSizeAxes = Axes.Both, AlwaysPresent = true },
                Colour = colourProvider.Highlight1.Lighten(1),
                BorderThickness = 3,
                BorderColour = Color4.White,
                Alpha = 0,
            });

            percentageText.Colour = colourProvider.Light1;
        }

        private void vote()
        {
            loadingSpinner.Show();
            percentageText.Hide();

            pollManager.Vote(Model.poll, Model.option).ContinueWith(_ => Schedule(() =>
            {
                loadingSpinner.Hide();
                percentageText.Show();
            }));
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            model.BindValueChanged(e =>
            {
                var (poll, option) = e.NewValue;

                text.Text = option.Name;
                progressBar.UpdateFrom(poll, option);
                percentageText.Text = poll.HasVoted
                    ? $"{Math.Round((float)option.VoteCount / poll.TotalVoteCount * 100)}%"
                    : string.Empty;

                outline.FadeTo(option.HasVoted ? 1 : 0);
            }, true);
        }

        public override bool HandlePositionalInput => !pollManager.HasActiveRequest;

        private partial class ProgressBar : CompositeDrawable
        {
            private Box background = null!;
            private TrianglesV2 triangles = null!;

            public ProgressBar()
            {
                RelativeSizeAxes = Axes.Both;
                Shear = OsuGame.SHEAR;
                Alpha = 0;
            }

            [Resolved]
            private OverlayColourProvider colourProvider { get; set; } = null!;

            [BackgroundDependencyLoader]
            private void load()
            {
                InternalChildren = new Drawable[]
                {
                    background = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        EdgeSmoothness = new Vector2(1, 0)
                    },
                    triangles = new TrianglesV2
                    {
                        RelativeSizeAxes = Axes.Both,
                        Alpha = 0,
                        ScaleAdjust = 0.5f,
                        Thickness = 0.04f,
                    }
                };
            }

            public void UpdateFrom(APIPoll poll, APIPollOption option)
            {
                FinishTransforms(true);

                if (poll.HasVoted)
                {
                    this.FadeIn();
                    this.ResizeWidthTo((float)option.VoteCount / poll.TotalVoteCount, 400, Easing.OutExpo);
                }
                else
                {
                    this
                        .ResizeWidthTo(0, 300, Easing.Out)
                        .Then()
                        .FadeOut();
                }

                triangles.FadeTo(option.HasVoted ? 1 : 0);
                background.FadeColour(option.HasVoted ? ColourInfo.GradientHorizontal(colourProvider.Highlight1, colourProvider.Highlight1.Lighten(0.35f)) : colourProvider.Colour3);
            }
        }
    }
}
