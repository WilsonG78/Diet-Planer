using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DietPlaner.Data;
using DietPlaner.Helpers;

namespace DietPlaner.Controllers;

public class AccountController : Controller
{
    private readonly DietPlanerContext _context;

    public AccountController(DietPlanerContext context)
    {
        _context = context;
    }

    public IActionResult Login()
    {
        if (HttpContext.Session.GetInt32("UserId") != null)
            return RedirectToAction("Index", "Home");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string loginName, string password)
    {
        var user = await _context.User.FirstOrDefaultAsync(u => u.LoginName == loginName);
        if (user != null && PasswordHelper.VerifyPassword(password, user.PasswordHash))
        {
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserName", user.Name + " " + user.Surname);
            HttpContext.Session.SetString("UserRole", user.UserRole.ToString());
            return RedirectToAction("Index", "Home");
        }
        ViewData["Error"] = "Invalid login or password.";
        return View();
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}
