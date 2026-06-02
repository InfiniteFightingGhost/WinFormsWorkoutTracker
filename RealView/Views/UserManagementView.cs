using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using WorkoutTracker.Data.Entities;

namespace WorkoutTracker.RealView.Views
{
    public class UserManagementView : BaseView
    {
        private DataGridView _grid = null!;

        public UserManagementView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            _grid = new DataGridView { Dock = DockStyle.Fill, BackgroundColor = UIStyle.Surface, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AllowUserToAddRows = false, ReadOnly = true };
            
            var btnPanel = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 40 };
            var delBtn = new Button { Text = "Delete User", Width = 120 };
            delBtn.Click += async (s, e) => {
                if (_grid.SelectedRows.Count > 0) {
                    var u = (User)_grid.SelectedRows[0].DataBoundItem;
                    if (MessageBox.Show($"Delete user {u.Username}?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes) {
                        await AppRuntime.User.DeleteAsync(u.Id);
                        await RefreshData();
                    }
                }
            };
            btnPanel.Controls.Add(delBtn);

            this.Controls.Add(_grid);
            this.Controls.Add(btnPanel);
        }

        public override async void OnNavigatedTo() => await RefreshData();
        private async Task RefreshData() => _grid.DataSource = (await AppRuntime.User.GetAllAsync()).ToList();
    }
}
