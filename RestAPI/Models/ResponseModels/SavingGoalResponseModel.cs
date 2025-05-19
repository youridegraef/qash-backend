namespace RestAPI.Models.ResponseModels;

public class SavingGoalResponseModel
{
    public string Name { get; set; }
    public double AmountSaved { get; set; }
    public double Target { get; set; }
    public string Deadline { get; set; }
    public string ColorHexCode { get; set; }
}