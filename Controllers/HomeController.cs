using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Важно для .Include()
using SportCoursework.Data;
using SportCoursework.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

[Authorize]
public class HomeController : Controller
{
    private readonly AppDbContext _db;
    private readonly UserManager<AppUser> _userManager;

    public HomeController(AppDbContext db, UserManager<AppUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        
        // пользователь и его тренировки
        var user = await _db.Users
            .Include(u => u.Trainings) 
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) return RedirectToAction("Login", "Account");

        // ИМТ 
        double heightInMeters = user.Height / 100;
        ViewBag.BMI = (heightInMeters > 0) ? Math.Round(user.Weight / (heightInMeters * heightInMeters), 1) : 0;

        var history = user.Trainings.OrderBy(t => t.Date).ToList();

        // для графиков данные
        ViewBag.Dates = history.Select(t => t.Date.ToString("dd.MM HH:mm")).ToArray();
        ViewBag.Values = history.Select(t => t.GetEfficiency(user.SportType)).ToArray();
        
        ViewBag.Weights = history.Select(t => t.WeightAtTraining ?? user.Weight).ToArray();

        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> AddTraining(double? timeMinutes, double? distance, int? actionsCount, double? weightAtTraining)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var training = new Training
        {
            UserId = user.Id,
            Date = DateTime.UtcNow,
            Distance = distance,
            TimeMinutes = timeMinutes,
            ActionsCount = actionsCount,
            WeightAtTraining = weightAtTraining 
        };

        // обновление веса, если он внесен
        if (weightAtTraining.HasValue)
        {
            user.Weight = weightAtTraining.Value;
        }

        _db.Trainings.Add(training);
        await _db.SaveChangesAsync();
        
        return RedirectToAction("Index");
    }
}