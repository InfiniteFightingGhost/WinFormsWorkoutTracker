using System;
using System.Drawing;
using System.Windows.Forms;
using Data.Entities;
using Data.Enums;

namespace RealView.Controls
{
    //public class SetRow : UserControl
    //{
    //    private ExerciseSet _set;
    //    private NumericUpDown _weightNum;
    //    private NumericUpDown _repsNum;
    //    private CheckBox _completedChk;
    //    private Label _setLabel;

    //    public SetRow(ExerciseSet set)
    //    {
    //        _set = set;
    //        InitializeComponent();
    //    }

    //    private void InitializeComponent()
    //    {
    //        this.Size = new Size(390, 40);
    //        this.Margin = new Padding(0, 2, 0, 2);

    //        _setLabel = new Label
    //        {
    //            Text = $"Set {_set.OrderIndex + 1}",
    //            Location = new Point(5, 10),
    //            Width = 45,
    //            Font = new Font("Segoe UI", 9)
    //        };

    //        _weightNum = new NumericUpDown
    //        {
    //            Value = (decimal)_set.Weight,
    //            Location = new Point(55, 8),
    //            Width = 70,
    //            DecimalPlaces = 1,
    //            Maximum = 1000
    //        };
    //        _weightNum.ValueChanged += (s, e) => SaveChanges();

    //        var kgLabel = new Label { Text = "kg", Location = new Point(127, 10), Width = 25 };

    //        _repsNum = new NumericUpDown
    //        {
    //            Value = _set.Repetitions,
    //            Location = new Point(155, 8),
    //            Width = 60,
    //            Maximum = 1000
    //        };
    //        _repsNum.ValueChanged += (s, e) => SaveChanges();

    //        var repsLabel = new Label { Text = "reps", Location = new Point(217, 10), Width = 35 };

    //        _completedChk = new CheckBox
    //        {
    //            Text = "Done",
    //            Checked = _set.Completed,
    //            Location = new Point(260, 8),
    //            Width = 60
    //        };
    //        _completedChk.CheckedChanged += (s, e) => SaveChanges();

    //        var removeBtn = new Button
    //        {
    //            Text = "X",
    //            Location = new Point(325, 6),
    //            Size = new Size(25, 25),
    //            FlatStyle = FlatStyle.Flat,
    //            ForeColor = Color.Red
    //        };
    //        removeBtn.FlatAppearance.BorderSize = 0;
    //        // TODO: Implement set removal if needed

    //        this.Controls.Add(_setLabel);
    //        this.Controls.Add(_weightNum);
    //        this.Controls.Add(kgLabel);
    //        this.Controls.Add(_repsNum);
    //        this.Controls.Add(repsLabel);
    //        this.Controls.Add(_completedChk);
    //        this.Controls.Add(removeBtn);
    //    }

    //    private async void SaveChanges()
    //    {
    //        _set.Weight = _weightNum.Value;
    //        _set.Repetitions = (int)_repsNum.Value;
    //        _set.Completed = _completedChk.Checked;

    //        try
    //        {
    //            await AppRuntime.WorkoutSet.UpdateExerciseSetAsync(_set);
    //        }
    //        catch (Exception ex)
    //        {
    //            // Silently fail or log
    //            Console.WriteLine($"Error saving set: {ex.Message}");
    //        }
    //    }
    //}
    public class SetRow : UserControl
    {
        private ExerciseSet _set;
        private ExerciseCard _parentCard;
        private NumericUpDown _weightNum;
        private NumericUpDown _repsNum;
        private CheckBox _completedChk;
        private Label _setLabel;
        private bool _isReadOnly;

        public SetRow(ExerciseSet set, ExerciseCard parentCard, bool isReadOnly = false)
        {
            _set = set;
            _parentCard = parentCard;
            _isReadOnly = isReadOnly;
            InitializeComponent();
            if (!_isReadOnly) SetupContextMenu();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(430, 40);
            this.Margin = new Padding(0, 2, 0, 2);

            _setLabel = new Label
            {
                Location = new Point(10, 10),
                Width = 30,
                Cursor = _isReadOnly ? Cursors.Default : Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = UIStyle.CaptionBold
            };

            // 💡 Increased width from 80 to 100, shifted right slightly
            _weightNum = new NumericUpDown
            {
                Value = (decimal)_set.Weight,
                Location = new Point(45, 8),
                Width = 100,
                DecimalPlaces = 1,
                Maximum = 1000,
                ReadOnly = _isReadOnly,
                Increment = _isReadOnly ? 0 : 1,
                Font = UIStyle.Body
            };
            if (!_isReadOnly) 
            {
                _weightNum.ValueChanged += (s, e) => SaveChanges();
                _weightNum.KeyDown += (s, e) => {
                    if (e.KeyCode == Keys.Enter) {
                        _repsNum.Focus();
                        e.Handled = e.SuppressKeyPress = true;
                    }
                };
            }

            var kgLabel = new Label { Text = "kg", Location = new Point(150, 10), Width = 30, ForeColor = UIStyle.TextSecondary, Font = UIStyle.Caption };

            // 💡 Increased width from 70 to 100, dynamically shifted over
            _repsNum = new NumericUpDown
            {
                Value = _set.Repetitions,
                Location = new Point(185, 8),
                Width = 100,
                Maximum = 1000,
                ReadOnly = _isReadOnly,
                Increment = _isReadOnly ? 0 : 1,
                Font = UIStyle.Body
            };
            if (!_isReadOnly) 
            {
                _repsNum.ValueChanged += (s, e) => SaveChanges();
                _repsNum.KeyDown += (s, e) => {
                    if (e.KeyCode == Keys.Enter) {
                        _parentCard.FocusNextSet(this);
                        e.Handled = e.SuppressKeyPress = true;
                    }
                };
            }

            var repsLabel = new Label { Text = "reps", Location = new Point(290, 10), Width = 40, ForeColor = UIStyle.TextSecondary, Font = UIStyle.Caption };

            // 💡 Expanded width to give the click target a larger footprint
            _completedChk = new CheckBox
            {
                Text = "Done",
                Checked = _set.Completed,
                Location = new Point(340, 8),
                Width = 80,
                Enabled = !_isReadOnly,
                Font = UIStyle.CaptionBold,
                ForeColor = UIStyle.TextPrimary
            };
            if (!_isReadOnly) _completedChk.CheckedChanged += (s, e) => SaveChanges();

            this.Controls.Add(_setLabel);
            this.Controls.Add(_weightNum);
            this.Controls.Add(kgLabel);
            this.Controls.Add(_repsNum);
            this.Controls.Add(repsLabel);
            this.Controls.Add(_completedChk);
        }

        public void FocusWeight()
        {
            _weightNum.Focus();
            // Select all text for easy editing
            _weightNum.Select(0, _weightNum.Text.Length);
        }

        private void SetupContextMenu()
        {
            var menu = new ContextMenuStrip
            {
                // 💡 Use our custom modern look instead of the system theme
                Renderer = new ToolStripProfessionalRenderer(new ModernColorTable()),
                ShowImageMargin = false, // 💡 Removes the blank icon strip on the left
                ShowCheckMargin = false,
                Font = new Font("Segoe UI", 10F, FontStyle.Regular) // Cleaner font size
            };

            // Create the items
            var regularItem = new ToolStripMenuItem("Regular Set", null, (s, e) => UpdateSetType(SetType.Regular));
            var warmUpItem = new ToolStripMenuItem("Warm-Up Set (W)", null, (s, e) => UpdateSetType(SetType.WarmUp));
            var dropSetItem = new ToolStripMenuItem("Drop Set", null, (s, e) => UpdateSetType(SetType.DropSet));

            var deleteItem = new ToolStripMenuItem("Delete Entire Set", null, async (s, e) => {
                if (MessageBox.Show("Are you sure you want to delete this set?", "Delete Set", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    await _parentCard.RemoveSetAsync(this, _set);
                }
            });
            deleteItem.ForeColor = Color.Crimson; // Modern red accent

            // 💡 Add extra padding to items so they look spacious, not cramped
            var menuPadding = new Padding(12, 6, 12, 6);
            regularItem.Padding = menuPadding;
            warmUpItem.Padding = menuPadding;
            dropSetItem.Padding = menuPadding;
            deleteItem.Padding = menuPadding;

            // Combine everything
            menu.Items.AddRange(new ToolStripItem[] {
                regularItem,
                warmUpItem,
                dropSetItem,
                new ToolStripSeparator(),
                deleteItem
            });

            _setLabel.ContextMenuStrip = menu;

            // Smooth trigger location logic
            _setLabel.Click += (s, e) => menu.Show(_setLabel, new Point(0, _setLabel.Height));
        }
        public void RefreshDisplay(ref int regularSetIndex)
        {
            switch (_set.SetType)
            {
                case SetType.WarmUp:
                    _setLabel.Text = "W";
                    _setLabel.ForeColor = Color.DarkGoldenrod;
                    _setLabel.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                    break;

                case SetType.DropSet:
                    _setLabel.Text = regularSetIndex++.ToString();
                    _setLabel.ForeColor = Color.DarkOrchid;
                    _setLabel.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                    break;

                case SetType.Regular:
                default:
                    _setLabel.Text = regularSetIndex++.ToString();
                    _setLabel.ForeColor = Color.Black;
                    _setLabel.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                    break;
            }
        }

        private void UpdateSetType(SetType newType)
        {
            _set.SetType = newType;
            _parentCard.UpdateSetNumbering();
            SaveChanges();
        }

        private async void SaveChanges()
        {
            _set.Weight = _weightNum.Value;
            _set.Repetitions = (int)_repsNum.Value;
            bool wasCompleted = _set.Completed;
            _set.Completed = _completedChk.Checked;

            try
            {
                await AppRuntime.WorkoutSet.UpdateExerciseSetAsync(_set);
                if (!wasCompleted && _set.Completed)
                {
                    AppRuntime.WorkoutState.NotifySetCompleted(_set);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving set: {ex.Message}");
            }
        }
    }
}
