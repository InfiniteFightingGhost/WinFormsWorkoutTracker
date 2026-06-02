using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using WorkoutTracker.Data.Entities;

namespace WorkoutTracker.RealView.Views
{
    public class WorkoutManagementView : BaseView
    {
        private DataGridView _grid = null!;
        private TextBox _titleTxt = null!;
        private TextBox _descTxt = null!;

        public WorkoutManagementView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2 };
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 70));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 30));

            _grid = new DataGridView { Dock = DockStyle.Fill, BackgroundColor = UIStyle.Surface, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AllowUserToAddRows = false, ReadOnly = true };
            
            var editPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, Padding = new Padding(10) };
            _titleTxt = new TextBox { Width = 300 };
            _descTxt = new TextBox { Width = 300 };
            var addBtn = new Button { Text = "Add Workout Template", Height = 40, Width = 200 };
            var delBtn = new Button { Text = "Delete Selected", Height = 40, Width = 200 };
            
            addBtn.Click += async (s, e) => {
                await AppRuntime.Workout.CreateWorkoutAsync(new Workout { Title = _titleTxt.Text, Description = _descTxt.Text });
                await RefreshData();
            };
            delBtn.Click += async (s, e) => {
                if (_grid.SelectedRows.Count > 0) {
                    var w = (Workout)_grid.SelectedRows[0].DataBoundItem;
                    await AppRuntime.Workout.DeleteWorkoutAsync(w.Id);
                    await RefreshData();
                }
            };

            editPanel.Controls.Add(new Label { Text = "Title:" });
            editPanel.Controls.Add(_titleTxt);
            editPanel.Controls.Add(new Label { Text = "Description:" });
            editPanel.Controls.Add(_descTxt);
            editPanel.Controls.Add(addBtn);
            editPanel.Controls.Add(delBtn);

            layout.Controls.Add(_grid, 0, 0);
            layout.Controls.Add(editPanel, 0, 1);
            this.Controls.Add(layout);
        }

        public override async void OnNavigatedTo() => await RefreshData();
        private async Task RefreshData() => _grid.DataSource = (await AppRuntime.Workout.GetWorkoutsAsync()).ToList();
    }
}
