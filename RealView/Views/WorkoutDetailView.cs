using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using Data.Entities;
using RealView.Controls;

namespace RealView.Views
{
    public class WorkoutDetailView : BaseView
    {
        private WorkoutSession _session;
        private FlowLayoutPanel _mainLayout = null!;
        private FlowLayoutPanel _exercisesPanel = null!;

        public WorkoutDetailView(WorkoutSession session)
        {
            _session = session;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.BackColor = Color.FromArgb(245, 247, 251);

            _mainLayout = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                Padding = new Padding(40),
                AutoScroll = true,
                WrapContents = false
            };

            var backBtn = new Button
            {
                Text = "← BACK TO HISTORY",
                Size = new Size(200, 35),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Margin = new Padding(0, 0, 0, 20),
                Cursor = Cursors.Hand
            };
            backBtn.FlatAppearance.BorderSize = 0;
            backBtn.Click += (s, e) => AppRuntime.Navigation.NavigateTo<HistoryView>();

            var title = new Label
            {
                Text = $"Workout on {_session.Start:f}",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 10)
            };

            string durationStr;
            if (_session.End.HasValue)
            {
                var ts = _session.End.Value - _session.Start;
                durationStr = ts.TotalHours >= 1 ? ts.ToString(@"hh\:mm\:ss") : ts.ToString(@"mm\:ss");
            }
            else
            {
                durationStr = "N/A";
            }

            var infoLabel = new Label
            {
                Text = $"Duration: {durationStr}",
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.DimGray,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 30)
            };

            _exercisesPanel = new FlowLayoutPanel
            {
                Width = 800,
                AutoSize = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false
            };

            _mainLayout.Controls.Add(backBtn);
            _mainLayout.Controls.Add(title);
            _mainLayout.Controls.Add(infoLabel);
            _mainLayout.Controls.Add(_exercisesPanel);

            this.Controls.Add(_mainLayout);

            LoadExercises();
        }

        private void LoadExercises()
        {
            _exercisesPanel.Controls.Clear();
            if (_session.Exercises != null)
            {
                foreach (var ex in _session.Exercises.OrderBy(e => e.OrderIndex))
                {
                    _exercisesPanel.Controls.Add(new ExerciseCard(ex, true));
                }
            }
        }
    }
}
