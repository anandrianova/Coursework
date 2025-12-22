using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SportCoursework.Models;

namespace SportCoursework.Controllers
{
    public class AccountController : Controller
    {
        // Сервисы для работы с аккаунтами
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> um, SignInManager<AppUser> sm)
        {
            _userManager = um;
            _signInManager = sm;
        }

        [HttpGet] public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(string email, string password, double? height, double? weight, int? age, string sportType)
        {
            // Мини-валидация на пустые поля
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Надо ввести почту и пароль");
                return View();
            }

            // Собираем объект юзера из того, что пришло из формы
            var user = new AppUser 
            { 
                UserName = email, 
                Email = email, 
                Height = height ?? 0, 
                Weight = weight ?? 0, 
                Age = age ?? 0, 
                SportType = sportType ?? "Running" // по дефолту бег
            };
            
            // Пытаемся сохранить в базу через Identity
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                // Если всё ок - логиним сразу и кидаем на главную
                await _signInManager.SignInAsync(user, isPersistent: true);
                return RedirectToAction("Index", "Home");
            }

            // Если не пролезло (пароль слабый или типа того) - пишем ошибки
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
                // Пометка для себя: чекнуть ошибки в логах докера если что
                Console.WriteLine($"[REG_ERROR]: {error.Description}");
            }

            return View();
        }

        [HttpGet] public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            // Проверка пароля. Запоминаем юзера (true)
            var result = await _signInManager.PasswordSignInAsync(email, password, true, false);
            if (result.Succeeded) return RedirectToAction("Index", "Home");
            
            ModelState.AddModelError("", "Косяк в логине или пароле");
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}