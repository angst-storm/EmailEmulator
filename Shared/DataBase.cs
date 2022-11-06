using System.Collections.Generic;
using Npgsql;

namespace Shared
{
    public class DataBase
    {
        private readonly NpgsqlConnection connection;

        public DataBase(string connString)
        {
            connection = new NpgsqlConnection(connString);
            connection.Open();
        }

        public void AddUser(IEnumerable<int> themes)
        {
            var countScalar = new NpgsqlCommand("select count(*) from users", connection).ExecuteScalar();
            var id = int.Parse(countScalar.ToString()) + 1;
            new NpgsqlCommand($"insert into users values ({id})", connection).ExecuteNonQuery();
            foreach (var theme in themes)
                new NpgsqlCommand($"insert into users_to_themes values ({id}, {theme})", connection).ExecuteNonQuery();
        }

        public IEnumerable<IEnumerable<int>> GetUsersThemes()
        {
            var countScalar = new NpgsqlCommand("select count(*) from users", connection).ExecuteScalar();
            var count = int.Parse(countScalar.ToString());
            for (var i = 1; i <= count; i++)
            {
                var reader = new NpgsqlCommand($"select * from users_to_themes where user_id={i}", connection).ExecuteReader();
                var themes = new List<int>();
                while (reader.Read())
                    themes.Add((int)reader.GetValue(1));
                yield return themes;
                reader.Close();
            }
            
        }

        public void InitializeTables()
        {
            new NpgsqlCommand("drop table if exists users", connection).ExecuteNonQuery();
            new NpgsqlCommand("create table users (id serial primary key)", connection)
                .ExecuteNonQuery();
            new NpgsqlCommand("drop table if exists users_to_themes", connection).ExecuteNonQuery();
            new NpgsqlCommand("create table users_to_themes (user_id integer, theme_id integer)", connection)
                .ExecuteNonQuery();
        }
    }
}