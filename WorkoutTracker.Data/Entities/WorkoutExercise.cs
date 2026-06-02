using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkoutTracker.Data.Entities
{
    [PrimaryKey(nameof(WorkoutId), nameof(ExerciseId))]
    [Index(nameof(OrderIndex))]
    public class WorkoutExercise
    {
        [Required]
        public int WorkoutId { get; set; }
        [ForeignKey(nameof(WorkoutId))]
        public WorkoutSession WorkoutSession { get; set; }
        [Required]
        public int ExerciseId { get; set; }
        [ForeignKey(nameof(ExerciseId))]
        public Exercise Exercise { get; set; }
        [Required]
        public int OrderIndex { get; set; }

        [InverseProperty(nameof(ExerciseSet.WorkoutExercise))]
        public ICollection<ExerciseSet> Sets { get; set; } = new List<ExerciseSet>();
    }
}
