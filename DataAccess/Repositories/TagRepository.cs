using Application.Domain;
using Application.Interfaces;
using MySql.Data.MySqlClient;

namespace DataAccess.Repositories;

public class TagRepository(DatabaseConnection _dbConnection) : ITagRepository
{
    public List<Tag> FindAll()
    {
        List<Tag> allTags = new List<Tag>();
        using MySqlConnection connection = _dbConnection.GetMySqlConnection();
        connection.Open();

        string sql = "SELECT * FROM tag";

        using MySqlCommand command = new MySqlCommand(sql, connection);
        using MySqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            allTags.Add(
                new Tag(
                    Convert.ToInt32(reader["id"]),
                    reader["name"].ToString(),
                    reader["color_hex_code"].ToString(),
                    Convert.ToInt32(reader["user_id"])
                )
            );
        }

        return allTags;
    }

    public Tag? FindById(int id)
    {
        using MySqlConnection connection = _dbConnection.GetMySqlConnection();
        connection.Open();

        string sql = "SELECT * FROM tag WHERE id = @id";

        using MySqlCommand command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);

        using MySqlDataReader reader = command.ExecuteReader();

        if (reader.Read())
        {
            return new Tag(
                Convert.ToInt32(reader["id"]),
                reader["name"].ToString(),
                reader["color_hex_code"].ToString(),
                Convert.ToInt32(reader["user_id"])
            );
        }

        return null;
    }

    public bool Add(Tag tag)
    {
        using MySqlConnection connection = _dbConnection.GetMySqlConnection();
        connection.Open();

        string sql = "INSERT INTO tag (name, color_hex_code, user_id) VALUES (@name, @color_hex_code, @user_id)";

        using MySqlCommand command = new MySqlCommand(sql, connection);

        command.Parameters.AddWithValue("@name", tag.Name);
        command.Parameters.AddWithValue("@color_hex_code", tag.ColorHexCode);
        command.Parameters.AddWithValue("@user_id", tag.UserId);

        int rowsAffected = command.ExecuteNonQuery();

        return rowsAffected > 0;
    }

    public bool Edit(Tag tag)
    {
        using MySqlConnection connection = _dbConnection.GetMySqlConnection();
        connection.Open();

        string sql = "UPDATE tag SET name = @name, color_hex_code = @color_hex_code, user_id = @user_id WHERE id = @id";

        using MySqlCommand command = new MySqlCommand(sql, connection);

        command.Parameters.AddWithValue("@id", tag.Id);
        command.Parameters.AddWithValue("@name", tag.Name);
        command.Parameters.AddWithValue("@color_hex_code", tag.ColorHexCode);
        command.Parameters.AddWithValue("@user_id", tag.UserId);

        int rowsAffected = command.ExecuteNonQuery();

        return rowsAffected > 0;
    }

    public bool Delete(Tag tag)
    {
        using MySqlConnection connection = _dbConnection.GetMySqlConnection();
        connection.Open();

        string sql = "DELETE FROM tag WHERE id = @id";

        using MySqlCommand command = new MySqlCommand(sql, connection);

        command.Parameters.AddWithValue("@id", tag.Id);

        int rowsAffected = command.ExecuteNonQuery();

        return rowsAffected > 0;
    }
}