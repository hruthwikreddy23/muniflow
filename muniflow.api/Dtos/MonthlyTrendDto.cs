namespace MuniFlow.Api.Dtos;

public class MonthlyTrendDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public string MonthName { get; set; } = string.Empty;
    public int ApprovedCount { get; set; }
    public decimal ApprovedAmount { get; set; }
    public decimal RunningTotal { get; set; }
}