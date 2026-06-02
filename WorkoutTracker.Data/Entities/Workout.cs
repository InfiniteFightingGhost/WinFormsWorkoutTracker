using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;

namespace WorkoutTracker.Data.Entities
{
    public class Workout
    {
        [Key]
        public int Id { get; set; }

        [StringLength(75)]
        [Unicode(false)]
        public string Title { get; set; }

        [StringLength(250)]
        [Unicode(false)]
        public string Description { get; set; }
    }
}
