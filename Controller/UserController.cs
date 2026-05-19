using Data;
using Data.DTOs;
using Data.Entities;
using Data.Enums;
using Microsoft.EntityFrameworkCore;

namespace Controller
{
    public class UserController
    {
        WorkoutDbContext _context;
        AuthController _auth;

        public UserController(WorkoutDbContext context, AuthController auth)
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
            var user = await _context.Users.FindAsync(id);
            if(user == null)
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
            var user = await _context.Users.FindAsync(id);
            if(user == null)
            {
                throw new Exception("User not found.");
            }
            user.Weight = weight;
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> DeleteAsync(int id)
        {
            _auth.IsAuthenticated(UserRole.Admin, id);

            var user = await _context.Users.FindAsync(id);
            if(user == null)
            {
                throw new Exception("User not found.");
            }
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return user;
        }
    }
}
