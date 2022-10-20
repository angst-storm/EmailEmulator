using Npgsql;

namespace Mailer;

public class DataBase
{
    private readonly NpgsqlConnection connection;

    public DataBase(string connString)
    {
        connection = new NpgsqlConnection(connString);
        connection.Open();
        InitializeTables();
    }

    public void AddUser(string username)
    {
        new NpgsqlCommand($"INSERT INTO users (username) VALUES ('{username}')", connection).ExecuteNonQuery();
    }

    public void AddMessage(string text)
    {
        new NpgsqlCommand($"INSERT INTO messages (text) VALUES ('{text}')", connection).ExecuteNonQuery();
    }

    public IEnumerable<string> GetUsers()
    {
        var reader = new NpgsqlCommand("SELECT * FROM users", connection).ExecuteReader();
        while (reader.Read())
            yield return (string)reader.GetValue(1);
        reader.Close();
    }

    private void InitializeTables()
    {
        new NpgsqlCommand("DROP TABLE IF EXISTS users", connection).ExecuteNonQuery();
        new NpgsqlCommand("CREATE TABLE users (id serial primary key, username VARCHAR(50))", connection)
            .ExecuteNonQuery();

        new NpgsqlCommand("DROP TABLE IF EXISTS messages", connection).ExecuteNonQuery();
        new NpgsqlCommand("CREATE TABLE messages (id serial primary key, text VARCHAR(100))", connection)
            .ExecuteNonQuery();
    }
}