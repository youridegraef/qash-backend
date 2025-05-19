namespace RestAPI.Models.ResponseModels;

public class SavingGoalResponseModel(
    string name,
    double amountSaved,
    double target,
    string deadline,
    string colorHexCode)
{
    public string Name { get; set; } = name;
    public double AmountSaved { get; set; } = amountSaved;
    public double Target { get; set; } = target;
    public string Deadline { get; set; } = deadline;
    public string ColorHexCode { get; set; } = colorHexCode;
}