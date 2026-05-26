using System;
using System.Drawing;
using System.Windows.Forms;

namespace RealView.Views
{
    public class AdminDashboardView : BaseView
    {
        private FlowLayoutPanel _mainLayout = null!;

        public AdminDashboardView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            _mainLayout = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                Padding = new Padding(40)
            };

            var title = new Label
            {
                Text = "Admin Control Panel",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 40)
            };

            _mainLayout.Controls.Add(title);

            AddNavButton("MANAGE EXERCISES", () => AppRuntime.Navigation.NavigateTo<ExerciseManagementView>());
            AddNavButton("MANAGE MUSCLE GROUPS", () => AppRuntime.Navigation.NavigateTo<MuscleGroupManagementView>());
            AddNavButton("MANAGE USERS", () => AppRuntime.Navigation.NavigateTo<UserManagementView>());
            AddNavButton("MANAGE WORKOUT TEMPLATES", () => AppRuntime.Navigation.NavigateTo<WorkoutManagementView>());

            this.Controls.Add(_mainLayout);
        }

        private void AddNavButton(string text, Action onClick)
        {
            var btn = new Button
            {
                Text = text,
                Size = new Size(400, 60),
                BackColor = Color.FromArgb(60, 64, 67),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Margin = new Padding(0, 0, 0, 20)
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Click += (s, e) => onClick();
            _mainLayout.Controls.Add(btn);
        }
    }
}
