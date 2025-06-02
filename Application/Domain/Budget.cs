namespace Application.Domain;

public class Budget
{
    public int Id { get; private set; }
    public DateOnly StartDate { get; private set; }
    public DateOnly EndDate { get; private set; }
    public double Target { get; private set; }
    public int CategoryId { get; private set; }

    public Budget(int id, DateOnly startDate, DateOnly endDate, double target, int categoryId)
    {
        Id = id;
        StartDate = startDate;
        EndDate = endDate;
        Target = target;
        CategoryId = categoryId;
    }

    public Budget(DateOnly startDate, DateOnly endDate, double target, int categoryId)
    {
        StartDate = startDate;
        EndDate = endDate;
        Target = target;
        CategoryId = categoryId;
    }
}