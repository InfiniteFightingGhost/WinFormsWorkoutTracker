using System;
using System.Drawing;
using System.Windows.Forms;
using Data.Entities;
using System.Collections.Generic;
using System.Linq;

namespace WorkoutTracker.View.Views
{
    public class ActiveWorkoutView : BaseView
    {
        private FlowLayoutPanel _mainLayout;
        private Label _timerLabel;
        private System.Windows.Forms.Timer _workoutTimer;
        private DateTime _startTime;
        private Button _addExerciseButton;
        private Button _finishWorkoutButton;
        private FlowLayoutPanel _exercisesPanel;

        public ActiveWorkoutView()
        {
            InitializeComponent();
            _workoutTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            _workoutTimer.Tick += WorkoutTimer_Tick;
        }

        private void InitializeComponent()
        {
            _mainLayout = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                Padding = new Padding(20),
                AutoScroll = true,
                WrapContents = false
            };

            var headerPanel = new Panel { Size = new Size(400, 60), Margin = new Padding(0, 0, 0, 20) };
            
            _timerLabel = new Label
            {
                Text = "00:00:00",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(0, 0)
            };
            headerPanel.Controls.Add(_timerLabel);

            _finishWorkoutButton = new Button
            {
                Text = "FINISH",
                BackColor = Color.ForestGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(100, 40),
                Location = new Point(280, 0)
            };
            _finishWorkoutButton.Click += FinishWorkoutButton_Click;
            headerPanel.Controls.Add(_finishWorkoutButton);

            _addExerciseButton = new Button
            {
                Text = "+ ADD EXERCISE",
                Size = new Size(380, 50),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Margin = new Padding(0, 0, 0, 20)
            };
            _addExerciseButton.Click += AddExerciseButton_Click;

            _exercisesPanel = new FlowLayoutPanel
            {
                Width = 400,
                AutoSize = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false
            };

            _mainLayout.Controls.Add(headerPanel);
            _mainLayout.Controls.Add(_addExerciseButton);
            _mainLayout.Controls.Add(_exercisesPanel);

            this.Controls.Add(_mainLayout);
        }

        public override void OnNavigatedTo()
        {
            var session = AppRuntime.WorkoutState.ActiveSession;
            if (session != null)
            {
                _startTime = session.Start;
                _workoutTimer.Start();
                LoadExercises();
            }
        }

        private void WorkoutTimer_Tick(object sender, EventArgs e)
        {
            var duration = DateTime.Now - _startTime;
            _timerLabel.Text = duration.ToString(@"hh\:mm\:ss");
        }

        private void LoadExercises()
        {
            _exercisesPanel.Controls.Clear();
            var session = AppRuntime.WorkoutState.ActiveSession;
            if (session.Exercises != null)
            {
                foreach (var exercise in session.Exercises)
                {
                    var card = new Controls.ExerciseCard(exercise);
                    _exercisesPanel.Controls.Add(card);
                }
            }
        }

        private async void AddExerciseButton_Click(object sender, EventArgs e)
        {
            using (var dialog = new Controls.ExerciseSelectionDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var session = AppRuntime.WorkoutState.ActiveSession;
                        var workoutExercise = await AppRuntime.WorkoutExercise.AddExerciseToWorkoutAsync(session.Id, dialog.SelectedExercise.Id);
                        
                        // We need the Exercise object to be loaded for the card to display its name
                        workoutExercise.Exercise = dialog.SelectedExercise;
                        
                        var card = new Controls.ExerciseCard(workoutExercise);
                        _exercisesPanel.Controls.Add(card);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error adding exercise: {ex.Message}");
                    }
                }
            }
        }

        private async void FinishWorkoutButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to finish this workout?", "Finish Workout", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    var session = AppRuntime.WorkoutState.ActiveSession;
                    await AppRuntime.WorkoutSession.UpdateWorkoutSession(session.Id, DateTime.Now, "Great workout!", null);
                    await AppRuntime.WorkoutSession.UpdateWorkoutSessionStatus(session.Id);
                    
                    _workoutTimer.Stop();
                    AppRuntime.WorkoutState.FinishWorkout();
                    AppRuntime.Navigation.NavigateTo<DashboardView>();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error finishing workout: {ex.Message}");
                }
            }
        }
    }
}
