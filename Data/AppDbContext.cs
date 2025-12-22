using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SportCoursework.Models;

namespace SportCoursework.Data
{
    // Наследуемся от Identity, чтобы таблицы юзеров создались сами
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        
        // Таблица для хранения тренировок
        public DbSet<Training> Trainings { get; set; }
    }
}