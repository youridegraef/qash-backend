using Application.Domain;
using Application.Exceptions;
using Application.Interfaces;
using MySql.Data.MySqlClient;

namespace DataAccess.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly DatabaseConnection _dbConnection = new DatabaseConnection();

    public List<Category> FindAll()
    {
        try
        {
            List<Category> allCategories = new List<Category>();
            using MySqlConnection connection = _dbConnection.GetMySqlConnection();
            connection.Open();

            string sql = "SELECT * FROM category";

            using MySqlCommand command = new MySqlCommand(sql, connection);
            using MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                allCategories.Add(
                    new Category(
                        Convert.ToInt32(reader["id"]),
                        reader["name"].ToString(),
                        Convert.ToInt32(reader["user_id"])
                    )
                );
            }

            return allCategories;
        }
        catch (MySqlException ex)
        {
            throw new DatabaseException("Error retrieving all categories from the database.", ex);
        }
    }

    public Category? FindById(int id)
    {
        try
        {
            using MySqlConnection connection = _dbConnection.GetMySqlConnection();
            connection.Open();

            string sql = "SELECT * FROM category WHERE id = @id";

            using MySqlCommand command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);

            using MySqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                return new Category(
                    Convert.ToInt32(reader["id"]),
                    reader["name"].ToString(),
                    Convert.ToInt32(reader["user_id"])
                );
            }

            throw new CategoryNotFoundException($"Category with ID {id} was not found.");
        }
        catch (MySqlException ex)
        {
            throw new DatabaseException($"Error retrieving category with ID {id} from the database.", ex);
        }
    }

    public int Add(Category category)
    {
        try
        {
            using MySqlConnection connection = _dbConnection.GetMySqlConnection();
            connection.Open();

            string sql = "INSERT INTO category (name, user_id) VALUES (@name, @user_id)";

            using MySqlCommand command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@name", category.Name);
            command.Parameters.AddWithValue("@user_id", category.UserId);

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
        catch (MySqlException ex)
        {
            throw new DatabaseException("Error adding a new category to the database.", ex);
        }
    }

    public bool Edit(Category category)
    {
        try
        {
            using MySqlConnection connection = _dbConnection.GetMySqlConnection();
            connection.Open();

            string sql = "UPDATE category SET name = @name, user_id = @user_id WHERE id = @id";

            using MySqlCommand command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", category.Id);
            command.Parameters.AddWithValue("@name", category.Name);
            command.Parameters.AddWithValue("@user_id", category.UserId);

            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                return true;
            }

            throw new CategoryNotFoundException($"Category with ID {category.Id} was not found for update.");
        }
        catch (MySqlException ex)
        {
            throw new DatabaseException($"Error updating category with ID {category.Id} in the database.", ex);
        }
    }

    public bool Delete(Category category)
    {
        try
        {
            using MySqlConnection connection = _dbConnection.GetMySqlConnection();
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
        catch (MySqlException ex)
        {
            throw new DatabaseException($"Error deleting category with ID {category.Id} from the database.", ex);
        }
    }
}