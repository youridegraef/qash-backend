using Application.Domain;

// ReSharper disable once CheckNamespace
namespace Application.Interfaces;

public interface ISavingGoalService
{
    public List<SavingGoal> GetAll();
    public SavingGoal GetById(int id);
    public List<SavingGoal> GetByUserId(int userId);
    public List<SavingGoal> GetActiveSavingGoals(int userId);
    public List<SavingGoal> GetCompletedSavingGoals(int userId);
    public List<SavingGoal> GetByName(string name);
    public List<SavingGoal> GetByNameAndUserId(string name, int userId);
    public List<SavingGoal> GetByDeadlineRange(DateOnly startDate, DateOnly endDate);
    public SavingGoal Add(string name, double target, DateOnly deadline, int userId, string colorHexCode);
    public bool Edit(SavingGoal savingGoal);
    public bool Delete(int id);
}