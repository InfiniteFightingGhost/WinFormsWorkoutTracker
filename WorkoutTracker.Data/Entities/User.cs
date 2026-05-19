using Data.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities
{
    [Index(nameof(Username),  IsUnique = true)]
    [Index(nameof(Email), IsUnique = true)]
    public class User
    {
        [Key]
        public int Id { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        [Required]
        public string Username { get; set; }
        [StringLength(80)]
        [Unicode(false)]
        [Required]
        public string Email { get; set; }
        [StringLength(80)]
        [Unicode(false)]
        [Required]
        public string Password { get; set; }
        [Required]
        public Gender Gender { get; set; }
        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal Height { get; set; }
        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal Weight { get; set; }
        [Required]
        public UserRole Role { get; set; }

        public ICollection<WorkoutSession> Sessions { get; set; } = new List<WorkoutSession>();
    }
}
