using WorkoutTracker.Data.DTOs;
using WorkoutTracker.Data.Entities;
using WorkoutTracker.Data.Enums;

namespace WorkoutTracker.Service.Interfaces
{
    public interface IAuthService
    {
        Task<User> RegisterAsync(CreateUserDTO dto);
        Task<User?> LoginAsync(string username, string password);
        void Logout();
        User? GetCurrentUser();
        void IsAuthenticated(UserRole neededRole);
        void IsAuthenticated(UserRole neededRole, int id);
    }
}
