using Npgsql;

namespace SQL_Proba
{
    internal class Program
    {
        static string connString =
            "Host=192.168.0.138;Port=5432;Username=myuser;Password=mypassword;Database=mydb";
        static NpgsqlConnection conn;
        delegate void SimpleDelegate();
        static SimpleDelegate del;

        static void Main(string[] args)
        {
            conn = new NpgsqlConnection(connString);
            //del = new SimpleDelegate(NewTable);
            Call();
            ReadTabl();
        }

        static void Call()
        {
            try
            {
                conn.Open();
                Console.WriteLine("CONNECTED!");

                using var cmd = new NpgsqlCommand("SELECT NOW()", conn);
                var result = cmd.ExecuteScalar();

                Console.WriteLine("Server time: " + result);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
            }
        }

        static void NewTable()
        {
            string sql = @"
            CREATE TABLE IF NOT EXISTS users (
                id SERIAL PRIMARY KEY,
                name TEXT NOT NULL,
                age INT NOT NULL
            );";
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
            Console.WriteLine("Таблица создана (или уже существовала)");
        }

        static void InsertUser(string name, int age)
        {
            string sql = "INSERT INTO users (name, age) VALUES (@name, @age);";
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("name", name);
            cmd.Parameters.AddWithValue("age", age);
            cmd.ExecuteNonQuery();
            Console.WriteLine("Пользователь добавлен");
        }

        static void ReadTabl()
        {
            string sql = "SELECT id, name, age FROM users;";
            using var cmd = new NpgsqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                string name = reader.GetString(1);
                int age = reader.GetInt32(2);
                Console.WriteLine($"ID: {id}, Name: {name}, Age: {age}");
            }
        }
    }
}
