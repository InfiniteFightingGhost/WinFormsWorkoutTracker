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
    public partial class DragGhostForm : Form
    {
        public DragGhostForm(string exerciseName)
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.BackColor = UIStyle.SurfaceVariant; // Darker tone background 
            this.Size = new Size(375, 52); // Noticeably scaled up from 360x45 standard sizes
            this.StartPosition = FormStartPosition.Manual;

            var nameLabel = new Label
            {
                Text = exerciseName,
                Font = UIStyle.BodySemibold,
                Location = new Point(15, 15),
                AutoSize = true,
                ForeColor = UIStyle.TextPrimary
            };

            var iconLabel = new Label
            {
                Text = "↕",
                Font = UIStyle.SubHeader,
                Location = new Point(332, 12),
                AutoSize = true,
                ForeColor = UIStyle.TextSecondary
            };

            this.Controls.Add(nameLabel);
            this.Controls.Add(iconLabel);

            this.Paint += (s, e) => {
                ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle, UIStyle.Border, ButtonBorderStyle.Solid);
            };
        }

        protected override bool ShowWithoutActivation => true;
    }
}
