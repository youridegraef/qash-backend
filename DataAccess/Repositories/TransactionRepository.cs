using Application.Domain;
using Application.Interfaces;
using Microsoft.Data.Sqlite;

namespace DataAccess.Repositories;

public class TransactionRepository(DatabaseConnection _dbConnection) : ITransactionRepository
{
    public List<Transaction> FindAll()
    {
        List<Transaction> allTransactions = new List<Transaction>();
        using SqliteConnection connection = _dbConnection.GetConnection();
        connection.Open();

        string sql = "SELECT * FROM transactions";

        using SqliteCommand command = new SqliteCommand(sql, connection);
        using SqliteDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            allTransactions.Add(
                new Transaction(
                    (int)reader["id"],
                    (double)reader["amount"],
                    DateOnly.FromDateTime((DateTime)reader["date"]),
                    (int)reader["user_id"]
                )
            );
        }

        return allTransactions;
    }

    public Transaction? FindById(int id)
    {
        using SqliteConnection connection = _dbConnection.GetConnection();
        connection.Open();

        string sql = "SELECT * FROM transactions WHERE id = @id";

        using SqliteCommand command = new SqliteCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);

        using SqliteDataReader reader = command.ExecuteReader();

        if (reader.Read())
        {
            return new Transaction(
                (int)reader["id"],
                (double)reader["amount"],
                DateOnly.FromDateTime((DateTime)reader["date"]),
                (int)reader["user_id"]
            );
        }

        return null;
    }

    public bool Add(Transaction transaction)
    {
        using SqliteConnection connection = _dbConnection.GetConnection();
        connection.Open();

        string sql = "INSERT INTO transactions (amount, date, user_id) VALUES (@amount, @date, @user_id)";

        using SqliteCommand command = new SqliteCommand(sql, connection);

        command.Parameters.AddWithValue("@amount", transaction.Amount);
        command.Parameters.AddWithValue("@date", transaction.Date);
        command.Parameters.AddWithValue("@user_id", transaction.UserId);

        int rowsAffected = command.ExecuteNonQuery();

        return rowsAffected > 0;
    }

    public bool Edit(Transaction transaction)
    {
        using SqliteConnection connection = _dbConnection.GetConnection();
        connection.Open();

        string sql =
            "UPDATE transactions SET amount = @amount, date = @date, user_id = @user_id WHERE id = @id";

        using SqliteCommand command = new SqliteCommand(sql, connection);

        command.Parameters.AddWithValue("@id", transaction.Id);
        command.Parameters.AddWithValue("@amount", transaction.Amount);
        command.Parameters.AddWithValue("@date", transaction.Date);
        command.Parameters.AddWithValue("@user_id", transaction.UserId);

        int rowsAffected = command.ExecuteNonQuery();

        return rowsAffected > 0;
    }

    public bool Delete(Transaction transaction)
    {
        using SqliteConnection connection = _dbConnection.GetConnection();
        connection.Open();

        string sql = "DELETE FROM transactions WHERE id = @id";

        using SqliteCommand command = new SqliteCommand(sql, connection);
        command.Parameters.AddWithValue("@id", transaction.Id);

        int rowsAffected = command.ExecuteNonQuery();

        return rowsAffected > 0;
    }

    public List<Tag> FindTransactionTags(int id)
    {
        List<Tag> transactionTags = new List<Tag>();
        SqliteConnection connection = _dbConnection.GetConnection();
        connection.Open();

        string sql = "SELECT t.* FROM tag t JOIN transaction_tag tt ON t.id = tt.tag_id WHERE tt.transaction_id = @id";

        using SqliteCommand command = new SqliteCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);
        using SqliteDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            transactionTags.Add(
                new Tag(
                    (int)reader["id"],
                    (string)reader["name"],
                    (string)reader["color_hex_code"],
                    (int)reader["user_id"]
                )
            );
        }

        return transactionTags;
    }
}