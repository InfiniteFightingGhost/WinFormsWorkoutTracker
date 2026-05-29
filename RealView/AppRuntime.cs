using Controller;
using Data;
using RealView.Services;
using WorkoutTracker.Service.Implementations;
using WorkoutTracker.Service.Interfaces;
using WorkoutTracker.Service.Validators;

namespace RealView
{
    public static class AppRuntime
    {
        public static WorkoutDbContext Context { get; private set; } = null!;
        
        // Services
        public static IAuthService AuthService { get; private set; } = null!;
        public static IExerciseService ExerciseService { get; private set; } = null!;
        public static IMuscleGroupService MuscleGroupService { get; private set; } = null!;
        public static IUserService UserService { get; private set; } = null!;
        public static IWorkoutService WorkoutService { get; private set; } = null!;
        public static IWorkoutExerciseService WorkoutExerciseService { get; private set; } = null!;
        public static IWorkoutSessionService WorkoutSessionService { get; private set; } = null!;
        public static IWorkoutSetService WorkoutSetService { get; private set; } = null!;

        // Controllers
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
        public static ToastService Toasts { get; private set; } = null!;

        public static void Initialize()
        {
            Context = new WorkoutDbContext();

            // Initialize Validators
            var createUserValidator = new CreateUserDTOValidator();
            var exerciseValidator = new ExerciseValidator();
            var workoutValidator = new WorkoutValidator();

            // Initialize Services
            AuthService = new AuthService(Context, createUserValidator);
            ExerciseService = new ExerciseService(Context, exerciseValidator);
            MuscleGroupService = new MuscleGroupService(Context, AuthService);
            UserService = new UserService(Context, AuthService);
            WorkoutService = new WorkoutService(Context, workoutValidator);
            WorkoutExerciseService = new WorkoutExerciseService(Context, AuthService);
            WorkoutSessionService = new WorkoutSessionService(Context, AuthService);
            WorkoutSetService = new WorkoutSetService(Context);

            // Initialize Controllers
            Auth = new AuthController(AuthService);
            Exercise = new ExerciseController(ExerciseService);
            MuscleGroup = new MuscleGroupController(MuscleGroupService);
            User = new UserController(UserService);
            Workout = new WorkoutController(WorkoutService);
            WorkoutExercise = new WorkoutExerciseController(WorkoutExerciseService);
            WorkoutSession = new WorkoutSessionController(WorkoutSessionService);
            WorkoutSet = new WorkoutSetController(WorkoutSetService);

            WorkoutState = new WorkoutStateService();
            Toasts = new ToastService();
        }
    }
}
