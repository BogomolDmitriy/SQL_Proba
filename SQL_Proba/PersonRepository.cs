using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace SQL_Proba
{
    public class PersonRepository
    {
        private string _ConnectionString;

        public PersonRepository(string connectionString)
        {
            _ConnectionString = connectionString;
        }

        public async Task CreateAsync(string name, int age)
        {
            try
            {
                await using var connection = new NpgsqlConnection(_ConnectionString);
                await connection.OpenAsync();

                string sql = "INSERT INTO users (name, age) VALUES (@name, @age);";
                await using var cmd = new NpgsqlCommand(sql, connection);
                cmd.Parameters.AddWithValue("name", name);
                cmd.Parameters.AddWithValue("age", age);
                await cmd.ExecuteNonQueryAsync();
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Error creating user: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Person>> GetAllAsync()
        {

            try
            {
                await using var connection = new NpgsqlConnection(_ConnectionString);
                await connection.OpenAsync();


                string sql = "SELECT id, name, age FROM users";

                List<Person> persons = new();

                await using var cmd = new NpgsqlCommand(sql, connection);
                await using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    Person person = new Person
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Age = reader.GetInt32(2)
                    };
                    persons.Add(person);
                }

                return persons;
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Error getting users: {ex.Message}");
                throw;
            }

        }

        public async Task UpdateAsync(int id, string name, int age)
        {

            try
            {
                await using var connection = new NpgsqlConnection(_ConnectionString);
                await connection.OpenAsync();

                string sql = "UPDATE users SET name = @name, age = @age WHERE id = @id;";
                await using var cmd = new NpgsqlCommand(sql, connection);
                cmd.Parameters.AddWithValue("name", name);
                cmd.Parameters.AddWithValue("age", age);
                cmd.Parameters.AddWithValue("id", id);
                int count = await cmd.ExecuteNonQueryAsync();
                Console.WriteLine(count > 0 ? "✔ User updated!" : "User not found!");
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Error updating user: {ex.Message}");
                throw;
            }
        }

        public async Task DeletePersonAsync(int id)
        {

            try
            {
                await using var connection = new NpgsqlConnection(_ConnectionString);
                await connection.OpenAsync();

                string sql = "DELETE FROM users WHERE id = @id;";
                await using var cmd = new NpgsqlCommand(sql, connection);
                cmd.Parameters.AddWithValue("id", id);
                int count = await cmd.ExecuteNonQueryAsync();
                Console.WriteLine(count > 0 ? "✔ User deleted!" : "User not found!");
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting user: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteAllPersonAsync()
        {
            try
            {
                await using var connection = new NpgsqlConnection(_ConnectionString);
                await connection.OpenAsync();

                string sql = "DELETE FROM users;";
                await using var cmd = new NpgsqlCommand(sql, connection);
                int count = await cmd.ExecuteNonQueryAsync();
                Console.WriteLine($"✔ {count} users deleted!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting users: {ex.Message}");
                throw;
            }
        }
    }
}



