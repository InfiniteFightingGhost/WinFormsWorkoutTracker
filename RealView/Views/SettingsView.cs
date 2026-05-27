using System;
using System.Drawing;
using System.Windows.Forms;

namespace RealView.Views
{
    public class SettingsView : BaseView
    {
        private Panel _card;
        private NumericUpDown _heightNum = null!;
        private NumericUpDown _weightNum = null!;

        public SettingsView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.BackColor = Color.FromArgb(245, 247, 251);

            _card = new Panel
            {
                Size = new Size(500, 600),
                BackColor = Color.White,
                Padding = new Padding(40)
            };

            this.Resize += (s, e) => {
                _card.Location = new Point((this.Width - _card.Width) / 2, (this.Height - _card.Height) / 2);
            };

            var layout = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false
            };

            var title = new Label
            {
                Text = "Settings",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 30)
            };

            var user = AppRuntime.Auth.GetCurrentUser();

            // Account Section
            var accountLabel = new Label { Text = "ACCOUNT", Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.Gray, Margin = new Padding(0, 0, 0, 10) };
            var usernameLabel = new Label { Text = $"Username: {user?.Username}", Font = new Font("Segoe UI", 11), AutoSize = true, Margin = new Padding(0, 0, 0, 5) };
            var emailLabel = new Label { Text = $"Email: {user?.Email}", Font = new Font("Segoe UI", 11), AutoSize = true, Margin = new Padding(0, 0, 0, 20) };

            // Metrics Section
            var metricsLabel = new Label { Text = "PERSONAL METRICS", Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.Gray, Margin = new Padding(0, 10, 0, 10) };
            
            _heightNum = CreateNumericInput("Height (cm)", user?.Height ?? 0, 300, layout);
            _weightNum = CreateNumericInput("Weight (kg)", user?.Weight ?? 0, 999, layout);

            var saveBtn = new Button
            {
                Text = "SAVE CHANGES",
                Size = new Size(420, 50),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Margin = new Padding(0, 30, 0, 0)
            };
            saveBtn.FlatAppearance.BorderSize = 0;
            saveBtn.Click += SaveBtn_Click;

            var logoutBtn = new Button
            {
                Text = "LOGOUT",
                Size = new Size(420, 50),
                BackColor = Color.White,
                ForeColor = Color.IndianRed,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Margin = new Padding(0, 15, 0, 0)
            };
            logoutBtn.FlatAppearance.BorderColor = Color.IndianRed;
            logoutBtn.FlatAppearance.BorderSize = 1;
            logoutBtn.Click += (s, e) => {
                AppRuntime.Auth.Logout();
                AppRuntime.Navigation.ClearCache();
                if (this.ParentForm is MainForm shell) shell.SetSidebarVisible(false);
                AppRuntime.Navigation.NavigateTo<LoginView>();
            };

            layout.Controls.Add(title);
            layout.Controls.Add(accountLabel);
            layout.Controls.Add(usernameLabel);
            layout.Controls.Add(emailLabel);
            layout.Controls.Add(metricsLabel);
            // Height and weight are added by CreateNumericInput
            layout.Controls.Add(saveBtn);
            layout.Controls.Add(logoutBtn);

            _card.Controls.Add(layout);
            this.Controls.Add(_card);

            _card.Paint += (s, e) => {
                ControlPaint.DrawBorder(e.Graphics, _card.ClientRectangle, Color.LightGray, ButtonBorderStyle.Solid);
            };
        }

        private NumericUpDown CreateNumericInput(string label, decimal value, decimal max, FlowLayoutPanel container)
        {
            var lbl = new Label { Text = label, AutoSize = true, Margin = new Padding(0, 10, 0, 5), Font = new Font("Segoe UI", 10) };
            var num = new NumericUpDown { Maximum = max, Width = 420, Font = new Font("Segoe UI", 11), DecimalPlaces = 1 };
            num.Value = value > max ? max : value;
            container.Controls.Add(lbl);
            container.Controls.Add(num);
            return num;
        }

        private async void SaveBtn_Click(object? sender, EventArgs e)
        {
            try
            {
                var user = AppRuntime.Auth.GetCurrentUser();
                if (user == null) return;
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
