using System;
using System.Drawing;
using System.Windows.Forms;
using Data.Entities;
using System.Threading.Tasks;

namespace RealView.Views
{
    public class WorkoutSummaryView : BaseView
    {
        private WorkoutSession _session;
        private TextBox _titleTxt;
        private TextBox _notesTxt;
        private Button _saveBtn;

        public WorkoutSummaryView(WorkoutSession session)
        {
            _session = session;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.BackColor = Color.FromArgb(245, 247, 251);

            var card = new Panel
            {
                Size = new Size(500, 500),
                BackColor = Color.White,
                Padding = new Padding(40)
            };

            this.Resize += (s, e) => {
                card.Location = new Point((this.Width - card.Width) / 2, (this.Height - card.Height) / 2);
            };

            var layout = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false
            };

            var titleLabel = new Label
            {
                Text = "Workout Summary",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 30)
            };

            var nameLabel = new Label { Text = "Workout Title", Font = new Font("Segoe UI", 10), AutoSize = true, Margin = new Padding(0, 0, 0, 5) };
            _titleTxt = new TextBox 
            { 
                Width = 420, 
                Font = new Font("Segoe UI", 12), 
                Text = $"{DateTime.Now.DayOfWeek} Workout",
                Margin = new Padding(0, 0, 0, 20) 
            };

            var notesLabel = new Label { Text = "Notes", Font = new Font("Segoe UI", 10), AutoSize = true, Margin = new Padding(0, 0, 0, 5) };
            _notesTxt = new TextBox 
            { 
                Width = 420, 
                Height = 100, 
                Multiline = true, 
                Font = new Font("Segoe UI", 12),
                Margin = new Padding(0, 0, 0, 30) 
            };

            _saveBtn = new Button
            {
                Text = "FINISH & SAVE",
                Size = new Size(420, 50),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            _saveBtn.FlatAppearance.BorderSize = 0;
            _saveBtn.Click += SaveBtn_Click;

            layout.Controls.Add(titleLabel);
            layout.Controls.Add(nameLabel);
            layout.Controls.Add(_titleTxt);
            layout.Controls.Add(notesLabel);
            layout.Controls.Add(_notesTxt);
            layout.Controls.Add(_saveBtn);

            card.Controls.Add(layout);
            this.Controls.Add(card);

            card.Paint += (s, e) => {
                ControlPaint.DrawBorder(e.Graphics, card.ClientRectangle, Color.LightGray, ButtonBorderStyle.Solid);
            };
        }

        private async void SaveBtn_Click(object? sender, EventArgs e)
        {
            try
            {
                _saveBtn.Enabled = false;
                await AppRuntime.WorkoutSession.UpdateWorkoutSession(_session.Id, DateTime.Now, _titleTxt.Text, _notesTxt.Text);
                
                AppRuntime.WorkoutState.FinishWorkout();
                AppRuntime.Navigation.NavigateTo<DashboardView>();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                _saveBtn.Enabled = true;
            }
        }
    }
}
