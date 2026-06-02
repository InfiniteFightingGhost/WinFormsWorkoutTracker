using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Windows.Forms;

namespace WorkoutTracker.RealView.Controls
{
    public class SkeletonCard : UserControl
    {
        private float _shimmerOffset = 0;
        private System.Windows.Forms.Timer _shimmerTimer;

        public SkeletonCard()
        {
            this.DoubleBuffered = true;
            this.Size = new Size(360, 150);
            this.BackColor = UIStyle.Surface;
            this.Margin = new Padding(0, 0, 0, 15);

            _shimmerTimer = new System.Windows.Forms.Timer { Interval = 20 };
            _shimmerTimer.Tick += (s, e) =>
            {
                _shimmerOffset += 0.03f;
                if (_shimmerOffset > 1.5f) _shimmerOffset = -0.5f;
                this.Invalidate();
            };
            _shimmerTimer.Start();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // 1. Draw Card Background
            using (GraphicsPath path = new GraphicsPath())
            {
                int r = UIStyle.BorderRadius;
                Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);
                path.AddArc(rect.X, rect.Y, r * 2, rect.Y + r * 2, 180, 90);
                path.AddArc(rect.Right - r * 2, rect.Y, r * 2, r * 2, 270, 90);
                path.AddArc(rect.Right - r * 2, rect.Bottom - r * 2, r * 2, r * 2, 0, 90);
                path.AddArc(rect.X, rect.Bottom - r * 2, r * 2, r * 2, 90, 90);
                path.CloseFigure();

                using (SolidBrush brush = new SolidBrush(UIStyle.Surface))
                {
                    g.FillPath(brush, path);
                }
                using (Pen pen = new Pen(UIStyle.Border, 1))
                {
                    g.DrawPath(pen, path);
                }
            }

            // 2. Define Skeleton Shapes
            var shapes = new List<Rectangle>
            {
                new Rectangle(20, 20, Width / 2, 20),      // Title
                new Rectangle(20, 50, Width / 3, 15),      // Subtitle
                new Rectangle(20, 85, 50, 50),             // Photo/Icon
                new Rectangle(85, 95, Width - 120, 15),    // Info Line 1
                new Rectangle(85, 120, Width - 180, 15)    // Info Line 2
            };

            // 3. Draw Shapes with Shimmer
            Color baseColor = UIStyle.SurfaceVariant;
            Color highlightColor = UIStyle.CurrentTheme == ThemeType.Light 
                ? Color.FromArgb(200, 255, 255, 255) 
                : Color.FromArgb(100, 80, 80, 80);

            foreach (var rect in shapes)
            {
                DrawSkeletonShape(g, rect, baseColor, highlightColor);
            }
        }

        private void DrawSkeletonShape(Graphics g, Rectangle rect, Color baseColor, Color highlightColor)
        {
            using (GraphicsPath path = new GraphicsPath())
            {
                int r = 6;
                path.AddArc(rect.X, rect.Y, r * 2, r * 2, 180, 90);
                path.AddArc(rect.Right - r * 2, rect.Y, r * 2, r * 2, 270, 90);
                path.AddArc(rect.Right - r * 2, rect.Bottom - r * 2, r * 2, r * 2, 0, 90);
                path.AddArc(rect.X, rect.Bottom - r * 2, r * 2, r * 2, 90, 90);
                path.CloseFigure();

                using (LinearGradientBrush brush = new LinearGradientBrush(
                    new Point(this.Width / -2 + (int)(this.Width * _shimmerOffset * 2), 0),
                    new Point(this.Width / 2 + (int)(this.Width * _shimmerOffset * 2), 0),
                    baseColor, baseColor))
                {
                    ColorBlend blend = new ColorBlend();
                    blend.Colors = new Color[] { baseColor, highlightColor, baseColor };
                    blend.Positions = new float[] { 0, 0.5f, 1 };
                    brush.InterpolationColors = blend;

                    g.FillPath(brush, path);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _shimmerTimer?.Stop();
                _shimmerTimer?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
