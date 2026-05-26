using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using Data.Entities;

namespace RealView.Views
{
    public class DashboardView : BaseView
    {
        private FlowLayoutPanel _mainLayout = null!;
        private Label _welcomeLabel = null!;
        private Button _startWorkoutBtn = null!;
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
            _mainLayout.Controls.Add(recentLabel);
            _mainLayout.Controls.Add(_recentPanel);

            this.Controls.Add(_mainLayout);
        }

        public override async void OnNavigatedTo()
        {
            if (AppRuntime.WorkoutState.ActiveSession != null)
            {
                AppRuntime.Navigation.NavigateTo<ActiveWorkoutView>();
                return; // Halt execution so the dashboard doesn't render
            }

            var user = AppRuntime.Auth.GetCurrentUser();
            if (user != null)
            {
                _welcomeLabel.Text = $"Welcome, {user.Username}!";
                await LoadRecentSessions(user.Id);
            }
        }
        private async Task LoadRecentSessions(int userId)
        {
            _recentPanel.Controls.Clear();
            var sessions = await AppRuntime.WorkoutSession.GetAllUserSessionsAsync(userId);
            
            foreach (var session in sessions.OrderByDescending(s => s.Start).Take(5))
            {
                _recentPanel.Controls.Add(CreateSessionCard(session));
            }
        }

        private Control CreateSessionCard(WorkoutSession session)
        {
            var panel = new Panel
            {
                Size = new Size(760, 100),
                BackColor = Color.White,
                Margin = new Padding(0, 0, 0, 15),
                Padding = new Padding(20)
            };

            var dateLabel = new Label
            {
                Text = session.Start.ToString("MMMM dd, yyyy @ HH:mm"),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };

            var statusLabel = new Label
            {
                Text = $"Status: {session.Status}",
                Font = new Font("Segoe UI", 10),
                Location = new Point(20, 50),
                AutoSize = true,
                ForeColor = Color.Gray
            };

            panel.Controls.Add(dateLabel);
            panel.Controls.Add(statusLabel);

            panel.Paint += (s, e) => {
                ControlPaint.DrawBorder(e.Graphics, panel.ClientRectangle, Color.LightGray, ButtonBorderStyle.Solid);
            };

            return panel;
        }

        private async void StartWorkoutBtn_Click(object? sender, EventArgs e)
        {
            try
            {
                _startWorkoutBtn.Enabled = false; // Prevent double clicks

                var user = AppRuntime.Auth.GetCurrentUser();
                var session = new WorkoutSession
                {
                    UserId = user.Id,
                    Start = DateTime.Now,
                    End = null,
                    Status = Data.Enums.WorkoutStatus.OnGoing,
                    Notes = ""
                };

                var created = await AppRuntime.WorkoutSession.CreateAsync(session);

                // Update global memory state
                AppRuntime.WorkoutState.StartWorkout(created);

                // 2. FORCE THE SIDEBAR TO UPDATE
                if (this.ParentForm is MainForm shell)
                {
                    shell.RefreshSidebar(); // We will create this method next
                }

                AppRuntime.Navigation.NavigateTo<ActiveWorkoutView>();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                _startWorkoutBtn.Enabled = true;
            }
        }

    }
}
