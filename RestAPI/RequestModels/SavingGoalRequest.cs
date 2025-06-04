namespace RestAPI.RequestModels;

public class SavingGoalRequest(
    string name,
    double target,
    DateOnly deadline,
    int userId)
{
    public string Name { get; set; } = name;
    public double Target { get; set; } = target;
    public DateOnly Deadline { get; set; } = deadline;
    public int UserId { get; set; } = userId;
}