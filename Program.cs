using Microsoft.EntityFrameworkCore;
using SportCoursework.Data;
using SportCoursework.Models;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Достаем строку подключения из окружения (для докера)
var connString = Environment.GetEnvironmentVariable("DB_CONNECTION");
builder.Services.AddDbContext<AppDbContext>(op => op.UseNpgsql(connString));

// Настройки безопасности паролей и т.д.
builder.Services.AddIdentity<AppUser, IdentityRole>(opts => {
    opts.Password.RequiredLength = 6;
    opts.Password.RequireNonAlphanumeric = true; 
    opts.Password.RequireUppercase = true;      
    opts.Password.RequireLowercase = true;
    opts.Password.RequireDigit = true;          
    opts.User.RequireUniqueEmail = true;
}).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Фишка, чтобы база создавалась сама при старте если её нет
using (var scope = app.Services.CreateScope()) {
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.UseStaticFiles();
app.UseRouting();

// Важно: аутентификация ПЕРЕД авторизацией
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();