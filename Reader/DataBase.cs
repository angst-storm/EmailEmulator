using Npgsql;

namespace Reader;

public class DataBase
{
    private readonly NpgsqlConnection connection;

    public DataBase(string connString)
    {
        connection = new NpgsqlConnection(connString);
        connection.Open();
        InitializeTables();
    }

    public void AddUser(string username, int[] themes)
    {
        new NpgsqlCommand($"INSERT INTO users (" +
                          $"username," +
                          $"theme1," +
                          $"theme2," +
                          $"theme3) VALUES (" +
                          $"'{username}'," +
                          $"{themes[0]}," +
                          $"{themes[1]}," +
                          $"{themes[2]})", connection).ExecuteNonQuery();
    }

    public IEnumerable<(string, int[])> GetUsers()
    {
        var reader = new NpgsqlCommand("SELECT * FROM users", connection).ExecuteReader();
        while (reader.Read())
            yield return (
                (string)reader.GetValue(1), new[]
                {
                    (int)reader.GetValue(2),
                    (int)reader.GetValue(3),
                    (int)reader.GetValue(4)
                }
            );
        reader.Close();
    }

    private void InitializeTables()
    {
        new NpgsqlCommand("DROP TABLE IF EXISTS users", connection).ExecuteNonQuery();
        new NpgsqlCommand(
                "CREATE TABLE users (" +
                "id serial primary key," +
                " username VARCHAR(50)," +
                " theme1 INTEGER," +
                " theme2 INTEGER," +
                " theme3 INTEGER)",
                connection)
            .ExecuteNonQuery();
    }
}