using Data;
using Data.DTOs;
using Data.Entities;
using Data.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controller
{
    public class AuthController
    {
        WorkoutDbContext _context;
        User currentUser = null;
        public AuthController(WorkoutDbContext context)
        {
            _context = context;
        }

        public void IsAuthenticated(UserRole neededRole)
        {
            if (neededRole != currentUser.Role)
            {
                throw new UnauthorizedAccessException("You dont have permission to perform this action.");
            }
        }
        public void IsAuthenticated(UserRole neededRole, int id)
        {
            if (currentUser.Id != id)
            {
                if (neededRole != currentUser.Role)
                {
                    throw new UnauthorizedAccessException("You dont have permission to perform this action.");
                }
            }
        }
        public async Task<User> RegisterAsync(CreateUserDTO dto)
        {
            if(await _context.Users.AnyAsync(u => u.Username == dto.Username))
            {
                throw new Exception("Username already exists.");
            }
            if(await _context.Users.AnyAsync(u => u.Email == dto.Email))
            {
                throw new Exception("An user with this email already exists.");
            }

            User user = new User()
            {
                Username = dto.Username,
                Email = dto.Email,
                Password = dto.Password,
                Gender = dto.Gender,
                Height = dto.Height,
                Weight = dto.Weight,
                Role = UserRole.User
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            currentUser = user; // Auto-login after registration
            return user;
        }

        public async Task<User?> LoginAsync(string username, string password)
        {
            if(!await _context.Users.AnyAsync(u => u.Username == username))
            {
                throw new Exception("A user with this username doesnt exist");
            }
            var user =  await _context.Users
                .Include(u => u.Sessions)
                .FirstOrDefaultAsync(u => u.Username == username && u.Password == password);
            currentUser = user;
            return user;
        }

        public void Logout()
        {
            currentUser = null;
        }

        public User? GetCurrentUser()
        {
            return currentUser;
        }
    }
}
