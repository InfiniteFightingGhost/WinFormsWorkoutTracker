using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WorkoutTracker.RealView.Controls
{
    public class SummaryCard : UserControl
    {
        public enum CardType
        {
            Completion,
            Milestone,
            PR
        }

        public enum DisplayMode
        {
            Mini,
            Normal,
            Detailed
        }

        private CardType _type;
        private string _value;
        private string _title;
        private string _subtitle;
        private string _username;
        private string? _tag;
        private ModernButton _optionsBtn = null!;
        private DisplayMode _mode = DisplayMode.Normal;

        public DisplayMode Mode 
        { 
            get => _mode; 
            set { if (_mode != value) { _mode = value; Invalidate(); } } 
        }

        public SummaryCard(CardType type, string value, string title, string subtitle, string username, string? tag = null)
        {
            _type = type;
            _value = value;
            _title = title;
            _subtitle = subtitle;
            _username = username;
            _tag = tag;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(360, 500);
            this.BackColor = UIStyle.Surface;
            this.Margin = new Padding(10);
            this.DoubleBuffered = true;

            // Options Button
            _optionsBtn = new ModernButton
            {
                Text = "⋮",
                Size = new Size(30, 30),
                Location = new Point(this.Width - 40, 15),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                NormalColor = Color.Transparent,
                HoverColor = UIStyle.HoverOverlay,
                ForeColor = UIStyle.TextSecondary,
                BorderRadius = 15
            };
            
            var menu = new ContextMenuStrip();
            menu.Items.Add("Copy Summary", null, (s, e) => {
                Clipboard.SetText($"{_title}: {_value}\n{_subtitle}\n@{_username}");
                AppRuntime.Toasts.Show("Summary copied to clipboard!");
            });
            menu.Items.Add("Save as Image", null, (s, e) => {
                AppRuntime.Toasts.Show("Saving card as image...");
            });
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add("Share to Stories", null, (s, e) => AppRuntime.Toasts.Show("Opening sharing dialog..."));

            _optionsBtn.Click += (s, e) => menu.Show(_optionsBtn, new Point(0, _optionsBtn.Height));
            this.Controls.Add(_optionsBtn);

            this.Paint += SummaryCard_Paint;
            this.Resize += (s, e) => {
                _optionsBtn.Left = this.Width - 40;
                this.Invalidate();
            };
        }

        private void SummaryCard_Paint(object? sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Mode-based adjustments
            bool isMini = _mode == DisplayMode.Mini;
            int borderRadius = isMini ? 12 : 24;
            int padding = isMini ? 15 : 25;
            
            // DYNAMIC FONT SCALING (Reduced by 6 points as requested)
            float valueFontSize = _mode switch {
                DisplayMode.Mini => 18, 
                DisplayMode.Detailed => 36, 
                _ => 30 
            };

            // Draw Background and Rounded Corners
            using (var path = GetRoundedPath(this.ClientRectangle, borderRadius))
            {
                this.Region = new Region(path);
                using (var brush = new SolidBrush(UIStyle.Surface))
                {
                    g.FillPath(brush, path);
                }
                
                if (!isMini) {
                    using (var pen = new Pen(UIStyle.Border, 1))
                        g.DrawPath(pen, path);
                } else {
                    using (var pen = new Pen(Color.FromArgb(30, 0, 0, 0), 1))
                        g.DrawPath(pen, path);
                }
            }

            // Draw Tag
            if (_tag != null)
            {
                var tagRect = new Rectangle(padding, padding, 55, 22);
                using (var path = GetRoundedPath(tagRect, 6))
                using (var brush = new SolidBrush(UIStyle.Highlight))
                {
                    g.FillPath(brush, path);
                    TextRenderer.DrawText(g, _tag, UIStyle.CaptionBold, tagRect, UIStyle.HighlightText, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter | TextFormatFlags.EndEllipsis);
                }
            }

            // Accent Bar
            Color accentColor = _type switch {
                CardType.PR => UIStyle.Warning,
                CardType.Completion => UIStyle.Success,
                _ => UIStyle.Primary
            };

            // Calculate vertical offsets dynamically based on height
            int contentStart = isMini ? 50 : 85;

            // Draw Value
            using (var valueFont = new Font("Segoe UI Variable Display", valueFontSize, FontStyle.Bold))
            {
                var align = isMini ? TextFormatFlags.HorizontalCenter : TextFormatFlags.Left;
                TextFormatFlags flags = align | TextFormatFlags.Top | TextFormatFlags.SingleLine | TextFormatFlags.NoPadding;

                // 1. Calculate the available layout width
                int availableWidth = this.Width - (padding * 2);

                // 2. Ask GDI exactly how much room this specific string needs
                Size measuredSize = TextRenderer.MeasureText(g, _value, valueFont, new Size(availableWidth, 0), flags);

                // 3. Use the actual measured height (with a tiny 2px safety buffer)
                int vHeight = measuredSize.Height + 2;
                var valueRect = new Rectangle(padding, contentStart, availableWidth, vHeight);

                // 4. Draw it safely
                TextRenderer.DrawText(g, _value, valueFont, valueRect, UIStyle.TextPrimary, flags);

                // Accent Bar position relative to the ACTUAL text top
                using (var pen = new Pen(accentColor, isMini ? 3 : 4))
                {
                    int barWidth = isMini ? 30 : 40;
                    int barX = isMini ? (this.Width - barWidth) / 2 : padding;
                    g.DrawLine(pen, barX, valueRect.Top - 8, barX + barWidth, valueRect.Top - 8);
                }

                // 5. Push the next element down based on real measurements
                contentStart += vHeight + (isMini ? 2 : 10);
            }
            // Draw Title
            int tHeight = isMini ? 25 : 35;
            var titleRect = new Rectangle(padding, contentStart, this.Width - (padding * 2), tHeight);
            var titleAlign = isMini ? TextFormatFlags.HorizontalCenter : TextFormatFlags.Left;
            TextRenderer.DrawText(g, _title, UIStyle.SubHeader, titleRect, UIStyle.TextSecondary, 
                titleAlign | TextFormatFlags.Top | TextFormatFlags.WordBreak | TextFormatFlags.NoPadding);
            contentStart += tHeight + (isMini ? 2 : 8);

            // Draw Subtitle (Only if there is vertical room)
            int subtitleHeight = this.Height - contentStart - (padding + 20);
            if (subtitleHeight > 15) {
                var subtitleRect = new Rectangle(padding, contentStart, this.Width - (padding * 2), subtitleHeight);
                TextRenderer.DrawText(g, _subtitle, UIStyle.Body, subtitleRect, UIStyle.TextTertiary, 
                    titleAlign | TextFormatFlags.Top | TextFormatFlags.WordBreak | TextFormatFlags.NoPadding);
            }

            // Footer
            var footerRect = new Rectangle(padding, this.Height - padding - 20, this.Width - (padding * 2), 20);
            TextRenderer.DrawText(g, $"@ {_username}", UIStyle.CaptionBold, footerRect, UIStyle.TextTertiary, (isMini ? TextFormatFlags.HorizontalCenter : TextFormatFlags.Left) | TextFormatFlags.Bottom | TextFormatFlags.EndEllipsis);
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
    }
}
