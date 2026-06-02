using WorkoutTracker.Data.Entities;
using WorkoutTracker.Service.Interfaces;

namespace WorkoutTracker.Controller
{
    public class WorkoutController
    {
        private readonly IWorkoutService _workoutService;

        public WorkoutController(IWorkoutService workoutService)
        {
            _workoutService = workoutService;
        }

        public async Task<ICollection<Workout>> GetWorkoutsAsync()
        {
            return await _workoutService.GetAllAsync();
        }

        public async Task<Workout?> GetWorkoutByIdAsync(int id)
        {
            return await _workoutService.GetByIdAsync(id);
        }

        public async Task<Workout> CreateWorkoutAsync(Workout workout)
        {
            return await _workoutService.CreateAsync(workout);
        }

        public async Task<Workout> UpdateWorkoutAsync(int id, string title, string description)
        {
            return await _workoutService.UpdateAsync(id, title, description);
        }

        public async Task<Workout> DeleteWorkoutAsync(int id)
        {
            return await _workoutService.DeleteAsync(id);
        }
    }
}
