using System;
using System.Drawing;
using System.Windows.Forms;
using Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RealView.Controls
{
    public class ExerciseSelectionDialog : Form
    {
        private ListBox _listBox;
        private ComboBox _muscleGroupFilter;
        private Button _selectButton;
        private TextBox _searchBox;
        public Exercise? SelectedExercise { get; private set; }

        public ExerciseSelectionDialog()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Select Exercise";
            this.Size = new Size(400, 600);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.White;

            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                Padding = new Padding(20)
            };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));

            _muscleGroupFilter = new ComboBox
            {
                Dock = DockStyle.Fill,
                DisplayMember = "Name",
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10)
            };
            _muscleGroupFilter.SelectedIndexChanged += async (s, e) => await LoadExercises();

            _searchBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                PlaceholderText = "Search exercises..."
            };
            _searchBox.TextChanged += async (s, e) => await LoadExercises();

            _listBox = new ListBox
            {
                Dock = DockStyle.Fill,
                DisplayMember = "Name",
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle,
                ItemHeight = 30
            };

            _selectButton = new Button
            {
                Text = "SELECT EXERCISE",
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            _selectButton.FlatAppearance.BorderSize = 0;
            _selectButton.Click += SelectButton_Click;

            mainLayout.Controls.Add(_muscleGroupFilter, 0, 0);
            mainLayout.Controls.Add(_searchBox, 0, 1);
            mainLayout.Controls.Add(_listBox, 0, 2);
            mainLayout.Controls.Add(_selectButton, 0, 3);

            this.Controls.Add(mainLayout);

            this.Load += async (s, e) => {
                try {
                    var mgs = await AppRuntime.MuscleGroup.GetMuscleGroupsAsync();
                    _muscleGroupFilter.Items.Clear();
                    _muscleGroupFilter.Items.Add(new MuscleGroup { Id = 0, Name = "All Muscle Groups" });
                    foreach (var mg in mgs) _muscleGroupFilter.Items.Add(mg);
                    _muscleGroupFilter.SelectedIndex = 0;
                } catch (Exception ex) {
                    MessageBox.Show($"Error loading muscle groups: {ex.Message}");
                }
            };
        }

        private async Task LoadExercises()
        {
            _listBox.Items.Clear();
            try {
                ICollection<Exercise> exercises;
                if (_muscleGroupFilter.SelectedItem is MuscleGroup mg && mg.Id > 0)
                {
                    exercises = await AppRuntime.Exercise.GetAllExercisesByMuscleGroup(mg.Id);
                }
                else
                {
                    exercises = await AppRuntime.Exercise.GetAllExercisesAsync();
                }

                string searchTerm = _searchBox.Text.ToLower();
                foreach (var ex in exercises)
                {
                    if (string.IsNullOrEmpty(searchTerm) || ex.Name.ToLower().Contains(searchTerm))
                    {
                        _listBox.Items.Add(ex);
                    }
                }
            } catch (Exception ex) {
                // Log or handle error
            }
        }

        private void SelectButton_Click(object? sender, EventArgs e)
        {
            if (_listBox.SelectedItem is Exercise ex)
            {
                SelectedExercise = ex;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Please select an exercise.");
            }
        }
    }
}
