using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DietPlaner.Data;
using DietPlaner.Models;
using DietPlaner.Filters;

namespace DietPlaner.Controllers.Api;

[ApiController]
[Route("api/patients")]
[ServiceFilter(typeof(ApiAuthFilter))]
public class PatientsApiController : ControllerBase
{
    private readonly DietPlanerContext _context;

    public PatientsApiController(DietPlanerContext context)
    {
        _context = context;
    }

    // GET api/patients
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var patients = await _context.Patient
            .Include(p => p.Diet)
            .Include(p => p.Ward)
            .Select(p => new
            {
                p.Id,
                p.Name,
                p.Surname,
                p.Pesel,
                Diet = p.Diet == null ? null : new { p.Diet.Id, p.Diet.Name, p.Diet.Kcal },
                Ward = p.Ward == null ? null : new { p.Ward.Id, p.Ward.Name, p.Ward.Floor }
            })
            .ToListAsync();

        return Ok(patients);
    }

    // GET api/patients/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var p = await _context.Patient
            .Include(p => p.Diet)
            .Include(p => p.Ward)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (p == null) return NotFound(new { error = $"Patient {id} not found." });

        return Ok(new
        {
            p.Id, p.Name, p.Surname, p.Pesel,
            Diet = p.Diet == null ? null : new { p.Diet.Id, p.Diet.Name, p.Diet.Kcal },
            Ward = p.Ward == null ? null : new { p.Ward.Id, p.Ward.Name, p.Ward.Floor }
        });
    }

    // POST api/patients
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PatientApiDto dto)
    {
        var patient = new Patient
        {
            Name = dto.Name,
            Surname = dto.Surname,
            Pesel = dto.Pesel,
            DietId = dto.DietId,
            WardId = dto.WardId
        };
        _context.Patient.Add(patient);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = patient.Id }, new { patient.Id });
    }

    // PUT api/patients/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] PatientApiDto dto)
    {
        var patient = await _context.Patient.FindAsync(id);
        if (patient == null) return NotFound(new { error = $"Patient {id} not found." });

        patient.Name = dto.Name;
        patient.Surname = dto.Surname;
        patient.Pesel = dto.Pesel;
        patient.DietId = dto.DietId;
        patient.WardId = dto.WardId;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE api/patients/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var patient = await _context.Patient.FindAsync(id);
        if (patient == null) return NotFound(new { error = $"Patient {id} not found." });

        _context.Patient.Remove(patient);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

public class PatientApiDto
{
    public required string Name { get; set; }
    public required string Surname { get; set; }
    public required string Pesel { get; set; }
    public int? DietId { get; set; }
    public int? WardId { get; set; }
}
