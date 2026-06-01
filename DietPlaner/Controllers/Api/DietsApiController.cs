using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DietPlaner.Data;
using DietPlaner.Models;
using DietPlaner.Filters;

namespace DietPlaner.Controllers.Api;

[ApiController]
[Route("api/diets")]
[ServiceFilter(typeof(ApiAuthFilter))]
public class DietsApiController : ControllerBase
{
    private readonly DietPlanerContext _context;

    public DietsApiController(DietPlanerContext context)
    {
        _context = context;
    }

    // GET api/diets
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var diets = await _context.Diet
            .Select(d => new { d.Id, d.Name, d.Kcal, DietType = d.DietType.ToString() })
            .ToListAsync();
        return Ok(diets);
    }

    // GET api/diets/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var diet = await _context.Diet.FindAsync(id);
        if (diet == null) return NotFound(new { error = $"Diet {id} not found." });
        return Ok(new { diet.Id, diet.Name, diet.Kcal, DietType = diet.DietType.ToString() });
    }

    // POST api/diets
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] DietApiDto dto)
    {
        var diet = new Diet { Name = dto.Name, Kcal = dto.Kcal, DietType = dto.DietType };
        _context.Diet.Add(diet);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = diet.Id }, new { diet.Id });
    }

    // PUT api/diets/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] DietApiDto dto)
    {
        var diet = await _context.Diet.FindAsync(id);
        if (diet == null) return NotFound(new { error = $"Diet {id} not found." });

        diet.Name = dto.Name;
        diet.Kcal = dto.Kcal;
        diet.DietType = dto.DietType;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE api/diets/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var diet = await _context.Diet.FindAsync(id);
        if (diet == null) return NotFound(new { error = $"Diet {id} not found." });

        _context.Diet.Remove(diet);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

public class DietApiDto
{
    public required string Name { get; set; }
    public int Kcal { get; set; }
    public DietType DietType { get; set; }
}
