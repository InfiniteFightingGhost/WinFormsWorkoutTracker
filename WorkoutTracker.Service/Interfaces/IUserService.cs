using Data.Entities;
using Data.Enums;

namespace WorkoutTracker.Service.Interfaces
{
    public interface IUserService
    {
        Task<ICollection<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task<User?> UpdateHeightAsync(int id, decimal height);
        Task<User?> UpdateWeightAsync(int id, decimal weight);
        Task<User?> UpdateGenderAsync(int id, Gender gender);
        Task<User?> UpdateProfileAsync(int id, Gender gender, decimal height, decimal weight);
        Task<User?> UpdatePhotoAsync(int id, string photoUrl);
        Task<User?> DeleteAsync(int id);
    }
}
