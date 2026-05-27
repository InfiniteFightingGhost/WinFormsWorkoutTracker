using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RealView.Views
{
    public class DashboardView : BaseView
    {
        private FlowLayoutPanel _mainLayout = null!;
        private Label _welcomeLabel = null!;
        private Button _startWorkoutBtn = null!;
        private FlowLayoutPanel _statsPanel = null!;
        private FlowLayoutPanel _recentPanel = null!;

        public DashboardView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
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
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 30)
            };

            _startWorkoutBtn = new Button
            {
                Text = "START NEW WORKOUT",
                Size = new Size(300, 60),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
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
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
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
                await LoadDashboardData(user.Id);
            }
        }

        private async Task LoadDashboardData(int userId)
        {
            _statsPanel.Controls.Clear();
            _recentPanel.Controls.Clear();

            // Load data sequentially to avoid DbContext concurrency issues
            var prs = await AppRuntime.WorkoutSet.GetUserPRsAsync(userId);
            var volume = await AppRuntime.WorkoutSet.GetMuscleVolumeAsync(userId);
            var sessions = await AppRuntime.WorkoutSession.GetAllUserSessionsAsync(userId);

            _statsPanel.Controls.Add(CreateHeatmapCard(volume));
            _statsPanel.Controls.Add(CreatePRCard(prs));

            foreach (var session in sessions.Where(s => s.Status == Data.Enums.WorkoutStatus.Finished).OrderByDescending(s => s.Start).Take(5))
            {
                _recentPanel.Controls.Add(CreateSessionCard(session));
            }
        }

        private Control CreateHeatmapCard(IEnumerable<dynamic> volumeData)
        {
            var panel = new Panel { Size = new Size(480, 300), BackColor = Color.White, Padding = new Padding(20), Margin = new Padding(0, 0, 20, 0) };
            var lbl = new Label { Text = "Muscle Volume (All-time)", Font = new Font("Segoe UI", 14, FontStyle.Bold), AutoSize = true, Location = new Point(20, 20) };
            panel.Controls.Add(lbl);

            var chartArea = new Panel { Location = new Point(20, 60), Size = new Size(440, 220) };
            chartArea.Paint += (s, e) => {
                var g = e.Graphics;
                var data = volumeData.ToList();
                if (!data.Any()) { g.DrawString("No data yet.", this.Font, Brushes.Gray, 10, 10); return; }

                float maxVolume = (float)data.Max(v => (decimal)v.TotalVolume);
                if (maxVolume == 0) maxVolume = 1;

                for (int i = 0; i < data.Count && i < 6; i++) {
                    float barWidth = (float)(decimal)data[i].TotalVolume / maxVolume * 300;
                    g.FillRectangle(Brushes.DodgerBlue, 100, i * 35, barWidth, 25);
                    g.DrawString(data[i].MuscleGroup, new Font("Segoe UI", 9), Brushes.Black, 0, i * 35 + 5);
                    g.DrawString($"{data[i].TotalVolume:0}", new Font("Segoe UI", 8), Brushes.DimGray, 105 + barWidth, i * 35 + 5);
                }
            };
            panel.Controls.Add(chartArea);

            panel.Paint += (s, e) => ControlPaint.DrawBorder(e.Graphics, panel.ClientRectangle, Color.LightGray, ButtonBorderStyle.Solid);
            return panel;
        }

        private Control CreatePRCard(IEnumerable<dynamic> prData)
        {
            var panel = new Panel { Size = new Size(480, 300), BackColor = Color.White, Padding = new Padding(20) };
            var lbl = new Label { Text = "Personal Records", Font = new Font("Segoe UI", 14, FontStyle.Bold), AutoSize = true, Location = new Point(20, 20) };
            panel.Controls.Add(lbl);

            var listPanel = new FlowLayoutPanel { Location = new Point(20, 60), Size = new Size(440, 220), FlowDirection = FlowDirection.TopDown };
            foreach (var pr in prData) {
                var prLbl = new Label { Text = $"{pr.ExerciseName}: {pr.MaxWeight}kg", Font = new Font("Segoe UI", 11), AutoSize = true, Margin = new Padding(0, 5, 0, 5) };
                var dateLbl = new Label { Text = $"Achieved on {pr.Date:MMM dd, yyyy}", Font = new Font("Segoe UI", 9), ForeColor = Color.Gray, AutoSize = true, Margin = new Padding(0, 0, 0, 10) };
                listPanel.Controls.Add(prLbl);
                listPanel.Controls.Add(dateLbl);
            }
            if (!prData.Any()) listPanel.Controls.Add(new Label { Text = "No records yet. Keep lifting!", Font = new Font("Segoe UI", 10), ForeColor = Color.Gray });

            panel.Controls.Add(listPanel);
            panel.Paint += (s, e) => ControlPaint.DrawBorder(e.Graphics, panel.ClientRectangle, Color.LightGray, ButtonBorderStyle.Solid);
            return panel;
        }

        private Control CreateSessionCard(WorkoutSession session)
        {
            var panel = new Panel { Size = new Size(760, 100), BackColor = Color.White, Margin = new Padding(0, 0, 0, 15), Padding = new Padding(20), Cursor = Cursors.Hand };
            panel.Click += (s, e) => AppRuntime.Navigation.NavigateTo<WorkoutDetailView>(session);

            var titleLabel = new Label { Text = string.IsNullOrEmpty(session.Title) ? session.Start.ToString("MMMM dd, yyyy") : session.Title, Font = new Font("Segoe UI", 12, FontStyle.Bold), Location = new Point(20, 20), AutoSize = true };
            
            string durationStr = "N/A";
            if (session.End.HasValue) {
                var ts = session.End.Value - session.Start;
                durationStr = ts.TotalHours >= 1 ? ts.ToString(@"hh\:mm\:ss") : ts.ToString(@"mm\:ss");
            }
            var infoLabel = new Label { Text = $"Duration: {durationStr}", Font = new Font("Segoe UI", 10), Location = new Point(20, 50), AutoSize = true, ForeColor = Color.Gray };

            panel.Controls.Add(titleLabel);
            panel.Controls.Add(infoLabel);
            panel.Paint += (s, e) => ControlPaint.DrawBorder(e.Graphics, panel.ClientRectangle, Color.LightGray, ButtonBorderStyle.Solid);
            
            // Add click events to children
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
            } catch (Exception ex) { MessageBox.Show(ex.Message); }
            finally { _startWorkoutBtn.Enabled = true; }
        }
    }
}
