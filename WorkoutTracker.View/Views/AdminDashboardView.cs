using System;
using System.Drawing;
using System.Windows.Forms;

namespace WorkoutTracker.View.Views
{
    public class AdminDashboardView : BaseView
    {
        private FlowLayoutPanel _mainLayout;

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
                Padding = new Padding(20)
            };

            var title = new Label
            {
                Text = "Admin Dashboard",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 20)
            };

            var btnExercises = CreateNavButton("Manage Exercises", () => AppRuntime.Navigation.NavigateTo<ExerciseManagementView>());
            var btnMuscleGroups = CreateNavButton("Manage Muscle Groups", () => AppRuntime.Navigation.NavigateTo<MuscleGroupManagementView>());
            var btnWorkouts = CreateNavButton("Manage Workout Templates", () => AppRuntime.Navigation.NavigateTo<WorkoutManagementView>());
            var btnUsers = CreateNavButton("Manage Users", () => AppRuntime.Navigation.NavigateTo<UserManagementView>());

            _mainLayout.Controls.Add(title);
            _mainLayout.Controls.Add(btnExercises);
            _mainLayout.Controls.Add(btnMuscleGroups);
            _mainLayout.Controls.Add(btnWorkouts);
            _mainLayout.Controls.Add(btnUsers);

            this.Controls.Add(_mainLayout);
        }

        private Button CreateNavButton(string text, Action onClick)
        {
            var btn = new Button
            {
                Text = text,
                Size = new Size(300, 50),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12),
                Margin = new Padding(0, 0, 0, 10)
            };
            btn.Click += (s, e) => onClick();
            return btn;
        }
    }
}
