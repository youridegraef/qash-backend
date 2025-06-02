using Application.Exceptions;
using Application.Interfaces;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using SQLitePCL;

namespace DataAccess.Repositories;

public class UserRepository(string connectionString, ILogger<UserRepository> logger) : IUserRepository {
    public User FindById(int id) {
        try {
            using MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            string sql = "SELECT id, name, email, password_hash FROM user WHERE id = @id";

            using MySqlCommand command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);

            using MySqlDataReader reader = command.ExecuteReader();

            if (reader.Read()) {
                return new User(
                    reader.GetInt32(reader.GetOrdinal("id")),
                    reader.GetString(reader.GetOrdinal("name")),
                    reader.GetString(reader.GetOrdinal("email")),
                    reader.GetString(reader.GetOrdinal("password_hash"))
                );
            }

            throw new UserNotFoundException($"User with ID {id} was not found.");
        }
        catch (UserNotFoundException ex) {
            logger.LogError(ex, $"User with ID {id} was not found.");
            throw;
        }
        catch (MySqlException ex) {
            logger.LogError(ex, $"Error retrieving user with ID {id} from the database.");
            throw new DatabaseException($"Error retrieving user with ID {id} from the database.", ex);
        }
    }

    public bool IsEmailAvailable(string email) {
        using MySqlConnection connection = new MySqlConnection(connectionString);
        connection.Open();

        string sql = "SELECT email FROM user WHERE email = @email";

        using MySqlCommand command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@email", email);

        using MySqlDataReader reader = command.ExecuteReader();

        return !reader.Read();
    }

    public User FindByEmail(string email) {
        try {
            using MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            string sql = "SELECT id, name, email, password_hash FROM user WHERE email = @email";

            using MySqlCommand command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@email", email);

            using MySqlDataReader reader = command.ExecuteReader();

            if (reader.Read()) {
                return new User(
                    reader.GetInt32(reader.GetOrdinal("id")),
                    reader.GetString(reader.GetOrdinal("name")),
                    reader.GetString(reader.GetOrdinal("email")),
                    reader.GetString(reader.GetOrdinal("password_hash"))
                );
            }

            throw new UserNotFoundException($"User with email {email} was not found.");
        }
        catch (MySqlException ex) {
            throw new DatabaseException($"Error retrieving user with email {email} from the database.", ex);
        }
    }

    public int Add(User user) {
        try {
            using MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            string sql =
                "INSERT INTO user (name, email, password_hash) " +
                "VALUES (@name, @email, @password_hash)";

            using MySqlCommand command = new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@name", user.Name);
            command.Parameters.AddWithValue("@email", user.Email);
            command.Parameters.AddWithValue("@password_hash", user.PasswordHash);

            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected > 0) {
                string selectIdSql = "SELECT LAST_INSERT_ID()";
                using MySqlCommand selectIdCommand = new MySqlCommand(selectIdSql, connection);
                int newId = Convert.ToInt32(selectIdCommand.ExecuteScalar());
                return newId;
            }

            throw new DatabaseException("Failed to add the user to the database. No rows were affected.");
        }
        catch (DatabaseException ex) {
            logger.LogError(ex, "Failed to add the user to the database. No rows were affected.");
            throw;
        }
        catch (MySqlException ex) {
            logger.LogError(ex, "Error adding a new user to the database.");
            throw new DatabaseException("Error adding a new user to the database.", ex);
        }
    }

    public bool Edit(User user) {
        try {
            using MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            string sql =
                "UPDATE user SET name = @name, email = @email, password_hash = @password_hash WHERE id = @id";

            using MySqlCommand command = new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@id", user.Id);
            command.Parameters.AddWithValue("@name", user.Name);
            command.Parameters.AddWithValue("@email", user.Email);
            command.Parameters.AddWithValue("@password_hash", user.PasswordHash);

            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected > 0) {
                return true;
            }

            throw new UserNotFoundException($"User with ID {user.Id} was not found for update.");
        }
        catch (MySqlException ex) {
            throw new DatabaseException($"Error updating user with ID {user.Id} in the database.", ex);
        }
    }

    public bool Delete(User user) {
        try {
            using MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            string sql = "DELETE FROM user WHERE id = @id";

            using MySqlCommand command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", user.Id);

            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected > 0) {
                return true;
            }

            throw new UserNotFoundException($"User with ID {user.Id} was not found for deletion.");
        }
        catch (MySqlException ex) {
            throw new DatabaseException($"Error deleting user with ID {user.Id} from the database.", ex);
        }
    }
}