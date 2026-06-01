using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data.DTOs;
using LiveChartsCore;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.WinForms;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace RealView.Views
{
    public class DashboardView : BaseView
    {
        private FlowLayoutPanel _mainLayout = null!;
        private Label _welcomeLabel = null!;
        private Button _startWorkoutBtn = null!;
        private FlowLayoutPanel _statsPanel = null!;
        private FlowLayoutPanel _recentPanel = null!;
        private List<MuscleVolumeDTO> _volumeData = new List<MuscleVolumeDTO>();

        public DashboardView()
        {
            InitializeComponent();
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

            _welcomeLabel = new Label
            {
                Text = "Welcome Back",
                Font = UIStyle.Header,
                ForeColor = UIStyle.TextPrimary,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 30)
            };

            _startWorkoutBtn = new Button
            {
                Text = "START NEW WORKOUT",
                Size = new Size(300, 60),
                BackColor = UIStyle.Primary,
                ForeColor = UIStyle.TextOnPrimary,
                FlatStyle = FlatStyle.Flat,
                Font = UIStyle.SubHeader,
                Margin = new Padding(0, 0, 0, 40)
            };
            _startWorkoutBtn.FlatAppearance.BorderSize = 0;
            _startWorkoutBtn.Click += StartWorkoutBtn_Click;

            // Stats Section (Heatmap & PRs)
            _statsPanel = new FlowLayoutPanel
            {
                Width = 1000,
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Margin = new Padding(0, 0, 0, 40)
            };

            var recentLabel = new Label
            {
                Text = "Recent Sessions",
                Font = UIStyle.SubHeader,
                ForeColor = UIStyle.TextPrimary,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 15)
            };

            _recentPanel = new FlowLayoutPanel
            {
                Width = 800,
                AutoSize = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false
            };

            _mainLayout.Controls.Add(_welcomeLabel);
            _mainLayout.Controls.Add(_startWorkoutBtn);
            _mainLayout.Controls.Add(_statsPanel);
            _mainLayout.Controls.Add(recentLabel);
            _mainLayout.Controls.Add(_recentPanel);

            this.Controls.Add(_mainLayout);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (_mainLayout == null) return;

            int availableWidth = _mainLayout.ClientSize.Width - _mainLayout.Padding.Horizontal - 20; // 20 for scrollbar buffer
            
            _statsPanel.Width = availableWidth;
            _recentPanel.Width = availableWidth;

            UpdateResponsiveLayout(availableWidth);
        }

        private void UpdateResponsiveLayout(int width)
        {
            // Cards in _statsPanel
            bool sideBySide = width > 900;
            int cardWidth = sideBySide ? (width - 20) / 2 : width;

            foreach (Control card in _statsPanel.Controls)
            {
                card.Width = cardWidth;
                // Adjust internal chart if present
                foreach (Control c in card.Controls)
                {
                    if (c is CartesianChart chart)
                    {
                        chart.Width = cardWidth - 40;
                    }
                }
            }

            // Cards in _recentPanel
            foreach (Control card in _recentPanel.Controls)
            {
                card.Width = width;
            }
        }

        public override async void OnNavigatedTo()
        {
            if (AppRuntime.WorkoutState.ActiveSession != null)
            {
                AppRuntime.Navigation.NavigateTo<ActiveWorkoutView>();
                return;
            }

            var user = AppRuntime.Auth.GetCurrentUser();
            if (user != null)
            {
                _welcomeLabel.Text = $"Welcome, {user.Username}!";
                
                // Show Skeletons
                ShowSkeletons(_statsPanel, 2);
                ShowSkeletons(_recentPanel, 3);
                
                await LoadDashboardData(user.Id);
            }
        }

        private void ShowSkeletons(FlowLayoutPanel panel, int count)
        {
            panel.Controls.Clear();
            for (int i = 0; i < count; i++)
            {
                panel.Controls.Add(new Controls.SkeletonCard { Width = panel.Width - 40 });
            }
        }

        private async Task LoadDashboardData(int userId)
        {
            _statsPanel.Controls.Clear();
            _recentPanel.Controls.Clear();

            var prs = await AppRuntime.WorkoutSet.GetUserPRsAsync(userId);
            var volume = await AppRuntime.WorkoutSet.GetMuscleVolumeAsync(userId);
            var sessions = await AppRuntime.WorkoutSession.GetAllUserSessionsAsync(userId);

            _statsPanel.Controls.Add(CreateVolumeChartCard(volume));
            _statsPanel.Controls.Add(CreatePRCard(prs));

            foreach (var session in sessions.Where(s => s.Status == Data.Enums.WorkoutStatus.Finished).OrderByDescending(s => s.Start).Take(5))
            {
                _recentPanel.Controls.Add(CreateSessionCard(session));
            }
        }

        private Control CreateVolumeChartCard(IEnumerable<MuscleVolumeDTO> volumeData)
        {
            _volumeData = volumeData.ToList();
            var panel = new Panel { Size = new Size(480, 320), BackColor = UIStyle.Surface, Padding = new Padding(20), Margin = new Padding(0, 0, 20, 0) };
            var lbl = new Label { Text = "Muscle Volume", Font = UIStyle.SubHeader, ForeColor = UIStyle.TextPrimary, AutoSize = true, Location = new Point(20, 20) };
            panel.Controls.Add(lbl);

            var chart = new CartesianChart
            {
                Location = new Point(20, 60),
                Size = new Size(440, 240),
                AnimationsSpeed = TimeSpan.FromMilliseconds(800),
                EasingFunction = LiveChartsCore.EasingFunctions.ExponentialOut,
                Series = new ISeries[]
                {
                    new ColumnSeries<decimal>
                    {
                        Values = _volumeData.Select(v => v.TotalVolume).ToArray(),
                        Fill = new SolidColorPaint(new SKColor(UIStyle.Primary.R, UIStyle.Primary.G, UIStyle.Primary.B)),
                        Name = "Total Volume",
                        Padding = 5,
                        MaxBarWidth = 40
                    }
                },
                XAxes = new Axis[]
                {
                    new Axis
                    {
                        Labels = _volumeData.Select(v => v.MuscleGroup).ToArray(),
                        LabelsPaint = new SolidColorPaint(new SKColor(UIStyle.TextSecondary.R, UIStyle.TextSecondary.G, UIStyle.TextSecondary.B)),
                        LabelsRotation = 15
                    }
                },
                YAxes = new Axis[]
                {
                    new Axis
                    {
                        LabelsPaint = new SolidColorPaint(new SKColor(UIStyle.TextSecondary.R, UIStyle.TextSecondary.G, UIStyle.TextSecondary.B))
                    }
                }
            };
            chart.DataPointerDown += VolumeChart_DataPointerDown;
            panel.Controls.Add(chart);

            panel.Paint += (s, e) => DrawCard(e.Graphics, panel.ClientRectangle);
            return panel;
        }

        private async void VolumeChart_DataPointerDown(object chart, IEnumerable<dynamic> points)
        {
            var point = points.FirstOrDefault();
            if (point == null) return;

            int index = (int)point.Index;
            if (index >= 0 && index < _volumeData.Count)
            {
                var data = _volumeData[index];
                var groups = await AppRuntime.MuscleGroup.GetMuscleGroupsAsync();
                var group = groups.FirstOrDefault(g => g.Name == data.MuscleGroup);
                if (group != null)
                {
                    AppRuntime.Navigation.NavigateTo<ExerciseManagementView>(group);
                }
            }
        }

        private Control CreatePRCard(IEnumerable<UserPRDTO> prData)
        {
            var panel = new Panel { Size = new Size(480, 320), BackColor = UIStyle.Surface, Padding = new Padding(20) };
            var lbl = new Label { Text = "Personal Records", Font = UIStyle.SubHeader, ForeColor = UIStyle.TextPrimary, AutoSize = true, Location = new Point(20, 20) };
            panel.Controls.Add(lbl);

            var listPanel = new FlowLayoutPanel { Location = new Point(20, 60), Size = new Size(440, 240), FlowDirection = FlowDirection.TopDown, BackColor = UIStyle.Surface };
            foreach (var pr in prData) {
                var prLbl = new Label { 
                    Text = $"{pr.ExerciseName}: {pr.MaxWeight}kg", 
                    Font = UIStyle.BodySemibold, 
                    ForeColor = UIStyle.Primary, // Primary to indicate clickable
                    AutoSize = true, 
                    Margin = new Padding(0, 5, 0, 2),
                    Cursor = Cursors.Hand
                };
                var dateLbl = new Label { 
                    Text = $"Achieved on {pr.Date:MMM dd, yyyy}", 
                    Font = UIStyle.Caption, 
                    ForeColor = UIStyle.TextTertiary, 
                    AutoSize = true, 
                    Margin = new Padding(0, 0, 0, 10),
                    Cursor = Cursors.Hand
                };

                prLbl.Click += async (s, e) => {
                    var ex = await AppRuntime.Exercise.GetExerciseByIdAsync(pr.ExerciseId);
                    if (ex != null) AppRuntime.Navigation.NavigateTo<ExerciseDetailView>(ex);
                };

                dateLbl.Click += async (s, e) => {
                    var session = await AppRuntime.WorkoutSession.GetByIdAsync(pr.WorkoutSessionId);
                    if (session != null) AppRuntime.Navigation.NavigateTo<WorkoutDetailView>(session);
                };

                listPanel.Controls.Add(prLbl);
                listPanel.Controls.Add(dateLbl);
            }
            if (!prData.Any()) listPanel.Controls.Add(new Label { Text = "No records yet. Keep lifting!", Font = UIStyle.Body, ForeColor = UIStyle.TextSecondary, AutoSize = true });

            panel.Controls.Add(listPanel);
            panel.Paint += (s, e) => DrawCard(e.Graphics, panel.ClientRectangle);
            return panel;
        }

        private Control CreateSessionCard(WorkoutSession session)
        {
            var panel = new Panel { Size = new Size(760, 100), BackColor = UIStyle.Surface, Margin = new Padding(0, 0, 0, 15), Padding = new Padding(20), Cursor = Cursors.Hand };
            panel.Click += (s, e) => AppRuntime.Navigation.NavigateTo<WorkoutDetailView>(session);

            if (!string.IsNullOrEmpty(session.PhotoUrl))
            {
                var pic = new PictureBox
                {
                    Size = new Size(60, 60),
                    Location = new Point(20, 20),
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Image = Services.PhotoService.LoadPhoto(session.PhotoUrl),
                    BackColor = UIStyle.SurfaceVariant
                };
                panel.Controls.Add(pic);
            }

            int textLeft = string.IsNullOrEmpty(session.PhotoUrl) ? 20 : 100;
            var titleLabel = new Label { 
                Text = string.IsNullOrEmpty(session.Title) ? session.Start.ToString("MMMM dd, yyyy") : session.Title, 
                Font = UIStyle.BodySemibold, 
                ForeColor = UIStyle.TextPrimary,
                Location = new Point(textLeft, 20), 
                AutoSize = true 
            };
            
            string durationStr = "N/A";
            if (session.End.HasValue) {
                var ts = session.End.Value - session.Start;
                durationStr = ts.TotalHours >= 1 ? ts.ToString(@"hh\:mm\:ss") : ts.ToString(@"mm\:ss");
            }
            var infoLabel = new Label { 
                Text = $"Duration: {durationStr}", 
                Font = UIStyle.Caption, 
                ForeColor = UIStyle.TextSecondary,
                Location = new Point(textLeft, 50), 
                AutoSize = true 
            };

            panel.Controls.Add(titleLabel);
            panel.Controls.Add(infoLabel);
            panel.Paint += (s, e) => DrawCard(e.Graphics, panel.ClientRectangle);
            
            foreach (Control c in panel.Controls) c.Click += (s, e) => AppRuntime.Navigation.NavigateTo<WorkoutDetailView>(session);

            return panel;
        }

        private async void StartWorkoutBtn_Click(object? sender, EventArgs e)
        {
            try {
                _startWorkoutBtn.Enabled = false;
                var user = AppRuntime.Auth.GetCurrentUser();
                var session = new WorkoutSession { UserId = user.Id, Start = DateTime.Now, Status = Data.Enums.WorkoutStatus.OnGoing };
                var created = await AppRuntime.WorkoutSession.CreateAsync(session);
                AppRuntime.WorkoutState.StartWorkout(created);
                AppRuntime.Navigation.NavigateTo<ActiveWorkoutView>();
            } catch (Exception ex) { 
                AppRuntime.Toasts.Show(ex.Message, true);
            }
            finally { _startWorkoutBtn.Enabled = true; }
        }
    }
}
