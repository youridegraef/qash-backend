namespace Application.Domain;

public class Budget
{
    public int Id { get; private set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public double BudgetAmount { get; set; }
    public int CategoryId { get; set; }

    public Budget(int id, DateOnly startDate, DateOnly endDate, double budgetAmount, int categoryId)
    {
        Id = id;
        StartDate = startDate;
        EndDate = endDate;
        BudgetAmount = budgetAmount;
        CategoryId = categoryId;
    }
}