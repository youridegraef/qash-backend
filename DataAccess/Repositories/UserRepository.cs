using Application.Domain;
using Application.Interfaces;
using MySql.Data.MySqlClient;

namespace DataAccess.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DatabaseConnection _dbConnection = new DatabaseConnection();

    public List<User> FindAll()
    {
        List<User> allUsers = new List<User>();
        using MySqlConnection connection = _dbConnection.GetMySqlConnection();
        connection.Open();

        string sql = "SELECT * FROM user";

        using MySqlCommand command = new MySqlCommand(sql, connection);
        using MySqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            allUsers.Add(
                new User(
                    Convert.ToInt32(reader["id"]),
                    reader["name"].ToString(),
                    reader["email"].ToString(),
                    reader["password_hash"].ToString(),
                    DateOnly.FromDateTime(Convert.ToDateTime(reader["date_of_birth"]))
                )
            );
        }

        return allUsers;
    }

    public User? FindById(int id)
    {
        using MySqlConnection connection = _dbConnection.GetMySqlConnection();
        connection.Open();

        string sql = "SELECT * FROM user WHERE id = @id";

        using MySqlCommand command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);

        using MySqlDataReader reader = command.ExecuteReader();

        if (reader.Read())
        {
            return new User(
                Convert.ToInt32(reader["id"]),
                reader["name"].ToString(),
                reader["email"].ToString(),
                reader["password_hash"].ToString(),
                DateOnly.FromDateTime(Convert.ToDateTime(reader["date_of_birth"]))
            );
        }

        return null;
    }

    public User? FindByEmail(string email)
    {
        using MySqlConnection connection = _dbConnection.GetMySqlConnection();
        connection.Open();

        string sql = "SELECT * FROM user WHERE email = @email";

        using MySqlCommand command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@email", email);

        using MySqlDataReader reader = command.ExecuteReader();

        if (reader.Read())
        {
            return new User(
                Convert.ToInt32(reader["id"]),
                reader["name"].ToString(),
                reader["email"].ToString(),
                reader["password_hash"].ToString(),
                DateOnly.FromDateTime(Convert.ToDateTime(reader["date_of_birth"]))
            );
        }

        return null;
    }

    public bool Add(User user)
    {
        using MySqlConnection connection = _dbConnection.GetMySqlConnection();
        connection.Open();

        string sql =
            "INSERT INTO user (name, email, password_hash, date_of_birth) " +
            "VALUES (@name, @email, @password_hash, @date_of_birth)";

        using MySqlCommand command = new MySqlCommand(sql, connection);

        command.Parameters.AddWithValue("@name", user.Name);
        command.Parameters.AddWithValue("@email", user.Email);
        command.Parameters.AddWithValue("@password_hash", user.PasswordHash);
        command.Parameters.AddWithValue("@date_of_birth", user.DateOfBirth.ToDateTime(TimeOnly.MinValue));

        int rowsAffected = command.ExecuteNonQuery();

        return rowsAffected > 0;
    }

    public bool Edit(User user)
    {
        using MySqlConnection connection = _dbConnection.GetMySqlConnection();
        connection.Open();

        string sql =
            "UPDATE user SET name = @name, email = @email, password_hash = @password_hash, date_of_birth = @date_of_birth WHERE id = @id";

        using MySqlCommand command = new MySqlCommand(sql, connection);

        command.Parameters.AddWithValue("@id", user.Id);
        command.Parameters.AddWithValue("@name", user.Name);
        command.Parameters.AddWithValue("@email", user.Email);
        command.Parameters.AddWithValue("@password_hash", user.PasswordHash);
        command.Parameters.AddWithValue("@date_of_birth", user.DateOfBirth.ToDateTime(TimeOnly.MinValue));

        int rowsAffected = command.ExecuteNonQuery();

        return rowsAffected > 0;
    }

    public bool Delete(User user)
    {
        using MySqlConnection connection = _dbConnection.GetMySqlConnection();
        connection.Open();

        string sql = "DELETE FROM user WHERE id = @id";

        using MySqlCommand command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", user.Id);

        int rowsAffected = command.ExecuteNonQuery();
        return rowsAffected > 0;
    }

    public List<Transaction> FindUserTransactions(int id)
    {
        List<Transaction> userTransactions = new List<Transaction>();
        using MySqlConnection connection = _dbConnection.GetMySqlConnection();
        connection.Open();

        string sql = "SELECT * FROM transactions WHERE user_id = @id";

        using MySqlCommand command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);

        using MySqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            userTransactions.Add(
                new Transaction(
                    Convert.ToInt32(reader["id"]),
                    Convert.ToDouble(reader["amount"]),
                    DateOnly.FromDateTime(Convert.ToDateTime(reader["date"])),
                    Convert.ToInt32(reader["user_id"]),
                    Convert.ToInt32(reader["category_id"])
                )
            );
        }

        return userTransactions;
    }

    public List<Tag> FindUserTags(int id)
    {
        List<Tag> userTags = new List<Tag>();
        using MySqlConnection connection = _dbConnection.GetMySqlConnection();
        connection.Open();

        string sql = "SELECT * FROM tag WHERE user_id = @id";

        using MySqlCommand command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);

        using MySqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            userTags.Add(
                new Tag(
                    Convert.ToInt32(reader["id"]),
                    reader["name"].ToString(),
                    reader["color_hex_code"].ToString(),
                    Convert.ToInt32(reader["user_id"])
                )
            );
        }

        return userTags;
    }

    public List<SavingGoal> FindUserSavingGoals(int id)
    {
        List<SavingGoal> userSavingGoals = new List<SavingGoal>();
        using MySqlConnection connection = _dbConnection.GetMySqlConnection();
        connection.Open();

        string sql = "SELECT * FROM saving_goal WHERE user_id = @id";

        using MySqlCommand command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);

        using MySqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            userSavingGoals.Add(
                new SavingGoal(
                    Convert.ToInt32(reader["id"]),
                    reader["name"].ToString(),
                    Convert.ToDouble(reader["target"]),
                    DateOnly.FromDateTime(Convert.ToDateTime(reader["deadline"])),
                    Convert.ToInt32(reader["user_id"])
                )
            );
        }

        return userSavingGoals;
    }

    public List<Category> FindUserCategories(int id)
    {
        List<Category> userCategories = new List<Category>();
        using MySqlConnection connection = _dbConnection.GetMySqlConnection();
        connection.Open();

        string sql = "SELECT * FROM category WHERE user_id = @id";

        using MySqlCommand command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);
        using MySqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            userCategories.Add(
                new Category(
                    Convert.ToInt32(reader["id"]),
                    reader["name"].ToString(),
                    Convert.ToInt32(reader["user_id"])
                )
            );
        }

        return userCategories;
    }
}