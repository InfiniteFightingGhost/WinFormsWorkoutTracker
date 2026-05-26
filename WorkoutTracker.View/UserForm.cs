using Controller;
using Data;
using System;
using System.Windows.Forms;
using WorkoutTracker.View;
using WorkoutTracker.View.Services;
using WorkoutTracker.View.Views;

namespace View
{
    public partial class UserForm : Form
    {
        public UserForm()
        {
            InitializeComponent();
            
            // Initialize navigation
            var contentPanel = new Panel { Dock = DockStyle.Fill };
            this.Controls.Add(contentPanel);
            contentPanel.BringToFront();
            panel1.BringToFront(); // Navigation bar stays on top/bottom

            AppRuntime.Navigation = new NavigationService(contentPanel);
            
            // Set up event handlers for bottom navigation (assuming these are the panels from designer)
            panel3.Click += (s, e) => AppRuntime.Navigation.NavigateTo<DashboardView>();
            panel4.Click += (s, e) => AppRuntime.Navigation.NavigateTo<HistoryView>(); 
            panel2.Click += (s, e) => AppRuntime.Navigation.NavigateTo<SettingsView>();

            var user = AppRuntime.Auth.GetCurrentUser();
            if (user != null && user.Role == Data.Enums.UserRole.Admin)
            {
                // Let's add a small admin button or hijack one for now.
                // For a "modern" feel, maybe a floating button or just another nav item.
                // Hijacking the label of panel2 if admin? No, let's just add it to the top or side.
                var adminBtn = new Button { 
                    Text = "ADMIN", 
                    Dock = DockStyle.Top, 
                    Height = 30, 
                    BackColor = System.Drawing.Color.DarkRed, 
                    ForeColor = System.Drawing.Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                adminBtn.Click += (s, e) => AppRuntime.Navigation.NavigateTo<AdminDashboardView>();
                this.Controls.Add(adminBtn);
                adminBtn.BringToFront();
            }
            
            // Navigate to initial view
            this.Load += (s, e) => AppRuntime.Navigation.NavigateTo<DashboardView>();
        }
    }
}
