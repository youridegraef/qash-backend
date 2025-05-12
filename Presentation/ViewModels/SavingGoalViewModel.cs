namespace Presentation.ViewModels;

public class SavingGoalViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public double Target { get; set; }
    public DateOnly Deadline { get; set; }
    public int UserId { get; set; }

    public SavingGoalViewModel(int id, string name, double target, DateOnly deadline, int userId)
    {
        Id = id;
        Name = name;
        Target = target;
        Deadline = deadline;
        UserId = userId;
    }
}