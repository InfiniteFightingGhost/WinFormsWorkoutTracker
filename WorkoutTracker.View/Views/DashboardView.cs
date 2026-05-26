using System;
using System.Drawing;
using System.Windows.Forms;
using Data.Entities;
using System.Collections.Generic;
using System.Linq;

namespace WorkoutTracker.View.Views
{
    public class DashboardView : BaseView
    {
        private FlowLayoutPanel _mainLayout;
        private Label _welcomeLabel;
        private Button _startWorkoutButton;
        private Panel _recentWorkoutsPanel;

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
                Padding = new Padding(20),
                AutoScroll = true,
                WrapContents = false
            };

            _welcomeLabel = new Label
            {
                Text = "Welcome back!",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 20)
            };

            _startWorkoutButton = new Button
            {
                Text = "START NEW WORKOUT",
                Size = new Size(300, 60),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Margin = new Padding(0, 0, 0, 30)
            };
            _startWorkoutButton.FlatAppearance.BorderSize = 0;
            _startWorkoutButton.Click += StartWorkoutButton_Click;

            var recentLabel = new Label
            {
                Text = "Recent Sessions",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 10)
            };

            _recentWorkoutsPanel = new FlowLayoutPanel
            {
                Width = 400,
                AutoSize = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false
            };

            _mainLayout.Controls.Add(_welcomeLabel);
            _mainLayout.Controls.Add(_startWorkoutButton);
            _mainLayout.Controls.Add(recentLabel);
            _mainLayout.Controls.Add(_recentWorkoutsPanel);

            this.Controls.Add(_mainLayout);
        }

        public override async void OnNavigatedTo()
        {
            var user = AppRuntime.Auth.GetCurrentUser();
            if (user != null)
            {
                _welcomeLabel.Text = $"Welcome, {user.Username}!";
                await LoadRecentSessions(user.Id);
            }
        }

        private async System.Threading.Tasks.Task LoadRecentSessions(int userId)
        {
            _recentWorkoutsPanel.Controls.Clear();
            var sessions = await AppRuntime.WorkoutSession.GetAllUserSessionsAsync(userId);
            
            foreach (var session in sessions.OrderByDescending(s => s.Start).Take(5))
            {
                var card = CreateSessionCard(session);
                _recentWorkoutsPanel.Controls.Add(card);
            }
        }

        private Control CreateSessionCard(WorkoutSession session)
        {
            var panel = new Panel
            {
                Size = new Size(380, 80),
                BackColor = Color.White,
                Margin = new Padding(0, 5, 0, 5),
                Padding = new Padding(10)
            };

            var title = new Label
            {
                Text = session.Start.ToString("MMMM dd, yyyy"),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(10, 10),
                AutoSize = true
            };

            var subTitle = new Label
            {
                Text = $"{session.Status} - {session.Notes ?? "No notes"}",
                Font = new Font("Segoe UI", 10),
                Location = new Point(10, 35),
                AutoSize = true,
                ForeColor = Color.Gray
            };

            panel.Controls.Add(title);
            panel.Controls.Add(subTitle);

            // Simple border/shadow effect
            panel.Paint += (s, e) => {
                ControlPaint.DrawBorder(e.Graphics, panel.ClientRectangle, Color.LightGray, ButtonBorderStyle.Solid);
            };

            return panel;
        }

        private async void StartWorkoutButton_Click(object sender, EventArgs e)
        {
            try 
            {
                var session = new WorkoutSession
                {
                    UserId = AppRuntime.Auth.GetCurrentUser().Id,
                    Start = DateTime.Now,
                    Status = Data.Enums.WorkoutStatus.OnGoing,
                    Notes = ""
                };

                var createdSession = await AppRuntime.WorkoutSession.CreateAsync(session);
                AppRuntime.WorkoutState.StartWorkout(createdSession);
                AppRuntime.Navigation.NavigateTo<ActiveWorkoutView>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting workout: {ex.Message}");
            }
        }
    }
}
