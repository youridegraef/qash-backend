using Application.Interfaces;
using Application.Services;

namespace TestLayer.MockData
{
    public class UserRepositoryMock : IUserRepository
    {
        private readonly List<User> _users =
        [
            new User(1, "John Doe", "johndoe@gmail.com", PasswordHasher.HashPassword("password"),
                DateOnly.FromDateTime(DateTime.Today)),

            new User(2, "Jane Doe", "janedoe@gmail.com", PasswordHasher.HashPassword("secret"),
                DateOnly.FromDateTime(DateTime.Today))
        ];

        public List<User> FindAll()
        {
            return _users.ToList();
        }

        public User FindById(int id)
        {
            return _users.FirstOrDefault(u => u.Id == id)!;
        }

        public User FindByEmail(string email)
        {
            return _users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase))!;
        }

        public int Add(User user)
        {
            int newId = _users.Max(u => u.Id) + 1;
            user.Id = newId;
            _users.Add(user);
            return newId;
        }

        public bool Edit(User user)
        {
            var index = _users.FindIndex(u => u.Id == user.Id);
            if (index == -1) return false;

            var updatedUser = new User(
                user.Id,
                user.Name,
                user.Email,
                user.PasswordHash,
                user.DateOfBirth
            );
            _users[index] = updatedUser;
            return true;
        }

        public bool Delete(User user)
        {
            return _users.RemoveAll(u => u.Id == user.Id) > 0;
        }
    }
}