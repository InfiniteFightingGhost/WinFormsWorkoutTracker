using System;

namespace WorkoutTracker.Data.DTOs
{
    public class UserPRDTO
    {
        public int ExerciseId { get; set; }
        public int WorkoutSessionId { get; set; }
        public string ExerciseName { get; set; } = null!;
        public decimal MaxWeight { get; set; }
        public DateTime Date { get; set; }
    }
}
