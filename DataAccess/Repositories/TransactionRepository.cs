using Application.Domain;
using Application.Interfaces;
using MySql.Data.MySqlClient;

namespace DataAccess.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly DatabaseConnection _dbConnection = new DatabaseConnection();

    public List<Transaction> FindAll()
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
                    Convert.ToDouble(reader["amount"]),
                    DateOnly.FromDateTime(Convert.ToDateTime(reader["date"])),
                    Convert.ToInt32(reader["user_id"]),
                    Convert.ToInt32(reader["category_id"])
                )
            );
        }

        return allTransactions;
    }

    public Transaction? FindById(int id)
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
                Convert.ToDouble(reader["amount"]),
                DateOnly.FromDateTime(Convert.ToDateTime(reader["date"])),
                Convert.ToInt32(reader["user_id"]),
                Convert.ToInt32(reader["category_id"])
            );
        }

        return null;
    }

    public int Add(Transaction transaction)
    {
        using MySqlConnection connection = _dbConnection.GetMySqlConnection();
        connection.Open();

        string sql =
            "INSERT INTO transactions (amount, date, user_id, category_id) VALUES (@amount, @date, @user_id, @category_id)";

        using MySqlCommand command = new MySqlCommand(sql, connection);

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

        return 0; // Of gooi een exception, afhankelijk van je foutafhandeling.
    }

    public bool Edit(Transaction transaction)
    {
        try
        {
            using MySqlConnection connection = _dbConnection.GetMySqlConnection();
            connection.Open();

            string sql =
                "UPDATE transactions SET amount = @amount, date = @date, user_id = @user_id, category_id = @category_id WHERE id = @id";

            using MySqlCommand command = new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@id", transaction.Id);
            command.Parameters.AddWithValue("@amount", transaction.Amount);
            command.Parameters.AddWithValue("@date", transaction.Date.ToDateTime(TimeOnly.MinValue));
            command.Parameters.AddWithValue("@user_id", transaction.UserId);
            command.Parameters.AddWithValue("@category_id", transaction.CategoryId);

            int rowsAffected = command.ExecuteNonQuery();

            return rowsAffected > 0;
        }
        catch (MySqlException ex)
        {
            Console.WriteLine($"Error updating user: {ex.Message}");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"General Error updating user: {ex.Message}");
            return false;
        }
    }

    public bool Delete(Transaction transaction)
    {
        using MySqlConnection connection = _dbConnection.GetMySqlConnection();
        connection.Open();

        string sql = "DELETE FROM transactions WHERE id = @id";

        using MySqlCommand command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", transaction.Id);

        int rowsAffected = command.ExecuteNonQuery();

        return rowsAffected > 0;
    }

    public List<Tag> FindTransactionTags(int id)
    {
        List<Tag> transactionTags = new List<Tag>();
        using MySqlConnection connection = _dbConnection.GetMySqlConnection();
        connection.Open();

        string sql = "SELECT t.* FROM tag t JOIN transaction_tag tt ON t.id = tt.tag_id WHERE tt.transaction_id = @id";

        using MySqlCommand command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);
        using MySqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            transactionTags.Add(
                new Tag(
                    Convert.ToInt32(reader["id"]),
                    reader["name"].ToString(),
                    reader["color_hex_code"].ToString(),
                    Convert.ToInt32(reader["user_id"])
                )
            );
        }

        return transactionTags;
    }

    public bool AssignTagToTransaction(int transactionId, int tagId)
    {
        using MySqlConnection connection = _dbConnection.GetMySqlConnection();
        connection.Open();

        string sql = "INSERT INTO transaction_tag (transaction_id, tag_id) VALUES (@transactionId, @tagId)";

        using MySqlCommand command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@transactionId", transactionId);
        command.Parameters.AddWithValue("@tagId", tagId);

        int rowsAffected = command.ExecuteNonQuery();

        return rowsAffected > 0;
    }

    public bool DeleteTagFromTransaction(int transactionId, int tagId)
    {
        using MySqlConnection connection = _dbConnection.GetMySqlConnection();
        connection.Open();

        string sql = "DELETE FROM transaction_tag WHERE transaction_id = @transactionId AND tag_id = @tagId";

        using MySqlCommand command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@transactionId", transactionId);
        command.Parameters.AddWithValue("@tagId", tagId);

        int rowsAffected = command.ExecuteNonQuery();

        return rowsAffected > 0;
    }
}