namespace RestAPI.Models;

public class CategoryModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string ColorHexCode { get; set; }
        
    public ICollection<TransactionModel> Transactions { get; set; }
}