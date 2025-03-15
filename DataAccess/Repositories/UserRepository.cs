using Application.Domain;
using Application.Interfaces;
using Microsoft.Data.Sqlite;

namespace DataAccess.Repositories;

public class UserRepository(DatabaseConnection _dbConnection) : IUserRepository
{
    public List<User> FindAll()
    {
        List<User> allUsers = new List<User>();
        using SqliteConnection connection = _dbConnection.GetConnection();
        connection.Open();

        string sql = "SELECT * FROM user";

        using SqliteCommand command = new SqliteCommand(sql, connection);
        using SqliteDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            allUsers.Add(
                new User(
                    (int)reader["id"],
                    (string)reader["name"],
                    (string)reader["email"],
                    (string)reader["password_hash"],
                    DateOnly.FromDateTime((DateTime)reader["date_of_birth"])
                )
            );
        }

        return allUsers;
    }

    public User? FindById(int id)
    {
        using SqliteConnection connection = _dbConnection.GetConnection();
        connection.Open();

        string sql = "SELECT * FROM user WHERE id = @id";

        using SqliteCommand command = new SqliteCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);

        using SqliteDataReader reader = command.ExecuteReader();

        if (reader.Read())
        {
            return new User(
                (int)reader["id"],
                (string)reader["name"],
                (string)reader["email"],
                (string)reader["password_hash"],
                DateOnly.FromDateTime((DateTime)reader["date_of_birth"])
            );
        }

        return null;
    }

    public User? FindByEmail(string email)
    {
        using SqliteConnection connection = _dbConnection.GetConnection();
        connection.Open();

        string sql = "SELECT * FROM user WHERE email = @email";

        using SqliteCommand command = new SqliteCommand(sql, connection);
        command.Parameters.AddWithValue("@email", email);

        using SqliteDataReader reader = command.ExecuteReader();

        if (reader.Read())
        {
            return new User(
                (long)reader["id"],
                (string)reader["name"],
                (string)reader["email"],
                (string)reader["password_hash"],
                DateOnly.FromDateTime(DateTime.Parse(reader["date_of_birth"].ToString()))
                );
        }

        return null;
    }

    public bool Add(User user)
    {
        using SqliteConnection connection = _dbConnection.GetConnection();
        connection.Open();

        string sql =
            "INSERT INTO user (name, email, password_hash, date_of_birth)" +
            "VALUES (@name, @email, @password_hash, @date_of_birth)";

        using SqliteCommand command = new SqliteCommand(sql, connection);

        command.Parameters.AddWithValue("@name", user.Name);
        command.Parameters.AddWithValue("@email", user.Email);
        command.Parameters.AddWithValue("@password_hash", user.PasswordHash);
        command.Parameters.AddWithValue("@date_of_birth", user.DateOfBirth);

        int rowsAffected = command.ExecuteNonQuery();

        return rowsAffected > 0;
    }

    public bool Edit(User user)
    {
        using SqliteConnection connection = _dbConnection.GetConnection();
        connection.Open();

        string sql =
            "UPDATE user SET name = @name, email = @email, password_hash = @password_hash, date_of_birth = @date_of_birth WHERE id = @id";

        using SqliteCommand command = new SqliteCommand(sql, connection);

        command.Parameters.AddWithValue("@id", user.Id);
        command.Parameters.AddWithValue("@name", user.Name);
        command.Parameters.AddWithValue("@email", user.Email);
        command.Parameters.AddWithValue("@password_hash", user.PasswordHash);
        command.Parameters.AddWithValue("@date_of_birth", user.DateOfBirth);

        int rowsAffected = command.ExecuteNonQuery();

        return rowsAffected > 0;
    }

    public bool Delete(User user)
    {
        using SqliteConnection connection = _dbConnection.GetConnection();
        connection.Open();

        string sql = "DELETE FROM user WHERE id = @id";

        using SqliteCommand command = new SqliteCommand(sql, connection);
        command.Parameters.AddWithValue("@id", user.Id);

        int rowsAffected = command.ExecuteNonQuery();
        return rowsAffected > 0;
    }

    public List<Transaction> FindUserTransactions(int id)
    {
        List<Transaction> userTransactions = new List<Transaction>();
        SqliteConnection connection = _dbConnection.GetConnection();
        connection.Open();

        string sql = "SELECT * FROM transaction WHERE user_id = @id";

        using SqliteCommand command = new SqliteCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);

        using SqliteDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            userTransactions.Add(
                new Transaction(
                    (int)reader["id"],
                    (double)reader["amount"],
                    DateOnly.FromDateTime((DateTime)reader["date"]),
                    (int)reader["user_id"]
                )
            );
        }

        return userTransactions;
    }

    public List<Tag> FindUserTags(int id)
    {
        List<Tag> userTags = new List<Tag>();
        SqliteConnection connection = _dbConnection.GetConnection();
        connection.Open();

        string sql = "SELECT * FROM tag WHERE user_id = @id";

        using SqliteCommand command = new SqliteCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);

        using SqliteDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            userTags.Add(
                new Tag(
                    (int)reader["id"],
                    (string)reader["name"],
                    (string)reader["color_hex_code"],
                    (int)reader["user_id"]
                )
            );
        }

        return userTags;
    }

    public List<SavingGoal> FindUserSavingGoals(int id)
    {
        List<SavingGoal> userSavingGoals = new List<SavingGoal>();
        SqliteConnection connection = _dbConnection.GetConnection();
        connection.Open();

        string sql = "SELECT * FROM saving_goal WHERE user_id = @id";

        using SqliteCommand command = new SqliteCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);

        using SqliteDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            userSavingGoals.Add(
                new SavingGoal(
                    (int)reader["id"],
                    (string)reader["name"],
                    (double)reader["target"],
                    DateOnly.FromDateTime((DateTime)reader["deadline"]),
                    (int)reader["user_id"]
                )
            );
        }

        return userSavingGoals;
    }

    public List<Category> FindUserCategories(int id)
    {
        List<Category> userCategories = new List<Category>();
        SqliteConnection connection = _dbConnection.GetConnection();
        connection.Open();

        string sql = "SELECT * FROM category WHERE user_id = @id";

        using SqliteCommand command = new SqliteCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);
        using SqliteDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            userCategories.Add(
                new Category(
                    (int)reader["id"],
                    (string)reader["name"],
                    (int)reader["user_id"]
                )
            );
        }

        return userCategories;
    }
}