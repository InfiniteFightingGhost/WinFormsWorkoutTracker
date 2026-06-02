using System;
using System.Drawing;
using System.Windows.Forms;
using WorkoutTracker.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WorkoutTracker.View.Controls
{
    public class ExerciseSelectionDialog : Form
    {
        private ListBox _listBox;
        private ComboBox _muscleGroupFilter;
        private Button _selectButton;
        public Exercise SelectedExercise { get; private set; }

        public ExerciseSelectionDialog()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Select Exercise";
            this.Size = new Size(300, 450);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;

            _muscleGroupFilter = new ComboBox
            {
                Dock = DockStyle.Top,
                DisplayMember = "Name",
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _muscleGroupFilter.SelectedIndexChanged += async (s, e) => await LoadExercises();

            _listBox = new ListBox
            {
                Dock = DockStyle.Fill,
                DisplayMember = "Name"
            };

            _selectButton = new Button
            {
                Text = "Select",
                Dock = DockStyle.Bottom,
                Height = 50
            };
            _selectButton.Click += SelectButton_Click;

            this.Controls.Add(_listBox);
            this.Controls.Add(_muscleGroupFilter);
            this.Controls.Add(_selectButton);

            this.Load += async (s, e) => {
                var mgs = await AppRuntime.MuscleGroup.GetMuscleGroupsAsync();
                var allMg = new MuscleGroup { Id = 0, Name = "All Muscle Groups" };
                _muscleGroupFilter.Items.Add(allMg);
                foreach (var mg in mgs) _muscleGroupFilter.Items.Add(mg);
                _muscleGroupFilter.SelectedIndex = 0;
            };
        }

        private async Task LoadExercises()
        {
            _listBox.Items.Clear();
            ICollection<Exercise> exercises;
            if (_muscleGroupFilter.SelectedItem is MuscleGroup mg && mg.Id > 0)
            {
                exercises = await AppRuntime.Exercise.GetAllExercisesByMuscleGroup(mg.Id);
            }
            else
            {
                exercises = await AppRuntime.Exercise.GetAllExercisesAsync();
            }

            foreach (var ex in exercises)
            {
                _listBox.Items.Add(ex);
            }
        }

        private void SelectButton_Click(object sender, EventArgs e)
        {
            if (_listBox.SelectedItem is Exercise ex)
            {
                SelectedExercise = ex;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}
