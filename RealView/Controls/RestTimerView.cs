using System;
using System.Drawing;
using System.Windows.Forms;

namespace WorkoutTracker.RealView.Controls
{
    public class RestTimerView : UserControl
    {
        private int _secondsRemaining;
        private System.Windows.Forms.Timer _timer;
        private Label _timerLabel;
        private Button _skipBtn;
        private Button _plus30Btn;

        public event EventHandler? Finished;

        public RestTimerView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(300, 150);
            this.BackColor = UIStyle.Sidebar;
            
            _timerLabel = new Label
            {
                Text = "00:00",
                ForeColor = UIStyle.TextOnSidebar,
                Font = UIStyle.Header,
                Size = new Size(300, 60),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, 10)
            };

            _skipBtn = new Button
            {
                Text = "SKIP",
                Size = new Size(100, 40),
                Location = new Point(40, 80),
                FlatStyle = FlatStyle.Flat,
                ForeColor = UIStyle.TextOnSidebar,
                Font = UIStyle.CaptionBold,
                Cursor = Cursors.Hand
            };
            _skipBtn.FlatAppearance.BorderColor = UIStyle.Border;
            _skipBtn.Click += (s, e) => Stop();

            _plus30Btn = new Button
            {
                Text = "+30s",
                Size = new Size(100, 40),
                Location = new Point(160, 80),
                FlatStyle = FlatStyle.Flat,
                ForeColor = UIStyle.TextOnSidebar,
                Font = UIStyle.CaptionBold,
                Cursor = Cursors.Hand
            };
            _plus30Btn.FlatAppearance.BorderColor = UIStyle.Border;
            _plus30Btn.Click += (s, e) => { _secondsRemaining += 30; UpdateDisplay(); };

            this.Controls.Add(_timerLabel);
            this.Controls.Add(_skipBtn);
            this.Controls.Add(_plus30Btn);

            _timer = new System.Windows.Forms.Timer { Interval = 1000 };
            _timer.Tick += (s, e) => {
                _secondsRemaining--;
                if (_secondsRemaining <= 0) Stop();
                else UpdateDisplay();
            };
        }

        public void Start(int seconds)
        {
            _secondsRemaining = seconds;
            UpdateDisplay();
            _timer.Start();
            this.Visible = true;
            this.BringToFront();
        }

        private void Stop()
        {
            _timer.Stop();
            this.Visible = false;
            Finished?.Invoke(this, EventArgs.Empty);
        }

        private void UpdateDisplay()
        {
            var ts = TimeSpan.FromSeconds(_secondsRemaining);
            _timerLabel.Text = ts.ToString(@"mm\:ss");
        }
    }
}
