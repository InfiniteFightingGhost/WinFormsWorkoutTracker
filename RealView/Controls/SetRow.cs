using System;
using System.Drawing;
using System.Windows.Forms;
using WorkoutTracker.Data.Entities;
using WorkoutTracker.Data.Enums;

namespace WorkoutTracker.RealView.Controls
{
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

            _weightNum = new NumericUpDown
            {
                Value = (decimal)_set.Weight,
                Location = new Point(45, 8),
                Width = 100,
                DecimalPlaces = 1,
                Maximum = 1000,
                ReadOnly = _isReadOnly,
                Increment = _isReadOnly ? 0 : 1,
                Font = UIStyle.Body,
                BackColor = UIStyle.Surface,
                ForeColor = UIStyle.TextPrimary
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

            _repsNum = new NumericUpDown
            {
                Value = _set.Repetitions,
                Location = new Point(185, 8),
                Width = 100,
                Maximum = 1000,
                ReadOnly = _isReadOnly,
                Increment = _isReadOnly ? 0 : 1,
                Font = UIStyle.Body,
                BackColor = UIStyle.Surface,
                ForeColor = UIStyle.TextPrimary
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
            _weightNum.Select(0, _weightNum.Text.Length);
        }

        private void SetupContextMenu()
        {
            var menu = new ContextMenuStrip
            {
                Renderer = new ToolStripProfessionalRenderer(new ModernColorTable()),
                ShowImageMargin = false, 
                ShowCheckMargin = false,
                Font = UIStyle.Body
            };

            var regularItem = new ToolStripMenuItem("Regular Set", null, (s, e) => UpdateSetType(SetType.Regular));
            var warmUpItem = new ToolStripMenuItem("Warm-Up Set (W)", null, (s, e) => UpdateSetType(SetType.WarmUp));
            var dropSetItem = new ToolStripMenuItem("Drop Set", null, (s, e) => UpdateSetType(SetType.DropSet));

            var deleteItem = new ToolStripMenuItem("Delete Entire Set", null, async (s, e) => {
                if (MessageBox.Show("Are you sure you want to delete this set?", "Delete Set", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    await _parentCard.RemoveSetAsync(this, _set);
                }
            });
            deleteItem.ForeColor = UIStyle.Danger; 

            var menuPadding = new Padding(12, 6, 12, 6);
            regularItem.Padding = menuPadding;
            warmUpItem.Padding = menuPadding;
            dropSetItem.Padding = menuPadding;
            deleteItem.Padding = menuPadding;

            menu.Items.AddRange(new ToolStripItem[] {
                regularItem,
                warmUpItem,
                dropSetItem,
                new ToolStripSeparator(),
                deleteItem
            });

            _setLabel.ContextMenuStrip = menu;
            _setLabel.Click += (s, e) => menu.Show(_setLabel, new Point(0, _setLabel.Height));
        }

        public void RefreshDisplay(ref int regularSetIndex)
        {
            switch (_set.SetType)
            {
                case SetType.WarmUp:
                    _setLabel.Text = "W";
                    _setLabel.ForeColor = UIStyle.WarmupSet;
                    _setLabel.Font = UIStyle.CaptionBold;
                    break;

                case SetType.DropSet:
                    _setLabel.Text = regularSetIndex++.ToString();
                    _setLabel.ForeColor = UIStyle.DropSet;
                    _setLabel.Font = UIStyle.CaptionBold;
                    break;

                case SetType.Regular:
                default:
                    _setLabel.Text = regularSetIndex++.ToString();
                    _setLabel.ForeColor = UIStyle.TextPrimary;
                    _setLabel.Font = UIStyle.CaptionBold;
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
