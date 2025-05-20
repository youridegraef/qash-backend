namespace RestAPI.Models;

public class UserModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }

    public ICollection<TransactionModel> Transactions { get; set; }
    public ICollection<BudgetModel> Budgets { get; set; }
    public ICollection<SavingGoalModel> SavingGoals { get; set; }
}