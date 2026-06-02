using WorkoutTracker.Data;
using WorkoutTracker.Data.Entities;
using WorkoutTracker.Data.Enums;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Service.Interfaces;

namespace WorkoutTracker.Service.Implementations
{
    public class UserService : IUserService
    {
        private readonly Func<WorkoutDbContext> _contextFactory;
        private readonly IAuthService _auth;

        public UserService(Func<WorkoutDbContext> contextFactory, IAuthService auth)
        {
            _contextFactory = contextFactory;
            _auth = auth;
        }

        public async Task<ICollection<User>> GetAllAsync()
        {
            _auth.IsAuthenticated(UserRole.Admin);
            using var context = _contextFactory();
            return await context.Users.ToListAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            using var context = _contextFactory();
            return await context.Users.FindAsync(id);
        }

        public async Task<User?> UpdateHeightAsync(int id, decimal height)
        {
            _auth.IsAuthenticated(UserRole.Admin, id);
            
            if (height <= 0) throw new Exception("Height must be positive.");

            using var context = _contextFactory();
            var user = await context.Users.FindAsync(id);
            if (user == null)
            {
                throw new Exception("User not found.");
            }
            user.Height = height;
            await context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> UpdateWeightAsync(int id, decimal weight)
        {
            _auth.IsAuthenticated(UserRole.Admin, id);

            if (weight <= 0) throw new Exception("Weight must be positive.");

            using var context = _contextFactory();
            var user = await context.Users.FindAsync(id);
            if (user == null)
            {
                throw new Exception("User not found.");
            }
            user.Weight = weight;
            await context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> UpdateGenderAsync(int id, Gender gender)
        {
            _auth.IsAuthenticated(UserRole.Admin, id);
            using var context = _contextFactory();
            var user = await context.Users.FindAsync(id);
            if (user == null)
            {
                throw new Exception("User not found.");
            }
            user.Gender = gender;
            await context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> UpdateProfileAsync(int id, Gender gender, decimal height, decimal weight)
        {
            _auth.IsAuthenticated(UserRole.Admin, id);

            if (height <= 0) throw new Exception("Height must be positive.");
            if (weight <= 0) throw new Exception("Weight must be positive.");

            using var context = _contextFactory();
            var user = await context.Users.FindAsync(id);
            if (user == null)
            {
                throw new Exception("User not found.");
            }
            user.Gender = gender;
            user.Height = height;
            user.Weight = weight;
            await context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> UpdatePhotoAsync(int id, string photoUrl)
        {
            _auth.IsAuthenticated(UserRole.Admin, id);
            using var context = _contextFactory();
            var user = await context.Users.FindAsync(id);
            if (user == null)
            {
                throw new Exception("User not found.");
            }
            user.PhotoUrl = photoUrl;
            await context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> DeleteAsync(int id)
        {
            _auth.IsAuthenticated(UserRole.Admin, id);

            using var context = _contextFactory();
            var user = await context.Users.FindAsync(id);
            if (user == null)
            {
                throw new Exception("User not found.");
            }
            context.Users.Remove(user);
            await context.SaveChangesAsync();
            return user;
        }
    }
}
