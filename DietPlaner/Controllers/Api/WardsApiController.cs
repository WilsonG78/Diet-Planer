using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DietPlaner.Data;
using DietPlaner.Models;
using DietPlaner.Filters;

namespace DietPlaner.Controllers.Api;

[ApiController]
[Route("api/wards")]
[ServiceFilter(typeof(ApiAuthFilter))]
public class WardsApiController : ControllerBase
{
    private readonly DietPlanerContext _context;

    public WardsApiController(DietPlanerContext context)
    {
        _context = context;
    }

    // GET api/wards
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var wards = await _context.Ward
            .Select(w => new { w.Id, w.Name, w.Floor })
            .ToListAsync();
        return Ok(wards);
    }

    // GET api/wards/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var ward = await _context.Ward
            .Include(w => w.Patients)
            .FirstOrDefaultAsync(w => w.Id == id);
        if (ward == null) return NotFound(new { error = $"Ward {id} not found." });

        return Ok(new
        {
            ward.Id, ward.Name, ward.Floor,
            PatientCount = ward.Patients?.Count ?? 0
        });
    }

    // POST api/wards
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] WardApiDto dto)
    {
        var ward = new Ward { Name = dto.Name, Floor = dto.Floor };
        _context.Ward.Add(ward);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = ward.Id }, new { ward.Id });
    }

    // PUT api/wards/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] WardApiDto dto)
    {
        var ward = await _context.Ward.FindAsync(id);
        if (ward == null) return NotFound(new { error = $"Ward {id} not found." });

        ward.Name = dto.Name;
        ward.Floor = dto.Floor;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE api/wards/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ward = await _context.Ward.FindAsync(id);
        if (ward == null) return NotFound(new { error = $"Ward {id} not found." });

        _context.Ward.Remove(ward);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

public class WardApiDto
{
    public required string Name { get; set; }
    public int Floor { get; set; }
}
