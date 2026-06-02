using WorkoutTracker.Data.Entities;
using WorkoutTracker.RealView.Controls;
using WorkoutTracker.RealView.Views;
using WorkoutTracker.RealView;

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

    private Panel _heroBanner = null!;
    private Panel _topHeader = null!; // FIX: Promoted to field to bypass Z-order index bugs
    private FlowLayoutPanel _shareButtonsPanel = null!;
    private Button _prevBtn = null!;
    private Button _nextBtn = null!;
    private Panel _bottomActionPanel = null!;
    private Label _sharePrompt = null!;

    public WorkoutSummaryView(WorkoutSession session)
    {
        _session = session;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.BackColor = UIStyle.Background;

        // 1. Bottom Action Panel (Docked)
        _bottomActionPanel = new Panel { Dock = DockStyle.Bottom, Height = 220, BackColor = Color.Transparent };

        _paginationLabel = new Label { Text = "• • •", Font = new Font("Segoe UI", 18), AutoSize = true, ForeColor = UIStyle.TextTertiary };
        _sharePrompt = new Label { Text = "Share workout - Tag @hevyapp", Font = new Font("Segoe UI Semibold", 10), ForeColor = UIStyle.TextTertiary, AutoSize = true };
        _shareButtonsPanel = new FlowLayoutPanel { Size = new Size(500, 100), FlowDirection = FlowDirection.LeftToRight, WrapContents = false, BackColor = Color.Transparent };

        string[] labels = { "Background", "Stories", "More", "Workout Link", "Copy Text" };
        foreach (var label in labels)
        {
            var container = new Panel { Size = new Size(110, 90) };
            var btn = new ModernButton
            {
                Size = new Size(50, 50),
                Location = new Point(20, 0),
                FlatStyle = FlatStyle.Flat,
                NormalColor = UIStyle.Surface,
                HoverColor = UIStyle.SurfaceVariant,
                Text = GetIconForLabel(label),
                BorderRadius = 25,
                Font = new Font("Segoe UI", 16),
                ForeColor = UIStyle.TextPrimary
            };
            string currentLabel = label;
            btn.Click += (s, e) => HandleShareClick(currentLabel);
            var lbl = new Label { Text = label, Size = new Size(110, 30), Location = new Point(0, 55), TextAlign = ContentAlignment.MiddleCenter, Font = UIStyle.Caption };
            container.Controls.Add(btn);
            container.Controls.Add(lbl);
            _shareButtonsPanel.Controls.Add(container);
        }

        _doneBtn = new ModernButton
        {
            Text = "DONE",
            Size = new Size(400, 55),
            NormalColor = UIStyle.Primary,
            HoverColor = UIStyle.PrimaryHover,
            ForeColor = UIStyle.TextOnPrimary,
            FlatStyle = FlatStyle.Flat,
            Font = UIStyle.SubHeader,
            Cursor = Cursors.Hand,
            BorderRadius = 28
        };
        _doneBtn.Click += (s, e) => AppRuntime.Navigation.NavigateTo<DashboardView>();

        _bottomActionPanel.Controls.Add(_paginationLabel);
        _bottomActionPanel.Controls.Add(_sharePrompt);
        _bottomActionPanel.Controls.Add(_shareButtonsPanel);
        _bottomActionPanel.Controls.Add(_doneBtn);
        this.Controls.Add(_bottomActionPanel);

        // 2. Hero Banner (Detailed Mode)
        _heroBanner = new Panel { Dock = DockStyle.Left, Width = 300, Visible = false, Padding = new Padding(40) };
        var heroTitle = new Label { Text = "Nice\nwork!", Font = new Font("Segoe UI Variable Display", 42, FontStyle.Bold), AutoSize = true, Location = new Point(40, 80) };
        _heroBanner.Controls.Add(heroTitle);
        this.Controls.Add(_heroBanner);

        // 3. Top Celebration Header
        _topHeader = new Panel { Dock = DockStyle.Top, Height = 140, Padding = new Padding(40, 20, 40, 0) };
        var niceWorkLbl = new Label { Text = "Nice work!", Font = new Font("Segoe UI Variable Display", 26, FontStyle.Bold), AutoSize = true, Location = new Point(40, 15) };
        _subtitleLbl = new Label { Text = "Calculating...", Font = new Font("Segoe UI Semibold", 13), ForeColor = UIStyle.TextSecondary, AutoSize = true, Location = new Point(42, 75) };
        _topHeader.Controls.Add(niceWorkLbl);
        _topHeader.Controls.Add(_subtitleLbl);
        this.Controls.Add(_topHeader);

        // 4. Content Container
        _carouselContainer = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
        _cardsPanel = new FlowLayoutPanel { WrapContents = false, AutoSize = true, FlowDirection = FlowDirection.LeftToRight, BackColor = Color.Transparent };
        _carouselContainer.Controls.Add(_cardsPanel);
        this.Controls.Add(_carouselContainer);

        _prevBtn = new Button { Text = "◀", Size = new Size(50, 50), FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = UIStyle.TextSecondary };
        _nextBtn = new Button { Text = "▶", Size = new Size(50, 50), FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = UIStyle.TextSecondary };
        _prevBtn.FlatAppearance.BorderSize = 0;
        _nextBtn.FlatAppearance.BorderSize = 0;
        _prevBtn.Click += (s, e) => NavigateCarousel(-1);
        _nextBtn.Click += (s, e) => NavigateCarousel(1);
        this.Controls.Add(_prevBtn);
        this.Controls.Add(_nextBtn);

        this.Resize += (s, e) => UpdateLayout();
        UpdateLayout();
    }

    private void UpdateLayout()
    {
        // 1. Force a clean layout suspension to prevent mid-resize rendering race conditions
        this.SuspendLayout();
        _carouselContainer.SuspendLayout();
        _cardsPanel.SuspendLayout();

        bool isMini = this.Width < 650;
        bool isDetailed = this.Width >= 1150;

        // 2. Explicit Visibility Controls (Using your exact class fields)
        _heroBanner.Visible = isDetailed;         // Large top detailed banner view
        _topHeader.Visible = !isDetailed;         // Left-aligned carousel mode header panel
        _paginationLabel.Visible = !isDetailed;
        _prevBtn.Visible = _nextBtn.Visible = !isDetailed;

        _carouselContainer.BringToFront();

        // 3. Action Footer Layout
        _bottomActionPanel.Height = isMini ? 160 : 220;
        _doneBtn.Width = isMini ? this.Width - 40 : 400;
        _doneBtn.Height = isMini ? 50 : 55;
        _doneBtn.Location = new Point((_bottomActionPanel.Width - _doneBtn.Width) / 2, _bottomActionPanel.Height - (isMini ? 60 : 75));

        _shareButtonsPanel.Width = Math.Min(500, this.Width - 20);
        _shareButtonsPanel.Location = new Point((_bottomActionPanel.Width - _shareButtonsPanel.Width) / 2, _doneBtn.Top - (isMini ? 80 : 100));
        foreach (Control c in _shareButtonsPanel.Controls)
        {
            if (c is Panel p)
            {
                p.Controls[1].Visible = !isMini;
                p.Width = isMini ? 60 : 110;
                p.Controls[0].Left = (p.Width - p.Controls[0].Width) / 2;
            }
        }

        _sharePrompt.Location = new Point((_bottomActionPanel.Width - _sharePrompt.Width) / 2, _shareButtonsPanel.Top - 20);
        _sharePrompt.Visible = !isMini;
        _paginationLabel.Location = new Point((_bottomActionPanel.Width - _paginationLabel.Width) / 2, 5);

        // 4. Content Area Layout Engine (Grid vs Carousel)
        if (isDetailed)
        {
            // ALWAYS turn off scrolling engines before wiping coordinates
            _carouselContainer.AutoScroll = false;
            _cardsPanel.AutoScroll = false;

            // Force reset the hidden Windows layout scroll values to absolute zero
            _cardsPanel.HorizontalScroll.Value = 0;
            _cardsPanel.VerticalScroll.Value = 0;
            _cardsPanel.AutoScrollPosition = new Point(0, 0);

            // Drop DockStyle.Fill completely. Explicitly map bounds to match the parent container perfectly.
            _cardsPanel.Dock = DockStyle.None;
            _cardsPanel.Location = new Point(0, 0);
            _cardsPanel.Size = _carouselContainer.ClientSize;

            _cardsPanel.WrapContents = true;
            _cardsPanel.AutoSize = false;
            _cardsPanel.Padding = new Padding(30);

            foreach (SummaryCard card in _cards)
            {
                card.Mode = SummaryCard.DisplayMode.Detailed;
                card.Size = new Size(320, 440);
            }

            // Enable vertical scrolling only after coordinates are locked down cleanly
            _cardsPanel.AutoScroll = true;
        }
        else
        {
            _carouselContainer.AutoScroll = false;
            _cardsPanel.AutoScroll = false;

            _cardsPanel.Dock = DockStyle.None;
            _cardsPanel.WrapContents = false;
            _cardsPanel.AutoSize = true;

            int cardWidth = isMini ? 300 : 380;
            int cardHeight = isMini ? 380 : 500;

            foreach (SummaryCard card in _cards)
            {
                card.Mode = isMini ? SummaryCard.DisplayMode.Mini : SummaryCard.DisplayMode.Normal;
                card.Size = new Size(cardWidth - 20, cardHeight);
            }

            // Mathematically center the current card in the viewport container
            _cardsPanel.Location = new Point(
                (_carouselContainer.Width - cardWidth) / 2 - (_currentCardIndex * cardWidth),
                (_carouselContainer.Height - cardHeight) / 2
            );

            _prevBtn.Location = new Point((this.Width - cardWidth) / 2 - 65, _carouselContainer.Top + (_carouselContainer.Height / 2) - 25);
            _nextBtn.Location = new Point((this.Width + cardWidth) / 2 + 15, _carouselContainer.Top + (_carouselContainer.Height / 2) - 25);

            _prevBtn.BringToFront();
            _nextBtn.BringToFront();
        }

        // 5. Resume and force instant visual update execution
        _cardsPanel.ResumeLayout(true);
        _carouselContainer.ResumeLayout(true);
        this.ResumeLayout(true);

        // Final defensive guard: Force UI redraw to wipe out ghost layouts
        this.Refresh();
    }
    private string GetIconForLabel(string label) => label switch
    {
        "Background" => "🖼️",
        "Stories" => "📸",
        "More" => "⋯",
        "Workout Link" => "🔗",
        "Copy Text" => "📝",
        _ => "⚡"
    };

    private void HandleShareClick(string label)
    {
        switch (label)
        {
            case "Copy Text":
                Clipboard.SetText($"I just finished my workout! volume lifted: {_subtitleLbl.Text}");
                AppRuntime.Toasts.Show("Workout summary copied!");
                break;
            case "Workout Link":
                Clipboard.SetText("https://workouttracker.app/s/session_" + _session.Id);
                AppRuntime.Toasts.Show("Link copied to clipboard!");
                break;
            default:
                AppRuntime.Toasts.Show($"{label} sharing coming soon!");
                break;
        }
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
        int workoutCount = sessions.Count;
        _subtitleLbl.Text = $"This is your {workoutCount}nd workout";

        await LoadCardsAsync();
    }

    private void NavigateCarousel(int direction)
    {
        _currentCardIndex += direction;
        if (_currentCardIndex < 0) _currentCardIndex = 0;
        if (_currentCardIndex >= _cards.Count) _currentCardIndex = _cards.Count - 1;

        UpdateLayout();
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
        UpdateLayout(); // Force an explicit layout cycle once controls are loaded
    }

    private void UpdatePaginationDots()
    {
        string dots = "";
        for (int i = 0; i < _cards.Count; i++)
        {
            dots += (i == _currentCardIndex) ? " ● " : " ○ ";
        }
        _paginationLabel.Text = dots;
    }
}
