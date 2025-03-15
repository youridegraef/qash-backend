using Application.Domain;
using Application.Interfaces;
using Microsoft.Data.Sqlite;

namespace DataAccess.Repositories;

public class CategoryRepository(DatabaseConnection _dbConnection) : ICategoryRepository
{
    public List<Category> FindAll()
    {
        List<Category> allCategories = new List<Category>();
        using SqliteConnection connection = _dbConnection.GetConnection();
        connection.Open();

        string sql = "SELECT * FROM category";

        using SqliteCommand command = new SqliteCommand(sql, connection);
        using SqliteDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            allCategories.Add(
                new Category(
                    (int)reader["id"],
                    (string)reader["name"],
                    (int)reader["user_id"]
                )
            );
        }

        return allCategories;
    }

    public Category? FindById(int id)
    {
        using SqliteConnection connection = _dbConnection.GetConnection();
        connection.Open();

        string sql = "SELECT * FROM category WHERE id = @id";

        using SqliteCommand command = new SqliteCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);

        using SqliteDataReader reader = command.ExecuteReader();

        if (reader.Read())
        {
            return new Category(
                (int)reader["id"],
                (string)reader["name"],
                (int)reader["user_id"]
            );
        }

        return null;
    }

    public bool Add(Category category)
    {
        using SqliteConnection connection = _dbConnection.GetConnection();
        connection.Open();

        string sql = "INSERT INTO category (name, user_id) VALUES (@name, @user_id)";

        using SqliteCommand command = new SqliteCommand(sql, connection);
        command.Parameters.AddWithValue("@name", category.Name);
        command.Parameters.AddWithValue("@user_id", category.UserId);

        int rowsAffected = command.ExecuteNonQuery();

        return rowsAffected > 0;
    }

    public bool Edit(Category category)
    {
        using SqliteConnection connection = _dbConnection.GetConnection();
        connection.Open();

        string sql = "UPDATE category SET name = @name, user_id = @user_id WHERE id = @id";

        using SqliteCommand command = new SqliteCommand(sql, connection);
        command.Parameters.AddWithValue("@id", category.Id);
        command.Parameters.AddWithValue("@name", category.Name);
        command.Parameters.AddWithValue("@user_id", category.UserId);
        
        int rowsAffected = command.ExecuteNonQuery();

        return rowsAffected > 0;
    }

    public bool Delete(Category category)
    {
        using SqliteConnection connection = _dbConnection.GetConnection();
        connection.Open();

        string sql = "DELETE FROM category WHERE id = @id";
        
        using SqliteCommand command = new SqliteCommand(sql, connection);
        
        command.Parameters.AddWithValue("@id", category.Id);
        
        int rowsAffected = command.ExecuteNonQuery();

        return rowsAffected > 0;
    }
}