// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Sample;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osu.Game.Online.API.Requests.Responses;
using osuTK;

namespace osu.Game.Overlays.Polls
{
    [Cached]
    public partial class PollOverlay : VisibilityContainer
    {
        [Cached]
        private readonly OverlayColourProvider colourProvider = new OverlayColourProvider(OverlayColourScheme.Purple);

        [Cached]
        private readonly PollManager pollManager;

        private readonly BindableBool expanded = new BindableBool();

        private readonly FillFlowContainer content;
        private readonly PollWedge pollWedge;
        private readonly FillFlowContainer<VoteButton> buttonsFlow;
        private readonly Dictionary<long, VoteButton> buttons = new Dictionary<long, VoteButton>();

        private ISample? popInSample;
        private ISample? popOutSample;

        public PollOverlay()
        {
            RelativeSizeAxes = Axes.Both;

            AddRangeInternal(new Drawable[]
            {
                pollManager = new PollManager(),
                content = new FillFlowContainer
                {
                    AutoSizeAxes = Axes.Both,
                    Direction = FillDirection.Vertical,
                    Anchor = Anchor.BottomLeft,
                    Origin = Anchor.BottomLeft,
                    Padding = new MarginPadding(20),
                    Children = new Drawable[]
                    {
                        new Container
                        {
                            AutoSizeAxes = Axes.Both,
                            Child = pollWedge = new PollWedge
                            {
                                Expanded = { BindTarget = expanded },
                                Poll = { BindTarget = pollManager.ActivePoll },
                                Anchor = Anchor.BottomLeft,
                                Origin = Anchor.BottomLeft,
                                Depth = -1,
                            },
                        },
                        new Container
                        {
                            AutoSizeAxes = Axes.Both,
                            AutoSizeDuration = 300,
                            AutoSizeEasing = Easing.OutExpo,
                            Child = buttonsFlow = new FillFlowContainer<VoteButton>
                            {
                                Margin = new MarginPadding { Top = 10 },
                                Shear = OsuGame.SHEAR,
                                AutoSizeAxes = Axes.Both,
                                Direction = FillDirection.Vertical,
                                Spacing = new Vector2(0, 5),
                                Anchor = Anchor.BottomLeft,
                                Origin = Anchor.BottomLeft,
                            },
                        }
                    }
                }
            });
        }

        [BackgroundDependencyLoader]
        private void load(AudioManager audio)
        {
            popInSample = audio.Samples.Get("UI/overlay-pop-in");
            popOutSample = audio.Samples.Get("UI/overlay-pop-out");
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            pollManager.ActivePoll.BindValueChanged(e =>
            {
                State.Value = e.NewValue != null ? Visibility.Visible : Visibility.Hidden;

                if (e.NewValue is APIPoll poll)
                    updateButtons(poll);
            }, true);

            expanded.BindValueChanged(e => expandedChanged(e.NewValue));
            expandedChanged(expanded.Value, false);

            FinishTransforms(true);
        }

        private void expandedChanged(bool expanded, bool playSample = true)
        {
            if (expanded)
            {
                if (playSample)
                    popInSample?.Play();

                buttonsFlow.BypassAutoSizeAxes = Axes.None;
                buttonsFlow.TransformTo("Spacing", new Vector2(0, 5), 300, Easing.OutQuart)
                           .FadeInFromZero(200, Easing.Out)
                           .MoveToX(0, 300, Easing.OutExpo);
            }
            else
            {
                if (playSample)
                    popOutSample?.Play();

                buttonsFlow.BypassAutoSizeAxes = Axes.Both;
                buttonsFlow.TransformTo("Spacing", new Vector2(0, -50), 300, Easing.OutQuart)
                           .FadeOut(100, Easing.Out)
                           .MoveToX(-50, 300, Easing.OutExpo);
            }
        }

        protected override void UpdateAfterChildren()
        {
            base.UpdateAfterChildren();

            pollWedge.Margin = new MarginPadding { Left = buttonsFlow.LayoutSize.Y * OsuGame.SHEAR.X };
        }

        private void updateButtons(APIPoll poll)
        {
            var toRemove = buttons.Keys.ToHashSet();

            foreach (var option in poll.Options)
            {
                toRemove.Remove(option.Id);

                if (buttons.TryGetValue(option.Id, out var button))
                {
                    button.Model = (poll, option);
                }
                else
                {
                    buttonsFlow.Add(buttons[option.Id] = new VoteButton
                    {
                        Model = (poll, option),
                        // the parent container is already sheared
                        Shear = Vector2.Zero
                    });
                }
            }

            foreach (long id in toRemove)
            {
                if (buttons.Remove(id, out var button))
                    button.Expire();
            }
        }

        protected override void PopIn()
        {
            content.FadeIn(300, Easing.In);
        }

        protected override void PopOut()
        {
            content.FadeOut(300, Easing.Out);
            pollWedge.Expanded.Value = false;
        }

        protected override bool OnClick(ClickEvent e)
        {
            if (pollManager.HasActiveRequest)
                return false;

            pollWedge.Expanded.Value = false;

            return false;
        }
    }
}
