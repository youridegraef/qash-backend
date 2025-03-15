using Application.Domain;
using Application.Interfaces;
using Microsoft.Data.Sqlite;

namespace DataAccess.Repositories;

public class BudgetRepository(DatabaseConnection _dbConnection) : IBudgetRepository
{
    public List<Budget> FindAll()
    {
        List<Budget> allBudgets = new List<Budget>();
        using SqliteConnection connection = _dbConnection.GetConnection();
        connection.Open();

        string sql = "SELECT * FROM budget";

        using SqliteCommand command = new SqliteCommand(sql, connection);
        using SqliteDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            allBudgets.Add(
                new Budget(
                    (int)reader["id"],
                    DateOnly.FromDateTime((DateTime)reader["start_date"]),
                    DateOnly.FromDateTime((DateTime)reader["end_date"]),
                    (double)reader["budget"],
                    (int)reader["category_id"]
                )
            );
        }

        return allBudgets;
    }

    public Budget? FindById(int id)
    {
        using SqliteConnection connection = _dbConnection.GetConnection();
        connection.Open();

        string sql = "SELECT * FROM budget WHERE id = @id";

        using SqliteCommand command = new SqliteCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);

        using SqliteDataReader reader = command.ExecuteReader();

        if (reader.Read())
        {
            return new Budget(
                (int)reader["id"],
                DateOnly.FromDateTime((DateTime)reader["start_date"]),
                DateOnly.FromDateTime((DateTime)reader["end_date"]),
                (double)reader["budget"],
                (int)reader["category_id"]
            );
        }

        return null;
    }

    public bool Add(Budget budget)
    {
        using SqliteConnection connection = _dbConnection.GetConnection();
        connection.Open();

        string sql = "INSERT INTO budget (start_date, end_date, budget, category_id) VALUES (@start_date, @end_date, @budget, @category_id)";

        using SqliteCommand command = new SqliteCommand(sql, connection);
        command.Parameters.AddWithValue("@start_date", budget.StartDate);
        command.Parameters.AddWithValue("@end_date", budget.EndDate);
        command.Parameters.AddWithValue("@budget", budget.BudgetAmount);
        command.Parameters.AddWithValue("@category_id", budget.CategoryId);
        
        
        int rowsAffected = command.ExecuteNonQuery();

        return rowsAffected > 0;
    }

    public bool Edit(Budget budget)
    {
        using SqliteConnection connection = _dbConnection.GetConnection();
        connection.Open();

        string sql = "UPDATE budget SET start_date = @start_date, end_date = @end_date, budget = @budget, category_id = @category_id WHERE id = @id";

        using SqliteCommand command = new SqliteCommand(sql, connection);
        
        command.Parameters.AddWithValue("@id", budget.Id);
        command.Parameters.AddWithValue("@start_date", budget.StartDate);
        command.Parameters.AddWithValue("@end_date", budget.EndDate);
        command.Parameters.AddWithValue("@budget", budget.BudgetAmount);
        command.Parameters.AddWithValue("@category_id", budget.CategoryId);

        int rowsAffected = command.ExecuteNonQuery();

        return rowsAffected > 0;
    }

    public bool Delete(Budget budget)
    {
        using SqliteConnection connection = _dbConnection.GetConnection();
        connection.Open();

        string sql = "DELETE FROM budget WHERE id = @id";

        using SqliteCommand command = new SqliteCommand(sql, connection);
        command.Parameters.AddWithValue("@id", budget.Id);

        int rowsAffected = command.ExecuteNonQuery();

        return rowsAffected > 0;
    }
}