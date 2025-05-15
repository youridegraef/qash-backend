using Application.Domain;
using Application.Exceptions;
using Application.Interfaces;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace DataAccess.Repositories;

public class BudgetRepository : IBudgetRepository
{
    private readonly string _connectionString;
    private readonly ILogger<BudgetRepository> _logger;

    public BudgetRepository(string connectionString, ILogger<BudgetRepository> logger)
    {
        _connectionString = connectionString;
        _logger = logger;
    }

    public List<Budget> FindAll()
    {
        try
        {
            List<Budget> allBudgets = new List<Budget>();
            using MySqlConnection connection = new MySqlConnection(_connectionString);
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
        catch (MySqlException ex)
        {
            throw new DatabaseException("Error retrieving all budgets from the database.", ex);
        }
    }

    public Budget? FindById(int id)
    {
        try
        {
            using MySqlConnection connection = new MySqlConnection(_connectionString);
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

            throw new BudgetNotFoundException($"Budget with ID {id} was not found.");
        }
        catch (MySqlException ex)
        {
            throw new DatabaseException($"Error retrieving budget with ID {id} from the database.", ex);
        }
    }

    public int Add(Budget budget)
    {
        try
        {
            using MySqlConnection connection = new MySqlConnection(_connectionString);
            connection.Open();

            string sql =
                "INSERT INTO budget (start_date, end_date, budget, category_id) VALUES (@start_date, @end_date, @budget, @category_id)";

            using MySqlCommand command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@start_date", budget.StartDate.ToDateTime(TimeOnly.MinValue));
            command.Parameters.AddWithValue("@end_date", budget.EndDate.ToDateTime(TimeOnly.MinValue));
            command.Parameters.AddWithValue("@budget", budget.Target);
            command.Parameters.AddWithValue("@category_id", budget.CategoryId);

            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                string selectIdSql = "SELECT LAST_INSERT_ID()";
                using MySqlCommand selectIdCommand = new MySqlCommand(selectIdSql, connection);
                int newId = Convert.ToInt32(selectIdCommand.ExecuteScalar());
                return newId;
            }

            throw new DatabaseException("Failed to add the budget to the database. No rows were affected.");
        }
        catch (MySqlException ex)
        {
            throw new DatabaseException("Error adding a new budget to the database.", ex);
        }
    }

    public bool Edit(Budget budget)
    {
        try
        {
            using MySqlConnection connection = new MySqlConnection(_connectionString);
            connection.Open();

            string sql =
                "UPDATE budget SET start_date = @start_date, end_date = @end_date, budget = @budget, category_id = @category_id WHERE id = @id";

            using MySqlCommand command = new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@id", budget.Id);
            command.Parameters.AddWithValue("@start_date", budget.StartDate.ToDateTime(TimeOnly.MinValue));
            command.Parameters.AddWithValue("@end_date", budget.EndDate.ToDateTime(TimeOnly.MinValue));
            command.Parameters.AddWithValue("@budget", budget.Target);
            command.Parameters.AddWithValue("@category_id", budget.CategoryId);

            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                return true;
            }

            throw new BudgetNotFoundException($"Budget with ID {budget.Id} was not found for update.");
        }
        catch (MySqlException ex)
        {
            throw new DatabaseException($"Error updating budget with ID {budget.Id} in the database.", ex);
        }
    }

    public bool Delete(Budget budget)
    {
        try
        {
            using MySqlConnection connection = new MySqlConnection(_connectionString);
            connection.Open();

            string sql = "DELETE FROM budget WHERE id = @id";

            using MySqlCommand command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", budget.Id);

            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                return true;
            }

            throw new BudgetNotFoundException($"Budget with ID {budget.Id} was not found for deletion.");
        }
        catch (MySqlException ex)
        {
            throw new DatabaseException($"Error deleting budget with ID {budget.Id} from the database.", ex);
        }
    }
}