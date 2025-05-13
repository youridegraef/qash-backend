using Application.Domain;
using Application.Exceptions;
using Application.Interfaces;
using MySql.Data.MySqlClient;

namespace DataAccess.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DatabaseConnection _dbConnection = new DatabaseConnection();

    public List<User> FindAll()
    {
        try
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
        catch (MySqlException ex)
        {
            throw new DatabaseException("Error retrieving all users from the database.", ex);
        }
    }

    public User? FindById(int id)
    {
        try
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

            throw new UserNotFoundException($"User with ID {id} was not found.");
        }
        catch (MySqlException ex)
        {
            throw new DatabaseException($"Error retrieving user with ID {id} from the database.", ex);
        }
    }

    public User? FindByEmail(string email)
    {
        try
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

            throw new UserNotFoundException($"User with email {email} was not found.");
        }
        catch (MySqlException ex)
        {
            throw new DatabaseException($"Error retrieving user with email {email} from the database.", ex);
        }
    }

    public int Add(User user)
    {
        try
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

            throw new DatabaseException("Failed to add the user to the database. No rows were affected.");
        }
        catch (MySqlException ex)
        {
            throw new DatabaseException("Error adding a new user to the database.", ex);
        }
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

            if (rowsAffected > 0)
            {
                return true;
            }

            throw new UserNotFoundException($"User with ID {user.Id} was not found for update.");
        }
        catch (MySqlException ex)
        {
            throw new DatabaseException($"Error updating user with ID {user.Id} in the database.", ex);
        }
    }

    public bool Delete(User user)
    {
        try
        {
            using MySqlConnection connection = _dbConnection.GetMySqlConnection();
            connection.Open();

            string sql = "DELETE FROM user WHERE id = @id";

            using MySqlCommand command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", user.Id);

            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                return true;
            }

            throw new UserNotFoundException($"User with ID {user.Id} was not found for deletion.");
        }
        catch (MySqlException ex)
        {
            throw new DatabaseException($"Error deleting user with ID {user.Id} from the database.", ex);
        }
    }
}