namespace Application.Dtos;

public class SavingGoalDto(
    int id,
    string name,
    double amountSaved,
    double target,
    DateOnly deadline)
{
    public int Id { get; set; } = id;
    public string Name { get; set; } = name;
    public double AmountSaved { get; set; } = amountSaved;
    public double Target { get; set; } = target;
    public DateOnly Deadline { get; set; } = deadline;
}