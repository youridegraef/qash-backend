namespace Presentation.ViewModels;

public class BudgetViewModel
{
    public string name;
    public int categoryId;
    public double spendings;
    public double? budget;

    public BudgetViewModel(string name, int categoryId, double spendings, double budget)
    {
        this.name = name;
        this.categoryId = categoryId;
        if (spendings != 0)
        {
            this.spendings = spendings * -1;
        }
        else
        {
            this.spendings = 0;
        }

        this.budget = budget;
    }
}