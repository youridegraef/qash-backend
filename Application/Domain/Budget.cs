namespace Application.Domain;

public class Budget
{
    public int Id { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public double Target { get; set; }
    public int CategoryId { get; set; }

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