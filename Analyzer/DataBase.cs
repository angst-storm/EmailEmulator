using Npgsql;

namespace Analyzer;

public class DataBase
{
    private readonly NpgsqlConnection connection;

    public DataBase(string connString)
    {
        connection = new NpgsqlConnection(connString);
        connection.Open();
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
}