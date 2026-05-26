using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using Data.Entities;

namespace RealView.Views
{
    public class ExerciseManagementView : BaseView
    {
        private DataGridView _grid = null!;
        private Panel _editPanel = null!;
        private TextBox _nameTxt = null!;
        private TextBox _descTxt = null!;
        private ComboBox _muscleGroupCb = null!;

        public ExerciseManagementView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2 };
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 60));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 40));

            _grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                ReadOnly = true
            };

            _editPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };
            
            var btnPanel = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 50 };
            var addBtn = new Button { Text = "Add New", Height = 40, Width = 100 };
            var delBtn = new Button { Text = "Delete Selected", Height = 40, Width = 150 };
            btnPanel.Controls.Add(addBtn);
            btnPanel.Controls.Add(delBtn);

            addBtn.Click += (s, e) => ShowEdit(new Exercise());
            delBtn.Click += async (s, e) => {
                if (_grid.SelectedRows.Count > 0) {
                    var ex = (Exercise)_grid.SelectedRows[0].DataBoundItem;
                    await AppRuntime.Exercise.DeleteExerciseAsync(ex.Id);
                    await RefreshData();
                }
            };

            var editContainer = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown };
            _nameTxt = CreateInput("Name", editContainer);
            _descTxt = CreateInput("Description", editContainer);
            
            var lblMG = new Label { Text = "Muscle Group", Height = 25 };
            _muscleGroupCb = new ComboBox { Width = 300, DisplayMember = "Name", DropDownStyle = ComboBoxStyle.DropDownList };
            editContainer.Controls.Add(lblMG);
            editContainer.Controls.Add(_muscleGroupCb);

            var saveBtn = new Button { Text = "SAVE", Height = 40, Width = 100, Margin = new Padding(0, 20, 0, 0) };
            saveBtn.Click += async (s, e) => await Save();
            editContainer.Controls.Add(saveBtn);

            _editPanel.Controls.Add(editContainer);

            var topPanel = new Panel { Dock = DockStyle.Fill };
            topPanel.Controls.Add(_grid);
            topPanel.Controls.Add(btnPanel);

            layout.Controls.Add(topPanel, 0, 0);
            layout.Controls.Add(_editPanel, 0, 1);
            this.Controls.Add(layout);
        }

        private TextBox CreateInput(string label, Control container)
        {
            var lbl = new Label { Text = label, Height = 25 };
            var txt = new TextBox { Width = 300 };
            container.Controls.Add(lbl);
            container.Controls.Add(txt);
            return txt;
        }

        private Exercise? _editing;
        private void ShowEdit(Exercise ex)
        {
            _editing = ex;
            _nameTxt.Text = ex.Name;
            _descTxt.Text = ex.Description;
        }

        private async Task Save()
        {
            if (_editing == null) return;
            try {
                if (_editing.Id == 0) {
                    var mg = (MuscleGroup)_muscleGroupCb.SelectedItem;
                    await AppRuntime.Exercise.CreateExerciseAsync(new Exercise { 
                        Name = _nameTxt.Text, 
                        Description = _descTxt.Text, 
                        MuscleGroupId = mg?.Id ?? 0,
                        Instructions = ""
                    });
                } else {
                    await AppRuntime.Exercise.UpdateExerciseAsync(_editing.Id, _nameTxt.Text, _descTxt.Text);
                }
                await RefreshData();
            } catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        public override async void OnNavigatedTo()
        {
            await RefreshData();
            var mgs = await AppRuntime.MuscleGroup.GetMuscleGroupsAsync();
            _muscleGroupCb.DataSource = mgs.ToList();
        }

        private async Task RefreshData()
        {
            var data = await AppRuntime.Exercise.GetAllExercisesAsync();
            _grid.DataSource = data.ToList();
        }
    }
}
