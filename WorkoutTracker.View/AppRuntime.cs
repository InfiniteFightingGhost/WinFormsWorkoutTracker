using Controller;
using Data;
using WorkoutTracker.View.Services;

namespace WorkoutTracker.View
{
    public static class AppRuntime
    {
        public static WorkoutDbContext Context { get; private set; }
        public static AuthController Auth { get; private set; }
        public static ExerciseController Exercise { get; private set; }
        public static MuscleGroupController MuscleGroup { get; private set; }
        public static UserController User { get; private set; }
        public static WorkoutController Workout { get; private set; }
        public static WorkoutExerciseController WorkoutExercise { get; private set; }
        public static WorkoutSessionController WorkoutSession { get; private set; }
        public static WorkoutSetController WorkoutSet { get; private set; }

        public static NavigationService Navigation { get; set; }
        public static WorkoutStateService WorkoutState { get; private set; }

        public static void Initialize()
        {
            Context = new WorkoutDbContext();
            Auth = new AuthController(Context);
            Exercise = new ExerciseController(Context);
            MuscleGroup = new MuscleGroupController(Context, Auth);
            User = new UserController(Context, Auth);
            Workout = new WorkoutController(Context);
            WorkoutExercise = new WorkoutExerciseController(Context, Auth);
            WorkoutSession = new WorkoutSessionController(Context, Auth);
            WorkoutSet = new WorkoutSetController(Context);

            WorkoutState = new WorkoutStateService();
        }
    }
}
