using Data;
using Data.Entities;
using Data.Enums;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Service.Interfaces;

namespace WorkoutTracker.Service.Implementations
{
    public class MuscleGroupService : IMuscleGroupService
    {
        private readonly WorkoutDbContext _context;
        private readonly IAuthService _auth;

        public MuscleGroupService(WorkoutDbContext context, IAuthService auth)
        {
            _context = context;
            _auth = auth;
        }

        public async Task<ICollection<MuscleGroup>> GetAllAsync()
        {
            return await _context.MuscleGroups.ToListAsync();
        }

        public async Task<MuscleGroup?> GetByIdAsync(int id)
        {
            return await _context.MuscleGroups.FindAsync(id);
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
            
            _context.MuscleGroups.Add(group);
            await _context.SaveChangesAsync();
            return group;
        }

        public async Task<MuscleGroup> UpdateAsync(int id, string name)
        {
            _auth.IsAuthenticated(UserRole.Admin);

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new Exception("Muscle group name is required.");
            }

            var group = await _context.MuscleGroups.FindAsync(id);
            if (group == null)
            {
                throw new Exception("Muscle group not found.");
            }
            
            group.Name = name;
            await _context.SaveChangesAsync();
            return group;
        }

        public async Task<MuscleGroup> DeleteAsync(int id)
        {
            _auth.IsAuthenticated(UserRole.Admin);

            var group = await _context.MuscleGroups.FindAsync(id);
            if (group == null)
            {
                throw new Exception("Muscle group not found.");
            }
            
            _context.MuscleGroups.Remove(group);
            await _context.SaveChangesAsync();
            return group;
        }
    }
}
