namespace WorkoutTracker.Data.DTOs
{
    public class MuscleVolumeDTO
    {
        public string MuscleGroup { get; set; } = null!;
        public int SetCount { get; set; }
        public decimal TotalVolume { get; set; }
    }
}
