namespace WorkoutTracker.RealView
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            
            // Initialize backend and services
            AppRuntime.Initialize();
            
            var mainForm = new MainForm();
            
            // Initial navigation to Login
            mainForm.Load += (s, e) => AppRuntime.Navigation.NavigateTo<Views.LoginView>();
            
            Application.Run(mainForm);
        }
    }
}
