using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQL_Proba
{
    public class PersonRepository
    {
        //public string _ConnectionString { get; set; }

        private NpgsqlConnection _Connection;

        public PersonRepository(string connectionString)
        {
            //_ConnectionString = connectionString;
            _Connection = new NpgsqlConnection(connectionString);
        }

        public void Create(string name, int age)
        {
            _Connection.Open();
            string sql = "INSERT INTO users (name, age) VALUES (@name, @age);";
            using var cmd = new NpgsqlCommand(sql, _Connection);
            cmd.Parameters.AddWithValue("name", name);
            cmd.Parameters.AddWithValue("age", age);

            cmd.ExecuteNonQuery();
            _Connection.Close();
            Console.WriteLine("✔ User added!");
        }

        public List<Person> GetAll()
        {
            string sql = "SELECT id, name, age FROM users";

            List<Person> persons = new();
            _Connection.Open();

            using var cmd = new NpgsqlCommand(sql, _Connection);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Person person = new Person
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Age = reader.GetInt32(2)
                };
                persons.Add(person);
            }

            _Connection.Close();

            return persons;
        }

        public void Update(int id, string name, int age)
        {
            _Connection.Open();
            string sql = "UPDATE users SET name = @name, age = @age WHERE id = @id;";
            using var cmd = new NpgsqlCommand(sql, _Connection);
            cmd.Parameters.AddWithValue("name", name);
            cmd.Parameters.AddWithValue("age", age);
            cmd.Parameters.AddWithValue("id", id);
            int caunt = cmd.ExecuteNonQuery();
            _Connection.Close();
            Console.WriteLine(caunt > 0 ? "✔ User updated!" : "User not found!");
        }

        public void Delete(int id)
        {
            _Connection.Open();
            string sql = "DELETE FROM users WHERE id = @id;";
            using var cmd = new NpgsqlCommand(sql, _Connection);
            cmd.Parameters.AddWithValue("id", id);
            int caunt = cmd.ExecuteNonQuery();
            _Connection.Close();
            Console.WriteLine(caunt > 0 ? "✔ User deleted!" : "User not found!");
        }
    }
}
