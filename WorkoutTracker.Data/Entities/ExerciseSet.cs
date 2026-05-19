using Data.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    [Index(nameof(OrderIndex))]
    public class ExerciseSet
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int WorkoutExerciseId { get; set; }
        [Required]
        public int ExerciseId { get; set;  }
        public WorkoutExercise WorkoutExercise { get; set; }
        [Required]
        public int Repetitions { get; set; }
        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal Weight { get; set; }

        [Required]
        public int OrderIndex { get; set; }

        [Required]
        public bool Completed { get; set; } = false;
        [Required]
        [DefaultValue(SetType.Regular)]
        public SetType SetType { get; set; } = SetType.Regular;
    }
}
