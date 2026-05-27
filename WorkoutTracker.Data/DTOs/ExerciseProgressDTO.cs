using System;

namespace Data.DTOs
{
    public class ExerciseProgressDTO
    {
        public DateTime Date { get; set; }
        public decimal MaxWeight { get; set; }
        public decimal MaxVolume { get; set; }
        public bool IsPotential { get; set; }
    }
}
