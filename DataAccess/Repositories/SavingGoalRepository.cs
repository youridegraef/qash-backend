using Application.Domain;
using Application.Interfaces;
using MySql.Data.MySqlClient;

namespace DataAccess.Repositories;

public class SavingGoalRepository(DatabaseConnection _dbConnection) : ISavingGoalRepository
{
    public List<SavingGoal> FindAll()
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

    public SavingGoal? FindById(int id)
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

        return null;
    }

    public int Add(SavingGoal savingGoal)
    {
        using MySqlConnection connection = _dbConnection.GetMySqlConnection();
        connection.Open();

        string sql = "INSERT INTO saving_goal (name, target, deadline, user_id) VALUES (@name, @target, @deadline, @user_id)";

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

        return 0;
    }

    public bool Edit(SavingGoal savingGoal)
    {
        using MySqlConnection connection = _dbConnection.GetMySqlConnection();
        connection.Open();

        string sql = "UPDATE saving_goal SET name = @name, target = @target, deadline = @deadline, user_id = @user_id WHERE id = @id";

        using MySqlCommand command = new MySqlCommand(sql, connection);

        command.Parameters.AddWithValue("@id", savingGoal.Id);
        command.Parameters.AddWithValue("@name", savingGoal.Name);
        command.Parameters.AddWithValue("@target", savingGoal.Target);
        command.Parameters.AddWithValue("@deadline", savingGoal.Deadline.ToDateTime(TimeOnly.MinValue));
        command.Parameters.AddWithValue("@user_id", savingGoal.UserId);

        int rowsAffected = command.ExecuteNonQuery();

        return rowsAffected > 0;
    }

    public bool Delete(SavingGoal savingGoal)
    {
        using MySqlConnection connection = _dbConnection.GetMySqlConnection();
        connection.Open();

        string sql = "DELETE FROM saving_goal WHERE id = @id";

        using MySqlCommand command = new MySqlCommand(sql, connection);

        command.Parameters.AddWithValue("@id", savingGoal.Id);

        int rowsAffected = command.ExecuteNonQuery();

        return rowsAffected > 0;
    }
}