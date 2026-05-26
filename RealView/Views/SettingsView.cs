using System;
using System.Drawing;
using System.Windows.Forms;

namespace RealView.Views
{
    public class SettingsView : BaseView
    {
        private FlowLayoutPanel _mainLayout = null!;
        private NumericUpDown _heightNum = null!;
        private NumericUpDown _weightNum = null!;

        public SettingsView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            _mainLayout = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                Padding = new Padding(40),
                AutoScroll = true,
                WrapContents = false
            };

            var title = new Label
            {
                Text = "Profile Settings",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 30)
            };

            var user = AppRuntime.Auth.GetCurrentUser();
            var infoLabel = new Label
            {
                Text = $"Username: {user?.Username}\nEmail: {user?.Email}\nRole: {user?.Role}",
                Font = new Font("Segoe UI", 12),
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 30)
            };

            _mainLayout.Controls.Add(title);
            _mainLayout.Controls.Add(infoLabel);

            _heightNum = CreateNumericInput("Height (cm)", user?.Height ?? 0, 300, _mainLayout);
            _weightNum = CreateNumericInput("Weight (kg)", user?.Weight ?? 0, 999, _mainLayout);

            var saveBtn = new Button
            {
                Text = "SAVE CHANGES",
                Size = new Size(200, 50),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Margin = new Padding(0, 30, 0, 0)
            };
            saveBtn.Click += SaveBtn_Click;
            _mainLayout.Controls.Add(saveBtn);

            this.Controls.Add(_mainLayout);
        }

        private NumericUpDown CreateNumericInput(string label, decimal value, decimal max, Control container)
        {
            var pnl = new FlowLayoutPanel { AutoSize = true, FlowDirection = FlowDirection.LeftToRight, Margin = new Padding(0, 0, 0, 15) };
            var lbl = new Label { Text = label, Width = 100, Font = new Font("Segoe UI", 10), Padding = new Padding(0, 5, 0, 0) };
            var num = new NumericUpDown { Value = value > max ? max : value, Maximum = max, Width = 100, Font = new Font("Segoe UI", 10), DecimalPlaces = 1 };
            pnl.Controls.Add(lbl);
            pnl.Controls.Add(num);
            container.Controls.Add(pnl);
            return num;
        }

        private async void SaveBtn_Click(object? sender, EventArgs e)
        {
            try
            {
                var user = AppRuntime.Auth.GetCurrentUser();
                await AppRuntime.User.UpdateHeightAsync(user.Id, _heightNum.Value);
                await AppRuntime.User.UpdateWeightAsync(user.Id, _weightNum.Value);
                
                MessageBox.Show("Profile updated successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
