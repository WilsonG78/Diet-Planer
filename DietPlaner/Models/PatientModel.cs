using System.ComponentModel.DataAnnotations;

namespace DietPlaner.Models;
public class Patient
{
    [Key]
    public int Id { get; set; }
    [Display(Name = "Imie")]
    public String Name {get; set;}
    [Display(Name = "Nazwisko")]
    public String Surname {get; set;}
    [Display(Name = "PESEL")]
    public String Pesel {get;set;}
    [Display(Name = "Dieta")]
    public Diet diet;

}