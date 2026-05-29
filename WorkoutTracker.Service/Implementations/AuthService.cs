using Data;
using Data.DTOs;
using Data.Entities;
using Data.Enums;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Service.Interfaces;
using WorkoutTracker.Service.Validators;

namespace WorkoutTracker.Service.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly WorkoutDbContext _context;
        private readonly IValidator<CreateUserDTO> _createUserValidator;
        private User? _currentUser = null;

        public AuthService(WorkoutDbContext context, IValidator<CreateUserDTO> createUserValidator)
        {
            _context = context;
            _createUserValidator = createUserValidator;
        }

        public async Task<User> RegisterAsync(CreateUserDTO dto)
        {
            var validationResult = await _createUserValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
            {
                throw new Exception("Username already exists.");
            }
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            {
                throw new Exception("A user with this email already exists.");
            }

            User user = new User()
            {
                Username = dto.Username,
                Email = dto.Email,
                Password = dto.Password, // In a real app, hash this!
                Gender = dto.Gender,
                Height = dto.Height,
                Weight = dto.Weight,
                Role = UserRole.User
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            _currentUser = user; 
            return user;
        }

        public async Task<User?> LoginAsync(string username, string password)
        {
            if (!await _context.Users.AnyAsync(u => u.Username == username))
            {
                throw new Exception("A user with this username doesn't exist");
            }

            var user = await _context.Users
                .Include(u => u.Sessions)
                .FirstOrDefaultAsync(u => u.Username == username && u.Password == password);

            if (user == null)
            {
                throw new Exception("Invalid password.");
            }

            _currentUser = user;
            return user;
        }

        public void Logout()
        {
            _currentUser = null;
        }

        public User? GetCurrentUser()
        {
            return _currentUser;
        }

        public void IsAuthenticated(UserRole neededRole)
        {
            if (_currentUser == null)
            {
                throw new UnauthorizedAccessException("You must be logged in.");
            }

            if (neededRole != _currentUser.Role)
            {
                throw new UnauthorizedAccessException("You don't have permission to perform this action.");
            }
        }

        public void IsAuthenticated(UserRole neededRole, int id)
        {
            if (_currentUser == null)
            {
                throw new UnauthorizedAccessException("You must be logged in.");
            }

            if (_currentUser.Id != id)
            {
                if (neededRole != _currentUser.Role)
                {
                    throw new UnauthorizedAccessException("You don't have permission to perform this action.");
                }
            }
        }
    }
}
