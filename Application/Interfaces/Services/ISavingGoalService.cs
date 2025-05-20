using Application.Domain;

// ReSharper disable once CheckNamespace
namespace Application.Interfaces;

public interface ISavingGoalService
{
    public SavingGoal GetById(int id);
    public List<SavingGoal> GetByUserId(int userId);
    public List<SavingGoal> GetByUserIdPaged(int userId, int page, int pageSize);
    public SavingGoal Add(string name, double target, DateOnly deadline, int userId, string colorHexCode);
    public bool Edit(SavingGoal savingGoal);
    public bool Delete(int id);
}