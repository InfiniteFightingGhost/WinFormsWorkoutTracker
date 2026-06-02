using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkoutTracker.Data.Entities
{
    [Index(nameof(Name), IsUnique = true)]
    public class MuscleGroup
    {
        [Key]
        public int Id { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string Name { get; set; }

        [InverseProperty(nameof(Exercise.MainMuscleGroup))]
        public ICollection<Exercise> Exercises { get; set; } = new List<Exercise>();
    }
}
