using Application.Domain;
using Application.Interfaces;

namespace Application.Services;

public class SavingGoalService : ISavingGoalService
{
    private readonly ISavingGoalRepository _savingGoalRepository;

    public SavingGoalService(ISavingGoalRepository savingGoalRepository)
    {
        _savingGoalRepository = savingGoalRepository;
    }

    public List<SavingGoal> GetAll()
    {
        try
        {
            return _savingGoalRepository.FindAll();
        }
        catch
        {
            throw new Exception("No saving goals found");
        }
    }

    public SavingGoal GetById(int id)
    {
        SavingGoal? savingGoal = _savingGoalRepository.FindById(id);

        if (savingGoal != null)
        {
            return savingGoal;
        }

        throw new Exception("Saving goal not found");
    }

    public List<SavingGoal> GetByUserId(int userId)
    {
        List<SavingGoal> allGoals = _savingGoalRepository.FindAll();

        if (allGoals == null)
        {
            throw new Exception("No saving goals found");
        }

        var filteredGoals = allGoals
            .Where(g => g.UserId == userId).ToList();

        return filteredGoals;
    }

    public List<SavingGoal> GetActiveSavingGoals(int userId)
    {
        List<SavingGoal> allGoals = _savingGoalRepository.FindAll();
        DateOnly today = DateOnly.FromDateTime(DateTime.Today);


        if (allGoals == null)
        {
            throw new Exception("No saving goals found");
        }

        var filteredGoals = allGoals
            .Where(g => g.Deadline > today).ToList();

        return filteredGoals;
    }

    public List<SavingGoal> GetCompletedSavingGoals(int userId)
    {
        List<SavingGoal> allGoals = _savingGoalRepository.FindAll();
        DateOnly today = DateOnly.FromDateTime(DateTime.Today);


        if (allGoals == null)
        {
            throw new Exception("No saving goals found");
        }

        var filteredGoals = allGoals
            .Where(g => g.Deadline < today).ToList();

        return filteredGoals;
    }

    public List<SavingGoal> GetByName(string name)
    {
        List<SavingGoal> allGoals = _savingGoalRepository.FindAll();


        if (allGoals == null)
        {
            throw new Exception("No saving goals found");
        }

        var filteredGoals = allGoals
            .Where(g => g.Name == name).ToList();

        return filteredGoals;
    }

    public List<SavingGoal> GetByNameAndUserId(string name, int userId)
    {
        List<SavingGoal> allGoals = _savingGoalRepository.FindAll();


        if (allGoals == null)
        {
            throw new Exception("No saving goals found");
        }

        var filteredGoals = allGoals
            .Where(g =>
                g.UserId == userId
                && g.Name == name)
            .ToList();

        return filteredGoals;
    }

    public List<SavingGoal> GetByDeadlineRange(DateOnly startDate, DateOnly endDate)
    {
        List<SavingGoal> allGoals = _savingGoalRepository.FindAll();


        if (allGoals == null)
        {
            throw new Exception("No saving goals found");
        }

        var filteredGoals = allGoals
            .Where(g =>
                g.Deadline >= startDate
                && g.Deadline <= endDate
            )
            .ToList();

        return filteredGoals;
    }

    public SavingGoal Add(string name, double target, DateOnly deadline, int userId, string colorHexCode)
    {
        try
        {
            SavingGoal savingGoal = new SavingGoal(name, target, deadline, userId, colorHexCode);
            savingGoal.Id = _savingGoalRepository.Add(savingGoal);
            return savingGoal;
        }
        catch
        {
            throw new Exception("");
        }
    }

    public bool Edit(SavingGoal savingGoal)
    {
        try
        {
            return _savingGoalRepository.Edit(savingGoal);
        }
        catch
        {
            throw new Exception("Invalid saving goal");
        }
    }

    public bool Delete(int id)
    {
        try
        {
            var savingGoal = _savingGoalRepository.FindById(id);
            return _savingGoalRepository.Delete(savingGoal);
        }
        catch
        {
            throw new Exception("Saving goal not deleted.");
        }
    }
}