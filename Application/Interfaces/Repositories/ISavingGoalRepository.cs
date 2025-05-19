using Application.Domain;

// ReSharper disable once CheckNamespace
namespace Application.Interfaces;

public interface ISavingGoalRepository
{
    public SavingGoal FindById(int id);
    public List<SavingGoal> FindByUserId(int userId);
    public List<SavingGoal> FindByUserIdPaged(int userId, int page, int pageSize);

    public int Add(SavingGoal savingGoal);

    public bool Edit(SavingGoal savingGoal);

    public bool Delete(SavingGoal savingGoal);
}