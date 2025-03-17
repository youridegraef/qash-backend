using MySql.Data.MySqlClient;

namespace DataAccess;

public class DatabaseConnection
{
    private readonly string _connectionString = "Server=localhost;Database=expense_tracker;Uid=root;Pwd=Rafensoes2019!;";

    public MySqlConnection GetMySqlConnection()
    {
        return new MySqlConnection(_connectionString);
    }
}