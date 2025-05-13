using Application.Domain;
using Application.Exceptions;
using Application.Interfaces;
using MySql.Data.MySqlClient;

namespace DataAccess.Repositories;

public class TagRepository : ITagRepository
{
    private readonly DatabaseConnection _dbConnection = new DatabaseConnection();

    public List<Tag> FindAll()
    {
        try
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
        catch (MySqlException ex)
        {
            throw new DatabaseException("Error retrieving all tags from the database.", ex);
        }
    }

    public Tag? FindById(int id)
    {
        try
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

            throw new TagNotFoundException($"Tag with ID {id} was not found.");
        }
        catch (MySqlException ex)
        {
            throw new DatabaseException($"Error retrieving tag with ID {id} from the database.", ex);
        }
    }

    public int Add(Tag tag)
    {
        try
        {
            using MySqlConnection connection = _dbConnection.GetMySqlConnection();
            connection.Open();

            string sql = "INSERT INTO tag (name, color_hex_code, user_id) VALUES (@name, @color_hex_code, @user_id)";

            using MySqlCommand command = new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@name", tag.Name);
            command.Parameters.AddWithValue("@color_hex_code", tag.ColorHexCode);
            command.Parameters.AddWithValue("@user_id", tag.UserId);

            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                string selectIdSql = "SELECT LAST_INSERT_ID()";
                using MySqlCommand selectIdCommand = new MySqlCommand(selectIdSql, connection);
                int newId = Convert.ToInt32(selectIdCommand.ExecuteScalar());
                return newId;
            }

            throw new DatabaseException("Failed to add the tag to the database. No rows were affected.");
        }
        catch (MySqlException ex)
        {
            throw new DatabaseException("Error adding a new tag to the database.", ex);
        }
    }

    public bool Edit(Tag tag)
    {
        try
        {
            using MySqlConnection connection = _dbConnection.GetMySqlConnection();
            connection.Open();

            string sql =
                "UPDATE tag SET name = @name, color_hex_code = @color_hex_code, user_id = @user_id WHERE id = @id";

            using MySqlCommand command = new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@id", tag.Id);
            command.Parameters.AddWithValue("@name", tag.Name);
            command.Parameters.AddWithValue("@color_hex_code", tag.ColorHexCode);
            command.Parameters.AddWithValue("@user_id", tag.UserId);

            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                return true;
            }

            throw new TagNotFoundException($"Tag with ID {tag.Id} was not found for update.");
        }
        catch (MySqlException ex)
        {
            throw new DatabaseException($"Error updating tag with ID {tag.Id} in the database.", ex);
        }
    }

    public bool Delete(Tag tag)
    {
        try
        {
            using MySqlConnection connection = _dbConnection.GetMySqlConnection();
            connection.Open();

            string sql = "DELETE FROM tag WHERE id = @id";

            using MySqlCommand command = new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@id", tag.Id);

            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                return true;
            }

            throw new TagNotFoundException($"Tag with ID {tag.Id} was not found for deletion.");
        }
        catch (MySqlException ex)
        {
            throw new DatabaseException($"Error deleting tag with ID {tag.Id} from the database.", ex);
        }
    }
}