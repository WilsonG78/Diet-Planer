using System.ComponentModel.DataAnnotations;

namespace DietPlaner.Models;

public class Ward
{
    [Key]
    public int Id { get; set; }
    [Display(Name = "Ward Name")]
    public required string Name { get; set; }
    [Display(Name = "Floor")]
    public int Floor { get; set; }
    public ICollection<Patient>? Patients { get; set; }
}
