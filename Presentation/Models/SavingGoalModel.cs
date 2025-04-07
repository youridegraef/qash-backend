namespace Presentation.Models;

public class SavingGoalModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public double Target { get; set; }
    public DateOnly Deadline { get; set; }
    public int UserId { get; set; }
}