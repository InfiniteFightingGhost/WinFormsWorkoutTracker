using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace RealView.Controls
{
    public class ModernScrollBar : UserControl
    {
        private float _value = 0;
        private float _max = 100;
        private float _largeChange = 10;
        
        private bool _isDragging = false;
        private Point _dragStart;
        private float _valueAtDragStart;

        public event EventHandler? Scroll;

        public float Value 
        { 
            get => _value; 
            set 
            { 
                _value = Math.Max(0, Math.Min(value, _max - _largeChange)); 
                Invalidate(); 
            } 
        }

        public float Maximum { get => _max; set { _max = value; Invalidate(); } }
        public float LargeChange { get => _largeChange; set { _largeChange = value; Invalidate(); } }

        public ModernScrollBar()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | 
                          ControlStyles.OptimizedDoubleBuffer | 
                          ControlStyles.UserPaint | 
                          ControlStyles.ResizeRedraw, true);
            this.Width = 8;
            this.BackColor = Color.Transparent;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            Rectangle thumbRect = GetThumbRect();
            if (thumbRect.Contains(e.Location))
            {
                _isDragging = true;
                _dragStart = e.Location;
                _valueAtDragStart = _value;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (_isDragging)
            {
                float deltaPx = e.Y - _dragStart.Y;
                float trackHeight = Height;
                float pxPerValue = trackHeight / _max;
                
                Value = _valueAtDragStart + (deltaPx / pxPerValue);
                Scroll?.Invoke(this, EventArgs.Empty);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _isDragging = false;
        }

        private Rectangle GetThumbRect()
        {
            if (_max <= 0) return Rectangle.Empty;
            
            float trackHeight = Height;
            float thumbHeight = Math.Max(20, (trackHeight * _largeChange) / _max);
            float thumbY = (trackHeight - thumbHeight) * (_value / (_max - _largeChange));
            
            if (float.IsNaN(thumbY)) thumbY = 0;

            return new Rectangle(1, (int)thumbY, Width - 2, (int)thumbHeight);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Draw track
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(20, UIStyle.TextSecondary)))
            {
                g.FillRoundedRectangle(brush, new Rectangle(2, 0, Width - 4, Height), 2);
            }

            // Draw thumb
            Rectangle thumbRect = GetThumbRect();
            if (thumbRect != Rectangle.Empty)
            {
                using (SolidBrush brush = new SolidBrush(_isDragging ? UIStyle.Primary : UIStyle.TextTertiary))
                {
                    g.FillRoundedRectangle(brush, thumbRect, thumbRect.Width / 2);
                }
            }
        }
    }

    public static class GraphicsExtensions
    {
        public static void FillRoundedRectangle(this Graphics g, Brush brush, Rectangle rect, int radius)
        {
            using (GraphicsPath path = new GraphicsPath())
            {
                int d = radius * 2;
                path.AddArc(rect.X, rect.Y, d, d, 180, 90);
                path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
                path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
                path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
                path.CloseFigure();
                g.FillPath(brush, path);
            }
        }
    }
}
