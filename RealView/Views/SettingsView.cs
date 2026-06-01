using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Threading.Tasks;
using RealView.Controls;
using System.Drawing.Drawing2D;

namespace RealView.Views
{
    public class SettingsView : BaseView
    {
        private Panel _mainContainer = null!;
        private Panel _menuPanel = null!;
        private Panel _contentPanel = null!;
        private Label _sectionTitle = null!;
        private FlowLayoutPanel _settingsLayout = null!;
        
        // Settings State
        private NumericUpDown _heightNum = null!;
        private NumericUpDown _weightNum = null!;
        private PictureBox _profilePic = null!;
        private string _activeSection = "Account";

        public SettingsView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.BackColor = UIStyle.Background;

            _mainContainer = new Panel
            {
                BackColor = UIStyle.Surface,
                Padding = new Padding(1)
            };

            // Left Side: Menu
            _menuPanel = new Panel
            {
                Width = 250,
                Dock = DockStyle.Left,
                BackColor = UIStyle.Surface,
                Padding = new Padding(10, 40, 10, 10)
            };

            var menuLayout = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false
            };

            AddMenuButton("👤 Account", "Account", menuLayout);
            AddMenuButton("📏 Metrics", "Metrics", menuLayout);
            AddMenuButton("🎨 Appearance", "Appearance", menuLayout);
            AddMenuButton("🔒 Security", "Security", menuLayout);
            AddMenuButton("🚀 About", "About", menuLayout);

            var logoutSpacer = new Panel { Height = 50, Dock = DockStyle.Bottom };
            var logoutBtn = new ModernButton
            {
                Text = "Logout",
                Icon = "🚪",
                Size = new Size(230, 45),
                NormalColor = Color.Transparent,
                HoverColor = Color.FromArgb(20, UIStyle.Danger),
                ForeColor = UIStyle.Danger,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(15, 0, 0, 0),
                BorderRadius = 8,
                Dock = DockStyle.Bottom
            };
            logoutBtn.Click += LogoutBtn_Click;

            _menuPanel.Controls.Add(menuLayout);
            _menuPanel.Controls.Add(logoutBtn);
            _menuPanel.Controls.Add(logoutSpacer);

            // Right Side: Content
            _contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = UIStyle.Background,
                Padding = new Padding(40)
            };

            _sectionTitle = new Label
            {
                Text = "Account Settings",
                Font = UIStyle.Header,
                ForeColor = UIStyle.TextPrimary,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 30)
            };

            _settingsLayout = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                Padding = new Padding(0, 20, 0, 0)
            };

            _contentPanel.Controls.Add(_settingsLayout);
            _contentPanel.Controls.Add(_sectionTitle);

            _mainContainer.Controls.Add(_contentPanel);
            _mainContainer.Controls.Add(new Panel { Dock = DockStyle.Left, Width = 1, BackColor = UIStyle.Border });
            _mainContainer.Controls.Add(_menuPanel);

            this.Controls.Add(_mainContainer);
            
            ShowSection("Account");
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (_mainContainer == null) return;

            // Maintain a nice 1000px centered container or fill if smaller
            int targetWidth = Math.Min(1000, this.Width - 80);
            int targetHeight = Math.Min(800, this.Height - 80);

            _mainContainer.Size = new Size(targetWidth, targetHeight);
            _mainContainer.Location = new Point((this.Width - targetWidth) / 2, (this.Height - targetHeight) / 2);

            // Responsive Menu
            if (targetWidth < 600)
            {
                _menuPanel.Width = 60;
                foreach (Control c in _menuPanel.Controls)
                    if (c is FlowLayoutPanel flp)
                        foreach (Control b in flp.Controls)
                            if (b is ModernButton mb) mb.ShowText = false;
            }
            else
            {
                _menuPanel.Width = 250;
                foreach (Control c in _menuPanel.Controls)
                    if (c is FlowLayoutPanel flp)
                        foreach (Control b in flp.Controls)
                            if (b is ModernButton mb) mb.ShowText = true;
            }
        }

        private void AddMenuButton(string text, string section, FlowLayoutPanel container)
        {
            var btn = new ModernButton
            {
                Text = text,
                Tag = section,
                Size = new Size(230, 45),
                NormalColor = Color.Transparent,
                HoverColor = UIStyle.Selection,
                ForeColor = UIStyle.TextPrimary,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(15, 0, 0, 0),
                Margin = new Padding(0, 2, 0, 2),
                BorderRadius = 8
            };
            btn.Click += (s, e) => ShowSection(section);
            container.Controls.Add(btn);
        }

        private void ShowSection(string section)
        {
            _activeSection = section;
            _sectionTitle.Text = $"{section} Settings";
            _settingsLayout.Controls.Clear();

            // Update Menu Selection Visuals
            foreach (Control c in _menuPanel.Controls)
                if (c is FlowLayoutPanel flp)
                    foreach (Control b in flp.Controls)
                        if (b is ModernButton mb)
                            mb.NormalColor = mb.Tag?.ToString() == section ? UIStyle.Selection : Color.Transparent;

            switch (section)
            {
                case "Account": LoadAccountSection(); break;
                case "Metrics": LoadMetricsSection(); break;
                case "Appearance": LoadAppearanceSection(); break;
                case "Security": LoadSecuritySection(); break;
                case "About": LoadAboutSection(); break;
            }
        }

        private void LoadAccountSection()
        {
            var user = AppRuntime.Auth.GetCurrentUser();
            if (user == null) return;

            AddSubHeader("Profile Photo");
            _profilePic = new PictureBox
            {
                Size = new Size(120, 120),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = UIStyle.SurfaceVariant,
                Image = Services.PhotoService.LoadPhoto(user.PhotoUrl),
                Margin = new Padding(0, 10, 0, 10)
            };
            _profilePic.Paint += (s, e) => {
                using (var path = new GraphicsPath()) {
                    path.AddEllipse(0, 0, _profilePic.Width - 1, _profilePic.Height - 1);
                    _profilePic.Region = new Region(path);
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    e.Graphics.DrawEllipse(new Pen(UIStyle.Border, 2), 0, 0, _profilePic.Width - 1, _profilePic.Height - 1);
                }
            };
            _settingsLayout.Controls.Add(_profilePic);

            var changePhotoBtn = CreateSecondaryButton("Change Photo", UpdatePhotoBtn_Click);
            _settingsLayout.Controls.Add(changePhotoBtn);

            AddDivider();

            AddSubHeader("Account Details");
            AddInfoField("Username", user.Username);
            AddInfoField("Email Address", user.Email);
            AddInfoField("Member Since", "June 2026");
        }

        private void LoadMetricsSection()
        {
            var user = AppRuntime.Auth.GetCurrentUser();
            if (user == null) return;

            AddSubHeader("Physical Information");
            _heightNum = CreateNumericInput("Height (cm)", user.Height, 300);
            _weightNum = CreateNumericInput("Weight (kg)", user.Weight, 999);

            var saveBtn = CreatePrimaryButton("Save Metrics", SaveMetrics_Click);
            _settingsLayout.Controls.Add(saveBtn);
        }

        private void LoadAppearanceSection()
        {
            AddSubHeader("Theme Preference");
            var themeBtn = new ModernButton
            {
                Text = UIStyle.CurrentTheme == ThemeType.Light ? "Switch to Dark Mode" : "Switch to Light Mode",
                Icon = UIStyle.CurrentTheme == ThemeType.Light ? "🌙" : "☀️",
                Size = new Size(300, 50),
                NormalColor = UIStyle.SurfaceVariant,
                HoverColor = UIStyle.Selection,
                ForeColor = UIStyle.TextPrimary,
                Font = UIStyle.BodySemibold,
                BorderRadius = 8,
                Margin = new Padding(0, 10, 0, 0)
            };
            themeBtn.Click += (s, e) => {
                UIStyle.SetTheme(UIStyle.CurrentTheme == ThemeType.Light ? ThemeType.Dark : ThemeType.Light);
                ShowSection("Appearance");
            };
            _settingsLayout.Controls.Add(themeBtn);
        }

        private void LoadSecuritySection()
        {
            AddSubHeader("Password Management");
            _settingsLayout.Controls.Add(CreateSecondaryButton("Change Password", (s, e) => AppRuntime.Toasts.Show("Feature coming soon!")));
            
            AddDivider();
            
            AddSubHeader("Privacy");
            AddInfoField("Profile Visibility", "Public");
            AddInfoField("Data Sharing", "Enabled");
        }

        private void LoadAboutSection()
        {
            AddSubHeader("Workout Tracker v2.0");
            AddInfoField("Developer", "Andrean Bashkehayov");
            
            AddDivider();
            
            var githubBtn = CreateSecondaryButton("View on GitHub", (s, e) => System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("https://github.com/InfiniteFightingGhost/WinFormsWorkoutTracker") { UseShellExecute = true }));
            _settingsLayout.Controls.Add(githubBtn);
        }

        // Helpers
        private void AddSubHeader(string text)
        {
            _settingsLayout.Controls.Add(new Label { Text = text.ToUpper(), Font = UIStyle.CaptionBold, ForeColor = UIStyle.TextSecondary, AutoSize = true, Margin = new Padding(0, 20, 0, 10) });
        }

        private void AddDivider()
        {
            _settingsLayout.Controls.Add(new Panel { Height = 1, Width = 500, BackColor = UIStyle.Border, Margin = new Padding(0, 20, 0, 20) });
        }

        private void AddInfoField(string label, string value)
        {
            var p = new Panel { Width = 500, Height = 40, Margin = new Padding(0, 5, 0, 5) };
            p.Controls.Add(new Label { Text = label, Font = UIStyle.Body, ForeColor = UIStyle.TextSecondary, AutoSize = true, Location = new Point(0, 10) });
            p.Controls.Add(new Label { Text = value, Font = UIStyle.BodySemibold, ForeColor = UIStyle.TextPrimary, AutoSize = true, Location = new Point(150, 10) });
            _settingsLayout.Controls.Add(p);
        }

        private NumericUpDown CreateNumericInput(string label, decimal value, decimal max)
        {
            var p = new Panel { Width = 500, Height = 70 };
            p.Controls.Add(new Label { Text = label, Font = UIStyle.CaptionBold, ForeColor = UIStyle.TextSecondary, AutoSize = true, Location = new Point(0, 5) });
            var num = new NumericUpDown { Maximum = max, Width = 300, Location = new Point(0, 30), Font = UIStyle.Body, DecimalPlaces = 1, BackColor = UIStyle.Surface, ForeColor = UIStyle.TextPrimary };
            num.Value = Math.Min(value, max);
            p.Controls.Add(num);
            _settingsLayout.Controls.Add(p);
            return num;
        }

        private ModernButton CreatePrimaryButton(string text, EventHandler onClick)
        {
            var btn = new ModernButton { Text = text, Size = new Size(300, 50), NormalColor = UIStyle.Primary, HoverColor = UIStyle.PrimaryHover, ForeColor = UIStyle.TextOnPrimary, Font = UIStyle.BodySemibold, BorderRadius = 8, Margin = new Padding(0, 20, 0, 0) };
            btn.Click += onClick;
            return btn;
        }

        private ModernButton CreateSecondaryButton(string text, EventHandler onClick)
        {
            var btn = new ModernButton { Text = text, Size = new Size(150, 35), NormalColor = UIStyle.SurfaceVariant, HoverColor = UIStyle.Selection, ForeColor = UIStyle.TextPrimary, Font = UIStyle.CaptionBold, BorderRadius = 6, Margin = new Padding(0, 5, 0, 5) };
            btn.Click += onClick;
            return btn;
        }

        // Event Handlers
        private async void UpdatePhotoBtn_Click(object? sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog { Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    var user = AppRuntime.Auth.GetCurrentUser();
                    if (user == null) return;
                    string? localPath = Services.PhotoService.SavePhoto(ofd.FileName, "Users");
                    if (localPath != null) {
                        await AppRuntime.User.UpdatePhotoAsync(user.Id, localPath);
                        user.PhotoUrl = localPath;
                        _profilePic.Image = Services.PhotoService.LoadPhoto(localPath);
                        AppRuntime.Toasts.Show("Photo updated!", Services.ToastType.Success);
                    }
                }
            }
        }

        private async void SaveMetrics_Click(object? sender, EventArgs e)
        {
            try {
                var user = AppRuntime.Auth.GetCurrentUser();
                if (user == null) return;
                await AppRuntime.User.UpdateHeightAsync(user.Id, _heightNum.Value);
                await AppRuntime.User.UpdateWeightAsync(user.Id, _weightNum.Value);
                AppRuntime.Toasts.Show("Metrics saved!", Services.ToastType.Success);
            } catch (Exception ex) { AppRuntime.Toasts.Show(ex.Message, Services.ToastType.Error); }
        }

        private void LogoutBtn_Click(object? sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to logout?", "Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
                AppRuntime.Auth.Logout();
                AppRuntime.Navigation.ClearCache();
                if (this.ParentForm is MainForm shell) shell.SetSidebarVisible(false);
                AppRuntime.Navigation.NavigateTo<LoginView>();
            }
        }
    }
}
