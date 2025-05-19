using Application.Domain;
using Application.Exceptions;
using Application.Interfaces;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace DataAccess.Repositories;

public class TransactionRepository(string connectionString, ILogger<TransactionRepository> logger)
    : ITransactionRepository
{
    public List<Transaction> FindAll()
    {
        try
        {
            List<Transaction> transactions = new List<Transaction>();
            using MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            string sql =
                "SELECT id, description, amount, date, user_id, category_id FROM transactions ORDER BY date DESC";

            using MySqlCommand command = new MySqlCommand(sql, connection);

            using MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                transactions.Add(
                    new Transaction(
                        reader.GetInt32(reader.GetOrdinal("id")),
                        reader.GetString(reader.GetOrdinal("description")),
                        reader.GetDouble(reader.GetOrdinal("amount")),
                        DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("date"))),
                        reader.GetInt32(reader.GetOrdinal("user_id")),
                        reader.GetInt32(reader.GetOrdinal("category_id"))
                    )
                );
            }

            return transactions;
        }
        catch (MySqlException ex)
        {
            logger.LogError(ex, "Error retrieving all transactions from the database.");
            throw new DatabaseException("Error retrieving all transactions from the database.", ex);
        }
    }

    public List<Transaction> FindAllPaged(int page, int pageSize)
    {
        try
        {
            List<Transaction> transactions = new List<Transaction>();
            using MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            int offset = (page - 1) * pageSize;
            string sql =
                "SELECT id, description, amount, date, user_id, category_id FROM transactions ORDER BY date DESC LIMIT @limit OFFSET @offset";

            using MySqlCommand command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@limit", pageSize);
            command.Parameters.AddWithValue("@offset", offset);

            using MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                transactions.Add(
                    new Transaction(
                        reader.GetInt32(reader.GetOrdinal("id")),
                        reader.GetString(reader.GetOrdinal("description")),
                        reader.GetDouble(reader.GetOrdinal("amount")),
                        DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("date"))),
                        reader.GetInt32(reader.GetOrdinal("user_id")),
                        reader.GetInt32(reader.GetOrdinal("category_id"))
                    )
                );
            }

            return transactions;
        }
        catch (MySqlException ex)
        {
            logger.LogError(ex, "Error retrieving all transactions from the database.");
            throw new DatabaseException("Error retrieving all transactions from the database.", ex);
        }
    }

    public List<Transaction> FindByUserIdPaged(int userId, int page, int pageSize)
    {
        try
        {
            List<Transaction> transactions = new List<Transaction>();
            using MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            int offset = (page - 1) * pageSize;
            string sql =
                "SELECT id, description, amount, date, user_id, category_id FROM transactions ORDER BY date DESC LIMIT @limit OFFSET @offset";

            using MySqlCommand command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@limit", pageSize);
            command.Parameters.AddWithValue("@offset", offset);

            using MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                transactions.Add(
                    new Transaction(
                        reader.GetInt32(reader.GetOrdinal("id")),
                        reader.GetString(reader.GetOrdinal("description")),
                        reader.GetDouble(reader.GetOrdinal("amount")),
                        DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("date"))),
                        reader.GetInt32(reader.GetOrdinal("user_id")),
                        reader.GetInt32(reader.GetOrdinal("category_id"))
                    )
                );
            }

            if (transactions.Count == 0)
            {
                throw new TransactionNotFoundException($"Transaction with UserID {userId} was not found.");
            }

            return transactions;
        }
        catch (TransactionNotFoundException ex)
        {
            logger.LogError(ex, $"Transaction with UserID {userId} was not found.");
            throw;
        }
        catch (MySqlException ex)
        {
            logger.LogError(ex, "Error retrieving paged transactions from the database.");
            throw new DatabaseException("Error retrieving paged transactions from the database.", ex);
        }
    }

    public Transaction FindById(int id)
    {
        try
        {
            using MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            string sql = "SELECT id, description, amount, date, user_id, category_id FROM transactions WHERE id = @id";

            using MySqlCommand command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);

            using MySqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                return new Transaction(
                    reader.GetInt32(reader.GetOrdinal("id")),
                    reader.GetString(reader.GetOrdinal("description")),
                    reader.GetDouble(reader.GetOrdinal("amount")),
                    DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("date"))),
                    reader.GetInt32(reader.GetOrdinal("user_id")),
                    reader.GetInt32(reader.GetOrdinal("category_id"))
                );
            }

            throw new TransactionNotFoundException($"Transaction with ID {id} was not found.");
        }
        catch (TransactionNotFoundException ex)
        {
            logger.LogError(ex, $"Transaction with ID {id} was not found.");
            throw;
        }
        catch (MySqlException ex)
        {
            logger.LogError(ex, $"Error retrieving transaction with ID {id} from the database.");
            throw new DatabaseException($"Error retrieving transaction with ID {id} from the database.", ex);
        }
    }

    public List<Transaction> FindByUserId(int userId)
    {
        try
        {
            List<Transaction> transactions = new List<Transaction>();

            using MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            string sql =
                "SELECT id, description, amount, date, user_id, category_id FROM transactions WHERE user_id = @user_id ORDER BY date DESC";

            using MySqlCommand command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@user_id", userId);

            using MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                transactions.Add(
                    new Transaction(
                        reader.GetInt32(reader.GetOrdinal("id")),
                        reader.GetString(reader.GetOrdinal("description")),
                        reader.GetDouble(reader.GetOrdinal("amount")),
                        DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("date"))),
                        reader.GetInt32(reader.GetOrdinal("user_id")),
                        reader.GetInt32(reader.GetOrdinal("category_id"))
                    )
                );
            }

            return transactions;
        }
        catch (TransactionNotFoundException ex)
        {
            logger.LogError(ex, $"Transaction with UserID {userId} was not found.");
            throw new TransactionNotFoundException($"Transaction with UserID {userId} was not found.");
        }
        catch (MySqlException ex)
        {
            logger.LogError(ex, $"Error retrieving transaction with UserID {userId} from the database.");
            throw new DatabaseException($"Error retrieving transaction with UserID {userId} from the database.", ex);
        }
    }

    public int Add(Transaction transaction)
    {
        try
        {
            using MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            string sql =
                "INSERT INTO transactions (description, amount, date, user_id, category_id) VALUES (@description, @amount, @date, @user_id, @category_id)";

            using MySqlCommand command = new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@description", transaction.Description);
            command.Parameters.AddWithValue("@amount", transaction.Amount);
            command.Parameters.AddWithValue("@date", transaction.Date.ToDateTime(TimeOnly.MinValue));
            command.Parameters.AddWithValue("@user_id", transaction.UserId);
            command.Parameters.AddWithValue("@category_id", transaction.CategoryId);

            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                string selectIdSql = "SELECT LAST_INSERT_ID()";
                using MySqlCommand selectIdCommand = new MySqlCommand(selectIdSql, connection);
                int newId = Convert.ToInt32(selectIdCommand.ExecuteScalar());
                return newId;
            }

            throw new DatabaseException("Failed to add the transaction to the database. No rows were affected.");
        }
        catch (DatabaseException ex)
        {
            logger.LogError(ex, "Failed to add the transaction to the database. No rows were affected.");
            throw;
        }
        catch (MySqlException ex)
        {
            logger.LogError(ex, "Error adding a new transaction to the database.");
            throw new DatabaseException("Error adding a new transaction to the database.", ex);
        }
    }

    public bool Edit(Transaction transaction)
    {
        try
        {
            using MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            string sql =
                "UPDATE transactions SET description = @description, amount = @amount, date = @date, user_id = @user_id, category_id = @category_id WHERE id = @id";

            using MySqlCommand command = new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@id", transaction.Id);
            command.Parameters.AddWithValue("@description", transaction.Description);
            command.Parameters.AddWithValue("@amount", transaction.Amount);
            command.Parameters.AddWithValue("@date", transaction.Date.ToDateTime(TimeOnly.MinValue));
            command.Parameters.AddWithValue("@user_id", transaction.UserId);
            command.Parameters.AddWithValue("@category_id", transaction.CategoryId);

            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                return true;
            }

            throw new TransactionNotFoundException($"Transaction with ID {transaction.Id} was not found for update.");
        }
        catch (TransactionNotFoundException ex)
        {
            logger.LogError(ex, $"Transaction with ID {transaction.Id} was not found for update.");
            throw;
        }
        catch (MySqlException ex)
        {
            logger.LogError(ex, $"Error updating transaction with ID {transaction.Id} in the database.");
            throw new DatabaseException($"Error updating transaction with ID {transaction.Id} in the database.", ex);
        }
    }

    public bool Delete(Transaction transaction)
    {
        try
        {
            using MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            string sql = "DELETE FROM transactions WHERE id = @id";

            using MySqlCommand command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", transaction.Id);

            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                return true;
            }

            throw new TransactionNotFoundException($"Transaction with ID {transaction.Id} was not found for deletion.");
        }
        catch (TransactionNotFoundException ex)
        {
            logger.LogError(ex, $"Transaction with ID {transaction.Id} was not found for deletion.");
            throw;
        }
        catch (MySqlException ex)
        {
            logger.LogError(ex, $"Error deleting transaction with ID {transaction.Id} from the database.");
            throw new DatabaseException($"Error deleting transaction with ID {transaction.Id} from the database.", ex);
        }
    }
}