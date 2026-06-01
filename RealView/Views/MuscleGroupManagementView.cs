using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using Data.Entities;

namespace RealView.Views
{
    public class MuscleGroupManagementView : BaseView
    {
        private DataGridView _grid = null!;
        private TextBox _nameTxt = null!;

        public MuscleGroupManagementView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2 };
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 80));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 20));

            _grid = new DataGridView { Dock = DockStyle.Fill, BackgroundColor = UIStyle.Surface, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AllowUserToAddRows = false, ReadOnly = true };
            
            var editPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            _nameTxt = new TextBox { Width = 200 };
            var addBtn = new Button { Text = "Add Muscle Group", Height = 30, Width = 150 };
            var delBtn = new Button { Text = "Delete Selected", Height = 30, Width = 150 };
            
            addBtn.Click += async (s, e) => {
                if (!string.IsNullOrWhiteSpace(_nameTxt.Text)) {
                    await AppRuntime.MuscleGroup.CreateMuscleGroupAsync(_nameTxt.Text);
                    _nameTxt.Text = "";
                    await RefreshData();
                }
            };
            delBtn.Click += async (s, e) => {
                if (_grid.SelectedRows.Count > 0) {
                    var mg = (MuscleGroup)_grid.SelectedRows[0].DataBoundItem;
                    await AppRuntime.MuscleGroup.DeleteMuscleGroupAsync(mg.Id);
                    await RefreshData();
                }
            };

            editPanel.Controls.Add(new Label { Text = "Name:", Width = 50 });
            editPanel.Controls.Add(_nameTxt);
            editPanel.Controls.Add(addBtn);
            editPanel.Controls.Add(delBtn);

            layout.Controls.Add(_grid, 0, 0);
            layout.Controls.Add(editPanel, 0, 1);
            this.Controls.Add(layout);
        }

        public override async void OnNavigatedTo() => await RefreshData();
        private async Task RefreshData() => _grid.DataSource = (await AppRuntime.MuscleGroup.GetMuscleGroupsAsync()).ToList();
    }
}
