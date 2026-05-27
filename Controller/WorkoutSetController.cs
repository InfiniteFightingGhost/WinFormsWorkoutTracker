using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Data.Entities;
using Data.Enums;
using Data.DTOs;

namespace Controller
{
    public class WorkoutSetController
    {
        WorkoutDbContext _context;
        public WorkoutSetController(WorkoutDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ExerciseProgressDTO>> GetExerciseProgressAsync(int userId, int exerciseId)
        {
            var sessions = await _context.WorkoutSessions
                .Where(s => s.UserId == userId && s.Status == WorkoutStatus.Finished)
                .Include(s => s.Exercises.Where(e => e.ExerciseId == exerciseId))
                    .ThenInclude(e => e.Sets)
                .OrderBy(s => s.Start)
                .ToListAsync();

            var progress = sessions
                .Select(s => {
                    var workoutExercise = s.Exercises.FirstOrDefault();
                    if (workoutExercise == null || workoutExercise.Sets.Count == 0) return null;

                    return new ExerciseProgressDTO
                    {
                        Date = s.Start,
                        MaxWeight = workoutExercise.Sets.Max(set => set.Weight),
                        MaxVolume = workoutExercise.Sets.Max(set => set.Weight * set.Repetitions),
                        IsPotential = false
                    };
                })
                .Where(p => p != null)
                .Cast<ExerciseProgressDTO>()
                .ToList();

            // Include Active Session if it has potential PRs (uncompleted sets)
            var activeSession = await _context.WorkoutSessions
                .Where(s => s.UserId == userId && s.Status == WorkoutStatus.OnGoing)
                .Include(s => s.Exercises.Where(e => e.ExerciseId == exerciseId))
                    .ThenInclude(e => e.Sets)
                .FirstOrDefaultAsync();

            if (activeSession != null)
            {
                var workoutEx = activeSession.Exercises.FirstOrDefault();
                if (workoutEx != null)
                {
                    var uncompletedSets = workoutEx.Sets.Where(s => !s.Completed).ToList();
                    if (uncompletedSets.Any())
                    {
                        progress.Add(new ExerciseProgressDTO
                        {
                            Date = DateTime.Now,
                            MaxWeight = uncompletedSets.Max(s => s.Weight),
                            MaxVolume = uncompletedSets.Max(s => s.Weight * s.Repetitions),
                            IsPotential = true
                        });
                    }
                }
            }

            return progress;
        }

        public async Task<IEnumerable<dynamic>> GetUserPRsAsync(int userId)
        {
            var sessions = await _context.WorkoutSessions
                .Where(s => s.UserId == userId && s.Status == WorkoutStatus.Finished)
                .Include(s => s.Exercises)
                    .ThenInclude(e => e.Exercise)
                .Include(s => s.Exercises)
                    .ThenInclude(e => e.Sets)
                .ToListAsync();

            var prs = sessions.SelectMany(s => s.Exercises)
                .GroupBy(e => e.ExerciseId)
                .Select(g => new {
                    ExerciseName = g.First().Exercise.Name,
                    MaxWeight = g.SelectMany(we => we.Sets).Any() ? g.SelectMany(we => we.Sets).Max(set => set.Weight) : 0,
                    Date = g.OrderByDescending(we => we.Sets.Any() ? we.Sets.Max(s => s.Weight) : 0).First().WorkoutSession.Start
                })
                .Where(p => p.MaxWeight > 0)
                .OrderByDescending(p => p.MaxWeight)
                .Take(5)
                .ToList<dynamic>();

            return prs;
        }

        public async Task<IEnumerable<dynamic>> GetMuscleVolumeAsync(int userId)
        {
            var sessions = await _context.WorkoutSessions
                .Where(s => s.UserId == userId && s.Status == WorkoutStatus.Finished)
                .Include(s => s.Exercises)
                    .ThenInclude(e => e.Exercise)
                        .ThenInclude(ex => ex.MainMuscleGroup)
                .Include(s => s.Exercises)
                    .ThenInclude(e => e.Sets)
                .ToListAsync();

            var volume = sessions.SelectMany(s => s.Exercises)
                .GroupBy(e => e.Exercise?.MainMuscleGroup?.Name ?? "Unknown")
                .Select(g => new {
                    MuscleGroup = g.Key,
                    SetCount = g.SelectMany(we => we.Sets).Count(),
                    TotalVolume = g.SelectMany(we => we.Sets).Sum(set => set.Weight * set.Repetitions)
                })
                .OrderByDescending(v => v.TotalVolume)
                .ToList<dynamic>();

            return volume;
        }

        public async Task<ICollection<ExerciseSet>> GetWorkoutSetsAsync()
        {
            return await _context.ExerciseSets.ToListAsync();
        }

        public async Task<ExerciseSet> GetExerciseSetByIdAsync(int id)
        {
            return await _context.ExerciseSets.FindAsync(id);
        }

        public async Task<ExerciseSet> CreateExerciseSetAsync(int workoutId, int exerciseId)
        {
            var exerciseSet = new ExerciseSet()
            {
                WorkoutExerciseId = workoutId,
                ExerciseId = exerciseId,
                Repetitions = 0,
                Weight = 0,
                Completed = false,
                OrderIndex = await _context.ExerciseSets
                .Where(es => es.WorkoutExerciseId == workoutId && es.ExerciseId == exerciseId)
                .CountAsync(),
                SetType = SetType.Regular
            };
            _context.ExerciseSets.Add(exerciseSet);
            await _context.SaveChangesAsync();
            return exerciseSet;
        }

        public async Task UpdateExerciseSetAsync(ExerciseSet set)
        {
            var existing = await _context.ExerciseSets.FindAsync(set.Id);
            if (existing != null)
            {
                existing.Weight = set.Weight;
                existing.Repetitions = set.Repetitions;
                existing.Completed = set.Completed;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteExerciseSetAsync(int id)
        {
            var existing = await _context.ExerciseSets.FindAsync(id);
            if (existing != null)
            {
                _context.ExerciseSets.Remove(existing);
                await _context.SaveChangesAsync();
            }
        }
    }
}
