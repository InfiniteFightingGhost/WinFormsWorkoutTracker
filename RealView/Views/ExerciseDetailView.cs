using System;
using System.Drawing;
using System.Windows.Forms;
using WorkoutTracker.Data.Entities;
using WorkoutTracker.Data.DTOs;
using System.Collections.Generic;
using System.Linq;
using LiveChartsCore;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.WinForms;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using LiveChartsCore.SkiaSharpView.Painting.Effects;

namespace WorkoutTracker.RealView.Views
{
    public class ExerciseDetailView : BaseView
    {
        private Exercise _exercise;
        private FlowLayoutPanel _mainLayout;
        private CartesianChart _chart;
        private List<ExerciseProgressDTO> _progressData = new List<ExerciseProgressDTO>();
        private bool _showWeight = true; // Toggle between Weight and Volume
        
        private FlowLayoutPanel _selectionContainer;
        private Label _valueLabel;
        private Label _dateLabel;

        public ExerciseDetailView(Exercise exercise)
        {
            _exercise = exercise;
            InitializeComponent();
        }

        public override async void OnNavigatedTo()
        {
            var user = AppRuntime.Auth.GetCurrentUser();
            if (user != null)
            {
                var data = await AppRuntime.WorkoutSet.GetExerciseProgressAsync(user.Id, _exercise.Id);
                _progressData = data.ToList();
                UpdateChart();
            }
        }

        private void InitializeComponent()
        {
            this.BackColor = UIStyle.Background;

            _mainLayout = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                Padding = new Padding(40),
                AutoScroll = true,
                WrapContents = false
            };

            var backBtn = new Button
            {
                Text = "← BACK",
                Size = new Size(100, 35),
                FlatStyle = FlatStyle.Flat,
                Font = UIStyle.CaptionBold,
                Margin = new Padding(0, 0, 0, 20),
                Cursor = Cursors.Hand
            };
            backBtn.FlatAppearance.BorderSize = 0;
            backBtn.Click += (s, e) => {
                if (AppRuntime.WorkoutState.IsWorkoutActive)
                    AppRuntime.Navigation.NavigateTo<ActiveWorkoutView>();
                else
                    AppRuntime.Navigation.NavigateTo<DashboardView>();
            };

            var title = new Label
            {
                Text = _exercise.Name,
                Font = UIStyle.Header,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 10)
            };

            var muscleGroup = new Label
            {
                Text = $"Target: {_exercise.MainMuscleGroup?.Name ?? "Unknown"}",
                Font = new Font(UIStyle.Body.FontFamily, 14, FontStyle.Italic),
                ForeColor = UIStyle.TextSecondary,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 30)
            };

            // SELECTION INFO CONTAINER
            _selectionContainer = new FlowLayoutPanel
            {
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                Margin = new Padding(0, 0, 0, 10),
                Height = 30
            };

            _valueLabel = new Label
            {
                Text = "Hover over chart ",
                Font = UIStyle.BodySemibold,
                ForeColor = UIStyle.TextSecondary,
                AutoSize = true,
                Margin = new Padding(0)
            };

            _dateLabel = new Label
            {
                Text = "to see details",
                Font = UIStyle.BodySemibold,
                ForeColor = UIStyle.Primary,
                AutoSize = true,
                Margin = new Padding(5, 0, 0, 0)
            };

            _selectionContainer.Controls.Add(_valueLabel);
            _selectionContainer.Controls.Add(_dateLabel);

            // PROGRESS SECTION
            var progressHeader = new Label { Text = "Progress History", Font = UIStyle.SubHeader, AutoSize = true, Margin = new Padding(0, 0, 0, 15) };
            
            var togglePanel = new FlowLayoutPanel { AutoSize = true, FlowDirection = FlowDirection.LeftToRight, Margin = new Padding(0, 0, 0, 10) };
            
            var weightToggle = CreateToggleButton("Weight", true);
            var volumeToggle = CreateToggleButton("Volume", false);

            weightToggle.Click += (s, e) => { 
                _showWeight = true; 
                weightToggle.BackColor = UIStyle.Primary; 
                weightToggle.ForeColor = UIStyle.TextOnPrimary; 
                volumeToggle.BackColor = UIStyle.Surface; 
                volumeToggle.ForeColor = UIStyle.TextPrimary; 
                UpdateChart(); 
            };
            volumeToggle.Click += (s, e) => { 
                _showWeight = false; 
                volumeToggle.BackColor = UIStyle.Primary; 
                volumeToggle.ForeColor = UIStyle.TextOnPrimary; 
                weightToggle.BackColor = UIStyle.Surface; 
                weightToggle.ForeColor = UIStyle.TextPrimary; 
                UpdateChart(); 
            };

            togglePanel.Controls.Add(weightToggle);
            togglePanel.Controls.Add(volumeToggle);

            var chartContainer = new Panel
            {
                Size = new Size(800, 350),
                BackColor = UIStyle.Surface,
                Margin = new Padding(0, 0, 0, 40)
            };
            chartContainer.Paint += (s, e) => DrawCard(e.Graphics, chartContainer.ClientRectangle);

            _chart = new CartesianChart
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                AnimationsSpeed = TimeSpan.FromMilliseconds(500),
                EasingFunction = LiveChartsCore.EasingFunctions.Lineal
            };
            _chart.DataPointerDown += Chart_DataPointerDown;
            chartContainer.Controls.Add(_chart);

            var descTitle = new Label { Text = "Description", Font = UIStyle.BodySemibold, AutoSize = true, Margin = new Padding(0, 0, 0, 5) };
            var desc = new Label
            {
                Text = string.IsNullOrEmpty(_exercise.Description) ? "No description provided." : _exercise.Description,
                Font = UIStyle.Body,
                Width = 800,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 30)
            };

            var instTitle = new Label { Text = "Instructions", Font = UIStyle.BodySemibold, AutoSize = true, Margin = new Padding(0, 0, 0, 5) };
            var inst = new Label
            {
                Text = string.IsNullOrEmpty(_exercise.Instructions) ? "No instructions provided." : _exercise.Instructions,
                Font = UIStyle.Body,
                Width = 800,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 30)
            };

            _mainLayout.Controls.Add(backBtn);
            _mainLayout.Controls.Add(title);
            _mainLayout.Controls.Add(muscleGroup);
            _mainLayout.Controls.Add(progressHeader);
            _mainLayout.Controls.Add(_selectionContainer);
            _mainLayout.Controls.Add(togglePanel);
            _mainLayout.Controls.Add(chartContainer);
            _mainLayout.Controls.Add(descTitle);
            _mainLayout.Controls.Add(desc);
            _mainLayout.Controls.Add(instTitle);
            _mainLayout.Controls.Add(inst);

            this.Controls.Add(_mainLayout);
        }

        private Button CreateToggleButton(string text, bool isActive)
        {
            return new Button
            {
                Text = text,
                Size = new Size(100, 35),
                FlatStyle = FlatStyle.Flat,
                BackColor = isActive ? UIStyle.Primary : UIStyle.Surface,
                ForeColor = isActive ? UIStyle.TextOnPrimary : UIStyle.TextPrimary,
                Font = UIStyle.CaptionBold,
                Cursor = Cursors.Hand
            };
        }

        private async void Chart_DataPointerDown(object chart, IEnumerable<dynamic> points)
        {
            var point = points.FirstOrDefault();
            if (point == null) return;

            // Get the index from the point. LiveCharts2 points have an Index property.
            int index = (int)point.Index;
            
            // We need to be careful with the index if we have multiple series.
            // But here the points are from the merged progress data anyway.
            
            if (index >= 0 && index < _progressData.Count)
            {
                var data = _progressData[index];
                if (data.WorkoutSessionId > 0 && !data.IsPotential)
                {
                    var session = await AppRuntime.WorkoutSession.GetByIdAsync(data.WorkoutSessionId);
                    if (session != null)
                    {
                        AppRuntime.Navigation.NavigateTo<WorkoutDetailView>(session);
                    }
                }
            }
        }

        private void UpdateChart()
        {
            if (_progressData.Count == 0) return;

            var actualData = _progressData.Where(p => !p.IsPotential).ToList();
            var potentialData = _progressData.Where(p => p.IsPotential).ToList();

            var primaryColor = new SKColor(UIStyle.Primary.R, UIStyle.Primary.G, UIStyle.Primary.B);
            var secondaryColor = new SKColor(UIStyle.TextSecondary.R, UIStyle.TextSecondary.G, UIStyle.TextSecondary.B);
            
            var linePaint = new SolidColorPaint(primaryColor, 3);
            var dashedPaint = new SolidColorPaint(primaryColor, 2)
            {
                PathEffect = new DashEffect(new float[] { 10, 5 })
            };

            var seriesList = new List<ISeries>();

            if (actualData.Count > 0)
            {
                var actualValues = actualData.Select((p, i) => new LiveChartsCore.Defaults.ObservablePoint(i, (double)(_showWeight ? p.MaxWeight : p.MaxVolume))).ToArray();
                seriesList.Add(new LineSeries<LiveChartsCore.Defaults.ObservablePoint>
                {
                    Values = actualValues,
                    Fill = null,
                    GeometrySize = 10,
                    Stroke = linePaint,
                    GeometryStroke = linePaint,
                    GeometryFill = new SolidColorPaint(SKColors.White),
                    Name = _showWeight ? "Actual Weight" : "Actual Volume"
                });
            }

            if (potentialData.Count > 0)
            {
                // To connect the potential line, include the last actual point if it exists
                var potentialPoints = new List<LiveChartsCore.Defaults.ObservablePoint>();
                int startIndex = actualData.Count > 0 ? actualData.Count - 1 : 0;
                
                if (actualData.Count > 0)
                {
                    var lastActual = actualData.Last();
                    potentialPoints.Add(new LiveChartsCore.Defaults.ObservablePoint(startIndex, (double)(_showWeight ? lastActual.MaxWeight : lastActual.MaxVolume)));
                }

                for (int i = 0; i < potentialData.Count; i++)
                {
                    var p = potentialData[i];
                    potentialPoints.Add(new LiveChartsCore.Defaults.ObservablePoint(startIndex + i + (actualData.Count > 0 ? 1 : 0), (double)(_showWeight ? p.MaxWeight : p.MaxVolume)));
                }

                seriesList.Add(new LineSeries<LiveChartsCore.Defaults.ObservablePoint>
                {
                    Values = potentialPoints,
                    Fill = null,
                    GeometrySize = 10,
                    Stroke = dashedPaint,
                    GeometryStroke = dashedPaint,
                    GeometryFill = new SolidColorPaint(SKColors.White),
                    Name = _showWeight ? "Potential Weight" : "Potential Volume"
                });
            }

            _chart.Series = seriesList;

            _chart.XAxes = new Axis[]
            {
                new Axis
                {
                    Labels = _progressData.Select(p => p.Date.ToString("MM/dd")).ToArray(),
                    LabelsPaint = new SolidColorPaint(secondaryColor)
                }
            };

            _chart.YAxes = new Axis[]
            {
                new Axis
                {
                    LabelsPaint = new SolidColorPaint(secondaryColor),
                    Labeler = v => $"{v:0.##} kg"
                }
            };
        }
    }
}
