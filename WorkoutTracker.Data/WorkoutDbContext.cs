using Data.Entities;
using Data.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Data
{
    public class WorkoutDbContext : DbContext
    {

        public DbSet<User> Users { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<MuscleGroup> MuscleGroups { get; set; }
        public DbSet<Workout> Workouts { get; set; }
        public DbSet<WorkoutExercise> WorkoutExercises { get; set; }
        public DbSet<WorkoutSession> WorkoutSessions { get; set; }
        public DbSet<ExerciseSet> ExerciseSets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(u =>
            {
                u.Property(u => u.Role).HasConversion<string>();
                u.Property(u => u.Gender).HasConversion<string>();
            });

            modelBuilder.Entity<User>().HasData(
                new User()
                {
                    Id = 1,
                    Username = "admin",
                    Email = "admin@admin.com",
                    Password = "admin123",
                    Height = 180,
                    Weight = 75,
                    Role = UserRole.Admin,
                    Gender = Gender.Male,
                });

            modelBuilder.Entity<WorkoutSession>(w =>
            {
                w.Property(u => u.Status).HasConversion<string>();
                w.Property(u => u.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<WorkoutExercise>(e =>
            {
                e.ToTable(t => t.HasCheckConstraint("CK_OrderIndex_Allowed", "[OrderIndex] >= 0"));
            });
            modelBuilder.Entity<ExerciseSet>(es =>
            {
                es.HasOne(es => es.WorkoutExercise)
                  .WithMany(we => we.Sets)
                  .HasForeignKey(es =>
                  new
                  {
                      es.WorkoutExerciseId,
                      es.ExerciseId
                  });
            });

            modelBuilder.Entity<MuscleGroup>().HasData(
                // Chest
                new MuscleGroup { Id = 1, Name = "Chest (Upper/Mid/Lower)" },

                // Back
                new MuscleGroup { Id = 2, Name = "Lats" },
                new MuscleGroup { Id = 3, Name = "Upper Back (Traps/Rhomboids)" },
                new MuscleGroup { Id = 4, Name = "Lower Back (Erectors)" },

                // Shoulders
                new MuscleGroup { Id = 5, Name = "Front Delts" },
                new MuscleGroup { Id = 6, Name = "Side Delts" },
                new MuscleGroup { Id = 7, Name = "Rear Delts" },

                // Arms
                new MuscleGroup { Id = 8, Name = "Biceps" },
                new MuscleGroup { Id = 9, Name = "Triceps" },
                new MuscleGroup { Id = 10, Name = "Forearms" },

                // Legs - Quads / Knee dominant
                new MuscleGroup { Id = 11, Name = "Quadriceps" },

                // Legs - Posterior chain
                new MuscleGroup { Id = 12, Name = "Hamstrings" },
                new MuscleGroup { Id = 13, Name = "Glutes" },

                // Hip / stability (important separation you pointed out)
                new MuscleGroup { Id = 14, Name = "Hip Abductors" },
                new MuscleGroup { Id = 15, Name = "Hip Adductors" },

                // Calves
                new MuscleGroup { Id = 16, Name = "Calves" },

                // Core
                new MuscleGroup { Id = 17, Name = "Abs (Rectus Abdominis)" },
                new MuscleGroup { Id = 18, Name = "Obliques" },
                new MuscleGroup { Id = 19, Name = "Deep Core (Transverse Abdominis)" }
            );
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var config = new ConfigurationBuilder();
            config.AddJsonFile("appsettings.json");
            var build = config.Build();
            optionsBuilder.UseSqlServer(build.GetConnectionString("DaskaloConnection"));
        }
    }
}
