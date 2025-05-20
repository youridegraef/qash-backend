using Application.Domain;
using Application.Exceptions;
using Application.Interfaces;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace DataAccess.Repositories;

public class CategoryRepository(string connectionString, ILogger<CategoryRepository> logger)
    : ICategoryRepository
{
    public Category FindById(int id)
    {
        try
        {
            using MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            string sql = "SELECT id, name, user_id FROM category WHERE id = @id";

            using MySqlCommand command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);

            using MySqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                return new Category(
                    reader.GetInt32(reader.GetOrdinal("id")),
                    reader.GetString(reader.GetOrdinal("name")),
                    reader.GetInt32(reader.GetOrdinal("user_id")),
                    reader.GetString(reader.GetOrdinal("color_hex_code"))
                );
            }

            throw new CategoryNotFoundException($"Category with ID {id} was not found.");
        }
        catch (CategoryNotFoundException ex)
        {
            logger.LogError(ex, "Category with ID {CategoryId} was not found.", id);
            throw;
        }
        catch (MySqlException ex)
        {
            logger.LogError(ex, "Error retrieving category with ID {CategoryId} from the database.", id);
            throw new DatabaseException($"Error retrieving category with ID {id} from the database.", ex);
        }
    }

    public List<Category> FindByUserId(int userId)
    {
        try
        {
            List<Category> categories = new List<Category>();

            using MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            string sql =
                "SELECT id, name, user_id, color_hex_code FROM tag WHERE user_id = @user_id";

            using MySqlCommand command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@user_id", userId);

            using MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                categories.Add(
                    new Category(
                        reader.GetInt32(reader.GetOrdinal("id")),
                        reader.GetString(reader.GetOrdinal("name")),
                        reader.GetInt32(reader.GetOrdinal("user_id")),
                        reader.GetString(reader.GetOrdinal("color_hex_code"))
                    ));
            }

            if (categories.Count == 0)
            {
                throw new CategoryNotFoundException($"Category with UserID {userId} was not found.");
            }

            return categories;
        }
        catch (CategoryNotFoundException ex)
        {
            logger.LogError(ex, $"Category with user ID {userId} was not found.");
            throw;
        }
        catch (MySqlException ex)
        {
            logger.LogError(ex, $"Error retrieving category with user ID {userId} from the database.");
            throw new DatabaseException($"Error retrieving category with user ID {userId} from the database.", ex);
        }
    }

    public List<Category> FindByUserIdPaged(int userId, int page, int pageSize)
    {
        try
        {
            List<Category> categories = new List<Category>();

            using MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            int offset = (page - 1) * pageSize;
            string sql =
                "SELECT id, name, user_id, color_hex_code FROM tag WHERE user_id = @user_id LIMIT @limit OFFSET @offset";

            using MySqlCommand command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@user_id", userId);
            command.Parameters.AddWithValue("@offset", offset);

            using MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                categories.Add(
                    new Category(
                        reader.GetInt32(reader.GetOrdinal("id")),
                        reader.GetString(reader.GetOrdinal("name")),
                        reader.GetInt32(reader.GetOrdinal("user_id")),
                        reader.GetString(reader.GetOrdinal("color_hex_code"))
                    ));
            }

            if (categories.Count == 0)
            {
                throw new CategoryNotFoundException($"Category with UserID {userId} was not found.");
            }

            return categories;
        }
        catch (CategoryNotFoundException ex)
        {
            logger.LogError(ex, $"Category with user ID {userId} was not found.");
            throw;
        }
        catch (MySqlException ex)
        {
            logger.LogError(ex, $"Error retrieving category with user ID {userId} from the database.");
            throw new DatabaseException($"Error retrieving category with user ID {userId} from the database.", ex);
        }
    }

    public int Add(Category category)
    {
        try
        {
            using MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            string sql =
                "INSERT INTO category (name, user_id, color_hex_code) VALUES (@name, @user_id, @color_hex_code)";

            using MySqlCommand command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@name", category.Name);
            command.Parameters.AddWithValue("@user_id", category.UserId);
            command.Parameters.AddWithValue("@color_hex_code", category.ColorHexCode);


            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                string selectIdSql = "SELECT LAST_INSERT_ID()";
                using MySqlCommand selectIdCommand = new MySqlCommand(selectIdSql, connection);
                int newId = Convert.ToInt32(selectIdCommand.ExecuteScalar());
                return newId;
            }

            throw new DatabaseException("Failed to add the category to the database. No rows were affected.");
        }
        catch (DatabaseException ex)
        {
            logger.LogError(ex, "Failed to add the category to the database. No rows were affected.");
            throw;
        }
        catch (MySqlException ex)
        {
            logger.LogError(ex, "Error adding a new category to the database.");
            throw new DatabaseException("Error adding a new category to the database.", ex);
        }
    }

    public bool Edit(Category category)
    {
        try
        {
            using MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            string sql =
                "UPDATE category SET name = @name, user_id = @user_id, color_hex_code = @color_hex_code WHERE id = @id";

            using MySqlCommand command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", category.Id);
            command.Parameters.AddWithValue("@name", category.Name);
            command.Parameters.AddWithValue("@user_id", category.UserId);
            command.Parameters.AddWithValue("@color_hex_code", category.ColorHexCode);

            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                return true;
            }

            throw new CategoryNotFoundException($"Category with ID {category.Id} was not found for update.");
        }
        catch (CategoryNotFoundException ex)
        {
            logger.LogError(ex, "Category with ID {CategoryId} was not found for update.", category.Id);
            throw;
        }
        catch (MySqlException ex)
        {
            logger.LogError(ex, "Error updating category with ID {CategoryId} in the database.", category.Id);
            throw new DatabaseException($"Error updating category with ID {category.Id} in the database.", ex);
        }
    }

    public bool Delete(Category category)
    {
        try
        {
            using MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            string sql = "DELETE FROM category WHERE id = @id";

            using MySqlCommand command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", category.Id);

            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                return true;
            }

            throw new CategoryNotFoundException($"Category with ID {category.Id} was not found for deletion.");
        }
        catch (CategoryNotFoundException ex)
        {
            logger.LogError(ex, "Category with ID {CategoryId} was not found for deletion.", category.Id);
            throw;
        }
        catch (MySqlException ex)
        {
            logger.LogError(ex, "Error deleting category with ID {CategoryId} from the database.", category.Id);
            throw new DatabaseException($"Error deleting category with ID {category.Id} from the database.", ex);
        }
    }
}