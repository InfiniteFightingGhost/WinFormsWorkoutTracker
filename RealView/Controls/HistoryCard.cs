using System;
using System.Drawing;
using System.Windows.Forms;
using WorkoutTracker.Data.Entities;

namespace WorkoutTracker.RealView.Controls
{
    public class HistoryCard : UserControl
    {
        private Label _titleLabel = null!;
        private Label _infoLabel = null!;
        private Label _notesLabel = null!;
        private Button _deleteBtn = null!;
        private WorkoutSession? _session;

        public event EventHandler<WorkoutSession>? DeleteClicked;

        public HistoryCard()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(800, 120);
            this.BackColor = UIStyle.Surface;
            this.Padding = new Padding(20);
            this.Cursor = Cursors.Hand;

            _titleLabel = new Label
            {
                Font = UIStyle.BodySemibold,
                ForeColor = UIStyle.TextPrimary,
                Location = new Point(20, 20),
                AutoSize = true,
                Cursor = Cursors.Hand
            };

            _infoLabel = new Label
            {
                Font = UIStyle.Body,
                ForeColor = UIStyle.TextSecondary,
                Location = new Point(20, 50),
                AutoSize = true,
                Cursor = Cursors.Hand
            };

            _notesLabel = new Label
            {
                Font = UIStyle.Caption,
                Location = new Point(20, 75),
                AutoSize = true,
                ForeColor = UIStyle.TextTertiary,
                Cursor = Cursors.Hand
            };

            _deleteBtn = new Button
            {
                Text = "Delete",
                ForeColor = UIStyle.Danger,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(80, 30)
            };
            _deleteBtn.FlatAppearance.BorderSize = 0;
            _deleteBtn.Click += (s, e) => {
                if (_session != null) DeleteClicked?.Invoke(this, _session);
            };

            this.Controls.Add(_titleLabel);
            this.Controls.Add(_infoLabel);
            this.Controls.Add(_notesLabel);
            this.Controls.Add(_deleteBtn);

            this.Click += Card_Click;
            foreach (Control c in this.Controls)
            {
                if (c != _deleteBtn) c.Click += Card_Click;
            }

            this.Paint += (s, e) => {
                // Use the BaseView.DrawCard logic here? 
                // Since this isn't a BaseView, we'll manually draw or use a helper.
                using (var path = new System.Drawing.Drawing2D.GraphicsPath())
                {
                    int r = UIStyle.BorderRadius;
                    var rect = this.ClientRectangle;
                    path.AddArc(rect.X, rect.Y, r * 2, r * 2, 180, 90);
                    path.AddArc(rect.Right - r * 2, rect.Y, r * 2, r * 2, 270, 90);
                    path.AddArc(rect.Right - r * 2, rect.Bottom - r * 2, r * 2, r * 2, 0, 90);
                    path.AddArc(rect.X, rect.Bottom - r * 2, r * 2, r * 2, 90, 90);
                    path.CloseFigure();
                    e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    using (var pen = new Pen(UIStyle.Border, 1)) e.Graphics.DrawPath(pen, path);
                }
            };
        }

        private void Card_Click(object? sender, EventArgs e)
        {
            if (_session != null) AppRuntime.Navigation.NavigateTo<Views.WorkoutDetailView>(_session);
        }

        public void Bind(WorkoutSession session)
        {
            _session = session;
            _titleLabel.Text = string.IsNullOrEmpty(session.Title) ? session.Start.ToString("f") : session.Title;
            
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
            _infoLabel.Text = $"Duration: {durationStr}";
            _notesLabel.Text = session.Notes ?? "No notes";
            
            _deleteBtn.Location = new Point(this.Width - 100, 20);
        }
    }
}
