using System;
using Data.Entities;

namespace WorkoutTracker.View.Services
{
    public class WorkoutStateService
    {
        public event EventHandler<WorkoutSession> WorkoutStarted;
        public event EventHandler<WorkoutSession> WorkoutFinished;
        public event EventHandler<WorkoutSession> WorkoutUpdated;

        private WorkoutSession _activeSession;

        public WorkoutSession ActiveSession
        {
            get => _activeSession;
            set
            {
                _activeSession = value;
                WorkoutUpdated?.Invoke(this, _activeSession);
            }
        }

        public bool IsWorkoutActive => _activeSession != null;

        public void StartWorkout(WorkoutSession session)
        {
            ActiveSession = session;
            WorkoutStarted?.Invoke(this, session);
        }

        public void FinishWorkout()
        {
            var session = _activeSession;
            ActiveSession = null;
            WorkoutFinished?.Invoke(this, session);
        }
    }
}
