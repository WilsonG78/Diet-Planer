using System.ComponentModel.DataAnnotations;

namespace DietPlaner.Models;

public class Patient
{
    [Key]
    public int Id { get; set; }
    [Display(Name = "First Name")]
    public required string Name { get; set; }
    [Display(Name = "Last Name")]
    public required string Surname { get; set; }
    [Display(Name = "PESEL")]
    public required string Pesel { get; set; }
    public int? DietId { get; set; }
    public Diet? Diet { get; set; }
    public int? WardId { get; set; }
    public Ward? Ward { get; set; }
}
