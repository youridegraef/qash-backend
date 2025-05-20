using Application.Domain;
using Application.Exceptions;
using Application.Interfaces;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace DataAccess.Repositories;

public class TagRepository(string connectionString, ILogger<TagRepository> logger) : ITagRepository
{
    public List<Tag> FindByUserIdPaged(int userId, int page, int pageSize)
    {
        try
        {
            List<Tag> tags = new List<Tag>();

            using MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            int offset = (page - 1) * pageSize;
            string sql =
                "SELECT id, name, color_hex_code, user_id FROM tag WHERE user_id = @user_id LIMIT @limit OFFSET @offset";

            using MySqlCommand command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@user_id", userId);
            command.Parameters.AddWithValue("@offset", offset);

            using MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                tags.Add(
                    new Tag(
                        reader.GetInt32(reader.GetOrdinal("id")),
                        reader.GetString(reader.GetOrdinal("name")),
                        reader.GetString(reader.GetOrdinal("color_hex_code")),
                        reader.GetInt32(reader.GetOrdinal("user_id"))
                    ));
            }

            if (tags.Count == 0)
            {
                throw new TagNotFoundException($"Tags with UserID {userId} was not found.");
            }

            return tags;
        }
        catch (TagNotFoundException ex)
        {
            logger.LogError(ex, $"Tag with user ID {userId} was not found.");
            throw;
        }
        catch (MySqlException ex)
        {
            logger.LogError(ex, $"Error retrieving tag with user ID {userId} from the database.");
            throw new DatabaseException($"Error retrieving tag with user ID {userId} from the database.", ex);
        }
    }

    public List<Tag> FindByUserId(int userId)
    {
        try
        {
            List<Tag> tags = new List<Tag>();

            using MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            string sql = "SELECT id, name, color_hex_code, user_id FROM tag WHERE user_id = @user_id";

            using MySqlCommand command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@user_id", userId);

            using MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                tags.Add(
                    new Tag(
                        reader.GetInt32(reader.GetOrdinal("id")),
                        reader.GetString(reader.GetOrdinal("name")),
                        reader.GetString(reader.GetOrdinal("color_hex_code")),
                        reader.GetInt32(reader.GetOrdinal("user_id"))
                    ));
            }

            if (tags.Count == 0)
            {
                throw new TagNotFoundException($"Tags with UserID {userId} was not found.");
            }

            return tags;
        }
        catch (TagNotFoundException ex)
        {
            logger.LogError(ex, $"Tag with user ID {userId} was not found.");
            throw;
        }
        catch (MySqlException ex)
        {
            logger.LogError(ex, $"Error retrieving tag with user ID {userId} from the database.");
            throw new DatabaseException($"Error retrieving tag with user ID {userId} from the database.", ex);
        }
    }

    public List<Tag> FindByTransactionId(int transactionId)
    {
        try
        {
            List<Tag> tags = new List<Tag>();

            using MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            string sql = @"
            SELECT t.id, t.name, t.color_hex_code, t.user_id
            FROM tag t
            INNER JOIN transaction_tag tt ON t.id = tt.tag_id
            WHERE tt.transaction_id = @transaction_id
        ";

            using MySqlCommand command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@transaction_id", transactionId);

            using MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                tags.Add(
                    new Tag(
                        reader.GetInt32(reader.GetOrdinal("id")),
                        reader.GetString(reader.GetOrdinal("name")),
                        reader.GetString(reader.GetOrdinal("color_hex_code")),
                        reader.GetInt32(reader.GetOrdinal("user_id"))
                    )
                );
            }

            if (tags.Count == 0)
            {
                logger.LogWarning("No tags found for transaction ID {TransactionId}", transactionId);
                throw new TagNotFoundException($"No tags found for transaction ID {transactionId}.");
            }

            return tags;
        }
        catch (TagNotFoundException ex)
        {
            logger.LogWarning(ex, "No tags found for transaction ID {TransactionId}", transactionId);
            throw;
        }
        catch (MySqlException ex)
        {
            logger.LogError(ex, "Database error while retrieving tags for transaction ID {TransactionId}",
                transactionId);
            throw new DatabaseException($"Error retrieving tags for transaction ID {transactionId} from the database.",
                ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while retrieving tags for transaction ID {TransactionId}",
                transactionId);
            throw;
        }
    }


    public Tag FindById(int id)
    {
        try
        {
            using MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            string sql = "SELECT id, name, color_hex_code, user_id FROM tag WHERE id = @id";

            using MySqlCommand command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);

            using MySqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                return new Tag(
                    reader.GetInt32(reader.GetOrdinal("id")),
                    reader.GetString(reader.GetOrdinal("name")),
                    reader.GetString(reader.GetOrdinal("color_hex_code")),
                    reader.GetInt32(reader.GetOrdinal("user_id"))
                );
            }

            throw new TagNotFoundException($"Tag with ID {id} was not found.");
        }
        catch (TagNotFoundException ex)
        {
            logger.LogError(ex, $"Tag with ID {id} was not found.");
            throw;
        }
        catch (MySqlException ex)
        {
            logger.LogError(ex, $"Error retrieving tag with ID {id} from the database.");
            throw new DatabaseException($"Error retrieving tag with ID {id} from the database.", ex);
        }
    }

    public int Add(Tag tag)
    {
        try
        {
            using MySqlConnection connection = new MySqlConnection(connectionString);
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
        catch (DatabaseException ex)
        {
            logger.LogError(ex, "Failed to add the tag to the database. No rows were affected.");
            throw;
        }
        catch (MySqlException ex)
        {
            logger.LogError(ex, "Error adding a new tag to the database.");
            throw new DatabaseException("Error adding a new tag to the database.", ex);
        }
    }

    public bool Edit(Tag tag)
    {
        try
        {
            using MySqlConnection connection = new MySqlConnection(connectionString);
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
        catch (TagNotFoundException ex)
        {
            logger.LogError(ex, $"Tag with ID {tag.Id} was not found for update.");
            throw;
        }
        catch (MySqlException ex)
        {
            logger.LogError(ex, $"Error updating tag with ID {tag.Id} in the database.");
            throw new DatabaseException($"Error updating tag with ID {tag.Id} in the database.", ex);
        }
    }

    public bool Delete(Tag tag)
    {
        try
        {
            using MySqlConnection connection = new MySqlConnection(connectionString);
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
        catch (TagNotFoundException ex)
        {
            logger.LogError(ex, $"Tag with ID {tag.Id} was not found for deletion.");
            throw;
        }
        catch (MySqlException ex)
        {
            logger.LogError(ex, $"Error deleting tag with ID {tag.Id} from the database.");
            throw new DatabaseException($"Error deleting tag with ID {tag.Id} from the database.", ex);
        }
    }
}