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
            this.BackColor = Color.White;
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
                    BackColor = Color.FromArgb(255, 235, 245),
                    ForeColor = Color.FromArgb(255, 105, 180),
                    AutoSize = true,
                    Padding = new Padding(5, 2, 5, 2),
                    Location = new Point(20, 20),
                    Font = new Font("Segoe UI", 8, FontStyle.Bold)
                };
                mainPanel.Controls.Add(tagLbl);
            }

            var valueLbl = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 36, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 100),
                ForeColor = Color.FromArgb(40, 40, 40)
            };
            mainPanel.Controls.Add(valueLbl);

            var titleLbl = new Label
            {
                Text = title,
                Font = new Font("Segoe UI Semibold", 14),
                AutoSize = true,
                Location = new Point(23, 170),
                ForeColor = Color.FromArgb(100, 100, 100)
            };
            mainPanel.Controls.Add(titleLbl);

            var subtitleLbl = new Label
            {
                Text = subtitle,
                Font = new Font("Segoe UI", 10),
                AutoSize = true,
                Location = new Point(23, 205),
                ForeColor = Color.FromArgb(160, 160, 160)
            };
            mainPanel.Controls.Add(subtitleLbl);

            // Bottom user info
            var userLbl = new Label
            {
                Text = $"@ {username}",
                Font = new Font("Segoe UI Semibold", 9),
                ForeColor = Color.FromArgb(180, 180, 180),
                AutoSize = true,
                Location = new Point(20, 450)
            };
            mainPanel.Controls.Add(userLbl);

            // Rounded corners
            this.Paint += (s, e) =>
            {
                using (var path = new System.Drawing.Drawing2D.GraphicsPath())
                {
                    int r = 20;
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
