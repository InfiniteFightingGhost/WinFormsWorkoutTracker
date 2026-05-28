using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Data.Entities;
using System.Threading.Tasks;
using RealView.Controls;

namespace RealView.Views
{
    public class WorkoutSummaryView : BaseView
    {
        private WorkoutSession _session;
        private Panel _carouselContainer = null!;
        private FlowLayoutPanel _cardsPanel = null!;
        private int _currentCardIndex = 0;
        private List<SummaryCard> _cards = new List<SummaryCard>();
        private Label _paginationLabel = null!;
        private Label _subtitleLbl = null!;
        private Button _doneBtn = null!;
        private string? _sessionPhotoUrl = null;

        public WorkoutSummaryView(WorkoutSession session)
        {
            _session = session;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.BackColor = Color.FromArgb(245, 247, 251);

            // 1. Top Celebration Header
            var header = new Panel { Dock = DockStyle.Top, Height = 140, Padding = new Padding(40, 20, 40, 0) };
            var niceWorkLbl = new Label { Text = "Nice work!", Font = new Font("Segoe UI", 32, FontStyle.Bold), AutoSize = true, Location = new Point(40, 15) };
            _subtitleLbl = new Label { Text = "Calculating...", Font = new Font("Segoe UI Semibold", 13), ForeColor = Color.FromArgb(140, 140, 140), AutoSize = true, Location = new Point(42, 75) };
            
            var celebIcon = new Button
            {
                Text = "✨",
                Size = new Size(60, 60),
                Location = new Point(this.Width - 110, 25),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 24),
                BackColor = Color.FromArgb(255, 235, 245),
                Cursor = Cursors.Hand
            };
            celebIcon.FlatAppearance.BorderSize = 0;
            // Rounded celeb button
            celebIcon.Paint += (s, e) => {
                using (var path = new System.Drawing.Drawing2D.GraphicsPath()) {
                    path.AddEllipse(0, 0, celebIcon.Width - 1, celebIcon.Height - 1);
                    celebIcon.Region = new Region(path);
                }
            };

            header.Controls.Add(niceWorkLbl);
            header.Controls.Add(_subtitleLbl);
            header.Controls.Add(celebIcon);
            this.Controls.Add(header);

            // 2. Carousel
            _carouselContainer = new Panel { Size = new Size(400, 550), Location = new Point((this.Width - 400) / 2, 160), BackColor = Color.Transparent };
            _cardsPanel = new FlowLayoutPanel { WrapContents = false, AutoSize = true, FlowDirection = FlowDirection.LeftToRight, BackColor = Color.Transparent };
            _carouselContainer.Controls.Add(_cardsPanel);
            this.Controls.Add(_carouselContainer);

            var prevBtn = new Button { Text = "◀", Size = new Size(45, 45), Location = new Point(_carouselContainer.Left - 60, 415), FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = Color.Gray };
            var nextBtn = new Button { Text = "▶", Size = new Size(45, 45), Location = new Point(_carouselContainer.Right + 15, 415), FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = Color.Gray };
            prevBtn.FlatAppearance.BorderSize = 0;
            nextBtn.FlatAppearance.BorderSize = 0;
            prevBtn.Click += (s, e) => NavigateCarousel(-1);
            nextBtn.Click += (s, e) => NavigateCarousel(1);
            this.Controls.Add(prevBtn);
            this.Controls.Add(nextBtn);

            // 3. Pagination & Sharing
            _paginationLabel = new Label { Text = "• • •", Font = new Font("Segoe UI", 18), AutoSize = true, Location = new Point((this.Width - 80) / 2, 720), ForeColor = Color.LightGray };
            this.Controls.Add(_paginationLabel);

            var sharePrompt = new Label { Text = "Share workout - Tag @hevyapp", Font = new Font("Segoe UI Semibold", 10), ForeColor = Color.FromArgb(160, 160, 160), AutoSize = true, Location = new Point((this.Width - 180) / 2, 765) };
            this.Controls.Add(sharePrompt);

            var shareButtonsPanel = new FlowLayoutPanel { Size = new Size(500, 100), Location = new Point((this.Width - 500) / 2, 800), FlowDirection = FlowDirection.LeftToRight, WrapContents = false };
            string[] labels = { "Background", "Stories", "More", "Workout Link", "Copy Text" };
            foreach (var label in labels)
            {
                var container = new Panel { Size = new Size(90, 90) };
                var btn = new Button { 
                    Size = new Size(50, 50), 
                    Location = new Point(20, 0),
                    FlatStyle = FlatStyle.Flat, 
                    BackColor = Color.White,
                    Text = "⚡"
                };
                btn.FlatAppearance.BorderColor = Color.FromArgb(230, 230, 230);
                // Circle button
                btn.Paint += (s, e) => {
                    using (var path = new System.Drawing.Drawing2D.GraphicsPath()) {
                        path.AddEllipse(0, 0, btn.Width - 1, btn.Height - 1);
                        btn.Region = new Region(path);
                    }
                };
                var lbl = new Label { Text = label, Size = new Size(90, 30), Location = new Point(0, 55), TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 7) };
                container.Controls.Add(btn);
                container.Controls.Add(lbl);
                shareButtonsPanel.Controls.Add(container);
            }
            this.Controls.Add(shareButtonsPanel);

            // 4. Bottom Action Button
            _doneBtn = new Button
            {
                Text = "DONE",
                Size = new Size(400, 60),
                BackColor = UIStyle.Primary,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = UIStyle.SubHeader,
                Location = new Point((this.Width - 400) / 2, this.Height - 100),
                Cursor = Cursors.Hand
            };
            _doneBtn.FlatAppearance.BorderSize = 0;
            // Rounded Done Button
            _doneBtn.Paint += (s, e) => {
                using (var path = new System.Drawing.Drawing2D.GraphicsPath()) {
                    int r = 30;
                    path.AddArc(0, 0, r, r, 180, 90);
                    path.AddArc(_doneBtn.Width - r, 0, r, r, 270, 90);
                    path.AddArc(_doneBtn.Width - r, _doneBtn.Height - r, r, r, 0, 90);
                    path.AddArc(0, _doneBtn.Height - r, r, r, 90, 90);
                    path.CloseFigure();
                    _doneBtn.Region = new Region(path);
                }
            };
            _doneBtn.Click += (s, e) => AppRuntime.Navigation.NavigateTo<DashboardView>();
            this.Controls.Add(_doneBtn);

            this.Resize += (s, e) => {
                _carouselContainer.Left = (this.Width - _carouselContainer.Width) / 2;
                prevBtn.Left = _carouselContainer.Left - 60;
                nextBtn.Left = _carouselContainer.Right + 15;
                _paginationLabel.Left = (this.Width - _paginationLabel.Width) / 2;
                sharePrompt.Left = (this.Width - sharePrompt.Width) / 2;
                shareButtonsPanel.Left = (this.Width - shareButtonsPanel.Width) / 2;
                _doneBtn.Left = (this.Width - _doneBtn.Width) / 2;
                _doneBtn.Top = Math.Max(880, this.Height - 100);
                celebIcon.Left = this.Width - 110;
            };
        }

        public override async void OnNavigatedTo()
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            var user = AppRuntime.Auth.GetCurrentUser();
            if (user == null) return;

            var sessions = await AppRuntime.WorkoutSession.GetAllUserSessionsAsync(user.Id);
            int workoutCount = sessions.Count; // Count already includes the finished one
            _subtitleLbl.Text = $"This is your {workoutCount}nd workout";

            await LoadCardsAsync();
        }

        private void NavigateCarousel(int direction)
        {
            _currentCardIndex += direction;
            if (_currentCardIndex < 0) _currentCardIndex = 0;
            if (_currentCardIndex >= _cards.Count) _currentCardIndex = _cards.Count - 1;

            // Scroll to the card
            _cardsPanel.Left = -(_currentCardIndex * 380); // Width of a SummaryCard
            UpdatePaginationDots();
        }

        private async Task LoadCardsAsync()
        {
            _cards.Clear();
            _cardsPanel.Controls.Clear();
            var user = AppRuntime.Auth.GetCurrentUser();
            if (user == null) return;
            
            // 1. Completion Card (Total Volume)
            decimal totalVolume = 0;
            int totalSets = 0;
            if (_session.Exercises != null)
            {
                foreach (var ex in _session.Exercises)
                {
                    if (ex.Sets != null)
                    {
                        totalSets += ex.Sets.Count;
                        totalVolume += ex.Sets.Sum(s => s.Weight * s.Repetitions);
                    }
                }
            }

            var compCard = new SummaryCard(SummaryCard.CardType.Completion, 
                $"{totalVolume:0} kg", 
                "Volume Lifted", 
                $"{totalSets} sets performed", 
                user.Username);
            _cards.Add(compCard);
            _cardsPanel.Controls.Add(compCard);

            // 2. Duration Card
            string durationStr = "0:00";
            if (_session.End.HasValue)
            {
                var ts = _session.End.Value - _session.Start;
                durationStr = ts.TotalHours >= 1 ? ts.ToString(@"hh\:mm") : ts.ToString(@"mm\:ss");
            }
            var durCard = new SummaryCard(SummaryCard.CardType.Milestone, 
                durationStr, 
                "Duration", 
                "Time spent training", 
                user.Username);
            _cards.Add(durCard);
            _cardsPanel.Controls.Add(durCard);

            // 3. PR Cards (Real detection)
            if (_session.Exercises != null)
            {
                foreach (var we in _session.Exercises)
                {
                    if (we.Sets != null && we.Sets.Any())
                    {
                        var sessionMax = we.Sets.Max(s => s.Weight);
                        // Simple PR check: get historical max excluding this session
                        var history = await AppRuntime.WorkoutSet.GetExerciseProgressAsync(user.Id, we.ExerciseId);
                        var historyMax = history.Where(p => p.Date < _session.Start).Select(p => p.MaxWeight).DefaultIfEmpty(0).Max();

                        if (sessionMax > historyMax)
                        {
                            var prCard = new SummaryCard(SummaryCard.CardType.PR, 
                                $"{sessionMax:0} kg", 
                                we.Exercise?.Name ?? "Exercise", 
                                "New Personal Record", 
                                user.Username, 
                                "PR");
                            _cards.Add(prCard);
                            _cardsPanel.Controls.Add(prCard);
                        }
                    }
                }
            }

            UpdatePaginationDots();
        }

        private void UpdatePaginationDots()
        {
            string dots = "";
            for (int i = 0; i < _cards.Count; i++)
            {
                // Use a more visual dot
                dots += (i == _currentCardIndex) ? " ● " : " ○ ";
            }
            _paginationLabel.Text = dots;
        }
    }
}
