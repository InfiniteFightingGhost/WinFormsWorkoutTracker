using Data.Entities;
using Data.Enums;
using WorkoutTracker.Service.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Controller
{
    public class UserController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<ICollection<User>> GetAllAsync()
        {
            return await _userService.GetAllAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _userService.GetByIdAsync(id);
        }

        public async Task<User?> UpdateHeightAsync(int id, decimal height)
        {
            return await _userService.UpdateHeightAsync(id, height);
        }

        public async Task<User?> UpdateWeightAsync(int id, decimal weight)
        {
            return await _userService.UpdateWeightAsync(id, weight);
        }

        public async Task<User?> UpdateGenderAsync(int id, Gender gender)
        {
            return await _userService.UpdateGenderAsync(id, gender);
        }

        public async Task<User?> UpdateProfileAsync(int id, Gender gender, decimal height, decimal weight)
        {
            return await _userService.UpdateProfileAsync(id, gender, height, weight);
        }

        public async Task<User?> UpdatePhotoAsync(int id, string photoUrl)
        {
            return await _userService.UpdatePhotoAsync(id, photoUrl);
        }

        public async Task<User?> DeleteAsync(int id)
        {
            return await _userService.DeleteAsync(id);
        }
    }
}
