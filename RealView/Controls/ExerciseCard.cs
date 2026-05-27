using System;
using System.Drawing;
using System.Windows.Forms;
using Data.Entities;
using System.Collections.Generic;

namespace RealView.Controls
{
    public class ExerciseCard : UserControl
    {
        private WorkoutExercise _workoutExercise;
        private Label _nameLabel;
        private Label _restTimerLabel = null!;
        private FlowLayoutPanel _setsPanel;
        private Button _addSetButton;
        private FlowLayoutPanel _mainLayout;
        private System.Windows.Forms.Timer? _internalRestTimer;
        private int _restSecondsRemaining;

        public event EventHandler? CardSelected;
        public WorkoutExercise WorkoutExerciseModel => _workoutExercise;

        private bool _isSelected;
        private bool _isReadOnly;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                this.BackColor = _isSelected ? Color.FromArgb(235, 245, 255) : Color.White; // Light blue highlight
                this.Invalidate();
            }
        }
        public ExerciseCard(WorkoutExercise workoutExercise, bool isReadOnly = false)
        {
            _workoutExercise = workoutExercise;
            _isReadOnly = isReadOnly;
            InitializeComponent();

            if (!isReadOnly)
            {
                _internalRestTimer = new System.Windows.Forms.Timer { Interval = 1000 };
                _internalRestTimer.Tick += InternalRestTimer_Tick;
                AppRuntime.WorkoutState.SetCompleted += WorkoutState_SetCompleted;
            }
        }

        private void WorkoutState_SetCompleted(object? sender, ExerciseSet set)
        {
            // Only start timer if it was a set from THIS exercise
            if (set.ExerciseId == _workoutExercise.ExerciseId)
            {
                _restSecondsRemaining = 90; // Default 90s
                UpdateRestTimerDisplay();
                _restTimerLabel.Visible = true;
                _internalRestTimer?.Start();
            }
        }

        private void InternalRestTimer_Tick(object? sender, EventArgs e)
        {
            _restSecondsRemaining--;
            if (_restSecondsRemaining <= 0)
            {
                _internalRestTimer?.Stop();
                _restTimerLabel.Visible = false;
            }
            else
            {
                UpdateRestTimerDisplay();
            }
        }

        private void UpdateRestTimerDisplay()
        {
            var ts = TimeSpan.FromSeconds(_restSecondsRemaining);
            _restTimerLabel.Text = $"Rest: {ts.ToString(@"mm\:ss")}";
        }

        private void InitializeComponent()
        {
            this.BackColor = Color.White;
            this.Padding = new Padding(15);
            this.Margin = new Padding(0, 0, 0, 15);
            this.Width = 460;
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            _mainLayout = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Location = new Point(15, 15)
            };

            var headerPanel = new Panel { Width = 430, Height = 40 };

            _nameLabel = new Label
            {
                Text = _workoutExercise.Exercise?.Name ?? "Exercise",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(0, 5)
            };

            _restTimerLabel = new Label
            {
                Text = "Rest: 01:30",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 120, 215), // Professional Blue
                AutoSize = true,
                Visible = false,
                Location = new Point(200, 10) // Positioned after name
            };

            // Dynamic positioning based on name length
            _nameLabel.SizeChanged += (s, e) => {
                _restTimerLabel.Left = _nameLabel.Right + 10;
            };

            var optionsBtn = new Button
            {
                Text = "?",
                Location = new Point(390, 0), // Shifted Y from 5 to 0 to account for larger font height
                Size = new Size(30, 35),      // Made it slightly taller
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 12, FontStyle.Bold), // Increased from 14 to 18
                Cursor = Cursors.Hand
            };
            optionsBtn.FlatAppearance.BorderSize = 0;

            // Context Menu Setup
            var contextMenu = new ContextMenuStrip();
            var viewItem = contextMenu.Items.Add("View Exercise");

            viewItem.Click += (s, e) => {
                if (_workoutExercise.Exercise != null)
                {
                    AppRuntime.Navigation.NavigateTo<Views.ExerciseDetailView>(_workoutExercise.Exercise);
                }
            };

            if (!_isReadOnly)
            {
                var switchItem = contextMenu.Items.Add("Switch Exercise");
                var removeItem = contextMenu.Items.Add("Remove");

                switchItem.Click += async (s, e) => await SwitchExerciseAsync();
                removeItem.Click += async (s, e) => {
                    if (MessageBox.Show("Remove this exercise from workout?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        await AppRuntime.WorkoutExercise.RemoveExerciseFromWorkoutAsync(_workoutExercise.WorkoutId, _workoutExercise.ExerciseId);
                        this.Parent?.Controls.Remove(this);
                    }
                };
            }

            optionsBtn.Click += (s, e) => contextMenu.Show(optionsBtn, new Point(0, optionsBtn.Height));

            headerPanel.Controls.Add(_nameLabel);
            headerPanel.Controls.Add(_restTimerLabel);
            headerPanel.Controls.Add(optionsBtn);

            _setsPanel = new FlowLayoutPanel
            {
                Width = 430,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Margin = new Padding(0, 10, 0, 10)
            };

            _addSetButton = new Button
            {
                Text = "+ ADD SET",
                Size = new Size(430, 35),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(240, 242, 245),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Margin = new Padding(0),
                Visible = !_isReadOnly
            };
            _addSetButton.FlatAppearance.BorderSize = 0;
            _addSetButton.Click += AddSetButton_Click;

            _mainLayout.Controls.Add(headerPanel);
            _mainLayout.Controls.Add(_setsPanel);
            _mainLayout.Controls.Add(_addSetButton);

            this.Controls.Add(_mainLayout);

            // Add existing sets
            if (_workoutExercise.Sets != null)
            {
                foreach (var set in _workoutExercise.Sets)
                {
                    AddSetRow(set);
                }
            }

            UpdateSetNumbering();

            this.Paint += (s, e) => {
                ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle, Color.FromArgb(220, 220, 220), ButtonBorderStyle.Solid);
            };
            this.Click += (s, e) => CardSelected?.Invoke(this, EventArgs.Empty);
            headerPanel.Click += (s, e) => CardSelected?.Invoke(this, EventArgs.Empty);
            _nameLabel.Click += (s, e) => CardSelected?.Invoke(this, EventArgs.Empty);
            
            if (!_isReadOnly)
            {
                headerPanel.MouseDown += InitiateDrag;
                _nameLabel.MouseDown += InitiateDrag;
            }
        }
        private void InitiateDrag(object? sender, MouseEventArgs e)
        {
            CardSelected?.Invoke(this, EventArgs.Empty);
            if (e.Button == MouseButtons.Left)
            {
                this.DoDragDrop(this, DragDropEffects.Move);
            }
        }
        private async Task SwitchExerciseAsync()
        {
            using (var dialog = new ExerciseSelectionDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK && dialog.SelectedExercise != null)
                {
                    // Assuming you have an Update method to switch the exercise ID in DB
                    _workoutExercise.ExerciseId = dialog.SelectedExercise.Id;
                    _workoutExercise.Exercise = dialog.SelectedExercise;
                    //await AppRuntime.WorkoutExercise.UpdateExerciseAsync(_workoutExercise); 

                    _nameLabel.Text = _workoutExercise.Exercise.Name;
                }
            }
        }
        private void AddSetRow(ExerciseSet set)
        {
            var row = new SetRow(set, this, _isReadOnly);
            _setsPanel.Controls.Add(row);
            UpdateSetNumbering();
        }

        // ?? Made PUBLIC so SetRow can trigger re-numbering
        public void UpdateSetNumbering()
        {
            int regularSetIndex = 1;
            foreach (Control control in _setsPanel.Controls)
            {
                if (control is SetRow row)
                {
                    row.RefreshDisplay(ref regularSetIndex);
                }
            }
        }

        // ?? New standard public method for handling deletions
        public async Task RemoveSetAsync(SetRow row, ExerciseSet set)
        {
            try
            {
                await AppRuntime.WorkoutSet.DeleteExerciseSetAsync(set.Id);
                _setsPanel.Controls.Remove(row);
                _workoutExercise.Sets?.Remove(set);
                row.Dispose(); // Clean up memory natively
                UpdateSetNumbering();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting set: {ex.Message}");
            }
        }

        private async void AddSetButton_Click(object? sender, EventArgs e)
        {
            try
            {
                var newSet = await AppRuntime.WorkoutSet.CreateExerciseSetAsync(_workoutExercise.WorkoutId, _workoutExercise.ExerciseId);
                _workoutExercise.Sets ??= new List<ExerciseSet>();
                _workoutExercise.Sets.Add(newSet);
                AddSetRow(newSet);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding set: {ex.Message}");
            }
        }
    }
}
