using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using WorkoutTracker.Data.Entities;
using System.Threading.Tasks;
using WorkoutTracker.RealView.Controls;

namespace WorkoutTracker.RealView.Views
{
    public class HistoryView : BaseView
    {
        private FlowLayoutPanel _mainLayout = null!;
        private VirtualFlowPanel<WorkoutSession> _virtualList = null!;

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
                AutoScroll = false,
                WrapContents = false
            };

            var title = new Label
            {
                Text = "Workout History",
                Font = UIStyle.Header,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 30)
            };

            _virtualList = new VirtualFlowPanel<WorkoutSession>(
                (session) => {
                    var card = new Controls.HistoryCard();
                    card.DeleteClicked += async (s, sess) => {
                        if (MessageBox.Show("Delete this session?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes) {
                            await AppRuntime.WorkoutSession.DeleteSessionAsync(sess.Id);
                            await LoadHistory();
                        }
                    };
                    return card;
                },
                (control, session) => ((Controls.HistoryCard)control).Bind(session)
            )
            {
                ItemHeight = 120,
                ItemPadding = 15,
                Dock = DockStyle.Fill
            };

            _mainLayout.Controls.Add(title);
            _mainLayout.Controls.Add(_virtualList);

            this.Controls.Add(_mainLayout);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (_virtualList == null) return;
            _virtualList.ItemWidth = this.Width - 100;
        }

        public override async void OnNavigatedTo()
        {
            await LoadHistory();
        }

        private async Task LoadHistory()
        {
            try
            {
                var user = AppRuntime.Auth.GetCurrentUser();
                if (user == null) return;

                var sessions = await AppRuntime.WorkoutSession.GetAllUserSessionsAsync(user.Id);
                var finishedSessions = sessions
                    .Where(s => s.Status == Data.Enums.WorkoutStatus.Finished)
                    .OrderByDescending(s => s.Start)
                    .ToList();

                _virtualList.Items = finishedSessions;
            }
            catch (Exception ex)
            {
                AppRuntime.Toasts.Show($"Error loading history: {ex.Message}", true);
            }
        }
    }
}
