namespace Application.Dtos;

public class BudgetDto(
    int id,
    string name,
    DateOnly startDate,
    DateOnly endDate,
    double spent,
    double target,
    int categoryId)
{
    public int Id { get; set; } = id;
    public string Name { get; set; } = name;
    public DateOnly StartDate { get; set; } = startDate;
    public DateOnly EndDate { get; set; } = endDate;
    public double Spent { get; set; } = spent * -1;
    public double Target { get; set; } = target;
    public int CategoryId { get; set; } = categoryId;
}