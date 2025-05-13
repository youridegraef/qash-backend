using Application.Domain;
using Application.Exceptions;
using Application.Interfaces;
using MySql.Data.MySqlClient;

namespace DataAccess.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DatabaseConnection _dbConnection = new DatabaseConnection();

    public List<UserAuthenticate> FindAll()
    {
        try
        {
            List<UserAuthenticate> allUsers = new List<UserAuthenticate>();
            using MySqlConnection connection = _dbConnection.GetMySqlConnection();
            connection.Open();

            string sql = "SELECT * FROM userAuthenticate";

            using MySqlCommand command = new MySqlCommand(sql, connection);
            using MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                allUsers.Add(
                    new UserAuthenticate(
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

    public UserAuthenticate? FindById(int id)
    {
        try
        {
            using MySqlConnection connection = _dbConnection.GetMySqlConnection();
            connection.Open();

            string sql = "SELECT * FROM userAuthenticate WHERE id = @id";

            using MySqlCommand command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);

            using MySqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                return new UserAuthenticate(
                    Convert.ToInt32(reader["id"]),
                    reader["name"].ToString(),
                    reader["email"].ToString(),
                    reader["password_hash"].ToString(),
                    DateOnly.FromDateTime(Convert.ToDateTime(reader["date_of_birth"]))
                );
            }

            throw new UserNotFoundException($"UserAuthenticate with ID {id} was not found.");
        }
        catch (MySqlException ex)
        {
            throw new DatabaseException($"Error retrieving userAuthenticate with ID {id} from the database.", ex);
        }
    }

    public UserAuthenticate? FindByEmail(string email)
    {
        try
        {
            using MySqlConnection connection = _dbConnection.GetMySqlConnection();
            connection.Open();

            string sql = "SELECT * FROM userAuthenticate WHERE email = @email";

            using MySqlCommand command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@email", email);

            using MySqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                return new UserAuthenticate(
                    Convert.ToInt32(reader["id"]),
                    reader["name"].ToString(),
                    reader["email"].ToString(),
                    reader["password_hash"].ToString(),
                    DateOnly.FromDateTime(Convert.ToDateTime(reader["date_of_birth"]))
                );
            }

            throw new UserNotFoundException($"UserAuthenticate with email {email} was not found.");
        }
        catch (MySqlException ex)
        {
            throw new DatabaseException($"Error retrieving userAuthenticate with email {email} from the database.", ex);
        }
    }

    public int Add(UserAuthenticate userAuthenticate)
    {
        try
        {
            using MySqlConnection connection = _dbConnection.GetMySqlConnection();
            connection.Open();

            string sql =
                "INSERT INTO userAuthenticate (name, email, password_hash, date_of_birth) " +
                "VALUES (@name, @email, @password_hash, @date_of_birth)";

            using MySqlCommand command = new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@name", userAuthenticate.Name);
            command.Parameters.AddWithValue("@email", userAuthenticate.Email);
            command.Parameters.AddWithValue("@password_hash", userAuthenticate.PasswordHash);
            command.Parameters.AddWithValue("@date_of_birth", userAuthenticate.DateOfBirth.ToDateTime(TimeOnly.MinValue));

            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                string selectIdSql = "SELECT LAST_INSERT_ID()";
                using MySqlCommand selectIdCommand = new MySqlCommand(selectIdSql, connection);
                int newId = Convert.ToInt32(selectIdCommand.ExecuteScalar());
                return newId;
            }

            throw new DatabaseException("Failed to add the userAuthenticate to the database. No rows were affected.");
        }
        catch (MySqlException ex)
        {
            throw new DatabaseException("Error adding a new userAuthenticate to the database.", ex);
        }
    }

    public bool Edit(UserAuthenticate userAuthenticate)
    {
        try
        {
            using MySqlConnection connection = _dbConnection.GetMySqlConnection();
            connection.Open();

            string sql =
                "UPDATE userAuthenticate SET name = @name, email = @email, password_hash = @password_hash, date_of_birth = @date_of_birth WHERE id = @id";

            using MySqlCommand command = new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@id", userAuthenticate.Id);
            command.Parameters.AddWithValue("@name", userAuthenticate.Name);
            command.Parameters.AddWithValue("@email", userAuthenticate.Email);
            command.Parameters.AddWithValue("@password_hash", userAuthenticate.PasswordHash);
            command.Parameters.AddWithValue("@date_of_birth", userAuthenticate.DateOfBirth);

            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                return true;
            }

            throw new UserNotFoundException($"UserAuthenticate with ID {userAuthenticate.Id} was not found for update.");
        }
        catch (MySqlException ex)
        {
            throw new DatabaseException($"Error updating userAuthenticate with ID {userAuthenticate.Id} in the database.", ex);
        }
    }

    public bool Delete(UserAuthenticate userAuthenticate)
    {
        try
        {
            using MySqlConnection connection = _dbConnection.GetMySqlConnection();
            connection.Open();

            string sql = "DELETE FROM userAuthenticate WHERE id = @id";

            using MySqlCommand command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", userAuthenticate.Id);

            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                return true;
            }

            throw new UserNotFoundException($"UserAuthenticate with ID {userAuthenticate.Id} was not found for deletion.");
        }
        catch (MySqlException ex)
        {
            throw new DatabaseException($"Error deleting userAuthenticate with ID {userAuthenticate.Id} from the database.", ex);
        }
    }
}