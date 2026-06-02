using WorkoutTracker.Data.DTOs;
using WorkoutTracker.Data.Entities;
using WorkoutTracker.Data.Enums;
using WorkoutTracker.Service.Interfaces;

namespace WorkoutTracker.Controller
{
    public class AuthController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        public void IsAuthenticated(UserRole neededRole)
        {
            _authService.IsAuthenticated(neededRole);
        }

        public void IsAuthenticated(UserRole neededRole, int id)
        {
            _authService.IsAuthenticated(neededRole, id);
        }

        public async Task<User> RegisterAsync(CreateUserDTO dto)
        {
            return await _authService.RegisterAsync(dto);
        }

        public async Task<User?> LoginAsync(string username, string password)
        {
            return await _authService.LoginAsync(username, password);
        }

        public void Logout()
        {
            _authService.Logout();
        }

        public User? GetCurrentUser()
        {
            return _authService.GetCurrentUser();
        }
    }
}
