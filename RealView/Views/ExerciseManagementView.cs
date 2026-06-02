using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using WorkoutTracker.Data.Entities;
using WorkoutTracker.Utility;

namespace WorkoutTracker.RealView.Views
{
    public class ExerciseManagementView : BaseView
    {
        private DataGridView _grid = null!;
        private Panel _editPanel = null!;
        private TextBox _nameTxt = null!;
        private TextBox _descTxt = null!;
        private ComboBox _muscleGroupCb = null!;
        private CheckedListBox _filterList = null!;
        private TextBox _filterSearchTxt = null!;
        private List<MuscleGroup> _initialFilters = new List<MuscleGroup>();

        public ExerciseManagementView() : this(null) { }

        public ExerciseManagementView(MuscleGroup? filter)
        {
            if (filter != null) _initialFilters.Add(filter);
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            var mainLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 2 };
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 250));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 60));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 40));

            // LEFT FILTER PANEL
            var filterPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(15), BackColor = UIStyle.SurfaceVariant };
            var filterTitle = new Label { Text = "FILTERS", Font = UIStyle.CaptionBold, ForeColor = UIStyle.TextSecondary, Dock = DockStyle.Top, Height = 30 };
            
            _filterSearchTxt = new TextBox { 
                Dock = DockStyle.Top, 
                Font = UIStyle.Body, 
                PlaceholderText = "Search by name..." 
            };
            _filterSearchTxt.TextChanged += async (s, e) => await RefreshData();

            var spacer = new Panel { Dock = DockStyle.Top, Height = 15 };

            _filterList = new CheckedListBox { 
                Dock = DockStyle.Fill, 
                BorderStyle = BorderStyle.None, 
                BackColor = UIStyle.SurfaceVariant,
                CheckOnClick = true,
                DisplayMember = "Name",
                Font = UIStyle.Body
            };
            _filterList.ItemCheck += (s, e) => {
                // Use BeginInvoke to wait for the check state to actually change
                this.BeginInvoke(new Action(async () => await RefreshData()));
            };

            var clearFiltersBtn = new Button { 
                Text = "CLEAR ALL", 
                Dock = DockStyle.Bottom, 
                Height = 35, 
                FlatStyle = FlatStyle.Flat,
                Font = UIStyle.CaptionBold
            };
            clearFiltersBtn.Click += async (s, e) => {
                _filterSearchTxt.Clear();
                for (int i = 0; i < _filterList.Items.Count; i++) _filterList.SetItemChecked(i, false);
                await RefreshData();
            };

            filterPanel.Controls.Add(_filterList);
            filterPanel.Controls.Add(spacer);
            filterPanel.Controls.Add(_filterSearchTxt);
            filterPanel.Controls.Add(filterTitle);
            filterPanel.Controls.Add(clearFiltersBtn);

            _grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = UIStyle.Surface,
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

            mainLayout.Controls.Add(filterPanel, 0, 0);
            mainLayout.SetRowSpan(filterPanel, 2);
            mainLayout.Controls.Add(topPanel, 1, 0);
            mainLayout.Controls.Add(_editPanel, 1, 1);
            this.Controls.Add(mainLayout);
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
            var mgs = await AppRuntime.MuscleGroup.GetMuscleGroupsAsync();
            var mgList = mgs.ToList();
            
            _filterList.Items.Clear();
            foreach (var mg in mgList)
            {
                int index = _filterList.Items.Add(mg);
                if (_initialFilters.Any(f => f.Id == mg.Id))
                {
                    _filterList.SetItemChecked(index, true);
                }
            }
            // Clear initial filters after first application
            _initialFilters.Clear();

            _muscleGroupCb.DataSource = mgList;
            await RefreshData();
        }

        CancellationTokenRegistration _searchDebounceReg = new();
        private async Task RefreshData()
        {
            var mgs = AppRuntime.MuscleGroup.GetMuscleGroupsAsync();
            
            var selectedGroups = _filterList.CheckedItems.Cast<MuscleGroup>().Select(g => g.Id).ToList();
            var searchText = _filterSearchTxt.Text.Trim().ToLower();

            Task.Delay(LogicalToDeviceUnits(350)).ContinueWith(async _ =>
            {
                var filtered = await AppRuntime.Exercise.GetExercisesWithFiltration(selectedGroups, searchText);
                if (!_searchDebounceReg.Token.IsCancellationRequested)
                {
                    _grid.Invoke(new Action(() => _grid.DataSource = filtered));
                }
            }, _searchDebounceReg.Token);
        }
    }
}
