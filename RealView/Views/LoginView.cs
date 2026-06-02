using System;
using System.Drawing;
using System.Windows.Forms;

namespace WorkoutTracker.RealView.Views
{
    public class LoginView : BaseView
    {
        private Panel _card;
        private TextBox _usernameTxt;
        private TextBox _passwordTxt;
        private Button _loginBtn;
        private LinkLabel _registerLink;

        public LoginView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.BackColor = UIStyle.Background;
            
            _card = new Panel
            {
                Size = new Size(400, 450),
                BackColor = UIStyle.Surface,
                Padding = new Padding(30)
            };
            
            // Center the card
            this.Resize += (s, e) => {
                _card.Location = new Point((this.Width - _card.Width) / 2, (this.Height - _card.Height) / 2);
            };

            var title = new Label
            {
                Text = "Login",
                Font = UIStyle.Header,
                Dock = DockStyle.Top,
                Height = 60,
                TextAlign = ContentAlignment.MiddleCenter
            };

            var userLabel = new Label { Text = "Username", Dock = DockStyle.Top, Height = 30, Margin = new Padding(0, 20, 0, 0), Font = UIStyle.CaptionBold };
            _usernameTxt = new TextBox { Dock = DockStyle.Top, Font = UIStyle.Body };

            var passLabel = new Label { Text = "Password", Dock = DockStyle.Top, Height = 30, Margin = new Padding(0, 15, 0, 0), Font = UIStyle.CaptionBold };
            _passwordTxt = new TextBox { Dock = DockStyle.Top, Font = UIStyle.Body, PasswordChar = '*' };

            var spacer = new Panel { Dock = DockStyle.Top, Height = 20 };

            _loginBtn = new Button
            {
                Text = "LOGIN",
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = UIStyle.Primary,
                ForeColor = UIStyle.TextOnPrimary,
                FlatStyle = FlatStyle.Flat,
                Font = UIStyle.BodySemibold
            };
            _loginBtn.FlatAppearance.BorderSize = 0;
            _loginBtn.Click += LoginBtn_Click;

            _registerLink = new LinkLabel
            {
                Text = "Don't have an account? Register",
                Dock = DockStyle.Bottom,
                Height = 30,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = UIStyle.Caption
            };
            _registerLink.LinkClicked += (s, e) => AppRuntime.Navigation.NavigateTo<RegistrationView>();

            _card.Controls.Add(_loginBtn);
            _card.Controls.Add(spacer);
            _card.Controls.Add(_passwordTxt);
            _card.Controls.Add(passLabel);
            _card.Controls.Add(_usernameTxt);
            _card.Controls.Add(userLabel);
            _card.Controls.Add(title);
            _card.Controls.Add(_registerLink);

            this.Controls.Add(_card);
            
            _card.Paint += (s, e) => {
                ControlPaint.DrawBorder(e.Graphics, _card.ClientRectangle, UIStyle.Border, ButtonBorderStyle.Solid);
            };
        }

        private async void LoginBtn_Click(object? sender, EventArgs e)
        {
            try
            {
                _loginBtn.Enabled = false;
                var user = await AppRuntime.Auth.LoginAsync(_usernameTxt.Text, _passwordTxt.Text);

                if (user != null)
                {
                    //  Fetch any unresolved session from the database for this user
                    var activeSession = await AppRuntime.WorkoutSession.GetActiveSessionAsync(user.Id);

                    if (activeSession != null)
                    {
                        // Hydrate the global state manager with the existing session
                        AppRuntime.WorkoutState.ResumeWorkout(activeSession);
                    }

                    if (this.ParentForm is MainForm shell)
                    {
                        shell.SetSidebarVisible(true);
                    }

                    // Route to Dashboard, which will instantly pivot to ActiveWorkoutView if needed
                    AppRuntime.Navigation.NavigateTo<DashboardView>();
                }
                else
                {
                    MessageBox.Show("Invalid username or password.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                _loginBtn.Enabled = true;
            }
        }
    }
}
