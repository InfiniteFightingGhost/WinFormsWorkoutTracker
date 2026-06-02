using WorkoutTracker.Data;
using WorkoutTracker.Data.Entities;
using WorkoutTracker.Data.Enums;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Service.Interfaces;

namespace WorkoutTracker.Service.Implementations
{
    public class MuscleGroupService : IMuscleGroupService
    {
        private readonly Func<WorkoutDbContext> _contextFactory;
        private readonly IAuthService _auth;

        public MuscleGroupService(Func<WorkoutDbContext> contextFactory, IAuthService auth)
        {
            _contextFactory = contextFactory;
            _auth = auth;
        }

        public async Task<ICollection<MuscleGroup>> GetAllAsync()
        {
            using var context = _contextFactory();
            return await context.MuscleGroups.ToListAsync();
        }

        public async Task<MuscleGroup?> GetByIdAsync(int id)
        {
            using var context = _contextFactory();
            return await context.MuscleGroups.FindAsync(id);
        }

        public async Task<MuscleGroup> CreateAsync(string name)
        {
            _auth.IsAuthenticated(UserRole.Admin);

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new Exception("Muscle group name is required.");
            }

            var group = new MuscleGroup()
            {
                Name = name
            };
            
            using var context = _contextFactory();
            context.MuscleGroups.Add(group);
            await context.SaveChangesAsync();
            return group;
        }

        public async Task<MuscleGroup> UpdateAsync(int id, string name)
        {
            _auth.IsAuthenticated(UserRole.Admin);

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new Exception("Muscle group name is required.");
            }

            using var context = _contextFactory();
            var group = await context.MuscleGroups.FindAsync(id);
            if (group == null)
            {
                throw new Exception("Muscle group not found.");
            }
            
            group.Name = name;
            await context.SaveChangesAsync();
            return group;
        }

        public async Task<MuscleGroup> DeleteAsync(int id)
        {
            _auth.IsAuthenticated(UserRole.Admin);

            using var context = _contextFactory();
            var group = await context.MuscleGroups.FindAsync(id);
            if (group == null)
            {
                throw new Exception("Muscle group not found.");
            }
            
            context.MuscleGroups.Remove(group);
            await context.SaveChangesAsync();
            return group;
        }

        public async Task<ICollection<MuscleGroup>> BulkCreateAsync(ICollection<string> names)
        {
            _auth.IsAuthenticated(UserRole.Admin);

            if (names == null || names.Count == 0)
            {
                throw new Exception("At least one muscle group name is required.");
            }

            var groups = names.Select(name => new MuscleGroup { Name = name }).ToList();
            using var context = _contextFactory();
            context.MuscleGroups.AddRange(groups);
            await context.SaveChangesAsync();
            return groups;
        }
    }
}
