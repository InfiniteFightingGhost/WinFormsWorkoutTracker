using System;
using System.Drawing;
using System.Windows.Forms;
using WorkoutTracker.Data.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkoutTracker.View.Views
{
    public class ExerciseManagementView : BaseView
    {
        private DataGridView _dataGridView;
        private Button _addButton;
        private Button _editButton;
        private Button _deleteButton;
        private Panel _formPanel;
        private TextBox _nameTxt;
        private TextBox _descTxt;
        private ComboBox _muscleGroupCb;

        public ExerciseManagementView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            var mainLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2 };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 70));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 30));

            _dataGridView = new DataGridView { Dock = DockStyle.Fill, AutoGenerateColumns = false, AllowUserToAddRows = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect };
            _dataGridView.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Id", HeaderText = "Id", Width = 50 });
            _dataGridView.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Name", HeaderText = "Name", Width = 150 });
            _dataGridView.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Description", HeaderText = "Description", Width = 200 });

            var buttonPanel = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 40 };
            _addButton = new Button { Text = "Add" };
            _editButton = new Button { Text = "Edit" };
            _deleteButton = new Button { Text = "Delete" };
            buttonPanel.Controls.AddRange(new Control[] { _addButton, _editButton, _deleteButton });

            _addButton.Click += (s, e) => ShowForm(new Exercise());
            _editButton.Click += (s, e) => {
                if (_dataGridView.SelectedRows.Count > 0)
                    ShowForm((Exercise)_dataGridView.SelectedRows[0].DataBoundItem);
            };
            _deleteButton.Click += async (s, e) => {
                if (_dataGridView.SelectedRows.Count > 0)
                {
                    var ex = (Exercise)_dataGridView.SelectedRows[0].DataBoundItem;
                    await AppRuntime.Exercise.DeleteExerciseAsync(ex.Id);
                    await RefreshData();
                }
            };

            _formPanel = new Panel { Dock = DockStyle.Fill, Visible = false };
            var lblName = new Label { Text = "Name:", Location = new Point(10, 10) };
            _nameTxt = new TextBox { Location = new Point(100, 10), Width = 200 };
            var lblDesc = new Label { Text = "Description:", Location = new Point(10, 40) };
            _descTxt = new TextBox { Location = new Point(100, 40), Width = 200 };
            var lblMG = new Label { Text = "Muscle Group:", Location = new Point(10, 70) };
            _muscleGroupCb = new ComboBox { Location = new Point(100, 70), Width = 200, DisplayMember = "Name" };
            var saveBtn = new Button { Text = "Save", Location = new Point(100, 100) };
            var cancelBtn = new Button { Text = "Cancel", Location = new Point(180, 100) };

            saveBtn.Click += async (s, e) => await SaveExercise();
            cancelBtn.Click += (s, e) => _formPanel.Visible = false;

            _formPanel.Controls.AddRange(new Control[] { lblName, _nameTxt, lblDesc, _descTxt, lblMG, _muscleGroupCb, saveBtn, cancelBtn });

            var topPanel = new Panel { Dock = DockStyle.Fill };
            topPanel.Controls.Add(_dataGridView);
            topPanel.Controls.Add(buttonPanel);

            mainLayout.Controls.Add(topPanel, 0, 0);
            mainLayout.Controls.Add(_formPanel, 0, 1);

            this.Controls.Add(mainLayout);
        }

        private Exercise _editingExercise;

        private void ShowForm(Exercise ex)
        {
            _editingExercise = ex;
            _nameTxt.Text = ex.Name;
            _descTxt.Text = ex.Description;
            _formPanel.Visible = true;
        }

        private async Task SaveExercise()
        {
            try
            {
                if (_editingExercise.Id == 0)
                {
                    var ex = new Exercise { Name = _nameTxt.Text, Description = _descTxt.Text, Instructions = "", MuscleGroupId = ((MuscleGroup)_muscleGroupCb.SelectedItem)?.Id ?? 0 };
                    await AppRuntime.Exercise.CreateExerciseAsync(ex);
                }
                else
                {
                    await AppRuntime.Exercise.UpdateExerciseAsync(_editingExercise.Id, _nameTxt.Text, _descTxt.Text);
                }
                _formPanel.Visible = false;
                await RefreshData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public override async void OnNavigatedTo()
        {
            await RefreshData();
            var muscleGroups = await AppRuntime.MuscleGroup.GetMuscleGroupsAsync();
            _muscleGroupCb.DataSource = muscleGroups.ToList();
        }

        private async Task RefreshData()
        {
            _dataGridView.DataSource = (await AppRuntime.Exercise.GetAllExercisesAsync()).ToList();
        }
    }
}
