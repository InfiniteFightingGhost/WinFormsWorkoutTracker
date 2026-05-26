using Controller;
using Data;
using RealView.Services;

namespace RealView
{
    public static class AppRuntime
    {
        public static WorkoutDbContext Context { get; private set; } = null!;
        public static AuthController Auth { get; private set; } = null!;
        public static ExerciseController Exercise { get; private set; } = null!;
        public static MuscleGroupController MuscleGroup { get; private set; } = null!;
        public static UserController User { get; private set; } = null!;
        public static WorkoutController Workout { get; private set; } = null!;
        public static WorkoutExerciseController WorkoutExercise { get; private set; } = null!;
        public static WorkoutSessionController WorkoutSession { get; private set; } = null!;
        public static WorkoutSetController WorkoutSet { get; private set; } = null!;

        public static NavigationService Navigation { get; set; } = null!;
        public static WorkoutStateService WorkoutState { get; private set; } = null!;

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
