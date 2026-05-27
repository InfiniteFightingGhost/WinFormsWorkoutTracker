using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using Data.Entities;
using System.Threading.Tasks;

namespace RealView.Views
{
    public class HistoryView : BaseView
    {
        private FlowLayoutPanel _mainLayout = null!;
        private FlowLayoutPanel _historyPanel = null!;

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
                Padding = new Padding(40),
                AutoScroll = true,
                WrapContents = false
            };

            var title = new Label
            {
                Text = "Workout History",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 30)
            };

            _historyPanel = new FlowLayoutPanel
            {
                Width = 800,
                AutoSize = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false
            };

            _mainLayout.Controls.Add(title);
            _mainLayout.Controls.Add(_historyPanel);

            this.Controls.Add(_mainLayout);
        }

        public override async void OnNavigatedTo()
        {
            await LoadHistory();
        }

        private async Task LoadHistory()
        {
            _historyPanel.Controls.Clear();
            var user = AppRuntime.Auth.GetCurrentUser();
            var sessions = await AppRuntime.WorkoutSession.GetAllUserSessionsAsync(user.Id);
            
            foreach (var session in sessions
                .Where(s => s.Status == Data.Enums.WorkoutStatus.Finished)
                .OrderByDescending(s => s.Start))
            {
                _historyPanel.Controls.Add(CreateHistoryCard(session));
            }
        }

        private Control CreateHistoryCard(WorkoutSession session)
        {
            var panel = new Panel
            {
                Size = new Size(760, 120),
                BackColor = Color.White,
                Margin = new Padding(0, 0, 0, 15),
                Padding = new Padding(20),
                Cursor = Cursors.Hand
            };
            panel.Click += (s, e) => AppRuntime.Navigation.NavigateTo<WorkoutDetailView>(session);

            var titleLabel = new Label
            {
                Text = string.IsNullOrEmpty(session.Title) ? session.Start.ToString("f") : session.Title,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true,
                Cursor = Cursors.Hand
            };

            string durationStr;
            if (session.End.HasValue)
            {
                var ts = session.End.Value - session.Start;
                durationStr = ts.TotalHours >= 1 ? ts.ToString(@"hh\:mm\:ss") : ts.ToString(@"mm\:ss");
            }
            else
            {
                durationStr = "Active Session";
            }

            var infoLabel = new Label
            {
                Text = $"Duration: {durationStr}",
                Font = new Font("Segoe UI", 10),
                Location = new Point(20, 50),
                AutoSize = true,
                Cursor = Cursors.Hand
            };

            var notesLabel = new Label
            {
                Text = session.Notes ?? "No notes",
                Font = new Font("Segoe UI", 9, FontStyle.Italic),
                Location = new Point(20, 75),
                AutoSize = true,
                ForeColor = Color.DimGray,
                Cursor = Cursors.Hand
            };

            var deleteBtn = new Button
            {
                Text = "Delete",
                ForeColor = Color.IndianRed,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(650, 20),
                Size = new Size(80, 30)
            };
            deleteBtn.Click += async (s, e) => {
                if (MessageBox.Show("Delete this session?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes) {
                    await AppRuntime.WorkoutSession.DeleteSessionAsync(session.Id);
                    await LoadHistory();
                }
            };

            panel.Controls.Add(titleLabel);
            panel.Controls.Add(infoLabel);
            panel.Controls.Add(notesLabel);
            panel.Controls.Add(deleteBtn);

            foreach (Control c in panel.Controls)
            {
                if (c != deleteBtn) c.Click += (s, e) => AppRuntime.Navigation.NavigateTo<WorkoutDetailView>(session);
            }

            panel.Paint += (s, e) => {
                ControlPaint.DrawBorder(e.Graphics, panel.ClientRectangle, Color.LightGray, ButtonBorderStyle.Solid);
            };

            return panel;
        }
    }
}
