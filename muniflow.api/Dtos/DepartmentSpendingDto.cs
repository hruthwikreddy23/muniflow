namespace MuniFlow.Api.Dtos;

public class DepartmentSpendingDto
{
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public decimal AnnualBudget { get; set; }
    public int TotalPurchaseOrders { get; set; }
    public int ApprovedCount { get; set; }
    public decimal TotalApprovedAmount { get; set; }
    public decimal AverageApprovedAmount { get; set; }
    public decimal BudgetUtilizationPercent { get; set; }
}