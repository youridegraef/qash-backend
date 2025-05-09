namespace Presentation.ViewModels;

public class TransactionViewModel
{
    public int Id { get; set; }
    public string Description { get; set; }
    public double Amount { get; set; }
    public DateOnly Date { get; set; }
    public int CategoryId { get; set; }

    public TransactionViewModel(int id, string description, double amount, DateOnly date, int categoryId)
    {
        
    }
}