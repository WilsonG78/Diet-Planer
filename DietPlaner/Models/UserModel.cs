using System.ComponentModel.DataAnnotations;

namespace DietPlaner.Models;
public class User
{
    [Key]
    public int Id;
    [Display(Name = "Imie")]
    public String Name {get;set;}
    [Display(Name = "Nazwisko")]
    public String Surname {get; set;}
    public UserRole userRole;
}