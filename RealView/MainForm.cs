using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using RealView.Controls;
using RealView.Services;

namespace RealView
{
    public partial class MainForm : Form
    {
        private Panel _titleBar = null!;
        private Panel _contentPanel = null!;
        private FlowLayoutPanel _sidebarPanel = null!;
        private Label _titleLabel = null!;

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        public MainForm()
        {
            InitializeComponent();
            SetupShell();
            
            // Listen for workout state changes to update sidebar
            AppRuntime.WorkoutState.WorkoutStarted += (s, e) => SetupSidebarButtons();
            AppRuntime.WorkoutState.WorkoutFinished += (s, e) => SetupSidebarButtons();
        }

        private void SetupShell()
        {
            this.Text = "Workout Tracker";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = UIStyle.Background;

            // 1. Create Title Bar
            _titleBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = UIStyle.TitleBarHeight,
                BackColor = UIStyle.Sidebar,
                Padding = new Padding(15, 0, 0, 0)
            };
            _titleBar.MouseDown += TitleBar_MouseDown;

            _titleLabel = new Label
            {
                Text = "Workout Tracker",
                ForeColor = Color.White,
                Font = UIStyle.CaptionBold,
                AutoSize = true,
                Location = new Point(15, (UIStyle.TitleBarHeight - 15) / 2),
                Enabled = false // Allow click-through to panel
            };
            _titleBar.Controls.Add(_titleLabel);

            // Control Buttons
            AddControlStack();

            // 2. Create Sidebar
            _sidebarPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Left,
                Width = 250,
                BackColor = UIStyle.Sidebar,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Visible = false // Hide until logged in
            };

            // 3. Create Content Panel
            _contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = UIStyle.Background
            };

            this.Controls.Add(_contentPanel);
            this.Controls.Add(_sidebarPanel);
            this.Controls.Add(_titleBar);

            AppRuntime.Navigation = new NavigationService(_contentPanel);
        }

        private void AddControlStack()
        {
            var controlPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Right,
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight
            };

            var closeBtn = CreateControlBtn("×", Color.FromArgb(232, 17, 35), () => Application.Exit());
            var maxBtn = CreateControlBtn("▢", null, () => {
                this.WindowState = this.WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized;
            });
            var minBtn = CreateControlBtn("−", null, () => this.WindowState = FormWindowState.Minimized);

            controlPanel.Controls.Add(minBtn);
            controlPanel.Controls.Add(maxBtn);
            controlPanel.Controls.Add(closeBtn);
            _titleBar.Controls.Add(controlPanel);
        }

        private Button CreateControlBtn(string text, Color? hoverColor, Action onClick)
        {
            var btn = new Button
            {
                Text = text,
                Size = new Size(45, UIStyle.TitleBarHeight),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10),
                BackColor = UIStyle.Sidebar,
                Margin = new Padding(0)
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = hoverColor ?? UIStyle.SidebarHover;
            btn.Click += (s, e) => onClick();
            return btn;
        }

        private void TitleBar_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        public void SetSidebarVisible(bool visible)
        {
            _sidebarPanel.Visible = visible;
            if (visible)
            {
                SetupSidebarButtons();
            }
        }

        private void SetupSidebarButtons()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(SetupSidebarButtons));
                return;
            }

            _sidebarPanel.Controls.Clear();
            
            var user = AppRuntime.Auth.GetCurrentUser();
            if (user == null) return;

            // Branding/Logo area
            var logoLabel = new Label
            {
                Text = "WORKOUT TRACKER",
                ForeColor = Color.White,
                Font = UIStyle.SubHeader,
                Size = new Size(250, 80),
                TextAlign = ContentAlignment.MiddleCenter,
                Margin = new Padding(0, 0, 0, 20)
            };
            _sidebarPanel.Controls.Add(logoLabel);

            // Active Workout (Dynamic)
            if (AppRuntime.WorkoutState.IsWorkoutActive)
            {
                AddSidebarButton("ACTIVE WORKOUT", () => AppRuntime.Navigation.NavigateTo<Views.ActiveWorkoutView>(), UIStyle.Success);
            }

            // Basic sidebar buttons
            AddSidebarButton("Dashboard", () => AppRuntime.Navigation.NavigateTo<Views.DashboardView>());
            AddSidebarButton("History", () => AppRuntime.Navigation.NavigateTo<Views.HistoryView>());
            AddSidebarButton("Settings", () => AppRuntime.Navigation.NavigateTo<Views.SettingsView>());

            if (user.Role == Data.Enums.UserRole.Admin)
            {
                var spacer = new Panel { Height = 20, Width = 250 };
                _sidebarPanel.Controls.Add(spacer);
                AddSidebarButton("Admin Panel", () => AppRuntime.Navigation.NavigateTo<Views.AdminDashboardView>(), UIStyle.SidebarHover);
            }
        }

        private void AddSidebarButton(string text, Action onClick, Color? backColor = null)
        {
            var btn = new ModernButton
            {
                Text = text,
                Size = new Size(230, 50),
                NormalColor = backColor ?? UIStyle.Sidebar,
                HoverColor = UIStyle.SidebarHover,
                Font = UIStyle.Body,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(15, 0, 0, 0),
                Margin = new Padding(10, 5, 10, 5),
                BorderRadius = 8
            };
            btn.Click += (s, e) => onClick();
            _sidebarPanel.Controls.Add(btn);
        }
    }
}
