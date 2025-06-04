using Application.Domain;
using Application.Exceptions;
using Application.Interfaces;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace DataAccess.Repositories;

public class BudgetRepository(string connectionString, ILogger<BudgetRepository> logger) : IBudgetRepository
{
    public Budget FindById(int id) {
        try {
            using MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            string sql = "SELECT id, start_date, end_date, budget, category_id FROM budget WHERE id = @id";

            using MySqlCommand command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);

            using MySqlDataReader reader = command.ExecuteReader();

            if (reader.Read()) {
                return new Budget(
                    reader.GetInt32(reader.GetOrdinal("id")),
                    DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("start_date"))),
                    DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("end_date"))),
                    reader.GetDouble(reader.GetOrdinal("budget")),
                    reader.GetInt32(reader.GetOrdinal("category_id"))
                );
            }

            throw new BudgetNotFoundException($"Budget with ID {id} was not found.");
        }
        catch (BudgetNotFoundException ex) {
            logger.LogError(ex, "Budget with ID {BudgetId} was not found.", id);
            throw;
        }
        catch (MySqlException ex) {
            logger.LogError(ex, "Error retrieving budget with ID {BudgetId} from the database.", id);
            throw new DatabaseException($"Error retrieving budget with ID {id} from the database.", ex);
        }
    }

    public Budget FindByCategoryId(int categoryId) {
        try {
            using MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            string sql =
                "SELECT id, start_date, end_date, budget, category_id FROM budget WHERE category_id = @categoryId";

            using MySqlCommand command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@categoryId", categoryId);

            using MySqlDataReader reader = command.ExecuteReader();

            if (reader.Read()) {
                return new Budget(
                    reader.GetInt32(reader.GetOrdinal("id")),
                    DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("start_date"))),
                    DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("end_date"))),
                    reader.GetDouble(reader.GetOrdinal("budget")),
                    reader.GetInt32(reader.GetOrdinal("category_id"))
                );
            }

            throw new BudgetNotFoundException($"Budget with category ID {categoryId} was not found.");
        }
        catch (BudgetNotFoundException ex) {
            logger.LogError(ex, "Budget with category ID {CategoryId} was not found.", categoryId);
            throw;
        }
        catch (MySqlException ex) {
            logger.LogError(ex, "Database error retrieving budget with category ID {CategoryId}", categoryId);
            throw new DatabaseException($"Database error retrieving budget with category ID {categoryId}", ex);
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error retrieving budget with category ID {CategoryId}", categoryId);
            throw new Exception($"Error retrieving budget with category ID {categoryId}", ex);
        }
    }

    public List<Budget> FindByUserId(int userId) {
        try {
            var budgets = new List<Budget>();
            using MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            string sql = @"
            SELECT b.id, b.start_date, b.end_date, b.budget, b.category_id
            FROM budget b
            JOIN category c ON b.category_id = c.id
            WHERE c.user_id = @user_id";

            using MySqlCommand command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@user_id", userId);

            using MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read()) {
                budgets.Add(new Budget(
                    reader.GetInt32(reader.GetOrdinal("id")),
                    DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("start_date"))),
                    DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("end_date"))),
                    reader.GetDouble(reader.GetOrdinal("budget")),
                    reader.GetInt32(reader.GetOrdinal("category_id"))
                ));
            }

            if (budgets.Count == 0) {
                throw new BudgetNotFoundException($"No budgets found for user ID {userId}.");
            }

            return budgets;
        }
        catch (BudgetNotFoundException ex) {
            logger.LogError(ex, "No budgets found for user ID {UserId}.", userId);
            throw;
        }
        catch (MySqlException ex) {
            logger.LogError(ex, "Database error retrieving budgets for user ID {UserId}", userId);
            throw new DatabaseException($"Database error retrieving budgets for user ID {userId}", ex);
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error retrieving budgets for user ID {UserId}", userId);
            throw new Exception($"Error retrieving budgets for user ID {userId}", ex);
        }
    }

    public Budget Add(Budget budget) {
        try {
            using MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            string sql =
                "INSERT INTO budget (start_date, end_date, budget, category_id) VALUES (@start_date, @end_date, @budget, @category_id)";

            using MySqlCommand command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@start_date", budget.StartDate.ToDateTime(TimeOnly.MinValue));
            command.Parameters.AddWithValue("@end_date", budget.EndDate.ToDateTime(TimeOnly.MinValue));
            command.Parameters.AddWithValue("@budget", budget.Target);
            command.Parameters.AddWithValue("@category_id", budget.CategoryId);

            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected > 0) {
                string selectIdSql = "SELECT LAST_INSERT_ID()";
                using MySqlCommand selectIdCommand = new MySqlCommand(selectIdSql, connection);
                int newId = Convert.ToInt32(selectIdCommand.ExecuteScalar());
                return new Budget(newId, budget.StartDate, budget.EndDate, budget.Target, budget.CategoryId);
            }

            throw new DatabaseException("Failed to add the budget to the database. No rows were affected.");
        }
        catch (DatabaseException ex) {
            logger.LogError(ex, "Failed to add the budget to the database. No rows were affected.");
            throw;
        }
        catch (MySqlException ex) {
            logger.LogError(ex, "Error adding a new budget to the database.");
            throw new DatabaseException("Error adding a new budget to the database.", ex);
        }
    }

    public bool Edit(Budget budget) {
        try {
            using MySqlConnection connection = new MySqlConnection(connectionString);
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

            if (rowsAffected > 0) {
                return true;
            }

            throw new BudgetNotFoundException($"Budget with ID {budget.Id} was not found for update.");
        }
        catch (BudgetNotFoundException ex) {
            logger.LogError(ex, "Budget with ID {BudgetId} was not found for update.", budget.Id);
            throw;
        }
        catch (MySqlException ex) {
            logger.LogError(ex, "Error updating budget with ID {BudgetId} in the database.", budget.Id);
            throw new DatabaseException($"Error updating budget with ID {budget.Id} in the database.", ex);
        }
    }

    public bool Delete(Budget budget) {
        try {
            using MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            string sql = "DELETE FROM budget WHERE id = @id";

            using MySqlCommand command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", budget.Id);

            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected > 0) {
                return true;
            }

            throw new BudgetNotFoundException($"Budget with ID {budget.Id} was not found for deletion.");
        }
        catch (BudgetNotFoundException ex) {
            logger.LogError(ex, "Budget with ID {BudgetId} was not found for deletion.", budget.Id);
            throw;
        }
        catch (MySqlException ex) {
            logger.LogError(ex, "Error deleting budget with ID {BudgetId} from the database.", budget.Id);
            throw new DatabaseException($"Error deleting budget with ID {budget.Id} from the database.", ex);
        }
    }

    public double CalculateBudgetSpending(int budgetId) {
        DateOnly budgetStartDate;
        DateOnly budgetEndDate;
        int budgetCategoryId;

        try {
            using MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            string budgetSql = "SELECT start_date, end_date, category_id FROM budget WHERE id = @budgetId";
            using MySqlCommand budgetCommand = new MySqlCommand(budgetSql, connection);
            budgetCommand.Parameters.AddWithValue("@budgetId", budgetId);

            using MySqlDataReader reader = budgetCommand.ExecuteReader();
            if (reader.Read()) {
                budgetStartDate = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("start_date")));
                budgetEndDate = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("end_date")));
                budgetCategoryId = reader.GetInt32(reader.GetOrdinal("category_id"));
            }
            else {
                throw new BudgetNotFoundException($"Budget with ID {budgetId} was not found.");
            }

            reader.Close();

            string spendingSql =
                "SELECT SUM(amount) FROM transactions WHERE category_id = @categoryId AND date >= @startDate AND date <= @endDate";
            using MySqlCommand spendingCommand = new MySqlCommand(spendingSql, connection);
            spendingCommand.Parameters.AddWithValue("@categoryId", budgetCategoryId);
            spendingCommand.Parameters.AddWithValue("@startDate", budgetStartDate.ToDateTime(TimeOnly.MinValue));
            spendingCommand.Parameters.AddWithValue("@endDate", budgetEndDate.ToDateTime(TimeOnly.MinValue));

            object result = spendingCommand.ExecuteScalar()!;
            if (result != null! && result != DBNull.Value) {
                return Convert.ToDouble(result);
            }

            return 0.0;
        }
        catch (BudgetNotFoundException ex) {
            logger.LogError(ex, "Budget with ID {BudgetId} was not found when calculating spending.", budgetId);
            throw;
        }
        catch (MySqlException ex) {
            logger.LogError(ex, "Database error calculating budget spending for BudgetID {BudgetId}.", budgetId);
            throw new DatabaseException($"Database error calculating budget spending for BudgetID {budgetId}.", ex);
        }
    }
}