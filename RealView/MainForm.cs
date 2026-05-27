using System.Windows.Forms;
using RealView.Services;

namespace RealView
{
    public partial class MainForm : Form
    {
        private Panel _contentPanel = null!;
        private FlowLayoutPanel _sidebarPanel = null!;

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

            // Create Content Panel
            _contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(245, 247, 251)
            };

            // Create Sidebar
            _sidebarPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Left,
                Width = 250,
                BackColor = Color.FromArgb(32, 33, 36),
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Visible = false // Hide until logged in
            };

            this.Controls.Add(_contentPanel);
            this.Controls.Add(_sidebarPanel);

            AppRuntime.Navigation = new NavigationService(_contentPanel);
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
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Size = new Size(250, 80),
                TextAlign = ContentAlignment.MiddleCenter,
                Margin = new Padding(0, 0, 0, 20)
            };
            _sidebarPanel.Controls.Add(logoLabel);

            // Active Workout (Dynamic)
            if (AppRuntime.WorkoutState.IsWorkoutActive)
            {
                AddSidebarButton("ACTIVE WORKOUT", () => AppRuntime.Navigation.NavigateTo<Views.ActiveWorkoutView>(), Color.FromArgb(40, 167, 69));
            }

            // Basic sidebar buttons
            AddSidebarButton("Dashboard", () => AppRuntime.Navigation.NavigateTo<Views.DashboardView>());
            AddSidebarButton("History", () => AppRuntime.Navigation.NavigateTo<Views.HistoryView>());
            AddSidebarButton("Settings", () => AppRuntime.Navigation.NavigateTo<Views.SettingsView>());

            if (user.Role == Data.Enums.UserRole.Admin)
            {
                var spacer = new Panel { Height = 20, Width = 250 };
                _sidebarPanel.Controls.Add(spacer);
                AddSidebarButton("Admin Panel", () => AppRuntime.Navigation.NavigateTo<Views.AdminDashboardView>(), Color.FromArgb(60, 64, 67));
            }
        }

        private void AddSidebarButton(string text, Action onClick, Color? backColor = null)
        {
            var btn = new Button
            {
                Text = text,
                Size = new Size(250, 60),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                BackColor = backColor ?? Color.Transparent,
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(20, 0, 0, 0),
                Margin = new Padding(0)
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 64, 67);
            btn.Click += (s, e) => onClick();
            _sidebarPanel.Controls.Add(btn);
        }
    }
}
