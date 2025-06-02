namespace RestAPI.RequestModels;

public class SavingGoalRequest(
    int id,
    string name,
    decimal amountSaved,
    decimal target,
    DateTime deadline,
    string colorHexCode,
    int userId,
    User user)
{
    public string Name { get; set; } = name;
    public decimal Target { get; set; } = target;
    public DateTime Deadline { get; set; } = deadline;
    public string ColorHexCode { get; set; } = colorHexCode;

    public int UserId { get; set; } = userId;
}