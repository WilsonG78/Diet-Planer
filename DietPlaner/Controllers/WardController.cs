using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DietPlaner.Data;
using DietPlaner.Models;

namespace DietPlaner.Controllers;

public class WardController : AuthenticatedController
{
    private readonly DietPlanerContext _context;

    public WardController(DietPlanerContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _context.Ward.Include(w => w.Patients).ToListAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var ward = await _context.Ward
            .Include(w => w.Patients)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (ward == null) return NotFound();
        return View(ward);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name,Floor")] Ward ward)
    {
        if (ModelState.IsValid)
        {
            _context.Add(ward);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(ward);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var ward = await _context.Ward.FindAsync(id);
        if (ward == null) return NotFound();
        return View(ward);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Floor")] Ward ward)
    {
        if (id != ward.Id) return NotFound();
        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(ward);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Ward.Any(e => e.Id == ward.Id))
                    return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(ward);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var ward = await _context.Ward
            .Include(w => w.Patients)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (ward == null) return NotFound();
        return View(ward);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var ward = await _context.Ward.FindAsync(id);
        if (ward != null)
            _context.Ward.Remove(ward);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
