namespace Application.Domain;

public class SavingGoal {
    public int Id { get; private set; }
    public string Name { get; private set; }
    public double Target { get; private set; }
    public DateOnly Deadline { get; private set; }
    public int UserId { get; private set; }

    public SavingGoal(int id, string name, double target, DateOnly deadline, int userId) {
        Id = id;
        Name = name;
        Target = target;
        Deadline = deadline;
        UserId = userId;
    }

    public SavingGoal(string name, double target, DateOnly deadline, int userId) {
        Name = name;
        Target = target;
        Deadline = deadline;
        UserId = userId;
    }
}