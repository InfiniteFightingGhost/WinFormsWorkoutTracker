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
    public partial class MiniReorderRow : UserControl
    {
        public WorkoutExercise Model { get; }
        public event EventHandler? RowSelected;

        private bool _isSelected;
        private DragGhostForm? _dragGhost;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                // Highlight color when navigated via keyboard or click
                this.BackColor = _isSelected ? UIStyle.Selection : UIStyle.Surface;
                this.Invalidate();
            }
        }

        public MiniReorderRow(WorkoutExercise model)
        {
            Model = model;
            MyInitializeComponent();
        }

        private void MyInitializeComponent()
        {
            this.Size = new Size(360, 45);
            this.Margin = new Padding(0, 0, 0, 8);
            this.BackColor = UIStyle.Surface;

            var dragIcon = new Label
            {
                Text = "↕",
                Font = UIStyle.SubHeader,
                ForeColor = UIStyle.TextTertiary,
                AutoSize = true,
                Cursor = Cursors.Hand
            };
            // Vertically center it and anchor it to the left side (X = 10)
            dragIcon.Location = new Point(10, (this.Height - dragIcon.PreferredHeight) / 2);

            var nameLabel = new Label
            {
                Text = Model.Exercise?.Name ?? "Exercise",
                Font = UIStyle.Body,
                // Shift the text to the right to make room for the icon
                Location = new Point(40, 12),
                AutoSize = true
            };

            this.Controls.Add(nameLabel);
            this.Controls.Add(dragIcon);

            // Selection triggers
            this.Click += (s, e) => RowSelected?.Invoke(this, EventArgs.Empty);
            nameLabel.Click += (s, e) => RowSelected?.Invoke(this, EventArgs.Empty);

            // Initiate dragging on either the entire row bar or handle icon
            this.MouseDown += InitiateDrag;
            dragIcon.MouseDown += InitiateDrag;

            this.Paint += (s, e) => {
                ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle, UIStyle.Border, ButtonBorderStyle.Solid);
            };
        }

        private void InitiateDrag(object? sender, MouseEventArgs e)
        {
            RowSelected?.Invoke(this, EventArgs.Empty);

            if (e.Button == MouseButtons.Left)
            {
                // Spawn miniature ghost container: set it slightly wider/taller & darker
                _dragGhost = new DragGhostForm(Model.Exercise?.Name ?? "Exercise");
                _dragGhost.Location = new Point(Cursor.Position.X + 10, Cursor.Position.Y + 10);
                _dragGhost.Show();

                this.GiveFeedback += Row_GiveFeedback;

                var originalColor = this.BackColor;
                this.BackColor = UIStyle.SurfaceVariant; // Dim the background item placeholder

                this.DoDragDrop(this, DragDropEffects.Move);

                // Teardown dragging states
                this.BackColor = originalColor;
                this.GiveFeedback -= Row_GiveFeedback;
                _dragGhost.Close();
                _dragGhost.Dispose();
                _dragGhost = null;
            }
        }

        private void Row_GiveFeedback(object? sender, GiveFeedbackEventArgs e)
        {
            e.UseDefaultCursors = true;
            if (_dragGhost != null)
            {
                _dragGhost.Location = new Point(Cursor.Position.X + 10, Cursor.Position.Y + 10);
            }
        }
    }
}
