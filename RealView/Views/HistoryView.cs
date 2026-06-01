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
                Font = UIStyle.Header,
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

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (_mainLayout == null) return;

            int availableWidth = _mainLayout.ClientSize.Width - _mainLayout.Padding.Horizontal - 20;
            _historyPanel.Width = availableWidth;

            foreach (Control card in _historyPanel.Controls)
            {
                card.Width = availableWidth;
                // Update delete button position
                foreach (Control c in card.Controls)
                {
                    if (c is Button btn && btn.Text == "Delete")
                    {
                        btn.Left = card.Width - btn.Width - 20;
                    }
                }
            }
        }

        public override async void OnNavigatedTo()
        {
            ShowSkeletons();
            await LoadHistory();
        }

        private void ShowSkeletons()
        {
            _historyPanel.Controls.Clear();
            for (int i = 0; i < 5; i++)
            {
                _historyPanel.Controls.Add(new Controls.SkeletonCard { Width = _historyPanel.Width - 40 });
            }
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
                Size = new Size(_historyPanel.Width, 120),
                BackColor = UIStyle.Surface,
                Margin = new Padding(0, 0, 0, 15),
                Padding = new Padding(20),
                Cursor = Cursors.Hand
            };
            panel.Click += (s, e) => AppRuntime.Navigation.NavigateTo<WorkoutDetailView>(session);

            var titleLabel = new Label
            {
                Text = string.IsNullOrEmpty(session.Title) ? session.Start.ToString("f") : session.Title,
                Font = UIStyle.BodySemibold,
                ForeColor = UIStyle.TextPrimary,
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
                Font = UIStyle.Body,
                ForeColor = UIStyle.TextSecondary,
                Location = new Point(20, 50),
                AutoSize = true,
                Cursor = Cursors.Hand
            };

            var notesLabel = new Label
            {
                Text = session.Notes ?? "No notes",
                Font = UIStyle.Caption,
                Location = new Point(20, 75),
                AutoSize = true,
                ForeColor = UIStyle.TextTertiary,
                Cursor = Cursors.Hand
            };

            var deleteBtn = new Button
            {
                Text = "Delete",
                ForeColor = UIStyle.Danger,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(panel.Width - 100, 20),
                Size = new Size(80, 30)
            };
            deleteBtn.FlatAppearance.BorderSize = 0;
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

            panel.Paint += (s, e) => DrawCard(e.Graphics, panel.ClientRectangle);

            return panel;
        }
    }
}
