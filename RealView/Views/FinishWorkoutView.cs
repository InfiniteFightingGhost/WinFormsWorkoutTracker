using System;
using System.Drawing;
using System.Windows.Forms;
using Data.Entities;
using RealView.Controls;

namespace RealView.Views
{
    public class FinishWorkoutView : BaseView
    {
        private WorkoutSession _session;
        private TextBox _titleTxt = null!;
        private TextBox _notesTxt = null!;
        private string? _photoPath;
        private PictureBox _photoPreview = null!;
        private Panel _card = null!;

        public FinishWorkoutView(WorkoutSession session)
        {
            _session = session;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.BackColor = UIStyle.Background;

            _card = new Panel
            {
                Size = new Size(500, 650),
                BackColor = Color.White,
                Padding = new Padding(30)
            };

            this.Resize += (s, e) => {
                _card.Location = new Point((this.Width - _card.Width) / 2, (this.Height - _card.Height) / 2);
            };

            var layout = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false
            };

            var titleLbl = new Label { Text = "Finish Workout", Font = UIStyle.Header, AutoSize = true, Margin = new Padding(0, 0, 0, 20) };
            
            var nameLbl = new Label { Text = "WORKOUT TITLE", Font = UIStyle.CaptionBold, ForeColor = UIStyle.TextSecondary, Margin = new Padding(0, 10, 0, 5) };
            _titleTxt = new TextBox { Width = 440, Font = UIStyle.Body, Text = _session.Title ?? "Morning Workout" };

            var notesLbl = new Label { Text = "NOTES", Font = UIStyle.CaptionBold, ForeColor = UIStyle.TextSecondary, Margin = new Padding(0, 20, 0, 5) };
            _notesTxt = new TextBox { Width = 440, Height = 100, Multiline = true, Font = UIStyle.Body, Text = _session.Notes };

            var photoLbl = new Label { Text = "ATTACH PHOTO", Font = UIStyle.CaptionBold, ForeColor = UIStyle.TextSecondary, Margin = new Padding(0, 20, 0, 5) };
            _photoPreview = new PictureBox { Size = new Size(150, 150), SizeMode = PictureBoxSizeMode.Zoom, BackColor = Color.FromArgb(245, 245, 245), BorderStyle = BorderStyle.FixedSingle };
            
            var addPhotoBtn = new ModernButton { Text = "📷 Add Photo", Size = new Size(150, 40), NormalColor = UIStyle.Primary, HoverColor = UIStyle.PrimaryHover, ForeColor = Color.White, BorderRadius = 8, Margin = new Padding(0, 10, 0, 0) };
            addPhotoBtn.Click += AddPhotoBtn_Click;

            var finishBtn = new ModernButton { Text = "FINALIZE WORKOUT", Size = new Size(440, 50), Margin = new Padding(0, 40, 0, 0), BorderRadius = 10, NormalColor = UIStyle.Success };
            finishBtn.Click += FinishBtn_Click;

            layout.Controls.Add(titleLbl);
            layout.Controls.Add(nameLbl);
            layout.Controls.Add(_titleTxt);
            layout.Controls.Add(notesLbl);
            layout.Controls.Add(_notesTxt);
            layout.Controls.Add(photoLbl);
            layout.Controls.Add(_photoPreview);
            layout.Controls.Add(addPhotoBtn);
            layout.Controls.Add(finishBtn);

            _card.Controls.Add(layout);
            this.Controls.Add(_card);
        }

        private void AddPhotoBtn_Click(object? sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog { Filter = "Image Files|*.jpg;*.jpeg;*.png" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    _photoPath = Services.PhotoService.SavePhoto(ofd.FileName, "Workouts");
                    _photoPreview.Image = Services.PhotoService.LoadPhoto(_photoPath);
                }
            }
        }

        private async void FinishBtn_Click(object? sender, EventArgs e)
        {
            try
            {
                await AppRuntime.WorkoutSession.UpdateWorkoutSession(_session.Id, DateTime.Now, _titleTxt.Text, _notesTxt.Text, _photoPath);
                AppRuntime.WorkoutState.FinishWorkout();
                AppRuntime.Navigation.NavigateTo<WorkoutSummaryView>(_session);
            }
            catch (Exception ex)
            {
                AppRuntime.Toasts.Show(ex.Message, true);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            DrawCard(e.Graphics, _card.Bounds);
        }
    }
}
