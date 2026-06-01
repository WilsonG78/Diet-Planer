using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DietPlaner.Data;
using DietPlaner.Models;
using DietPlaner.Helpers;

namespace DietPlaner.Controllers
{
    public class UserController : AuthenticatedController
    {
        private readonly DietPlanerContext _context;

        public UserController(DietPlanerContext context)
        {
            _context = context;
        }

        // GET: User
        public async Task<IActionResult> Index()
        {
            if (!IsAdmin()) return Forbid();
            return View(await _context.User.ToListAsync());
        }

        // GET: User/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!IsAdmin()) return Forbid();
            if (id == null) return NotFound();
            var user = await _context.User.FirstOrDefaultAsync(m => m.Id == id);
            if (user == null) return NotFound();
            return View(user);
        }

        // GET: User/Create
        public IActionResult Create()
        {
            if (!IsAdmin()) return Forbid();
            return View();
        }

        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Surname,UserRole,LoginName")] User user, string password)
        {
            if (!IsAdmin()) return Forbid();
            if (string.IsNullOrEmpty(password))
                ModelState.AddModelError("password", "Password is required.");
            if (await _context.User.AnyAsync(u => u.LoginName == user.LoginName))
                ModelState.AddModelError("LoginName", "This login is already taken.");

            if (ModelState.IsValid)
            {
                user.PasswordHash = PasswordHelper.HashPassword(password);
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: User/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsAdmin()) return Forbid();
            if (id == null) return NotFound();
            var user = await _context.User.FindAsync(id);
            if (user == null) return NotFound();
            return View(user);
        }

        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Surname,UserRole,LoginName")] User userForm, string? newPassword)
        {
            if (!IsAdmin()) return Forbid();
            if (id != userForm.Id) return NotFound();

            var user = await _context.User.FindAsync(id);
            if (user == null) return NotFound();

            if (await _context.User.AnyAsync(u => u.LoginName == userForm.LoginName && u.Id != id))
                ModelState.AddModelError("LoginName", "This login is already taken.");

            if (ModelState.IsValid)
            {
                user.Name = userForm.Name;
                user.Surname = userForm.Surname;
                user.UserRole = userForm.UserRole;
                user.LoginName = userForm.LoginName;
                if (!string.IsNullOrEmpty(newPassword))
                    user.PasswordHash = PasswordHelper.HashPassword(newPassword);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(userForm);
        }

        // GET: User/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!IsAdmin()) return Forbid();
            if (id == null) return NotFound();
            var user = await _context.User.FirstOrDefaultAsync(m => m.Id == id);
            if (user == null) return NotFound();
            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsAdmin()) return Forbid();
            var user = await _context.User.FindAsync(id);
            if (user != null)
                _context.User.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
