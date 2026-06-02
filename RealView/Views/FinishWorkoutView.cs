using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using WorkoutTracker.Data.Entities;
using WorkoutTracker.RealView.Controls;

namespace WorkoutTracker.RealView.Views
{
    public class FinishWorkoutView : BaseView
    {
        private WorkoutSession _session;
        private TextBox _titleTxt = null!;
        private TextBox _notesTxt = null!;
        private string? _photoPath;
        private PictureBox _photoPreview = null!;
        private Panel _card = null!;

        public FinishWorkoutView(WorkoutSession session)
        {
            _session = session;
            InitializeComponent();
        }

        private Panel _detailedStats = null!;
        private TableLayoutPanel _mainSplit = null!;
        private FlowLayoutPanel _formLayout = null!;

        private void InitializeComponent()
        {
            this.BackColor = UIStyle.Background;

            _card = new Panel { BackColor = UIStyle.Surface, Padding = new Padding(30), AutoScroll = true };
            
            _mainSplit = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, BackColor = Color.Transparent };
            _mainSplit.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55f));
            _mainSplit.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45f));

            _formLayout = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, AutoScroll = true };

            _detailedStats = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };
            _detailedStats.Paint += (s, e) => {
                if (_detailedStats.Visible) {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    using (var pen = new Pen(UIStyle.Border, 1)) {
                        e.Graphics.DrawLine(pen, 0, 20, 0, _detailedStats.Height - 20);
                    }
                }
            };

            // Header Section
            var headerLayout = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, WrapContents = false, AutoSize = true, Margin = new Padding(0, 0, 0, 20) };
            var celebIcon = new Label { Text = "✨", Font = new Font("Segoe UI", 28), AutoSize = true, Margin = new Padding(0, 0, 10, 0) };
            var titleLbl = new Label { Text = "Finish Workout", Font = UIStyle.Header, AutoSize = true, Margin = new Padding(0, 8, 0, 0) };
            headerLayout.Controls.Add(celebIcon);
            headerLayout.Controls.Add(titleLbl);
            
            _titleTxt = new TextBox { Font = UIStyle.BodySemibold, Text = _session.Title ?? "Morning Workout" };
            var titleContainer = CreateInputWrapper(_titleTxt, "Workout Title", 45);

            _notesTxt = new TextBox { Multiline = true, Font = UIStyle.Body, Text = _session.Notes };
            var notesContainer = CreateInputWrapper(_notesTxt, "Notes", 120);

            var photoLbl = new Label { Text = "ATTACH PHOTO", Font = UIStyle.CaptionBold, ForeColor = UIStyle.TextSecondary, Margin = new Padding(5, 10, 0, 5) };
            _photoPreview = new PictureBox { Size = new Size(440, 200), SizeMode = PictureBoxSizeMode.Zoom, BackColor = UIStyle.SurfaceVariant, Margin = new Padding(0, 0, 0, 10) };
            _photoPreview.Paint += (s, e) => {
                if (_photoPreview.Image == null)
                    TextRenderer.DrawText(e.Graphics, "📷 No photo attached", UIStyle.Body, _photoPreview.ClientRectangle, UIStyle.TextTertiary, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            };

            var addPhotoBtn = new ModernButton { Text = "Choose Photo", Size = new Size(150, 40), NormalColor = UIStyle.Surface, HoverColor = UIStyle.SurfaceVariant, ForeColor = UIStyle.TextPrimary, BorderRadius = 8, Margin = new Padding(0, 0, 0, 30) };
            addPhotoBtn.Click += AddPhotoBtn_Click;

            var finishBtn = new ModernButton { Text = "FINALIZE WORKOUT", Size = new Size(440, 55), Margin = new Padding(0, 10, 0, 0), BorderRadius = 12, NormalColor = UIStyle.Success, Font = UIStyle.SubHeader };
            finishBtn.Click += FinishBtn_Click;

            _formLayout.Controls.Add(headerLayout);
            _formLayout.Controls.Add(titleContainer);
            _formLayout.Controls.Add(notesContainer);
            _formLayout.Controls.Add(photoLbl);
            _formLayout.Controls.Add(_photoPreview);
            _formLayout.Controls.Add(addPhotoBtn);
            _formLayout.Controls.Add(finishBtn);

            _mainSplit.Controls.Add(_formLayout, 0, 0);
            _mainSplit.Controls.Add(_detailedStats, 1, 0);

            _card.Controls.Add(_mainSplit);
            this.Controls.Add(_card);

            this.Resize += (s, e) => UpdateLayout();
            UpdateLayout();
        }

        private Panel CreateInputWrapper(Control inner, string label, int height)
        {
            var container = new FlowLayoutPanel { FlowDirection = FlowDirection.TopDown, WrapContents = false, AutoSize = true, Margin = new Padding(0, 10, 0, 15) };
            var lbl = new Label { Text = label.ToUpper(), Font = UIStyle.CaptionBold, ForeColor = UIStyle.TextSecondary, AutoSize = true, Margin = new Padding(5, 0, 0, 5) };
            
            var wrapper = new Panel { Size = new Size(440, height), BackColor = UIStyle.SurfaceVariant, Padding = new Padding(12, 10, 12, 10) };
            wrapper.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = GetRoundedPath(wrapper.ClientRectangle, 10))
                using (var pen = new Pen(UIStyle.Border, 1))
                {
                    e.Graphics.DrawPath(pen, path);
                }
            };
            
            if (inner is TextBox tb) tb.BorderStyle = BorderStyle.None;
            inner.BackColor = UIStyle.SurfaceVariant;
            inner.Dock = DockStyle.Fill;
            
            wrapper.Controls.Add(inner);
            container.Controls.Add(lbl);
            container.Controls.Add(wrapper);
            return container;
        }

        private void UpdateLayout()
        {
            bool isMini = this.Width < 650;
            bool isDetailed = this.Width >= 1150;

            // 1. Card Sizing
            if (isMini) {
                _card.Size = this.ClientSize;
                _card.Location = Point.Empty;
                _card.Padding = new Padding(15);
            } else {
                int targetWidth = isDetailed ? 1100 : Math.Min(600, this.Width - 80);
                int targetHeight = Math.Min(850, this.Height - 80);
                _card.Size = new Size(targetWidth, targetHeight);
                _card.Location = new Point((this.Width - _card.Width) / 2, (this.Height - _card.Height) / 2);
                _card.Padding = new Padding(30);
            }

            // 2. Multi-column logic
            _detailedStats.Visible = isDetailed;
            if (isDetailed) {
                _mainSplit.ColumnStyles[0].Width = 55f;
                _mainSplit.ColumnStyles[1].Width = 45f;
                if (_detailedStats.Controls.Count == 0) PopulateDetailedStats();
            } else {
                _mainSplit.ColumnStyles[0].Width = 100f;
                _mainSplit.ColumnStyles[1].Width = 0f;
            }

            // 3. Internal Element Sizing
            int contentWidth = isDetailed ? (_card.Width / 2) - 60 : _card.Width - 60;
            foreach (Control c in _formLayout.Controls) {
                c.Width = contentWidth;
                if (c is FlowLayoutPanel flp) {
                    foreach(Control child in flp.Controls) if(child is Panel p) p.Width = contentWidth;
                }
            }
            _photoPreview.Width = contentWidth;
            _photoPreview.Height = isDetailed ? 300 : 200;
        }

        private void PopulateDetailedStats()
        {
            var layout = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false };
            var statsLbl = new Label { Text = "SESSION SNAPSHOT", Font = UIStyle.CaptionBold, ForeColor = UIStyle.TextSecondary, AutoSize = true, Margin = new Padding(10, 0, 0, 20) };
            
            void AddStat(string label, string value) {
                var p = new Panel { Width = 400, Height = 60, Margin = new Padding(0, 0, 0, 10) };
                var v = new Label { Text = value, Font = UIStyle.SubHeader, AutoSize = true, Location = new Point(10, 5) };
                var l = new Label { Text = label, Font = UIStyle.Caption, ForeColor = UIStyle.TextTertiary, AutoSize = true, Location = new Point(12, 30) };
                p.Controls.Add(v); p.Controls.Add(l);
                layout.Controls.Add(p);
            }

            decimal vol = _session.Exercises?.Sum(e => e.Sets?.Sum(s => s.Weight * s.Repetitions) ?? 0) ?? 0;
            int sets = _session.Exercises?.Sum(e => e.Sets?.Count ?? 0) ?? 0;
            
            AddStat("Total Volume", $"{vol:N0} kg");
            AddStat("Sets Performed", $"{sets} sets");
            AddStat("Exercises", $"{_session.Exercises?.Count ?? 0} movements");
            
            _detailedStats.Controls.Add(statsLbl);
            _detailedStats.Controls.Add(layout);
        }

        private GraphicsPath GetRoundedPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int d = radius * 2;
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        private void AddPhotoBtn_Click(object? sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog { Filter = "Image Files|*.jpg;*.jpeg;*.png" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    _photoPath = Services.PhotoService.SavePhoto(ofd.FileName, "Workouts");
                    _photoPreview.Image = Services.PhotoService.LoadPhoto(_photoPath);
                }
            }
        }

        private async void FinishBtn_Click(object? sender, EventArgs e)
        {
            try
            {
                await AppRuntime.WorkoutSession.UpdateWorkoutSession(_session.Id, DateTime.Now, _titleTxt.Text, _notesTxt.Text, _photoPath);
                AppRuntime.WorkoutState.FinishWorkout();
                AppRuntime.Navigation.NavigateTo<WorkoutSummaryView>(_session);
            }
            catch (Exception ex)
            {
                AppRuntime.Toasts.Show(ex.Message, true);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            DrawCard(e.Graphics, _card.Bounds);
        }
    }
}
