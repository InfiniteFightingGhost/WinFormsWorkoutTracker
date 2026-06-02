using WorkoutTracker.Data.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace WorkoutTracker.Data.Entities
{
    [Index(nameof(Start))]
    public class WorkoutSession
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
        //[Required]
        //public int WorkoutId { get; set; }

        //[ForeignKey(nameof(WorkoutId))]
        //public Workout Workout { get; set; }

        [Column(TypeName = "datetime2")]
        [Required]
        public DateTime Start { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? End { get; set; }

        [Required]
        public WorkoutStatus Status { get; set; }

        [StringLength(100)]
        [Unicode(false)]
        public string? Title { get; set; }

        [StringLength(200)]
        [Unicode(false)]
        public string? Notes { get; set; }

        [StringLength(255)]
        public string? PhotoUrl { get; set; }

        [InverseProperty(nameof(WorkoutExercise.WorkoutSession))]
        public ICollection<WorkoutExercise> Exercises { get; set; }
    }
}
