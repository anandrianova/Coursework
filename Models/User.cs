using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace SportCoursework.Models
{
    // Расширяем стандартного юзера своими полями
    public class AppUser : IdentityUser
    {
        public double Height { get; set; }
        public double Weight { get; set; }
        public int Age { get; set; }
        public string SportType { get; set; } = "Running";

        // Навигация: список всех тренировок этого человека
        public virtual ICollection<Training> Trainings { get; set; } = new List<Training>();
    }
}