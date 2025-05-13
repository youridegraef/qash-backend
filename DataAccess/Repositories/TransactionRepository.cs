using Application.Domain;
using Application.Exceptions;
using Application.Interfaces;
using MySql.Data.MySqlClient;

namespace DataAccess.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly DatabaseConnection _dbConnection = new DatabaseConnection();

    public List<Transaction> FindAll()
    {
        try
        {
            List<Transaction> allTransactions = new List<Transaction>();
            using MySqlConnection connection = _dbConnection.GetMySqlConnection();
            connection.Open();

            string sql = "SELECT * FROM transactions";

            using MySqlCommand command = new MySqlCommand(sql, connection);
            using MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                allTransactions.Add(
                    new Transaction(
                        Convert.ToInt32(reader["id"]),
                        (string)reader["description"],
                        Convert.ToDouble(reader["amount"]),
                        DateOnly.FromDateTime(Convert.ToDateTime(reader["date"])),
                        Convert.ToInt32(reader["user_id"]),
                        Convert.ToInt32(reader["category_id"])
                    )
                );
            }

            return allTransactions;
        }
        catch (MySqlException ex)
        {
            throw new DatabaseException("Error retrieving all transactions from the database.", ex);
        }
    }

    public Transaction? FindById(int id)
    {
        try
        {
            using MySqlConnection connection = _dbConnection.GetMySqlConnection();
            connection.Open();

            string sql = "SELECT * FROM transactions WHERE id = @id";

            using MySqlCommand command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);

            using MySqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                return new Transaction(
                    Convert.ToInt32(reader["id"]),
                    (string)reader["description"],
                    Convert.ToDouble(reader["amount"]),
                    DateOnly.FromDateTime(Convert.ToDateTime(reader["date"])),
                    Convert.ToInt32(reader["user_id"]),
                    Convert.ToInt32(reader["category_id"])
                );
            }

            throw new TransactionNotFoundException($"Transaction with ID {id} was not found.");
        }
        catch (MySqlException ex)
        {
            throw new DatabaseException($"Error retrieving transaction with ID {id} from the database.", ex);
        }
    }

    public int Add(Transaction transaction)
    {
        try
        {
            using MySqlConnection connection = _dbConnection.GetMySqlConnection();
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
        catch (MySqlException ex)
        {
            throw new DatabaseException("Error adding a new transaction to the database.", ex);
        }
    }

    public bool Edit(Transaction transaction)
    {
        try
        {
            using MySqlConnection connection = _dbConnection.GetMySqlConnection();
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
        catch (MySqlException ex)
        {
            throw new DatabaseException($"Error updating transaction with ID {transaction.Id} in the database.", ex);
        }
    }

    public bool Delete(Transaction transaction)
    {
        try
        {
            using MySqlConnection connection = _dbConnection.GetMySqlConnection();
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
        catch (MySqlException ex)
        {
            throw new DatabaseException($"Error deleting transaction with ID {transaction.Id} from the database.", ex);
        }
    }

    public List<Tag> GetTagsByTransactionId(int transactionId)
    {
        try
        {
            var tags = new List<Tag>();
            using MySqlConnection connection = _dbConnection.GetMySqlConnection();
            connection.Open();

            string sql = "SELECT t.id, t.name, t.color_hex_code, t.user_id FROM tag t " +
                         "INNER JOIN transaction_tag tt ON t.id = tt.tag_id " +
                         "WHERE tt.transaction_id = @TransactionId";

            using MySqlCommand command = new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@TransactionId", transactionId);

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    tags.Add(new Tag(
                        (int)reader["id"],
                        (string)reader["name"],
                        (string)reader["color_hex_code"],
                        (int)reader["user_id"]
                    ));
                }
            }

            return tags;
        }
        catch (MySqlException ex)
        {
            throw new DatabaseException($"Error retrieving tags for transaction with ID {transactionId}.", ex);
        }
    }
}