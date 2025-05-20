namespace RestAPI.Models;

public class CategoryModel(int id, string name, string colorHexCode)
{
    public int Id { get; set; } = id;
    public string Name { get; set; } = name;
    public string ColorHexCode { get; set; } = colorHexCode;

    public ICollection<TransactionModel> Transactions { get; set; }
}