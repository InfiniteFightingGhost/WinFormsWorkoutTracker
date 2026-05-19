using Data;
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
    public class MuscleGroupController
    {
        WorkoutDbContext _context;
        AuthController _auth;

        public MuscleGroupController(WorkoutDbContext context, AuthController auth)
        {
            _context = context;
            _auth = auth;
        }

        public async Task<ICollection<MuscleGroup>> GetMuscleGroupsAsync()
        {
            return await _context.MuscleGroups.ToListAsync();
        }

        public async Task<MuscleGroup?> GetMuscleGroupById(int id)
        {
            return await _context.MuscleGroups.FindAsync(id);
        }

        //public async Task<MuscleGroup> CreateMuscleGroupAsync(string name)
        //{
        //    var group = new MuscleGroup()
        //    {
        //        Name = name
        //    };
        //    _auth.IsAuthenticated(UserRole.Admin);
        //    _context.MuscleGroups.Add(group);
        //    await _context.SaveChangesAsync();
        //    return group;
        //}

        //public async Task<MuscleGroup> UpdateMuscleGroupAsync(int id, string name)
        //{
        //    var group = await _context.MuscleGroups.FindAsync(id);
        //    if (group == null)
        //    {
        //        throw new Exception("Muscle group not found.");
        //    }
        //    _auth.IsAuthenticated(UserRole.Admin);
        //    group.Name = name;
        //    await _context.SaveChangesAsync();
        //    return group;
        //}

        //public async Task<MuscleGroup> DeleteMuscleGroupAsync(int id)
        //{
        //    var group = await _context.MuscleGroups.FindAsync(id);
        //    if (group == null)
        //    {
        //        throw new Exception("Muscle group not found.");
        //    }
        //    _auth.IsAuthenticated(UserRole.Admin);
        //    _context.MuscleGroups.Remove(group);
        //    await _context.SaveChangesAsync();
        //    return group;
        //}
    }
}
