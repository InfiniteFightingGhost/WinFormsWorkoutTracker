using System;
using System.Drawing;
using System.Windows.Forms;
using Data.Entities;
using System.Collections.Generic;
using System.Linq;

namespace WorkoutTracker.View.Views
{
    public class HistoryView : BaseView
    {
        private FlowLayoutPanel _mainLayout;
        private Label _titleLabel;
        private FlowLayoutPanel _historyPanel;

        public HistoryView()
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

            _titleLabel = new Label
            {
                Text = "Workout History",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 20)
            };

            _historyPanel = new FlowLayoutPanel
            {
                Width = 400,
                AutoSize = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false
            };

            _mainLayout.Controls.Add(_titleLabel);
            _mainLayout.Controls.Add(_historyPanel);

            this.Controls.Add(_mainLayout);
        }

        public override async void OnNavigatedTo()
        {
            await LoadHistory();
        }

        private async System.Threading.Tasks.Task LoadHistory()
        {
            _historyPanel.Controls.Clear();
            var user = AppRuntime.Auth.GetCurrentUser();
            var sessions = await AppRuntime.WorkoutSession.GetAllUserSessionsAsync(user.Id);
            
            foreach (var session in sessions.OrderByDescending(s => s.Start))
            {
                var card = CreateHistoryCard(session);
                _historyPanel.Controls.Add(card);
            }
        }

        private Control CreateHistoryCard(WorkoutSession session)
        {
            var panel = new Panel
            {
                Size = new Size(380, 100),
                BackColor = Color.White,
                Margin = new Padding(0, 5, 0, 5),
                Padding = new Padding(10)
            };

            var date = new Label
            {
                Text = session.Start.ToString("f"),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(10, 10),
                AutoSize = true
            };
            //FIX ME
            //var duration = session.End != default ? (session.End - session.Start).ToString(@"hh\:mm") : "In Progress";
            var info = new Label
            {
                //Text = $"Duration: {duration} | Status: {session.Status}",
                Font = new Font("Segoe UI", 10),
                Location = new Point(10, 35),
                AutoSize = true
            };

            var notes = new Label
            {
                Text = session.Notes ?? "No notes",
                Font = new Font("Segoe UI", 9, FontStyle.Italic),
                Location = new Point(10, 60),
                AutoSize = true,
                ForeColor = Color.DimGray
            };

            var deleteBtn = new Button { 
                Text = "X", 
                Location = new Point(340, 10), 
                Size = new Size(30, 30), 
                ForeColor = Color.Red,
                FlatStyle = FlatStyle.Flat
            };
            deleteBtn.FlatAppearance.BorderSize = 0;
            deleteBtn.Click += async (s, e) => {
                if (MessageBox.Show("Delete this session?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes) {
                    await AppRuntime.WorkoutSession.DeleteSessionAsync(session.Id);
                    await LoadHistory();
                }
            };

            panel.Controls.Add(date);
            panel.Controls.Add(info);
            panel.Controls.Add(notes);
            panel.Controls.Add(deleteBtn);

            panel.Paint += (s, e) => {
                ControlPaint.DrawBorder(e.Graphics, panel.ClientRectangle, Color.LightGray, ButtonBorderStyle.Solid);
            };

            return panel;
        }
    }
}
