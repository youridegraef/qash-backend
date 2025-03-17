using Application.Domain;

// ReSharper disable once CheckNamespace
namespace Application.Interfaces;

public interface ISavingGoalService
{
    public List<SavingGoal> GetAlL();
    public SavingGoal GetById(int id);
    public List<SavingGoal> GetByUserId(int userId);
    List<SavingGoal> GetActiveSavingGoals(int userId);
    List<SavingGoal> GetCompletedSavingGoals(int userId);
    public List<SavingGoal> GetByName(string name);
    public List<SavingGoal> GetByNameAndUserId(string name, int userId);
    List<SavingGoal> GetByDeadlineRange(DateOnly startDate, DateOnly endDate);
    List<SavingGoal> GetByUserIdAndTargetRange(int userId, double minTarget, double maxTarget);
    public SavingGoal Add(string name, double target, DateOnly deadline, int userId);
    public bool Edit(SavingGoal savingGoal);
    public void Delete(int id);
}