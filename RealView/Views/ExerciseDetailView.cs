using System;
using System.Drawing;
using System.Windows.Forms;
using Data.Entities;
using Data.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace RealView.Views
{
    public class ExerciseDetailView : BaseView
    {
        private Exercise _exercise;
        private FlowLayoutPanel _mainLayout;
        private Panel _chartPanel;
        private List<ExerciseProgressDTO> _progressData = new List<ExerciseProgressDTO>();
        private bool _showWeight = true; // Toggle between Weight and Volume
        
        private int _hoverIndex = -1;
        private int _selectedIndex = -1;
        private List<PointF> _currentPoints = new List<PointF>();
        
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
                _chartPanel.Invalidate(); // Redraw chart
            }
        }

        private void InitializeComponent()
        {
            this.BackColor = Color.FromArgb(245, 247, 251);

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
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
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
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 10)
            };

            var muscleGroup = new Label
            {
                Text = $"Target: {_exercise.MainMuscleGroup?.Name ?? "Unknown"}",
                Font = new Font("Segoe UI", 14, FontStyle.Italic),
                ForeColor = Color.DimGray,
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
                Text = "Select a point ",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.Gray,
                AutoSize = true,
                Margin = new Padding(0)
            };

            _dateLabel = new Label
            {
                Text = "to see details",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 120, 215),
                AutoSize = true,
                Margin = new Padding(5, 0, 0, 0)
            };

            _selectionContainer.Controls.Add(_valueLabel);
            _selectionContainer.Controls.Add(_dateLabel);

            // PROGRESS SECTION
            var progressHeader = new Label { Text = "Progress History", Font = new Font("Segoe UI", 16, FontStyle.Bold), AutoSize = true, Margin = new Padding(0, 0, 0, 15) };
            
            var togglePanel = new FlowLayoutPanel { AutoSize = true, FlowDirection = FlowDirection.LeftToRight, Margin = new Padding(0, 0, 0, 10) };
            
            var weightToggle = CreateToggleButton("Weight", true);
            var volumeToggle = CreateToggleButton("Volume", false);

            weightToggle.Click += (s, e) => { 
                _showWeight = true; 
                ResetSelection();
                weightToggle.BackColor = Color.FromArgb(0, 120, 215); 
                weightToggle.ForeColor = Color.White; 
                volumeToggle.BackColor = Color.White; 
                volumeToggle.ForeColor = Color.Black; 
                _chartPanel.Invalidate(); 
            };
            volumeToggle.Click += (s, e) => { 
                _showWeight = false; 
                ResetSelection();
                volumeToggle.BackColor = Color.FromArgb(0, 120, 215); 
                volumeToggle.ForeColor = Color.White; 
                weightToggle.BackColor = Color.White; 
                weightToggle.ForeColor = Color.Black; 
                _chartPanel.Invalidate(); 
            };

            togglePanel.Controls.Add(weightToggle);
            togglePanel.Controls.Add(volumeToggle);

            _chartPanel = new Panel
            {
                Size = new Size(800, 350),
                BackColor = Color.White,
                Margin = new Padding(0, 0, 0, 40)
            };
            _chartPanel.Paint += ChartPanel_Paint;
            _chartPanel.MouseMove += ChartPanel_MouseMove;
            _chartPanel.MouseClick += ChartPanel_MouseClick;
            _chartPanel.MouseLeave += (s, e) => { _hoverIndex = -1; _chartPanel.Invalidate(); };

            var descTitle = new Label { Text = "Description", Font = new Font("Segoe UI", 12, FontStyle.Bold), AutoSize = true, Margin = new Padding(0, 0, 0, 5) };
            var desc = new Label
            {
                Text = string.IsNullOrEmpty(_exercise.Description) ? "No description provided." : _exercise.Description,
                Font = new Font("Segoe UI", 11),
                Width = 800,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 30)
            };

            var instTitle = new Label { Text = "Instructions", Font = new Font("Segoe UI", 12, FontStyle.Bold), AutoSize = true, Margin = new Padding(0, 0, 0, 5) };
            var inst = new Label
            {
                Text = string.IsNullOrEmpty(_exercise.Instructions) ? "No instructions provided." : _exercise.Instructions,
                Font = new Font("Segoe UI", 11),
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
            _mainLayout.Controls.Add(_chartPanel);
            _mainLayout.Controls.Add(descTitle);
            _mainLayout.Controls.Add(desc);
            _mainLayout.Controls.Add(instTitle);
            _mainLayout.Controls.Add(inst);

            this.Controls.Add(_mainLayout);
        }

        private void ResetSelection()
        {
            _selectedIndex = -1;
            _valueLabel.Text = "Select a point ";
            _dateLabel.Text = "to see details";
        }

        private Button CreateToggleButton(string text, bool isActive)
        {
            return new Button
            {
                Text = text,
                Size = new Size(100, 35),
                FlatStyle = FlatStyle.Flat,
                BackColor = isActive ? Color.FromArgb(0, 120, 215) : Color.White,
                ForeColor = isActive ? Color.White : Color.Black,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
        }

        private void ChartPanel_MouseMove(object? sender, MouseEventArgs e)
        {
            int oldHover = _hoverIndex;
            _hoverIndex = -1;
            
            for (int i = 0; i < _currentPoints.Count; i++)
            {
                var p = _currentPoints[i];
                if (Math.Abs(p.X - e.X) < 10 && Math.Abs(p.Y - e.Y) < 10)
                {
                    _hoverIndex = i;
                    break;
                }
            }

            if (oldHover != _hoverIndex)
            {
                _chartPanel.Invalidate();
            }
        }

        private void ChartPanel_MouseClick(object? sender, MouseEventArgs e)
        {
            for (int i = 0; i < _currentPoints.Count; i++)
            {
                var p = _currentPoints[i];
                if (Math.Abs(p.X - e.X) < 15 && Math.Abs(p.Y - e.Y) < 15)
                {
                    _selectedIndex = i;
                    var data = _progressData[i];
                    var val = _showWeight ? data.MaxWeight : data.MaxVolume;
                    var unit = "kg";
                    
                    _valueLabel.Text = $"{val:0.##} {unit}";
                    _dateLabel.Text = $"on {data.Date:MMM dd, yyyy}";
                    
                    _chartPanel.Invalidate();
                    return;
                }
            }
        }

        private void ChartPanel_Paint(object? sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            var rect = _chartPanel.ClientRectangle;
            rect.X += 60; rect.Y += 20; rect.Width -= 80; rect.Height -= 70;

            // Draw border/background
            ControlPaint.DrawBorder(g, _chartPanel.ClientRectangle, Color.LightGray, ButtonBorderStyle.Solid);

            if (_progressData.Count < 2)
            {
                var msg = _progressData.Count == 0 ? "No data available." : "Need more data for chart.";
                g.DrawString(msg, new Font("Segoe UI", 12), Brushes.Gray, new PointF(rect.Left + 20, rect.Top + 20));
                return;
            }

            // Calculate scales
            var maxVal = _showWeight ? _progressData.Max(p => p.MaxWeight) : _progressData.Max(p => p.MaxVolume);
            if (maxVal == 0) maxVal = 1;

            _currentPoints.Clear();
            float stepX = rect.Width / (float)(_progressData.Count - 1);

            for (int i = 0; i < _progressData.Count; i++)
            {
                var val = _showWeight ? _progressData[i].MaxWeight : _progressData[i].MaxVolume;
                float x = rect.Left + (i * stepX);
                float y = rect.Bottom - ((float)val / (float)maxVal * rect.Height);
                _currentPoints.Add(new PointF(x, y));
            }

            // Draw Grid Lines (Y-Axis)
            using (var gridPen = new Pen(Color.FromArgb(240, 240, 240), 1))
            {
                for (int i = 0; i <= 4; i++)
                {
                    float y = rect.Bottom - (rect.Height * i / 4f);
                    g.DrawLine(gridPen, rect.Left, y, rect.Right, y);
                    
                    var labelVal = maxVal * i / 4;
                    g.DrawString($"{labelVal:0.#}", new Font("Segoe UI", 8), Brushes.Gray, new PointF(rect.Left - 50, y - 7));
                }
            }

            // Draw line segments
            for (int i = 0; i < _currentPoints.Count - 1; i++)
            {
                bool isPotential = _progressData[i + 1].IsPotential;
                using (var pen = new Pen(Color.FromArgb(0, 120, 215), 3))
                {
                    if (isPotential)
                    {
                        pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                    }
                    g.DrawLine(pen, _currentPoints[i], _currentPoints[i+1]);
                }
            }

            // Draw dots and labels
            for (int i = 0; i < _currentPoints.Count; i++)
            {
                var p = _currentPoints[i];
                bool isSelected = (i == _selectedIndex);
                bool isHovered = (i == _hoverIndex);
                bool isPotential = _progressData[i].IsPotential;

                if (isSelected)
                {
                    g.FillEllipse(Brushes.DodgerBlue, p.X - 6, p.Y - 6, 12, 12);
                }
                
                g.FillEllipse(Brushes.White, p.X - 4, p.Y - 4, 8, 8);
                
                using (var circlePen = new Pen(isSelected ? Color.DarkBlue : Color.DodgerBlue, 2))
                {
                    if (isPotential) circlePen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                    g.DrawEllipse(circlePen, p.X - 4, p.Y - 4, 8, 8);
                }

                // Draw Date Labels (X-Axis)
                if (i % Math.Max(1, _progressData.Count / 5) == 0 || i == _progressData.Count - 1)
                {
                    var dateStr = isPotential ? "Potential" : _progressData[i].Date.ToString("MM/dd");
                    g.DrawString(dateStr, new Font("Segoe UI", 8), Brushes.Gray, new PointF(p.X - 15, rect.Bottom + 10));
                }

                if (isHovered)
                {
                    var val = _showWeight ? _progressData[i].MaxWeight : _progressData[i].MaxVolume;
                    var hoverText = $"{(isPotential ? "[Pot.] " : "")}{val:0.##}";
                    var textSize = g.MeasureString(hoverText, new Font("Segoe UI", 9, FontStyle.Bold));
                    g.FillRectangle(Brushes.Black, p.X + 10, p.Y - 20, textSize.Width + 4, textSize.Height + 2);
                    g.DrawString(hoverText, new Font("Segoe UI", 9, FontStyle.Bold), Brushes.White, p.X + 12, p.Y - 18);
                }
            }

            // Y-Axis Label (Vertical)
            var yLabel = _showWeight ? "Weight (kg)" : "Volume (kg*reps)";
            var state = g.Save();
            g.TranslateTransform(20, rect.Top + rect.Height / 2);
            g.RotateTransform(-90);
            g.DrawString(yLabel, new Font("Segoe UI", 10, FontStyle.Bold), Brushes.DimGray, new PointF(-g.MeasureString(yLabel, new Font("Segoe UI", 10)).Width / 2, 0));
            g.Restore(state);
        }
    }
}
