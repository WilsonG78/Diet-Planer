namespace DietPlaner.Models;

public class DietSummaryViewModel
{
    public required Diet Diet { get; set; }
    public int PatientCount { get; set; }
    public List<WardPatientCount> ByWard { get; set; } = new();
}

public class WardPatientCount
{
    public required string WardName { get; set; }
    public int Count { get; set; }
}
