using System.ComponentModel.DataAnnotations;
using Humanizer;


namespace DietPlaner.Models;

public class Diet
{
    [Key]
    public int Id;
    [Display(Name = "Nazwa")]
    public String name;
    [Display(Name = "kcal")]
    public int kcal;
     
}