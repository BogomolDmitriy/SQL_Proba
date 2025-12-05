using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;

namespace SQL_Proba
{
    public class PersonRepository : IDisposable
    {
        //public string _ConnectionString { get; set; }

        private readonly NpgsqlConnection _Connection;
        private bool _disposed = false;

        public PersonRepository(string connectionString)
        {
            //_ConnectionString = connectionString;
            _Connection = new NpgsqlConnection(connectionString);
        }

        public void Create(string name, int age)
        {
            try
            {
                if (_Connection.State != ConnectionState.Open)
                {
                    _Connection.Open();
                }

                string sql = "INSERT INTO users (name, age) VALUES (@name, @age);";
                using var cmd = new NpgsqlCommand(sql, _Connection);
                cmd.Parameters.AddWithValue("name", name);
                cmd.Parameters.AddWithValue("age", age);
                cmd.ExecuteNonQuery();
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Error creating user: {ex.Message}");
                throw;
            }

            finally 
                {
                    if (_Connection.State == ConnectionState.Open)
                    {
                        _Connection.Close();
                    }
                }
        }

        public List<Person> GetAll()
        {

            try
            {
                if (_Connection.State != ConnectionState.Open)
                {
                    _Connection.Open();
                }


                string sql = "SELECT id, name, age FROM users";

                List<Person> persons = new();

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

                return persons;
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Error getting users: {ex.Message}");
                throw;
            }

            finally
            {
                if (_Connection.State == ConnectionState.Open)
                {
                    _Connection.Close();
                }
            }
        }

        public void Update(int id, string name, int age)
        {

            try
            {
                if (_Connection.State != ConnectionState.Open)
                {
                    _Connection.Open();
                }

                string sql = "UPDATE users SET name = @name, age = @age WHERE id = @id;";
                using var cmd = new NpgsqlCommand(sql, _Connection);
                cmd.Parameters.AddWithValue("name", name);
                cmd.Parameters.AddWithValue("age", age);
                cmd.Parameters.AddWithValue("id", id);
                int count = cmd.ExecuteNonQuery();
                Console.WriteLine(count > 0 ? "✔ User updated!" : "User not found!");
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Error updating user: {ex.Message}");
                throw;
            }

            finally
            {
                if (_Connection.State == ConnectionState.Open)
                {
                    _Connection.Close();
                }
            }
        }

        public void DeletePerson(int id)
        {

            try
            {
                if (_Connection.State != ConnectionState.Open)
                {
                    _Connection.Open();
                }

                string sql = "DELETE FROM users WHERE id = @id;";
                using var cmd = new NpgsqlCommand(sql, _Connection);
                cmd.Parameters.AddWithValue("id", id);
                int count = cmd.ExecuteNonQuery();
                Console.WriteLine(count > 0 ? "✔ User deleted!" : "User not found!");
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting user: {ex.Message}");
                throw;
            }

            finally
            {
                if (_Connection.State == ConnectionState.Open)
                {
                    _Connection.Close();
                }
            }
        }

        public void DeleteAllPerson()
        {
            try
            {
                if (_Connection.State != ConnectionState.Open)
                {
                    _Connection.Open();
                }
                string sql = "DELETE FROM users;";
                using var cmd = new NpgsqlCommand(sql, _Connection);
                int count = cmd.ExecuteNonQuery();
                Console.WriteLine($"✔ {count} users deleted!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting users: {ex.Message}");
                throw;
            }
            finally
            {
                if (_Connection.State == ConnectionState.Open)
                {
                    _Connection.Close();
                }
            }
        }

        public void Dispose() // ліквідація ресурсів
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _Connection?.Dispose();
                }

                _disposed = true;
            }
        }
    }
}
