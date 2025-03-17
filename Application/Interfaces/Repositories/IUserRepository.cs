using Application.Domain;

// ReSharper disable once CheckNamespace
namespace Application.Interfaces;

public interface IUserRepository
{
    public List<User> FindAll();

    public User? FindById(int id);

    public User? FindByEmail(string email);

    public bool Add(User user);

    public bool Edit(User user);

    public bool Delete(User user);

    public List<Transaction> FindUserTransactions(int id);

    public List<Tag> FindUserTags(int id);

    public List<SavingGoal> FindUserSavingGoals(int id);

    public List<Category> FindUserCategories(int id);
}