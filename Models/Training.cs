using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportCoursework.Models
{
    public class Training
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; } = "";
        
        [ForeignKey("UserId")]
        public virtual AppUser? User { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;

        // Поля для хранения данных
        public double? WeightAtTraining { get; set; }
        public double? Distance { get; set; } 
        public double? TimeMinutes { get; set; } 
        public int? ActionsCount { get; set; } 

        // эффективность для графика
        public double GetEfficiency(string sportType)
        {
            if (TimeMinutes == null || TimeMinutes <= 0) 
                return ActionsCount ?? 0;

            return sportType switch
            {
                "Running" => Distance > 0 ? Math.Round((double)(Distance / TimeMinutes * 60), 2) : 0,
                "Swimming" => Distance > 0 ? Math.Round((double)(Distance / TimeMinutes), 2) : 0,
                "Volleyball" => ActionsCount ?? 0,
                "Dancing" => Math.Round((double)TimeMinutes * 0.7, 1), 
                _ => 0
            };
        }
    }
}