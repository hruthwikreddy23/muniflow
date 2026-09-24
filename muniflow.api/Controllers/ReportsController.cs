using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using muniflow.api.Data;
using MuniFlow.Api.Dtos;
using System.Globalization;

namespace MuniFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly MuniFlowDbContext _context;

    public ReportsController(MuniFlowDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Aggregate spending grouped by department with budget-utilization %.
    /// Mirrors the kind of budget report finance teams pull from Tyler Munis.
    /// </summary>
    [HttpGet("spending-by-department")]
    public async Task<ActionResult<IEnumerable<DepartmentSpendingDto>>> GetSpendingByDepartment()
    {
        var report = await _context.Departments
            .Select(d => new DepartmentSpendingDto
            {
                DepartmentId = d.DepartmentId,
                DepartmentName = d.Name,
                AnnualBudget = d.AnnualBudget,
                TotalPurchaseOrders = d.PurchaseOrders.Count(),
                ApprovedCount = d.PurchaseOrders.Count(po => po.Status == "Approved"),
                TotalApprovedAmount = d.PurchaseOrders
                    .Where(po => po.Status == "Approved")
                    .Sum(po => (decimal?)po.Amount) ?? 0m,
                AverageApprovedAmount = d.PurchaseOrders
                    .Where(po => po.Status == "Approved")
                    .Average(po => (decimal?)po.Amount) ?? 0m,
                BudgetUtilizationPercent = d.AnnualBudget == 0 ? 0 :
                    Math.Round(
                        (d.PurchaseOrders.Where(po => po.Status == "Approved").Sum(po => (decimal?)po.Amount) ?? 0m)
                        / d.AnnualBudget * 100m, 2)
            })
            .OrderByDescending(r => r.TotalApprovedAmount)
            .ToListAsync();

        return Ok(report);
    }

    /// <summary>
    /// Approved PO totals grouped by month with a running (cumulative) total.
    /// Useful for fiscal-year budget tracking and trend analysis.
    /// </summary>
    [HttpGet("monthly-trends")]
    public async Task<ActionResult<IEnumerable<MonthlyTrendDto>>> GetMonthlyTrends()
    {
        var grouped = await _context.PurchaseOrders
            .Where(po => po.Status == "Approved" && po.ProcessedAt != null)
            .GroupBy(po => new { po.ProcessedAt!.Value.Year, po.ProcessedAt!.Value.Month })
            .Select(g => new
            {
                g.Key.Year,
                g.Key.Month,
                ApprovedCount = g.Count(),
                ApprovedAmount = g.Sum(x => x.Amount)
            })
            .OrderBy(g => g.Year).ThenBy(g => g.Month)
            .ToListAsync();

        decimal running = 0m;
        var trends = grouped.Select(r =>
        {
            running += r.ApprovedAmount;
            return new MonthlyTrendDto
            {
                Year = r.Year,
                Month = r.Month,
                MonthName = $"{CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(r.Month)} {r.Year}",
                ApprovedCount = r.ApprovedCount,
                ApprovedAmount = r.ApprovedAmount,
                RunningTotal = running
            };
        }).ToList();

        return Ok(trends);
    }
}