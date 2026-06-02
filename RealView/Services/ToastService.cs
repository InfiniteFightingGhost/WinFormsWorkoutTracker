using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WorkoutTracker.RealView.Services
{
    public enum ToastType
    {
        Info,
        Success,
        Error
    }

    public class ToastService
    {
        public void Show(string message, ToastType type = ToastType.Info)
        {
            var toast = new ToastForm(message, type);
            toast.Show();
        }

        // Maintain backward compatibility for boolean parameter
        public void Show(string message, bool isError)
        {
            Show(message, isError ? ToastType.Error : ToastType.Info);
        }
    }

    internal class ToastForm : Form
    {
        private System.Windows.Forms.Timer _lifeTimer;
        private System.Windows.Forms.Timer _animationTimer;
        private float _opacity = 0;
        private int _targetY;
        private int _startY;

        public ToastForm(string message, ToastType type)
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.Size = new Size(300, 60);
            
            this.BackColor = type switch
            {
                ToastType.Success => UIStyle.Success,
                ToastType.Error => UIStyle.Danger,
                _ => UIStyle.Primary
            };

            this.Opacity = 0;

            var lbl = new Label
            {
                Text = message,
                ForeColor = UIStyle.TextOnPrimary,
                Font = UIStyle.BodySemibold,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Padding = new Padding(10)
            };
            this.Controls.Add(lbl);

            // Rounded corners
            this.Paint += (s, e) =>
            {
                using (var path = new GraphicsPath())
                {
                    int r = 15;
                    path.AddArc(0, 0, r, r, 180, 90);
                    path.AddArc(Width - r, 0, r, r, 270, 90);
                    path.AddArc(Width - r, Height - r, r, r, 0, 90);
                    path.AddArc(0, Height - r, r, r, 90, 90);
                    path.CloseFigure();
                    this.Region = new Region(path);
                }
            };

            // Positioning
            Rectangle screen = Screen.PrimaryScreen.WorkingArea;
            _targetY = screen.Bottom - Height - 20;
            _startY = screen.Bottom;
            this.Location = new Point(screen.Right - Width - 20, _startY);

            _lifeTimer = new System.Windows.Forms.Timer { Interval = 3000 };
            _lifeTimer.Tick += (s, e) => FadeOut();

            _animationTimer = new System.Windows.Forms.Timer { Interval = 15 };
            _animationTimer.Tick += AnimationTick;
            _animationTimer.Start();
        }

        private void AnimationTick(object? sender, EventArgs e)
        {
            if (_opacity < 1)
            {
                _opacity += 0.1f;
                if (_opacity > 1) _opacity = 1;
                this.Opacity = _opacity;

                // Slide up
                int currentY = this.Top;
                int step = (currentY - _targetY) / 2;
                if (step < 1) step = 1;
                this.Top -= step;

                if (this.Top <= _targetY)
                {
                    this.Top = _targetY;
                    _lifeTimer.Start();
                }
            }
        }

        private void FadeOut()
        {
            _lifeTimer.Stop();
            _animationTimer.Tick -= AnimationTick;
            _animationTimer.Tick += (s, e) =>
            {
                _opacity -= 0.1f;
                if (_opacity <= 0)
                {
                    _animationTimer.Stop();
                    this.Close();
                }
                this.Opacity = _opacity;
            };
            _animationTimer.Start();
        }
    }
}
