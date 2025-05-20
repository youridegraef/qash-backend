namespace RestAPI.ResponseModels;

public class SavingGoalResponse(
    int id,
    string name,
    decimal amountSaved,
    decimal target,
    DateTime deadline,
    string colorHexCode,
    int userId,
    User user)
{
    public int Id { get; set; } = id;
    public string Name { get; set; } = name;
    public decimal AmountSaved { get; set; } = amountSaved;
    public decimal Target { get; set; } = target;
    public DateTime Deadline { get; set; } = deadline;
    public string ColorHexCode { get; set; } = colorHexCode;

    public int UserId { get; set; } = userId;
    public User User { get; set; } = user;
}