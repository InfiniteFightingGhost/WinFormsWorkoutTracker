using Controller;
using Data;
using Data.Entities;
using Data.Enums;
using View;

namespace WorkoutTracker.View
{
    public partial class LoginForm : Form
    {
        WorkoutDbContext _context;
        AuthController _auth;
        ExerciseController _exercise;
        MuscleGroupController _muscleGroup;
        UserController _user;
        WorkoutController _workout;
        WorkoutExerciseController _workoutExercise;
        WorkoutSessionController _workoutSession;
        WorkoutSetController _workoutSet;
        public LoginForm()
        {
            _context = new WorkoutDbContext();
            _auth = new AuthController(_context);
            _exercise = new ExerciseController(_context);
            _muscleGroup = new MuscleGroupController(_context, _auth);
            _user = new UserController(_context, _auth);
            _workout = new WorkoutController(_context);
            _workoutExercise = new WorkoutExerciseController(_context, _auth);
            _workoutSession = new WorkoutSessionController(_context, _auth);
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var form = new RegistrationForm(_auth);
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
                var user = await _auth.LoginAsync(username, password);
                textBox1.Text = string.Empty;
                textBox2.Text = string.Empty;

                //TODO: MAKE NEW USER 
                if(user == null)
                {
                    throw new Exception("The username or password is incorrect.");
                }
                if(user.Role == UserRole.Admin)
                {
                    var form = new UserForm(_exercise, _muscleGroup, _workout, _workoutExercise, _workoutSession, _workoutSet);
                    this.Hide();
                    var result = form.ShowDialog();
                    this.Show();
                }
            }
            catch(Exception x)
            {
                MessageBox.Show(x.Message);
            }
        }
    }
}
