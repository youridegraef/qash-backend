namespace RestAPI.RequestModels;

public class SavingGoalRequest(
    string name,
    decimal target,
    DateTime deadline)
{
    public string Name { get; set; } = name;
    public decimal Target { get; set; } = target;
    public DateTime Deadline { get; set; } = deadline;
}