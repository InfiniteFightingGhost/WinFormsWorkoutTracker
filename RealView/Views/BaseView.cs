using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace RealView.Views
{
    public class BaseView : UserControl
    {
        public virtual void OnNavigatedTo() { }
        
        protected void ShowLoading(bool show)
        {
            if (this.ParentForm is MainForm shell)
            {
                // shell.SetLoading(show); // Implementation in MainForm
            }
        }

        protected void DrawCard(Graphics g, Rectangle rect)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (GraphicsPath path = new GraphicsPath())
            {
                int r = UIStyle.BorderRadius;
                path.AddArc(rect.X, rect.Y, r * 2, r * 2, 180, 90);
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
        }
    }
}
