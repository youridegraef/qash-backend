namespace Application.Domain;

public class SavingGoal
{
    public int Id { get; set; }
    public string Name { get; set; }
    public double Target { get; set; }
    public DateOnly Deadline { get; set; }
    public int UserId { get; set; }
    public string ColorHexCode { get; set; }

    public SavingGoal(int id, string name, double target, DateOnly deadline, int userId, string colorHexCode)
    {
        Id = id;
        Name = name;
        Target = target;
        Deadline = deadline;
        UserId = userId;
        colorHexCode = ColorHexCode;
    }

    public SavingGoal(string name, double target, DateOnly deadline, int userId, string colorHexCode)
    {
        Name = name;
        Target = target;
        Deadline = deadline;
        UserId = userId;
        colorHexCode = ColorHexCode;
    }
}