using Application.Domain;
using Application.Dtos;

// ReSharper disable once CheckNamespace
namespace Application.Interfaces;

public interface ISavingGoalService {
    public SavingGoalDto GetById(int id);
    public List<SavingGoalDto> GetByUserId(int userId);
    public List<SavingGoalDto> GetByUserIdPaged(int userId, int page, int pageSize);
    public SavingGoalDto Add(string name, double target, DateOnly deadline, int userId);
    public bool Edit(SavingGoal savingGoal);
    public bool Delete(int id);
}