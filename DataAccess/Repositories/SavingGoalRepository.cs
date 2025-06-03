using Application.Domain;
using Application.Exceptions;
using Application.Interfaces;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace DataAccess.Repositories;

public class SavingGoalRepository(string connectionString, ILogger<SavingGoalRepository> logger)
    : ISavingGoalRepository {
    public SavingGoal FindById(int id) {
        try {
            using MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            string sql = "SELECT id, name, target, deadline, user_id FROM saving_goal WHERE id = @id";

            using MySqlCommand command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);
            using MySqlDataReader reader = command.ExecuteReader();

            if (reader.Read()) {
                return new SavingGoal(
                    reader.GetInt32(reader.GetOrdinal("id")),
                    reader.GetString(reader.GetOrdinal("name")),
                    reader.GetDouble(reader.GetOrdinal("target")),
                    DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("deadline"))),
                    reader.GetInt32(reader.GetOrdinal("user_id"))
                );
            }

            throw new SavingGoalNotFoundException($"Saving goal with ID {id} was not found.");
        }
        catch (SavingGoalNotFoundException ex) {
            logger.LogError(ex, $"Saving goal with ID {id} was not found.");
            throw;
        }
        catch (MySqlException ex) {
            logger.LogError(ex, $"Error retrieving saving goal with ID {id} from the database.");
            throw new DatabaseException($"Error retrieving saving goal with ID {id} from the database.", ex);
        }
    }

    public List<SavingGoal> FindByUserId(int userId) {
        try {
            List<SavingGoal> savingGoals = new List<SavingGoal>();

            using MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            string sql =
                "SELECT id, name, target, deadline, user_id FROM tag WHERE user_id = @user_id";

            using MySqlCommand command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@user_id", userId);

            using MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read()) {
                savingGoals.Add(
                    new SavingGoal(
                        reader.GetInt32(reader.GetOrdinal("id")),
                        reader.GetString(reader.GetOrdinal("name")),
                        reader.GetInt32(reader.GetOrdinal("target")),
                        DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("deadline"))),
                        reader.GetInt32(reader.GetOrdinal("user_id"))
                    ));
            }

            if (savingGoals.Count == 0) {
                throw new SavingGoalNotFoundException($"Saving goal with UserID {userId} was not found.");
            }

            return savingGoals;
        }
        catch (SavingGoalNotFoundException ex) {
            logger.LogError(ex, $"Saving goal with user ID {userId} was not found.");
            throw;
        }
        catch (MySqlException ex) {
            logger.LogError(ex, $"Error retrieving saving goal with user ID {userId} from the database.");
            throw new DatabaseException($"Error retrieving saving goal with user ID {userId} from the database.", ex);
        }
    }

    public List<SavingGoal> FindByUserIdPaged(int userId, int page, int pageSize) {
        try {
            List<SavingGoal> savingGoals = new List<SavingGoal>();

            using MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            int offset = (page - 1) * pageSize;
            string sql =
                "SELECT id, name, target, deadline, user_id FROM tag WHERE user_id = @user_id LIMIT @limit OFFSET @offset";

            using MySqlCommand command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@user_id", userId);
            command.Parameters.AddWithValue("@offset", offset);

            using MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read()) {
                savingGoals.Add(
                    new SavingGoal(
                        reader.GetInt32(reader.GetOrdinal("id")),
                        reader.GetString(reader.GetOrdinal("name")),
                        reader.GetInt32(reader.GetOrdinal("target")),
                        DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("deadline"))),
                        reader.GetInt32(reader.GetOrdinal("user_id"))
                    ));
            }

            if (savingGoals.Count == 0) {
                throw new SavingGoalNotFoundException($"Saving goal with UserID {userId} was not found.");
            }

            return savingGoals;
        }
        catch (SavingGoalNotFoundException ex) {
            logger.LogError(ex, $"Saving goal with user ID {userId} was not found.");
            throw;
        }
        catch (MySqlException ex) {
            logger.LogError(ex, $"Error retrieving saving goal with user ID {userId} from the database.");
            throw new DatabaseException($"Error retrieving saving goal with user ID {userId} from the database.", ex);
        }
    }

    public SavingGoal Add(SavingGoal savingGoal) {
        try {
            using MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            string sql =
                "INSERT INTO saving_goal (name, target, deadline, user_id) VALUES (@name, @target, @deadline, @user_id)";

            using MySqlCommand command = new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@name", savingGoal.Name);
            command.Parameters.AddWithValue("@target", savingGoal.Target);
            command.Parameters.AddWithValue("@deadline", savingGoal.Deadline.ToDateTime(TimeOnly.MinValue));
            command.Parameters.AddWithValue("@user_id", savingGoal.UserId);

            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected > 0) {
                string selectIdSql = "SELECT LAST_INSERT_ID()";
                using MySqlCommand selectIdCommand = new MySqlCommand(selectIdSql, connection);
                int newId = Convert.ToInt32(selectIdCommand.ExecuteScalar());
                return new SavingGoal(newId, savingGoal.Name, savingGoal.Target, savingGoal.Deadline,
                    savingGoal.UserId);
            }

            throw new DatabaseException("Failed to add the saving goal to the database. No rows were affected.");
        }
        catch (DatabaseException ex) {
            logger.LogError(ex, "Failed to add the saving goal to the database. No rows were affected.");
            throw;
        }
        catch (MySqlException ex) {
            logger.LogError(ex, "Error adding a new saving goal to the database.");
            throw new DatabaseException("Error adding a new saving goal to the database.", ex);
        }
    }

    public bool Edit(SavingGoal savingGoal) {
        try {
            using MySqlConnection connection = new MySqlConnection(connectionString);
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

            if (rowsAffected > 0) {
                return true;
            }

            throw new SavingGoalNotFoundException($"Saving goal with ID {savingGoal.Id} was not found for update.");
        }
        catch (SavingGoalNotFoundException ex) {
            logger.LogError(ex, $"Saving goal with ID {savingGoal.Id} was not found for update.");
            throw;
        }
        catch (MySqlException ex) {
            logger.LogError(ex, $"Error updating saving goal with ID {savingGoal.Id} in the database.");
            throw new DatabaseException($"Error updating saving goal with ID {savingGoal.Id} in the database.", ex);
        }
    }

    public bool Delete(SavingGoal savingGoal) {
        try {
            using MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            string sql = "DELETE FROM saving_goal WHERE id = @id";

            using MySqlCommand command = new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@id", savingGoal.Id);

            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected > 0) {
                return true;
            }

            throw new SavingGoalNotFoundException($"Saving goal with ID {savingGoal.Id} was not found for deletion.");
        }
        catch (SavingGoalNotFoundException ex) {
            logger.LogError(ex, $"Saving goal with ID {savingGoal.Id} was not found for deletion.");
            throw;
        }
        catch (MySqlException ex) {
            logger.LogError(ex, $"Error deleting saving goal with ID {savingGoal.Id} from the database.");
            throw new DatabaseException($"Error deleting saving goal with ID {savingGoal.Id} from the database.", ex);
        }
    }
}