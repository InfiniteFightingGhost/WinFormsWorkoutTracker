using Data.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Data.Entities
{
    [PrimaryKey(nameof(UserId))]
    [Index(nameof(Start))]
    public class WorkoutSession
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
        //[Required]
        //public int WorkoutId { get; set; }

        //[ForeignKey(nameof(WorkoutId))]
        //public Workout Workout { get; set; }

        [Column(TypeName = "datetime")]
        [Required]
        public DateTime Start { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime End { get; set; }
        [Required]
        public WorkoutStatus Status { get; set; }
        [StringLength(200)]
        [Unicode(false)]
        public string Notes { get; set; }
        [InverseProperty(nameof(WorkoutExercise.WorkoutSession))]
        public ICollection<WorkoutExercise> Exercises { get; set; }
    }
}
