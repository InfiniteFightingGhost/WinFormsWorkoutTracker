using System;
using System.Drawing;
using System.Windows.Forms;

namespace RealView.Controls
{
    public class SummaryCard : UserControl
    {
        public enum CardType
        {
            Completion,
            Milestone,
            PR
        }

        public SummaryCard(CardType type, string value, string title, string subtitle, string username, string? tag = null)
        {
            InitializeComponent(type, value, title, subtitle, username, tag);
        }

        private void InitializeComponent(CardType type, string value, string title, string subtitle, string username, string? tag)
        {
            this.Size = new Size(360, 500);
            this.BackColor = UIStyle.Surface;
            this.Margin = new Padding(10);

            // Styling based on Hevy/Strava style cards
            var mainPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };
            this.Controls.Add(mainPanel);

            // Type icon/tag
            if (tag != null)
            {
                var tagLbl = new Label
                {
                    Text = tag,
                    BackColor = UIStyle.Highlight,
                    ForeColor = UIStyle.HighlightText,
                    AutoSize = true,
                    Padding = new Padding(5, 2, 5, 2),
                    Location = new Point(20, 20),
                    Font = UIStyle.CaptionBold
                };
                mainPanel.Controls.Add(tagLbl);
            }

            var valueLbl = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 36, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 100),
                ForeColor = UIStyle.TextPrimary
            };
            mainPanel.Controls.Add(valueLbl);

            var titleLbl = new Label
            {
                Text = title,
                Font = UIStyle.SubHeader,
                AutoSize = true,
                Location = new Point(23, 170),
                ForeColor = UIStyle.TextSecondary
            };
            mainPanel.Controls.Add(titleLbl);

            var subtitleLbl = new Label
            {
                Text = subtitle,
                Font = UIStyle.Body,
                AutoSize = true,
                Location = new Point(23, 205),
                ForeColor = UIStyle.TextTertiary
            };
            mainPanel.Controls.Add(subtitleLbl);

            // Bottom user info
            var userLbl = new Label
            {
                Text = $"@ {username}",
                Font = UIStyle.CaptionBold,
                ForeColor = UIStyle.TextTertiary,
                AutoSize = true,
                Location = new Point(20, 450)
            };
            mainPanel.Controls.Add(userLbl);

            // Rounded corners
            this.Paint += (s, e) =>
            {
                using (var path = new System.Drawing.Drawing2D.GraphicsPath())
                {
                    int r = UIStyle.BorderRadius * 2; // Using a larger radius for the summary card as per original design (20)
                    path.AddArc(0, 0, r, r, 180, 90);
                    path.AddArc(this.Width - r, 0, r, r, 270, 90);
                    path.AddArc(this.Width - r, this.Height - r, r, r, 0, 90);
                    path.AddArc(0, this.Height - r, r, r, 90, 90);
                    path.CloseFigure();
                    this.Region = new Region(path);
                }
            };
        }
    }
}
