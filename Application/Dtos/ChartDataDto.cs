namespace Application.Dtos;

public class ChartDataDto
{
    public DateOnly Date { get; set; }
    public double Balance { get; set; }

    public ChartDataDto(DateOnly date, double balance)
    {
        Date = date;
        Balance = balance;
    }
    
}