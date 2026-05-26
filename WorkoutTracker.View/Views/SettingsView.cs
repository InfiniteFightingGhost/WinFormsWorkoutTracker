using System;
using System.Drawing;
using System.Windows.Forms;
using Data.Entities;

namespace WorkoutTracker.View.Views
{
    public class SettingsView : BaseView
    {
        private FlowLayoutPanel _mainLayout;
        private Label _titleLabel;
        private PropertyGrid _userPropertyGrid;
        private Button _logoutButton;

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
                Padding = new Padding(20),
                AutoScroll = true,
                WrapContents = false
            };

            _titleLabel = new Label
            {
                Text = "Profile & Settings",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 20)
            };

            var user = AppRuntime.Auth.GetCurrentUser();
            var infoLabel = new Label
            {
                Text = $"Logged in as: {user?.Username}\nEmail: {user?.Email}\nRole: {user?.Role}",
                Font = new Font("Segoe UI", 12),
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 20)
            };

            var heightPanel = new FlowLayoutPanel { AutoSize = true };
            var lblHeight = new Label { Text = "Height (cm):", AutoSize = true, Margin = new Padding(0, 5, 5, 0) };
            var txtHeight = new TextBox { Text = user?.Height.ToString() ?? "0", Width = 60 };
            var btnHeight = new Button { Text = "Update", Height = 25 };
            btnHeight.Click += async (s, e) => {
                await AppRuntime.User.UpdateHeightAsync(user.Id, decimal.Parse(txtHeight.Text));
                MessageBox.Show("Height updated!");
            };
            heightPanel.Controls.AddRange(new Control[] { lblHeight, txtHeight, btnHeight });

            var weightPanel = new FlowLayoutPanel { AutoSize = true };
            var lblWeight = new Label { Text = "Weight (kg):", AutoSize = true, Margin = new Padding(0, 5, 5, 0) };
            var txtWeight = new TextBox { Text = user?.Weight.ToString() ?? "0", Width = 60 };
            var btnWeight = new Button { Text = "Update", Height = 25 };
            btnWeight.Click += async (s, e) => {
                await AppRuntime.User.UpdateWeightAsync(user.Id, decimal.Parse(txtWeight.Text));
                MessageBox.Show("Weight updated!");
            };
            weightPanel.Controls.AddRange(new Control[] { lblWeight, txtWeight, btnWeight });

            _logoutButton = new Button
            {
                Text = "LOGOUT",
                BackColor = Color.IndianRed,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(150, 40),
                Margin = new Padding(0, 20, 0, 0)
            };
            _logoutButton.Click += LogoutButton_Click;

            _mainLayout.Controls.Add(_titleLabel);
            _mainLayout.Controls.Add(infoLabel);
            _mainLayout.Controls.Add(heightPanel);
            _mainLayout.Controls.Add(weightPanel);
            _mainLayout.Controls.Add(_logoutButton);

            this.Controls.Add(_mainLayout);
        }

        private void LogoutButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to logout?", "Logout", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Application.Restart();
            }
        }
    }
}
