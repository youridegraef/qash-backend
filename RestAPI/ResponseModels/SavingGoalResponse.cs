namespace RestAPI.ResponseModels;

public class SavingGoalResponse(
    int id,
    string name,
    double saved,
    double target,
    DateOnly deadline
)
{
    public int Id { get; set; } = id;
    public string Name { get; set; } = name;
    public double Saved { get; set; } = saved;
    public double Target { get; set; } = target;
    public DateOnly Deadline { get; set; } = deadline;
}