using Data;
using Data.DTOs;
using Data.Entities;
using Data.Enums;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Service.Interfaces;

namespace WorkoutTracker.Service.Implementations
{
    public class WorkoutSetService : IWorkoutSetService
    {
        private readonly WorkoutDbContext _context;

        public WorkoutSetService(WorkoutDbContext context)
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
                        WorkoutSessionId = s.Id,
                        Date = s.Start,
                        MaxWeight = workoutExercise.Sets.Max(set => set.Weight),
                        MaxVolume = workoutExercise.Sets.Max(set => set.Weight * set.Repetitions),
                        IsPotential = false
                    };
                })
                .Where(p => p != null)
                .Cast<ExerciseProgressDTO>()
                .ToList();

            var activeSession = await _context.WorkoutSessions
                .Where(s => s.UserId == userId && s.Status == WorkoutStatus.OnGoing)
                .Include(s => s.Exercises.Where(e => e.ExerciseId == exerciseId))
                    .ThenInclude(e => e.Sets)
                .FirstOrDefaultAsync();

            if (activeSession != null)
            {
                var workoutEx = activeSession.Exercises.FirstOrDefault();
                if (workoutEx != null && workoutEx.Sets.Any())
                {
                    var completedSets = workoutEx.Sets.Where(s => s.Completed).ToList();
                    if (completedSets.Any())
                    {
                        progress.Add(new ExerciseProgressDTO
                        {
                            WorkoutSessionId = activeSession.Id,
                            Date = activeSession.Start,
                            MaxWeight = completedSets.Max(s => s.Weight),
                            MaxVolume = completedSets.Max(s => s.Weight * s.Repetitions),
                            IsPotential = false
                        });
                    }

                    var uncompletedSets = workoutEx.Sets.Where(s => !s.Completed).ToList();
                    if (uncompletedSets.Any())
                    {
                        var potWeight = uncompletedSets.Max(s => s.Weight);
                        var potVolume = uncompletedSets.Max(s => s.Weight * s.Repetitions);

                        var currentMax = progress.Any() ? progress.Max(p => p.MaxWeight) : 0;
                        if (potWeight >= currentMax)
                        {
                            progress.Add(new ExerciseProgressDTO
                            {
                                WorkoutSessionId = activeSession.Id,
                                Date = DateTime.Now,
                                MaxWeight = potWeight,
                                MaxVolume = potVolume,
                                IsPotential = true
                            });
                        }
                    }
                }
            }

            return progress;
        }

        public async Task<IEnumerable<UserPRDTO>> GetUserPRsAsync(int userId)
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
                .Select(g => {
                    var topSession = g.OrderByDescending(we => we.Sets.Any() ? we.Sets.Max(s => s.Weight) : 0).First();
                    return new UserPRDTO {
                        ExerciseId = g.Key,
                        WorkoutSessionId = topSession.WorkoutId,
                        ExerciseName = topSession.Exercise.Name,
                        MaxWeight = g.SelectMany(we => we.Sets).Any() ? g.SelectMany(we => we.Sets).Max(set => set.Weight) : 0,
                        Date = topSession.WorkoutSession.Start
                    };
                })
                .Where(p => p.MaxWeight > 0)
                .OrderByDescending(p => p.MaxWeight)
                .Take(5)
                .ToList();

            return prs;
        }

        public async Task<IEnumerable<MuscleVolumeDTO>> GetMuscleVolumeAsync(int userId)
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
                .Select(g => new MuscleVolumeDTO {
                    MuscleGroup = g.Key,
                    SetCount = g.SelectMany(we => we.Sets).Count(),
                    TotalVolume = g.SelectMany(we => we.Sets).Sum(set => set.Weight * set.Repetitions)
                })
                .OrderByDescending(v => v.TotalVolume)
                .ToList();

            return volume;
        }

        public async Task<ICollection<ExerciseSet>> GetAllAsync()
        {
            return await _context.ExerciseSets.ToListAsync();
        }

        public async Task<ExerciseSet?> GetByIdAsync(int id)
        {
            return await _context.ExerciseSets.FindAsync(id);
        }

        public async Task<ExerciseSet> CreateAsync(int workoutId, int exerciseId)
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

        public async Task UpdateAsync(ExerciseSet set)
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

        public async Task DeleteAsync(int id)
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
