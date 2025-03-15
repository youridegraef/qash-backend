using Application.Domain;
using Application.Interfaces;
using Microsoft.Data.Sqlite;

namespace DataAccess.Repositories;

public class TagRepository(DatabaseConnection _dbConnection) : ITagRepository
{
    public List<Tag> FindAll()
    {
        List<Tag> allTags = new List<Tag>();
        using SqliteConnection connection = _dbConnection.GetConnection();
        connection.Open();

        string sql = "SELECT * FROM tag";

        using SqliteCommand command = new SqliteCommand(sql, connection);
        using SqliteDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            allTags.Add(
                new Tag(
                    (int)reader["id"],
                    (string)reader["name"],
                    (string)reader["color_hex_code"],
                    (int)reader["user_id"]
                )
            );
        }

        return allTags;
    }

    public Tag? FindById(int id)
    {
        using SqliteConnection connection = _dbConnection.GetConnection();
        connection.Open();

        string sql = "SELECT * FROM tag WHERE id = @id";

        using SqliteCommand command = new SqliteCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);

        using SqliteDataReader reader = command.ExecuteReader();

        if (reader.Read())
        {
            return new Tag(
                (int)reader["id"],
                (string)reader["name"],
                (string)reader["color_hex_code"],
                (int)reader["user_id"]
            );
        }

        return null;
    }

    public bool Add(Tag tag)
    {
        using SqliteConnection connection = _dbConnection.GetConnection();
        connection.Open();

        string sql = "INSERT INTO tag (name, color_hex_code, user_id) VALUES (@name, @color_hex_code, @user_id)";

        using SqliteCommand command = new SqliteCommand(sql, connection);
        
        command.Parameters.AddWithValue("@name", tag.Name);
        command.Parameters.AddWithValue("@color_hex_code", tag.ColorHexCode);
        command.Parameters.AddWithValue("@user_id", tag.UserId);

        int rowsAffected = command.ExecuteNonQuery();

        return rowsAffected > 0;
    }

    public bool Edit(Tag tag)
    {
        using SqliteConnection connection = _dbConnection.GetConnection();
        connection.Open();

        string sql = "UPDATE tag SET name = @name, color_hex_code = @color_hex_code, user_id = @user_id WHERE id= @id";

        SqliteCommand command = new SqliteCommand(sql, connection);

        command.Parameters.AddWithValue("@id", tag.Id);
        command.Parameters.AddWithValue("@name", tag.Name);
        command.Parameters.AddWithValue("@color_hex_code", tag.ColorHexCode);
        command.Parameters.AddWithValue("@user_id", tag.UserId);

        int rowsAffected = command.ExecuteNonQuery();

        return rowsAffected > 0;
    }

    public bool Delete(Tag tag)
    {
        using SqliteConnection connection = _dbConnection.GetConnection();
        connection.Open();

        string sql = "DELETE FROM tag WHERE id = @id";

        SqliteCommand command = new SqliteCommand(sql, connection);

        command.Parameters.AddWithValue("@id", tag.Id);

        int rowsAffected = command.ExecuteNonQuery();

        return rowsAffected > 0;
    }
    
    //TODO: Add TagTransactions method
}