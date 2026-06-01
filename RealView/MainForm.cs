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
        private Panel _sidebarIndicator = null!;

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        private const int WM_NCHITTEST = 0x84;
        private const int HTLEFT = 10;
        private const int HTRIGHT = 11;
        private const int HTTOP = 12;
        private const int HTTOPLEFT = 13;
        private const int HTTOPRIGHT = 14;
        private const int HTBOTTOM = 15;
        private const int HTBOTTOMLEFT = 16;
        private const int HTBOTTOMRIGHT = 17;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style |= 0x00040000; // WS_THICKFRAME
                return cp;
            }
        }

        public MainForm()
        {
            InitializeComponent();
            SetupShell();
            
            this.Resize += MainForm_Resize;

            // Listen for workout state changes to update sidebar
            AppRuntime.WorkoutState.WorkoutStarted += (s, e) => SetupSidebarButtons();
            AppRuntime.WorkoutState.WorkoutFinished += (s, e) => SetupSidebarButtons();

            // Listen for theme changes
            UIStyle.ThemeChanged += (s, e) => ApplyTheme();

            AppRuntime.Navigation.Navigated += (s, view) => UpdateSidebarIndicator(view.GetType());
        }

        private void UpdateSidebarIndicator(Type viewType)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => UpdateSidebarIndicator(viewType)));
                return;
            }

            foreach (Control c in _sidebarPanel.Controls)
            {
                if (c is ModernButton btn && btn.Tag as Type == viewType)
                {
                    _sidebarIndicator.Visible = _sidebarPanel.Visible;
                    _sidebarIndicator.Height = btn.Height - 20;
                    _sidebarIndicator.Location = new Point(_sidebarPanel.Left, _sidebarPanel.Top + btn.Top + 10);
                    _sidebarIndicator.BringToFront();
                    return;
                }
            }
            _sidebarIndicator.Visible = false;
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_NCHITTEST)
            {
                Point pos = PointToClient(Cursor.Position);
                const int gripSize = 10;

                if (pos.X <= gripSize && pos.Y <= gripSize) { m.Result = (IntPtr)HTTOPLEFT; return; }
                if (pos.X >= ClientSize.Width - gripSize && pos.Y <= gripSize) { m.Result = (IntPtr)HTTOPRIGHT; return; }
                if (pos.X <= gripSize && pos.Y >= ClientSize.Height - gripSize) { m.Result = (IntPtr)HTBOTTOMLEFT; return; }
                if (pos.X >= ClientSize.Width - gripSize && pos.Y >= ClientSize.Height - gripSize) { m.Result = (IntPtr)HTBOTTOMRIGHT; return; }
                if (pos.X <= gripSize) { m.Result = (IntPtr)HTLEFT; return; }
                if (pos.X >= ClientSize.Width - gripSize) { m.Result = (IntPtr)HTRIGHT; return; }
                if (pos.Y <= gripSize) { m.Result = (IntPtr)HTTOP; return; }
                if (pos.Y >= ClientSize.Height - gripSize) { m.Result = (IntPtr)HTBOTTOM; return; }
            }
            base.WndProc(ref m);
        }

        private void MainForm_Resize(object? sender, EventArgs e)
        {
            UpdateSidebarMode();
        }

        private void UpdateSidebarMode()
        {
            if (!_sidebarPanel.Visible) return;

            bool miniMode = this.Width < 1000;
            int targetWidth = miniMode ? UIStyle.SidebarMiniWidth : UIStyle.SidebarWidth;
            
            if (_sidebarPanel.Width != targetWidth)
            {
                _sidebarPanel.Width = targetWidth;
                SetupSidebarButtons();
            }
        }

        private void ApplyTheme()
        {
            this.BackColor = UIStyle.Background;
            _titleBar.BackColor = UIStyle.Sidebar;
            _titleLabel.ForeColor = UIStyle.TextOnSidebar;
            _sidebarPanel.BackColor = UIStyle.Sidebar;
            _contentPanel.BackColor = UIStyle.Background;

            // Refresh sidebar to update button colors
            SetupSidebarButtons();

            // Re-navigate to current view to refresh its theme
            AppRuntime.Navigation.RefreshCurrentView();

            // Update title bar control buttons
            foreach (Control c in _titleBar.Controls)
            {
                if (c is FlowLayoutPanel flp)
                {
                    foreach (Control btn in flp.Controls)
                    {
                        if (btn is Button b)
                        {
                            b.BackColor = UIStyle.Sidebar;
                            b.ForeColor = UIStyle.TextOnSidebar;
                            b.FlatAppearance.MouseOverBackColor = b.Text == "×" ? UIStyle.Danger : UIStyle.SidebarHover;
                        }
                    }
                }
            }
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
                ForeColor = UIStyle.TextOnSidebar,
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

            _sidebarIndicator = new Panel
            {
                Width = 4,
                BackColor = UIStyle.Primary,
                Visible = false
            };

            this.Controls.Add(_sidebarIndicator);
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

            var closeBtn = CreateControlBtn("×", UIStyle.Danger, () => Application.Exit());
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
                ForeColor = UIStyle.TextOnSidebar,
                Font = UIStyle.Body,
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

            bool miniMode = _sidebarPanel.Width <= UIStyle.SidebarMiniWidth;

            // Branding/Logo area
            var logoLabel = new Label
            {
                Text = miniMode ? "WT" : "WORKOUT TRACKER",
                ForeColor = UIStyle.TextOnSidebar,
                Font = UIStyle.SubHeader,
                Size = new Size(_sidebarPanel.Width, 80),
                TextAlign = ContentAlignment.MiddleCenter,
                Margin = new Padding(0, 0, 0, 20)
            };
            _sidebarPanel.Controls.Add(logoLabel);

            // Active Workout (Dynamic)
            if (AppRuntime.WorkoutState.IsWorkoutActive)
            {
                AddSidebarButton("ACTIVE WORKOUT", "🔥", () => AppRuntime.Navigation.NavigateTo<Views.ActiveWorkoutView>(), typeof(Views.ActiveWorkoutView), UIStyle.Success);
            }

            // Basic sidebar buttons
            AddSidebarButton("Dashboard", "📊", () => AppRuntime.Navigation.NavigateTo<Views.DashboardView>(), typeof(Views.DashboardView));
            AddSidebarButton("History", "🕒", () => AppRuntime.Navigation.NavigateTo<Views.HistoryView>(), typeof(Views.HistoryView));
            AddSidebarButton("Settings", "⚙️", () => AppRuntime.Navigation.NavigateTo<Views.SettingsView>(), typeof(Views.SettingsView));

            if (user.Role == Data.Enums.UserRole.Admin)
            {
                var spacer = new Panel { Height = 20, Width = _sidebarPanel.Width };
                _sidebarPanel.Controls.Add(spacer);
                AddSidebarButton("Admin Panel", "🛡️", () => AppRuntime.Navigation.NavigateTo<Views.AdminDashboardView>(), typeof(Views.AdminDashboardView), UIStyle.SidebarHover);
            }
        }

        private void AddSidebarButton(string text, string icon, Action onClick, Type viewType, Color? backColor = null)
        {
            bool miniMode = _sidebarPanel.Width <= UIStyle.SidebarMiniWidth;
            var btn = new ModernButton
            {
                Text = text,
                Icon = icon,
                ShowText = !miniMode,
                Tag = viewType,
                Size = miniMode ? new Size(60, 60) : new Size(230, 50),
                NormalColor = backColor ?? UIStyle.Sidebar,
                HoverColor = UIStyle.SidebarHover,
                Font = UIStyle.Body,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(15, 0, 0, 0),
                Margin = miniMode ? new Padding(10, 10, 10, 10) : new Padding(10, 5, 10, 5),
                BorderRadius = 8
            };
            btn.Click += (s, e) => onClick();
            _sidebarPanel.Controls.Add(btn);
        }
    }
}
