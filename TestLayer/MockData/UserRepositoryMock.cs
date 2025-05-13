using Application.Interfaces;

namespace TestLayer.MockData;

public class UserRepositoryMock : IUserRepository
{
    public List<User> FindAll()
    {
        throw new NotImplementedException();
    }

    public User? FindById(int id)
    {
        throw new NotImplementedException();
    }

    public User? FindByEmail(string email)
    {
        throw new NotImplementedException();
    }

    public int Add(User user)
    {
        throw new NotImplementedException();
    }

    public bool Edit(User user)
    {
        throw new NotImplementedException();
    }

    public bool Delete(User user)
    {
        throw new NotImplementedException();
    }
}