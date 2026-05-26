using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RealView.Controls
{
    public partial class DragGhostForm : Form
    {
        public DragGhostForm(string exerciseName)
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.BackColor = Color.FromArgb(205, 210, 220); // Darker tone background 
            this.Size = new Size(375, 52); // Noticeably scaled up from 360x45 standard sizes
            this.StartPosition = FormStartPosition.Manual;

            var nameLabel = new Label
            {
                Text = exerciseName,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(15, 15),
                AutoSize = true,
                ForeColor = Color.Black
            };

            var iconLabel = new Label
            {
                Text = "↕",
                Font = new Font("Segoe UI", 15, FontStyle.Bold),
                Location = new Point(332, 12),
                AutoSize = true,
                ForeColor = Color.FromArgb(50, 50, 50)
            };

            this.Controls.Add(nameLabel);
            this.Controls.Add(iconLabel);

            this.Paint += (s, e) => {
                ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle, Color.FromArgb(130, 135, 145), ButtonBorderStyle.Solid);
            };
        }

        protected override bool ShowWithoutActivation => true;
    }
}
