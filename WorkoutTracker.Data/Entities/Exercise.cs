using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkoutTracker.Data.Entities
{
    public class Exercise
    {
        [Key]
        public int Id { get; set; }
        [StringLength(40)]
        [Unicode(false)]
        public string Name { get; set; }
        [StringLength(200)]
        [Unicode(false)]
        public string Description { get; set; }
        [StringLength(500)]
        [Unicode(false)]
        public string Instructions { get; set; }
        [Required]
        public int MuscleGroupId { get; set; }
        [ForeignKey(nameof(MuscleGroupId))]
        public MuscleGroup MainMuscleGroup { get; set; }

        public override bool Equals(object? obj)
        {
            var ex = obj as Exercise;
            if (ex == null) return false;
            return Name == ex.Name && MuscleGroupId == ex.MuscleGroupId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, MuscleGroupId);
        }
    }
}
