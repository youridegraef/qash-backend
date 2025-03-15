namespace Application.Domain;

public class SavingGoal
{
    public int Id { get; private set; }
    public string Name { get; set; }
    public double Target { get; set; }
    public DateOnly Deadline { get; set; }
    public int UserId { get; set; }

    public SavingGoal(int id, string name, double target, DateOnly deadline, int userId)
    {
        Id = id;
        Name = name;
        Target = target;
        Deadline = deadline;
        UserId = userId;
    }
}