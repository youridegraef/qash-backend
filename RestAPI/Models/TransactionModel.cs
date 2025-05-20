namespace RestAPI.Models;

public class TransactionModel
{
    public int Id { get; set; }
    public string Description { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
        
    public int CategoryId { get; set; }
    public CategoryModel Category { get; set; }
        
    public int UserId { get; set; }
    public User User { get; set; }
        
    public ICollection<TagModel> Tags { get; set; }
}