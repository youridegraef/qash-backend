using Application.Domain;
using Application.Interfaces;
using MySql.Data.MySqlClient;

namespace DataAccess.Repositories;

public class CategoryRepository(DatabaseConnection _dbConnection) : ICategoryRepository
{
    public List<Category> FindAll()
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

    public Category? FindById(int id)
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

        return null;
    }

    public bool Add(Category category)
    {
        using MySqlConnection connection = _dbConnection.GetMySqlConnection();
        connection.Open();

        string sql = "INSERT INTO category (name, user_id) VALUES (@name, @user_id)";

        using MySqlCommand command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@name", category.Name);
        command.Parameters.AddWithValue("@user_id", category.UserId);

        int rowsAffected = command.ExecuteNonQuery();

        return rowsAffected > 0;
    }

    public bool Edit(Category category)
    {
        using MySqlConnection connection = _dbConnection.GetMySqlConnection();
        connection.Open();

        string sql = "UPDATE category SET name = @name, user_id = @user_id WHERE id = @id";

        using MySqlCommand command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", category.Id);
        command.Parameters.AddWithValue("@name", category.Name);
        command.Parameters.AddWithValue("@user_id", category.UserId);

        int rowsAffected = command.ExecuteNonQuery();

        return rowsAffected > 0;
    }

    public bool Delete(Category category)
    {
        using MySqlConnection connection = _dbConnection.GetMySqlConnection();
        connection.Open();

        string sql = "DELETE FROM category WHERE id = @id";

        using MySqlCommand command = new MySqlCommand(sql, connection);

        command.Parameters.AddWithValue("@id", category.Id);

        int rowsAffected = command.ExecuteNonQuery();

        return rowsAffected > 0;
    }
}