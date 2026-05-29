using Data;
using Data.Entities;
using Data.Enums;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Service.Interfaces;

namespace WorkoutTracker.Service.Implementations
{
    public class UserService : IUserService
    {
        private readonly WorkoutDbContext _context;
        private readonly IAuthService _auth;

        public UserService(WorkoutDbContext context, IAuthService auth)
        {
            _context = context;
            _auth = auth;
        }

        public async Task<ICollection<User>> GetAllAsync()
        {
            _auth.IsAuthenticated(UserRole.Admin);
            return await _context.Users.ToListAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User?> UpdateHeightAsync(int id, decimal height)
        {
            _auth.IsAuthenticated(UserRole.Admin, id);
            
            if (height <= 0) throw new Exception("Height must be positive.");

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                throw new Exception("User not found.");
            }
            user.Height = height;
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> UpdateWeightAsync(int id, decimal weight)
        {
            _auth.IsAuthenticated(UserRole.Admin, id);

            if (weight <= 0) throw new Exception("Weight must be positive.");

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                throw new Exception("User not found.");
            }
            user.Weight = weight;
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> UpdateGenderAsync(int id, Gender gender)
        {
            _auth.IsAuthenticated(UserRole.Admin, id);
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                throw new Exception("User not found.");
            }
            user.Gender = gender;
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> UpdateProfileAsync(int id, Gender gender, decimal height, decimal weight)
        {
            _auth.IsAuthenticated(UserRole.Admin, id);

            if (height <= 0) throw new Exception("Height must be positive.");
            if (weight <= 0) throw new Exception("Weight must be positive.");

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                throw new Exception("User not found.");
            }
            user.Gender = gender;
            user.Height = height;
            user.Weight = weight;
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> UpdatePhotoAsync(int id, string photoUrl)
        {
            _auth.IsAuthenticated(UserRole.Admin, id);
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                throw new Exception("User not found.");
            }
            user.PhotoUrl = photoUrl;
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> DeleteAsync(int id)
        {
            _auth.IsAuthenticated(UserRole.Admin, id);

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                throw new Exception("User not found.");
            }
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return user;
        }
    }
}
