using System.ComponentModel.DataAnnotations;

namespace DietPlaner.Models;

public class User
{
    [Key]
    public int Id { get; set; }
    [Display(Name = "First Name")]
    public required string Name { get; set; }
    [Display(Name = "Last Name")]
    public required string Surname { get; set; }
    [Display(Name = "Login")]
    public required string LoginName { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    [Display(Name = "API Token")]
    public string ApiToken { get; set; } = string.Empty;
    public UserRole UserRole { get; set; }
}
