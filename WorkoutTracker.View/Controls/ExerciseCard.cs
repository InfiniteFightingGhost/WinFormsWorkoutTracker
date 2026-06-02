using System;
using System.Drawing;
using System.Windows.Forms;
using WorkoutTracker.Data.Entities;
using System.Collections.Generic;

namespace WorkoutTracker.View.Controls
{
    public class ExerciseCard : UserControl
    {
        private WorkoutExercise _workoutExercise;
        private Label _nameLabel;
        private FlowLayoutPanel _setsPanel;
        private Button _addSetButton;

        public ExerciseCard(WorkoutExercise workoutExercise)
        {
            _workoutExercise = workoutExercise;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(380, 150);
            this.BackColor = Color.White;
            this.Padding = new Padding(10);
            this.Margin = new Padding(0, 0, 0, 15);
            this.AutoSize = true;
            this.MinimumSize = new Size(380, 100);

            _nameLabel = new Label
            {
                Text = _workoutExercise.Exercise?.Name ?? "Exercise",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(10, 10)
            };

            var removeBtn = new Button { 
                Text = "Remove", 
                Location = new Point(280, 10), 
                Size = new Size(80, 25),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.Gray
            };
            removeBtn.Click += async (s, e) => {
                if (MessageBox.Show("Remove this exercise from workout?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes) {
                    await AppRuntime.WorkoutExercise.RemoveExerciseFromWorkoutAsync(_workoutExercise.WorkoutId, _workoutExercise.ExerciseId);
                    this.Parent.Controls.Remove(this);
                }
            };

            _setsPanel = new FlowLayoutPanel
            {
                Location = new Point(10, 40),
                Width = 360,
                AutoSize = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false
            };

            _addSetButton = new Button
            {
                Text = "+ ADD SET",
                Size = new Size(360, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(240, 240, 240)
            };
            _addSetButton.Click += AddSetButton_Click;

            this.Controls.Add(_nameLabel);
            this.Controls.Add(_setsPanel);
            
            // Add existing sets
            if (_workoutExercise.Sets != null)
            {
                foreach (var set in _workoutExercise.Sets)
                {
                    AddSetRow(set);
                }
            }
            
            // Positioning the add button dynamically is tricky with AutoSize, 
            // but FlowLayoutPanel inside this control will handle it if we add it there or after.
            // Let's just put it in a separate layout or handle it manually.
            
            var bottomLayout = new FlowLayoutPanel {
                AutoSize = true,
                FlowDirection = FlowDirection.TopDown,
                Width = 360,
                Location = new Point(10, 40) // Will be updated by layout
            };
            bottomLayout.Controls.Add(_setsPanel);
            bottomLayout.Controls.Add(_addSetButton);
            this.Controls.Add(bottomLayout);

            this.Paint += (s, e) => {
                ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle, Color.LightGray, ButtonBorderStyle.Solid);
            };
        }

        private void AddSetRow(ExerciseSet set)
        {
            var row = new SetRow(set);
            _setsPanel.Controls.Add(row);
        }

        private async void AddSetButton_Click(object sender, EventArgs e)
        {
            try
            {
                //var newSet = await AppRuntime.WorkoutSet.CreateExerciseSetAsync(_workoutExercise.WorkoutId);
                //AddSetRow(newSet);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding set: {ex.Message}");
            }
        }
    }

    public class SetRow : UserControl
    {
        private ExerciseSet _set;
        public SetRow(ExerciseSet set)
        {
            _set = set;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(350, 40);
            
            var lblSet = new Label { Text = $"Set {_set.OrderIndex + 1}", Location = new Point(0, 10), Width = 50 };
            
            var txtWeight = new TextBox { Text = _set.Weight.ToString(), Location = new Point(60, 8), Width = 60 };
            var lblKg = new Label { Text = "kg", Location = new Point(125, 10), Width = 30 };

            var txtReps = new TextBox { Text = _set.Repetitions.ToString(), Location = new Point(160, 8), Width = 60 };
            var lblReps = new Label { Text = "reps", Location = new Point(225, 10), Width = 40 };

            var chkDone = new CheckBox { Checked = _set.Completed, Location = new Point(280, 8), Width = 50, Text = "Done" };

            this.Controls.Add(lblSet);
            this.Controls.Add(txtWeight);
            this.Controls.Add(lblKg);
            this.Controls.Add(txtReps);
            this.Controls.Add(lblReps);
            this.Controls.Add(chkDone);
            
            // TODO: Add save logic on change
        }
    }
}
