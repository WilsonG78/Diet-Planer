using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DietPlaner.Data;
using DietPlaner.Models;

namespace DietPlaner.Controllers;

public class ReportsController : AuthenticatedController
{
    private readonly DietPlanerContext _context;

    public ReportsController(DietPlanerContext context)
    {
        _context = context;
    }

    // Zestawienie pacjentów per oddział z rozkładem diet i średnią kalorycznością
    public async Task<IActionResult> WardOverview()
    {
        var wards = await _context.Ward
            .Include(w => w.Patients!)
            .ThenInclude(p => p.Diet)
            .OrderBy(w => w.Floor)
            .ToListAsync();

        var summaries = wards.Select(w =>
        {
            var patients = w.Patients ?? new List<Patient>();
            var withDiet = patients.Where(p => p.Diet != null).ToList();

            return new WardSummaryViewModel
            {
                Ward = w,
                TotalPatients = patients.Count,
                PatientsWithDiet = withDiet.Count,
                AverageKcal = withDiet.Any() ? withDiet.Average(p => p.Diet!.Kcal) : 0,
                DietBreakdown = withDiet
                    .GroupBy(p => p.Diet!)
                    .Select(g => new DietCount
                    {
                        DietName = g.Key.Name,
                        Count = g.Count(),
                        Kcal = g.Key.Kcal
                    })
                    .OrderByDescending(d => d.Count)
                    .ToList()
            };
        }).ToList();

        ViewBag.TotalPatients = await _context.Patient.CountAsync();
        ViewBag.TotalWards = wards.Count;

        return View(summaries);
    }

    // Rozkład diet: ile pacjentów na każdej diecie, z podziałem na oddziały
    public async Task<IActionResult> DietDistribution()
    {
        var diets = await _context.Diet
            .Include(d => d.Patients!)
            .ThenInclude(p => p.Ward)
            .ToListAsync();

        int totalPatients = await _context.Patient.CountAsync();
        int unassigned = await _context.Patient.CountAsync(p => p.DietId == null);

        var summaries = diets
            .Select(d =>
            {
                var patients = d.Patients ?? new List<Patient>();
                return new DietSummaryViewModel
                {
                    Diet = d,
                    PatientCount = patients.Count,
                    ByWard = patients
                        .GroupBy(p => p.Ward?.Name ?? "No ward assigned")
                        .Select(g => new WardPatientCount { WardName = g.Key, Count = g.Count() })
                        .OrderByDescending(x => x.Count)
                        .ToList()
                };
            })
            .OrderByDescending(x => x.PatientCount)
            .ToList();

        ViewBag.TotalPatients = totalPatients;
        ViewBag.UnassignedCount = unassigned;

        return View(summaries);
    }

    // Pacjenci bez przypisanej diety lub oddziału
    public async Task<IActionResult> Unassigned()
    {
        var patients = await _context.Patient
            .Include(p => p.Diet)
            .Include(p => p.Ward)
            .Where(p => p.DietId == null || p.WardId == null)
            .OrderBy(p => p.Surname)
            .ToListAsync();

        ViewBag.NoDiet = patients.Count(p => p.DietId == null);
        ViewBag.NoWard = patients.Count(p => p.WardId == null);
        ViewBag.BothMissing = patients.Count(p => p.DietId == null && p.WardId == null);

        return View(patients);
    }
}
