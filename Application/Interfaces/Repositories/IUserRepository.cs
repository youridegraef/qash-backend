// ReSharper disable once CheckNamespace

using Application.Domain;

namespace Application.Interfaces;

public interface IUserRepository {
    public User FindById(int id);

    public User FindByEmail(string email);
    public bool IsEmailAvailable(string email);

    public int Add(User user);

    public bool Edit(User user);

    public bool Delete(User user);
}