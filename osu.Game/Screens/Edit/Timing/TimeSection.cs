// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Utils;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Graphics.UserInterface;
using osu.Game.Graphics.UserInterfaceV2;
using osuTK;

namespace osu.Game.Screens.Edit.Timing
{
    internal partial class TimeSection : CompositeDrawable
    {
        private LabelledTextBox timeTextBox = null!;

        private LabelledTextBox offsetTextBox = null!;

        private Container offsetContainer = null!;

        private OsuButton button = null!;

        [Resolved]
        protected EditorBeatmap Beatmap { get; private set; } = null!;

        [Resolved]
        private EditorClock clock { get; set; } = null!;

        [Resolved]
        private IEditorChangeHandler? changeHandler { get; set; }

        [Resolved]
        private ControlPointSelectionManager selectionManager { get; set; } = null!;

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;

            Padding = new MarginPadding(10) { Bottom = 0 };

            InternalChildren = new Drawable[]
            {
                new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Spacing = new Vector2(10),
                    Direction = FillDirection.Vertical,
                    Children = new Drawable[]
                    {
                        timeTextBox = new LabelledTextBox
                        {
                            Label = "Time",
                            SelectAllOnFocus = true,
                        },
                        offsetContainer = new Container
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Children = new Drawable[]
                            {
                                new Container
                                {
                                    AutoSizeAxes = Axes.Y,
                                    RelativeSizeAxes = Axes.X,
                                    Padding = new MarginPadding { Right = 80 },
                                    Child = offsetTextBox = new LabelledTextBox
                                    {
                                        Label = "Offset",
                                        SelectAllOnFocus = true,
                                    },
                                },
                                new RoundedButton
                                {
                                    Text = "Move",
                                    Anchor = Anchor.TopRight,
                                    Origin = Anchor.TopRight,
                                    Width = 80,
                                    Action = () => applyOffsetToSelection()
                                }
                            }
                        },
                        button = new RoundedButton
                        {
                            Text = "Use current time",
                            RelativeSizeAxes = Axes.X,
                            Action = () => changeSelectionTime(clock.CurrentTime)
                        }
                    }
                },
            };

            timeTextBox.OnCommit += (sender, isNew) =>
            {
                if (!isNew)
                    return;

                if (double.TryParse(sender.Text, out double newTime))
                {
                    changeSelectionTime(newTime);
                }
                else
                {
                    selectionChanged();
                }
            };

            offsetTextBox.OnCommit += (sender, isNew) =>
            {
                if (!isNew)
                    return;

                if (!double.TryParse(sender.Text, out double offset))
                    selectionChanged();
            };

            selectionManager.SelectionChanged += _ => Scheduler.AddOnce(selectionChanged);
            selectionChanged();
        }

        private ControlPointSelectionFacade<ControlPoint> selectionFacade = null!;

        private void selectionChanged()
        {
            selectionFacade?.Expire();
            AddInternal(selectionFacade = new ControlPointSelectionFacade<ControlPoint>(selectionManager.Selection));

            if (!selectionManager.AnySelected())
            {
                timeTextBox.Show();
                offsetContainer.Hide();

                timeTextBox.Text = string.Empty;

                // cannot use textBox.Current.Disabled due to https://github.com/ppy/osu-framework/issues/3919
                timeTextBox.ReadOnly = true;
                button.Enabled.Value = false;
                return;
            }

            timeTextBox.ReadOnly = false;

            if (selectionFacade.Time.Indeterminate.Value)
            {
                timeTextBox.Hide();
                offsetContainer.Show();
                offsetTextBox.Text = "0";
            }
            else
            {
                timeTextBox.Show();
                offsetContainer.Hide();

                timeTextBox.Text = $"{selectionFacade.Time.Value:n0}";
            }

            button.Enabled.Value = true;
        }

        private void changeSelectionTime(double time)
        {
            if (!selectionManager.AnySelected() || (!selectionFacade.Time.Indeterminate.Value && time == selectionFacade.Time.Value))
                return;

            updateSelectionTime(_ => time);
        }

        private void applyOffsetToSelection()
        {
            if (!selectionManager.AnySelected())
                return;

            if (double.TryParse(offsetTextBox.Text, out double offset))
            {
                if (Precision.AlmostEquals(offset, 0))
                    return;

                updateSelectionTime(cp => cp.Time + offset);
            }
        }

        private void updateSelectionTime(Func<ControlPoint, double> getTime)
        {
            changeHandler?.BeginChange();

            var controlPoints = selectionManager.Selection.ToList();

            foreach (var controlPoint in controlPoints)
                controlPoint.Group?.Remove(controlPoint);

            foreach (var cp in controlPoints)
                Beatmap.ControlPointInfo.Add(getTime(cp), cp);

            changeHandler?.EndChange();

            selectionManager.SetSelection(controlPoints);
        }
    }
}
