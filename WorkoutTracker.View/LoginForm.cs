using WorkoutTracker.Controller;
using WorkoutTracker.Data;
using WorkoutTracker.Data.Entities;
using WorkoutTracker.Data.Enums;
using WorkoutTracker.View;

namespace WorkoutTracker.View
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var form = new RegistrationForm(AppRuntime.Auth);
            this.Hide();
            var result = form.ShowDialog();
            this.Show();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string username = textBox1.Text;
                string password = textBox2.Text;
                if (string.IsNullOrEmpty(username))
                {
                    throw new Exception("Username can't be empty");
                }
                if (string.IsNullOrEmpty(password))
                {
                    throw new Exception("Password can't be empty");
                }
                var user = await AppRuntime.Auth.LoginAsync(username, password);
                
                if(user == null)
                {
                    throw new Exception("The username or password is incorrect.");
                }

                var form = new UserForm();
                this.Hide();
                form.ShowDialog();
                this.Show();
                
                textBox1.Text = string.Empty;
                textBox2.Text = string.Empty;
            }
            catch(Exception x)
            {
                MessageBox.Show(x.Message);
            }
        }
    }
}
