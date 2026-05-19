using Controller;
using Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace View
{
    public partial class UserForm : Form
    {
        //WorkoutDbContext _context;
        ExerciseController _exercise;
        MuscleGroupController _muscleGroup;
        UserController _user;
        WorkoutController _workout;
        WorkoutExerciseController _workoutExercise;
        WorkoutSessionController _workoutSession;
        WorkoutSetController _workoutSet;

        public UserForm(ExerciseController exercise, MuscleGroupController muscleGroup, WorkoutController workout, WorkoutExerciseController workoutExercise, WorkoutSessionController workoutSession, WorkoutSetController workoutSet)
        {
            _exercise = exercise;
            _muscleGroup = muscleGroup;
            _workout = workout;
            _workoutExercise = workoutExercise;
            _workoutSession = workoutSession;
            _workoutSet = workoutSet;
            InitializeComponent();
        }
    }
}
