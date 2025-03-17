using Application.Domain;

// ReSharper disable once CheckNamespace
namespace Application.Interfaces;

public interface ISavingGoalRepository
{
    public List<SavingGoal> FindAll();

    public SavingGoal? FindById(int id);

    public bool Add(SavingGoal savingGoal);

    public bool Edit(SavingGoal savingGoal);

    public bool Delete(SavingGoal savingGoal);
}