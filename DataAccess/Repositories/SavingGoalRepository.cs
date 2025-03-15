using Application.Domain;
using Application.Interfaces;
using Microsoft.Data.Sqlite;

namespace DataAccess.Repositories;

public class SavingGoalRepository(DatabaseConnection _dbConnection) : ISavingGoalRepository
{
    public List<SavingGoal> FindAll()
    {
        List<SavingGoal> allSavingGoals = new List<SavingGoal>();
        using SqliteConnection connection = _dbConnection.GetConnection();
        connection.Open();

        string sql = "SELECT * FROM saving_goal";

        using SqliteCommand command = new SqliteCommand(sql, connection);
        using SqliteDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            allSavingGoals.Add(
                new SavingGoal(
                    (int)reader["id"],
                    (string)reader["name"],
                    (double)reader["target"],
                    DateOnly.FromDateTime((DateTime)reader["deadline"]),
                    (int)reader["user_id"]
                )
            );
        }

        return allSavingGoals;
    }

    public SavingGoal? FindById(int id)
    {
        using SqliteConnection connection = _dbConnection.GetConnection();
        connection.Open();

        string sql = "SELECT * FROM saving_goal WHERE id = @id";

        using SqliteCommand command = new SqliteCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);
        using SqliteDataReader reader = command.ExecuteReader();

        if (reader.Read())
        {
            return new SavingGoal(
                (int)reader["id"],
                (string)reader["name"],
                (double)reader["target"],
                DateOnly.FromDateTime((DateTime)reader["deadline"]),
                (int)reader["user_id"]
            );
        }

        return null;
    }

    public bool Add(SavingGoal savingGoal)
    {
        using SqliteConnection connection = _dbConnection.GetConnection();
        connection.Open();

        string sql = "INSERT INTO saving_goal (name, target, deadline, user_id) VALUES (@name, @target, @deadline, @user_id)";

        using SqliteCommand command = new SqliteCommand(sql, connection);
        
        command.Parameters.AddWithValue("@name", savingGoal.Name);
        command.Parameters.AddWithValue("@target", savingGoal.Target);
        command.Parameters.AddWithValue("@deadline", savingGoal.Deadline);
        command.Parameters.AddWithValue("@user_id", savingGoal.UserId);

        int rowsAffected = command.ExecuteNonQuery();

        return rowsAffected > 0;
    }

    public bool Edit(SavingGoal savingGoal)
    {
        using SqliteConnection connection = _dbConnection.GetConnection();
        connection.Open();

        string sql = "UPDATE saving_goal SET name = @name, target = @target, deadline = @deadline, user_id = @user_id WHERE id = @id";

        using SqliteCommand command = new SqliteCommand(sql, connection);

        command.Parameters.AddWithValue("@id", savingGoal.Id);
        command.Parameters.AddWithValue("@name", savingGoal.Name);
        command.Parameters.AddWithValue("@target", savingGoal.Target);
        command.Parameters.AddWithValue("@deadline", savingGoal.Deadline);
        command.Parameters.AddWithValue("@user_id", savingGoal.UserId);

        int rowsAffected = command.ExecuteNonQuery();

        return rowsAffected > 0;
    }

    public bool Delete(SavingGoal savingGoal)
    {
        using SqliteConnection connection = _dbConnection.GetConnection();
        connection.Open();

        string sql = "DELETE FROM saving_goal WHERE id = @id";

        using SqliteCommand command = new SqliteCommand(sql, connection);

        command.Parameters.AddWithValue("@id", savingGoal.Id);

        int rowsAffected = command.ExecuteNonQuery();

        return rowsAffected > 0;
    }
}