using Accessibility;
using WorkoutTracker.Controller;
using WorkoutTracker.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WorkoutTracker.RealView.Controls
{
    public partial class WorkoutReorderDialog : Form
    {
        private FlowLayoutPanel _listPanel = null!;
        private Button _saveBtn = null!;
        private Button _cancelBtn = null!;
        private MiniReorderRow? _selectedRow;
        private List<WorkoutExercise> _exercises;
        private readonly WorkoutExerciseController _workoutExerciseController;
        public WorkoutReorderDialog(List<WorkoutExercise> exercises)
        {
            _exercises = exercises;
            MyInitializeComponent();
            LoadRows();
        }

        private void MyInitializeComponent()
        {
            this.Text = "Reorder Exercises";
            this.Size = new Size(400, 520);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            _listPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 400,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                AllowDrop = true,
                Padding = new Padding(12)
            };

            _listPanel.DragEnter += (s, e) => e.Effect = e.Data!.GetDataPresent(typeof(MiniReorderRow)) ? DragDropEffects.Move : DragDropEffects.None;
            _listPanel.DragDrop += ListPanel_DragDrop;

            var bottomPanel = new Panel { Dock = DockStyle.Fill };

            _saveBtn = new Button { Text = "SAVE ORDER", DialogResult = DialogResult.OK, Location = new Point(180, 12), Size = new Size(100, 35), FlatStyle = FlatStyle.Flat, BackColor = UIStyle.Success, ForeColor = UIStyle.TextOnPrimary, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            _cancelBtn = new Button { Text = "CANCEL", DialogResult = DialogResult.Cancel, Location = new Point(290, 12), Size = new Size(80, 35), FlatStyle = FlatStyle.Flat };

            _saveBtn.Click += async (s, e) => await SaveOrderAsync();

            bottomPanel.Controls.Add(_saveBtn);
            bottomPanel.Controls.Add(_cancelBtn);
            this.Controls.Add(bottomPanel);
            this.Controls.Add(_listPanel);
        }

        private void LoadRows()
        {
            _listPanel.Controls.Clear();
            foreach (var ex in _exercises)
            {
                var row = new MiniReorderRow(ex);
                row.RowSelected += (s, e) => SelectRow(row);
                _listPanel.Controls.Add(row);
            }
            if (_listPanel.Controls.Count > 0) SelectRow((MiniReorderRow)_listPanel.Controls[0]);
        }

        private void SelectRow(MiniReorderRow row)
        {
            if (_selectedRow != null) _selectedRow.IsSelected = false;
            _selectedRow = row;
            if (_selectedRow != null) _selectedRow.IsSelected = true;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (_selectedRow == null || _listPanel.Controls.Count <= 1)
                return base.ProcessCmdKey(ref msg, keyData);

            int currentIndex = _listPanel.Controls.GetChildIndex(_selectedRow);

            // Shift + Up/Down: Change Highlight focus
            if (keyData == (Keys.Shift | Keys.Up)) { ChangeHighlight(currentIndex - 1); return true; }
            if (keyData == (Keys.Shift | Keys.Down)) { ChangeHighlight(currentIndex + 1); return true; }

            // Up/Down: Shift layout indices
            if (keyData == Keys.Up) { MoveRowIndex(_selectedRow, currentIndex - 1); return true; }
            if (keyData == Keys.Down) { MoveRowIndex(_selectedRow, currentIndex + 1); return true; }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void ChangeHighlight(int targetIndex)
        {
            if (targetIndex >= 0 && targetIndex < _listPanel.Controls.Count)
                SelectRow((MiniReorderRow)_listPanel.Controls[targetIndex]);
        }

        private void MoveRowIndex(MiniReorderRow row, int targetIndex)
        {
            if (targetIndex >= 0 && targetIndex < _listPanel.Controls.Count)
            {
                _listPanel.Controls.SetChildIndex(row, targetIndex);
                row.Focus();
            }
        }

        private void ListPanel_DragDrop(object? sender, DragEventArgs e)
        {
            if (e.Data!.GetData(typeof(MiniReorderRow)) is MiniReorderRow draggedRow)
            {
                Point localClientPoint = _listPanel.PointToClient(new Point(e.X, e.Y));
                int targetIdx = _listPanel.Controls.Count - 1;

                for (int i = 0; i < _listPanel.Controls.Count; i++)
                {
                    Control c = _listPanel.Controls[i];
                    if (localClientPoint.Y < c.Bounds.Top + (c.Height / 2))
                    {
                        targetIdx = i;
                        break;
                    }
                }
                MoveRowIndex(draggedRow, targetIdx);
            }
        }

        private async Task SaveOrderAsync()
        {
            try
            {
                int counter = 1;

                // 1. Iterate through the visual list in top-to-bottom order
                foreach (var row in _listPanel.Controls.OfType<MiniReorderRow>())
                {
                    // Update the underlying memory object
                    row.Model.OrderIndex = counter++;
                }

                // 2. Trigger your single, batched database save
                // (Assuming you pass the modified list or the controller tracks them)
                await AppRuntime.WorkoutExercise.SaveOrderIndexChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save order: {ex.Message}");
            }
        }
    }
}
