using WorkoutTracker.Controller;
using WorkoutTracker.Data;
using WorkoutTracker.View.Services;
using WorkoutTracker.Service.Implementations;
using WorkoutTracker.Service.Interfaces;
using WorkoutTracker.Service.Validators;

namespace WorkoutTracker.View
{
    public static class AppRuntime
    {
        public static WorkoutDbContext Context { get; private set; }
        
        // Services
        public static IAuthService AuthService { get; private set; }
        public static IExerciseService ExerciseService { get; private set; }
        public static IMuscleGroupService MuscleGroupService { get; private set; }
        public static IUserService UserService { get; private set; }
        public static IWorkoutService WorkoutService { get; private set; }
        public static IWorkoutExerciseService WorkoutExerciseService { get; private set; }
        public static IWorkoutSessionService WorkoutSessionService { get; private set; }
        public static IWorkoutSetService WorkoutSetService { get; private set; }

        // Controllers
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
        }
    }
}
