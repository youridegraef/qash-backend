using Application.Domain;
using Application.Exceptions;
using Application.Interfaces;
using MySql.Data.MySqlClient;

namespace DataAccess.Repositories;

public class SavingGoalRepository : ISavingGoalRepository
{
    private readonly DatabaseConnection _dbConnection = new DatabaseConnection();

    public List<SavingGoal> FindAll()
    {
        try
        {
            List<SavingGoal> allSavingGoals = new List<SavingGoal>();
            using MySqlConnection connection = _dbConnection.GetMySqlConnection();
            connection.Open();

            string sql = "SELECT * FROM saving_goal";

            using MySqlCommand command = new MySqlCommand(sql, connection);
            using MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                allSavingGoals.Add(
                    new SavingGoal(
                        Convert.ToInt32(reader["id"]),
                        reader["name"].ToString(),
                        Convert.ToDouble(reader["target"]),
                        DateOnly.FromDateTime(Convert.ToDateTime(reader["deadline"])),
                        Convert.ToInt32(reader["user_id"])
                    )
                );
            }

            return allSavingGoals;
        }
        catch (MySqlException ex)
        {
            throw new DatabaseException("Error retrieving all saving goals from the database.", ex);
        }
    }

    public SavingGoal? FindById(int id)
    {
        try
        {
            using MySqlConnection connection = _dbConnection.GetMySqlConnection();
            connection.Open();

            string sql = "SELECT * FROM saving_goal WHERE id = @id";

            using MySqlCommand command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);
            using MySqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                return new SavingGoal(
                    Convert.ToInt32(reader["id"]),
                    reader["name"].ToString(),
                    Convert.ToDouble(reader["target"]),
                    DateOnly.FromDateTime(Convert.ToDateTime(reader["deadline"])),
                    Convert.ToInt32(reader["user_id"])
                );
            }

            throw new SavingGoalNotFoundException($"Saving goal with ID {id} was not found.");
        }
        catch (MySqlException ex)
        {
            throw new DatabaseException($"Error retrieving saving goal with ID {id} from the database.", ex);
        }
    }

    public int Add(SavingGoal savingGoal)
    {
        try
        {
            using MySqlConnection connection = _dbConnection.GetMySqlConnection();
            connection.Open();

            string sql =
                "INSERT INTO saving_goal (name, target, deadline, user_id) VALUES (@name, @target, @deadline, @user_id)";

            using MySqlCommand command = new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@name", savingGoal.Name);
            command.Parameters.AddWithValue("@target", savingGoal.Target);
            command.Parameters.AddWithValue("@deadline", savingGoal.Deadline.ToDateTime(TimeOnly.MinValue));
            command.Parameters.AddWithValue("@user_id", savingGoal.UserId);

            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                string selectIdSql = "SELECT LAST_INSERT_ID()";
                using MySqlCommand selectIdCommand = new MySqlCommand(selectIdSql, connection);
                int newId = Convert.ToInt32(selectIdCommand.ExecuteScalar());
                return newId;
            }

            throw new DatabaseException("Failed to add the saving goal to the database. No rows were affected.");
        }
        catch (MySqlException ex)
        {
            throw new DatabaseException("Error adding a new saving goal to the database.", ex);
        }
    }

    public bool Edit(SavingGoal savingGoal)
    {
        try
        {
            using MySqlConnection connection = _dbConnection.GetMySqlConnection();
            connection.Open();

            string sql =
                "UPDATE saving_goal SET name = @name, target = @target, deadline = @deadline, user_id = @user_id WHERE id = @id";

            using MySqlCommand command = new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@id", savingGoal.Id);
            command.Parameters.AddWithValue("@name", savingGoal.Name);
            command.Parameters.AddWithValue("@target", savingGoal.Target);
            command.Parameters.AddWithValue("@deadline", savingGoal.Deadline.ToDateTime(TimeOnly.MinValue));
            command.Parameters.AddWithValue("@user_id", savingGoal.UserId);

            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                return true;
            }

            throw new SavingGoalNotFoundException($"Saving goal with ID {savingGoal.Id} was not found for update.");
        }
        catch (MySqlException ex)
        {
            throw new DatabaseException($"Error updating saving goal with ID {savingGoal.Id} in the database.", ex);
        }
    }

    public bool Delete(SavingGoal savingGoal)
    {
        try
        {
            using MySqlConnection connection = _dbConnection.GetMySqlConnection();
            connection.Open();

            string sql = "DELETE FROM saving_goal WHERE id = @id";

            using MySqlCommand command = new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@id", savingGoal.Id);

            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                return true;
            }

            throw new SavingGoalNotFoundException($"Saving goal with ID {savingGoal.Id} was not found for deletion.");
        }
        catch (MySqlException ex)
        {
            throw new DatabaseException($"Error deleting saving goal with ID {savingGoal.Id} from the database.", ex);
        }
    }
}