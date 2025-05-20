namespace RestAPI.Models;

public class SavingGoalModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal AmountSaved { get; set; }
    public decimal Target { get; set; }
    public DateTime Deadline { get; set; }
    public string ColorHexCode { get; set; }
        
    public int UserId { get; set; }
    public User User { get; set; }
}