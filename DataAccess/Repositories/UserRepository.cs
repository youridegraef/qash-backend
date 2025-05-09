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

    public int Add(User user)
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

        if (rowsAffected > 0)
        {
            string selectIdSql = "SELECT LAST_INSERT_ID()";
            using MySqlCommand selectIdCommand = new MySqlCommand(selectIdSql, connection);
            int newId = Convert.ToInt32(selectIdCommand.ExecuteScalar());
            return newId;
        }

        return 0;
    }

    public bool Edit(User user)
    {
        try
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
            command.Parameters.AddWithValue("@date_of_birth", user.DateOfBirth);


            int rowsAffected = command.ExecuteNonQuery();

            return rowsAffected > 0;
        }
        catch (MySqlException ex)
        {
            Console.WriteLine($"Error updating user: {ex.Message}");
            return false;
        }
        catch(Exception ex)
        {
            Console.WriteLine($"General Error updating user: {ex.Message}");
            return false;
        }
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
}