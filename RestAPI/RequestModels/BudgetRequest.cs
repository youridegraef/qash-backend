using Application.Domain;

namespace RestAPI.RequestModels;

public class BudgetRequest(DateOnly startDate, DateOnly endDate, double target, int categoryId, int userId)
{
    public DateOnly StartDate { get; set; } = startDate;
    public DateOnly EndDate { get; set; } = endDate;
    public double Target { get; set; } = target;
    public int CategoryId { get; set; } = categoryId;
    public int UserId { get; set; } = userId;
}