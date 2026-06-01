namespace DietPlaner.Models;

public class WardSummaryViewModel
{
    public required Ward Ward { get; set; }
    public int TotalPatients { get; set; }
    public int PatientsWithDiet { get; set; }
    public int PatientsWithoutDiet => TotalPatients - PatientsWithDiet;
    public double AverageKcal { get; set; }
    public List<DietCount> DietBreakdown { get; set; } = new();
}

public class DietCount
{
    public required string DietName { get; set; }
    public int Count { get; set; }
    public int Kcal { get; set; }
}
