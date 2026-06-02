using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WorkoutTracker.RealView.Controls
{
    public class ModernButton : Button
    {
        private Color _targetColor;
        private Color _currentColor;
        private System.Windows.Forms.Timer _animationTimer;
        private int _borderRadius = UIStyle.BorderRadius;

        public Color HoverColor { get; set; } = UIStyle.PrimaryHover;
        private Color _normalColor = UIStyle.Primary;
        public Color NormalColor 
        { 
            get => _normalColor; 
            set 
            { 
                _normalColor = value; 
                if (!_animationTimer.Enabled)
                {
                    _currentColor = value;
                    _targetColor = value;
                    Invalidate();
                }
            } 
        }
        public int BorderRadius
        {
            get => _borderRadius;
            set
            {
                _borderRadius = value;
                UpdateControlRegion(); // <-- ADD THIS LINE
                Invalidate();
            }
        }
        public string? Icon { get; set; }
        public bool ShowText { get; set; } = true;

        public ModernButton()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | 
                          ControlStyles.OptimizedDoubleBuffer | 
                          ControlStyles.UserPaint | 
                          ControlStyles.ResizeRedraw, true);

            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.BorderSize = 0;
            this.BackColor = Color.Transparent; 
            this.ForeColor = UIStyle.TextOnPrimary;
            this.Font = UIStyle.BodySemibold;
            this.Cursor = Cursors.Hand;
            
            _currentColor = UIStyle.Primary;
            _targetColor = UIStyle.Primary;

            _animationTimer = new System.Windows.Forms.Timer { Interval = 15 };
            _animationTimer.Tick += AnimationTimer_Tick;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            _targetColor = HoverColor;
            _animationTimer.Start();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _targetColor = NormalColor;
            _animationTimer.Start();
        }

        private void AnimationTimer_Tick(object? sender, EventArgs e)
        {
            if (_currentColor.ToArgb() == _targetColor.ToArgb())
            {
                _animationTimer.Stop();
                return;
            }

            int a = Interpolate(_currentColor.A, _targetColor.A);
            int r = Interpolate(_currentColor.R, _targetColor.R);
            int g = Interpolate(_currentColor.G, _targetColor.G);
            int b = Interpolate(_currentColor.B, _targetColor.B);

            _currentColor = Color.FromArgb(a, r, g, b);
            this.Invalidate();
        }

        private int Interpolate(int current, int target)
        {
            if (current == target) return target;
            int step = Math.Max(1, Math.Abs(current - target) / 4); // Adaptive step for smoothness
            return current < target ? Math.Min(current + step, target) : Math.Max(current - step, target);
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            InvokePaintBackground(this, pevent);

            Graphics g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            using (GraphicsPath path = new GraphicsPath())
            {
                int d = Math.Min(_borderRadius * 2, Math.Min(Width, Height));
                if (d > 0)
                {
                    path.AddArc(0, 0, d, d, 180, 90);
                    path.AddArc(Width - d, 0, d, d, 270, 90);
                    path.AddArc(Width - d, Height - d, d, d, 0, 90);
                    path.AddArc(0, Height - d, d, d, 90, 90);
                }
                else
                {
                    path.AddRectangle(new Rectangle(0, 0, Width, Height));
                }
                path.CloseFigure();

                using (SolidBrush brush = new SolidBrush(_currentColor))
                {
                    g.FillPath(brush, path);
                }
            }

            if (!string.IsNullOrEmpty(Icon))
            {
                if (ShowText)
                {
                    // Draw Icon on the left
                    TextRenderer.DrawText(g, Icon, UIStyle.SubHeader, new Rectangle(15, 0, 30, Height), this.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
                    // Draw Text
                    TextRenderer.DrawText(g, this.Text, this.Font, new Rectangle(50, 0, Width - 60, Height), this.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
                }
                else
                {
                    // Draw only Icon in the center
                    TextRenderer.DrawText(g, Icon, UIStyle.SubHeader, ClientRectangle, this.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
                }
            }
            else
            {
                TextRenderer.DrawText(g, this.Text, this.Font, ClientRectangle, this.ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }

        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            UpdateControlRegion();
        }

        private void UpdateControlRegion()
        {
            if (Width <= 0 || Height <= 0) return;

            // Safely clear the old GDI handle before creating a new one
            this.Region?.Dispose();

            using (GraphicsPath path = new GraphicsPath())
            {
                int d = Math.Min(_borderRadius * 2, Math.Min(Width, Height));
                if (d > 0)
                {
                    path.AddArc(0, 0, d, d, 180, 90);
                    path.AddArc(Width - d, 0, d, d, 270, 90);
                    path.AddArc(Width - d, Height - d, d, d, 0, 90);
                    path.AddArc(0, Height - d, d, d, 90, 90);
                }
                else
                {
                    path.AddRectangle(new Rectangle(0, 0, Width, Height));
                }
                path.CloseFigure();

                this.Region = new Region(path);
            }
        }
    }
}
