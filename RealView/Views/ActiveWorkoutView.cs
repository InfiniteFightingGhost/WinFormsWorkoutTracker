using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using Data.Entities;
using RealView.Controls;

namespace RealView.Views
{
    public class ActiveWorkoutView : BaseView
    {
        private FlowLayoutPanel _mainLayout = null!;
        private Label _timerLabel = null!;
        private System.Windows.Forms.Timer _timer = null!;
        private FlowLayoutPanel _exercisesPanel = null!;
        private Button _addExerciseBtn = null!;
        private Button _finishBtn = null!;

        public ActiveWorkoutView()
        {
            InitializeComponent();
            _timer = new System.Windows.Forms.Timer { Interval = 1000 };
            _timer.Tick += (s, e) => UpdateTimer();
        }

        private void InitializeComponent()
        {
            _mainLayout = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                Padding = new Padding(40),
                AutoScroll = true,
                WrapContents = false
            };

            var header = new Panel { Width = 800, Height = 60, Margin = new Padding(0, 0, 0, 30) };
            
            _timerLabel = new Label
            {
                Text = "00:00:00",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(0, 0)
            };
            header.Controls.Add(_timerLabel);

            var reorderBtn = new Button
            {
                Text = "REORDER",
                BackColor = Color.FromArgb(0, 123, 255), // Blue Accent highlight,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Size = new Size(150, 50),
                Location = new Point(430, 0) // Fits well into the 800px header container
            };
            reorderBtn.Click += ReorderBtn_Click;

            header.Controls.Add(reorderBtn);
            _finishBtn = new Button
            {
                Text = "FINISH",
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Size = new Size(200, 50),
                Location = new Point(600, 0)
            };
            _finishBtn.Click += FinishBtn_Click;
            header.Controls.Add(_finishBtn);

            _exercisesPanel = new FlowLayoutPanel
            {
                Width = 800,
                AutoSize = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false
            };

            _addExerciseBtn = new Button
            {
                Text = "+ ADD EXERCISE",
                Size = new Size(800, 50),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                BackColor = Color.FromArgb(240, 240, 240),
                Margin = new Padding(0, 20, 0, 40)
            };
            _addExerciseBtn.Click += AddExerciseBtn_Click;

            _mainLayout.Controls.Add(header);
            _mainLayout.Controls.Add(_exercisesPanel);
            _mainLayout.Controls.Add(_addExerciseBtn);

            var discardBtn = new Button
            {
                Text = "DISCARD WORKOUT",
                Size = new Size(800, 50),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.IndianRed,
                BackColor = Color.FromArgb(255, 235, 235),
                Margin = new Padding(0, 0, 0, 100)
            };
            discardBtn.FlatAppearance.BorderColor = Color.IndianRed;
            discardBtn.Click += async (s, e) => {
                if (MessageBox.Show("Are you sure you want to discard this workout? This action cannot be undone.", "Discard", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    try
                    {
                        var session = AppRuntime.WorkoutState.ActiveSession;
                        if (session != null)
                        {
                            await AppRuntime.WorkoutSession.DeleteSessionAsync(session.Id);
                            _timer.Stop();
                            AppRuntime.WorkoutState.FinishWorkout(); // This clears the active session from state
                            AppRuntime.Navigation.NavigateTo<DashboardView>();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            };
            _mainLayout.Controls.Add(discardBtn);

            this.Controls.Add(_mainLayout);
        }

        public override void OnNavigatedTo()
        {
            var session = AppRuntime.WorkoutState.ActiveSession;
            if (session != null)
            {
                _timer.Start();
                LoadExercises();
            }
        }
        private void ReorderBtn_Click(object? sender, EventArgs e)
        {
            var session = AppRuntime.WorkoutState.ActiveSession;
            if (session?.Exercises == null) return;

            // Open the mini form populated with active exercise elements
            using (var dialog = new WorkoutReorderDialog(session.Exercises.OrderBy(ex => ex.OrderIndex).ToList()))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    // Re-render the main view cards layout structure to follow new ordered parameters
                    LoadExercises();
                }
            }
        }
        private void UpdateTimer()
        {
            var session = AppRuntime.WorkoutState.ActiveSession;
            if (session != null)
            {
                var duration = DateTime.Now - session.Start;
                _timerLabel.Text = duration.ToString(@"hh\:mm\:ss");
            }
        }
        private void LoadExercises()
        {
            _exercisesPanel.Controls.Clear();
            var session = AppRuntime.WorkoutState.ActiveSession;
            if (session?.Exercises != null)
            {
                // Enforce sequence logic sorting rules before binding full layout controls
                foreach (var ex in session.Exercises.OrderBy(e => e.OrderIndex))
                {
                    _exercisesPanel.Controls.Add(new ExerciseCard(ex));
                }
            }
        }
        private async void AddExerciseBtn_Click(object? sender, EventArgs e)
        {
            using (var dialog = new ExerciseSelectionDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK && dialog.SelectedExercise != null)
                {
                    try
                    {
                        var session = AppRuntime.WorkoutState.ActiveSession;
                        if (session == null) return;

                        var workoutEx = await AppRuntime.WorkoutExercise.AddExerciseToWorkoutAsync(session.Id, dialog.SelectedExercise.Id);
                        
                        // Manually attach exercise for display
                        workoutEx.Exercise = dialog.SelectedExercise;
                        
                        // Sync with in-memory model to avoid duplicates on refresh
                        session.Exercises ??= new List<WorkoutExercise>();
                        session.Exercises.Add(workoutEx);

                        // Clear and reload to ensure order and state are consistent
                        LoadExercises();
                    }
                    catch (Exception ex)
                    {
                        AppRuntime.Toasts.Show(ex.Message, true);
                    }
                }
            }
        }

        private void FinishBtn_Click(object? sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to finish this workout?", "Finish", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                var session = AppRuntime.WorkoutState.ActiveSession;
                if (session != null)
                {
                    _timer.Stop();
                    AppRuntime.Navigation.NavigateTo<FinishWorkoutView>(session);
                }
            }
        }
    }
}
