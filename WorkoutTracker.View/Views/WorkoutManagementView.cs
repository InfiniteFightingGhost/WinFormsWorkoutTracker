using System;
using System.Drawing;
using System.Windows.Forms;
using WorkoutTracker.Data.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkoutTracker.View.Views
{
    public class WorkoutManagementView : BaseView
    {
        private DataGridView _dataGridView;
        private Button _addButton;
        private Panel _formPanel;
        private TextBox _titleTxt;
        private TextBox _descTxt;

        public WorkoutManagementView()
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
            _dataGridView.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Title", HeaderText = "Title", Width = 150 });
            _dataGridView.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Description", HeaderText = "Description", Width = 200 });

            var buttonPanel = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 40 };
            _addButton = new Button { Text = "Add" };
            var deleteButton = new Button { Text = "Delete" };
            buttonPanel.Controls.AddRange(new Control[] { _addButton, deleteButton });

            _addButton.Click += (s, e) => {
                _titleTxt.Text = "";
                _descTxt.Text = "";
                _formPanel.Visible = true;
            };

            deleteButton.Click += async (s, e) => {
                if (_dataGridView.SelectedRows.Count > 0) {
                    var w = (Workout)_dataGridView.SelectedRows[0].DataBoundItem;
                    await AppRuntime.Workout.DeleteWorkoutAsync(w.Id);
                    await RefreshData();
                }
            };

            _formPanel = new Panel { Dock = DockStyle.Fill, Visible = false };
            var lblTitle = new Label { Text = "Title:", Location = new Point(10, 10) };
            _titleTxt = new TextBox { Location = new Point(100, 10), Width = 200 };
            var lblDesc = new Label { Text = "Description:", Location = new Point(10, 40) };
            _descTxt = new TextBox { Location = new Point(100, 40), Width = 200 };
            var saveBtn = new Button { Text = "Save", Location = new Point(100, 70) };
            
            saveBtn.Click += async (s, e) => {
                await AppRuntime.Workout.CreateWorkoutAsync(new Workout { Title = _titleTxt.Text, Description = _descTxt.Text });
                _formPanel.Visible = false;
                await RefreshData();
            };

            _formPanel.Controls.AddRange(new Control[] { lblTitle, _titleTxt, lblDesc, _descTxt, saveBtn });

            var topPanel = new Panel { Dock = DockStyle.Fill };
            topPanel.Controls.Add(_dataGridView);
            topPanel.Controls.Add(buttonPanel);

            mainLayout.Controls.Add(topPanel, 0, 0);
            mainLayout.Controls.Add(_formPanel, 0, 1);

            this.Controls.Add(mainLayout);
        }

        public override async void OnNavigatedTo()
        {
            await RefreshData();
        }

        private async Task RefreshData()
        {
            _dataGridView.DataSource = (await AppRuntime.Workout.GetWorkoutsAsync()).ToList();
        }
    }
}
