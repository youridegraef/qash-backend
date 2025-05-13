using Application.Domain;

// ReSharper disable once CheckNamespace
namespace Application.Interfaces;

public interface IUserRepository
{
    public List<UserAuthenticate> FindAll();

    public UserAuthenticate? FindById(int id);

    public UserAuthenticate? FindByEmail(string email);

    public int Add(UserAuthenticate userAuthenticate);

    public bool Edit(UserAuthenticate userAuthenticate);

    public bool Delete(UserAuthenticate userAuthenticate);
}