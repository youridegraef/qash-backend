using Application.Interfaces;

namespace TestLayer.MockData;

public class UserRepositoryMock : IUserRepository
{
    public List<UserAuthenticate> FindAll()
    {
        throw new NotImplementedException();
    }

    public UserAuthenticate? FindById(int id)
    {
        throw new NotImplementedException();
    }

    public UserAuthenticate? FindByEmail(string email)
    {
        throw new NotImplementedException();
    }

    public int Add(UserAuthenticate userAuthenticate)
    {
        throw new NotImplementedException();
    }

    public bool Edit(UserAuthenticate userAuthenticate)
    {
        throw new NotImplementedException();
    }

    public bool Delete(UserAuthenticate userAuthenticate)
    {
        throw new NotImplementedException();
    }
}