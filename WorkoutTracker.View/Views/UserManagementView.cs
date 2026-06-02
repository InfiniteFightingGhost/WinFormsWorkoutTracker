using System;
using System.Drawing;
using System.Windows.Forms;
using WorkoutTracker.Data.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkoutTracker.View.Views
{
    public class UserManagementView : BaseView
    {
        private DataGridView _dataGridView;

        public UserManagementView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            _dataGridView = new DataGridView { Dock = DockStyle.Fill, AutoGenerateColumns = false, AllowUserToAddRows = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect };
            _dataGridView.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Id", HeaderText = "Id", Width = 50 });
            _dataGridView.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Username", HeaderText = "Username", Width = 150 });
            _dataGridView.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Email", HeaderText = "Email", Width = 200 });
            _dataGridView.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Role", HeaderText = "Role", Width = 100 });

            var buttonPanel = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 40 };
            var deleteBtn = new Button { Text = "Delete User" };
            buttonPanel.Controls.Add(deleteBtn);

            deleteBtn.Click += async (s, e) => {
                if (_dataGridView.SelectedRows.Count > 0) {
                    var user = (User)_dataGridView.SelectedRows[0].DataBoundItem;
                    if (MessageBox.Show($"Are you sure you want to delete user {user.Username}?", "Delete User", MessageBoxButtons.YesNo) == DialogResult.Yes) {
                        await AppRuntime.User.DeleteAsync(user.Id);
                        await RefreshData();
                    }
                }
            };

            this.Controls.Add(_dataGridView);
            this.Controls.Add(buttonPanel);
        }

        public override async void OnNavigatedTo()
        {
            await RefreshData();
        }

        private async Task RefreshData()
        {
            _dataGridView.DataSource = (await AppRuntime.User.GetAllAsync()).ToList();
        }
    }
}
