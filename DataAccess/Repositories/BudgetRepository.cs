using Application.Domain;
using Application.Interfaces;
using MySql.Data.MySqlClient;

namespace DataAccess.Repositories;

public class BudgetRepository(DatabaseConnection _dbConnection) : IBudgetRepository
{
    public List<Budget> FindAll()
    {
        List<Budget> allBudgets = new List<Budget>();
        using MySqlConnection connection = _dbConnection.GetMySqlConnection();
        connection.Open();

        string sql = "SELECT * FROM budget";

        using MySqlCommand command = new MySqlCommand(sql, connection);
        using MySqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            allBudgets.Add(
                new Budget(
                    Convert.ToInt32(reader["id"]),
                    DateOnly.FromDateTime(Convert.ToDateTime(reader["start_date"])),
                    DateOnly.FromDateTime(Convert.ToDateTime(reader["end_date"])),
                    Convert.ToDouble(reader["budget"]),
                    Convert.ToInt32(reader["category_id"])
                )
            );
        }

        return allBudgets;
    }

    public Budget? FindById(int id)
    {
        using MySqlConnection connection = _dbConnection.GetMySqlConnection();
        connection.Open();

        string sql = "SELECT * FROM budget WHERE id = @id";

        using MySqlCommand command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);

        using MySqlDataReader reader = command.ExecuteReader();

        if (reader.Read())
        {
            return new Budget(
                Convert.ToInt32(reader["id"]),
                DateOnly.FromDateTime(Convert.ToDateTime(reader["start_date"])),
                DateOnly.FromDateTime(Convert.ToDateTime(reader["end_date"])),
                Convert.ToDouble(reader["budget"]),
                Convert.ToInt32(reader["category_id"])
            );
        }

        return null;
    }

    public bool Add(Budget budget)
    {
        using MySqlConnection connection = _dbConnection.GetMySqlConnection();
        connection.Open();

        string sql = "INSERT INTO budget (start_date, end_date, budget, category_id) VALUES (@start_date, @end_date, @budget, @category_id)";

        using MySqlCommand command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@start_date", budget.StartDate.ToDateTime(TimeOnly.MinValue));
        command.Parameters.AddWithValue("@end_date", budget.EndDate.ToDateTime(TimeOnly.MinValue));
        command.Parameters.AddWithValue("@budget", budget.BudgetAmount);
        command.Parameters.AddWithValue("@category_id", budget.CategoryId);

        int rowsAffected = command.ExecuteNonQuery();

        return rowsAffected > 0;
    }

    public bool Edit(Budget budget)
    {
        using MySqlConnection connection = _dbConnection.GetMySqlConnection();
        connection.Open();

        string sql = "UPDATE budget SET start_date = @start_date, end_date = @end_date, budget = @budget, category_id = @category_id WHERE id = @id";

        using MySqlCommand command = new MySqlCommand(sql, connection);

        command.Parameters.AddWithValue("@id", budget.Id);
        command.Parameters.AddWithValue("@start_date", budget.StartDate.ToDateTime(TimeOnly.MinValue));
        command.Parameters.AddWithValue("@end_date", budget.EndDate.ToDateTime(TimeOnly.MinValue));
        command.Parameters.AddWithValue("@budget", budget.BudgetAmount);
        command.Parameters.AddWithValue("@category_id", budget.CategoryId);

        int rowsAffected = command.ExecuteNonQuery();

        return rowsAffected > 0;
    }

    public bool Delete(Budget budget)
    {
        using MySqlConnection connection = _dbConnection.GetMySqlConnection();
        connection.Open();

        string sql = "DELETE FROM budget WHERE id = @id";

        using MySqlCommand command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", budget.Id);

        int rowsAffected = command.ExecuteNonQuery();

        return rowsAffected > 0;
    }
}