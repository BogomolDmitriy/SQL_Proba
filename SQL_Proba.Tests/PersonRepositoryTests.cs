using Xunit;
using System;
using System.Linq;
using System.Reflection.Metadata;

namespace SQL_Proba.Tests
{
    public class PersonRepositoryTests : IDisposable
    {
        private readonly PersonRepository _repository;
        private readonly string _connectionString;

        private PersonRepositoryTests()
        {
            // Настройте свою строку подключения к тестовой БД
            _connectionString = "Host=192.168.0.138;Port=5432;Username=myuser;Password=mypassword;Database=mydb";
            _repository = new PersonRepository(_connectionString);

            // Очистка таблицы перед каждым тестом
            _repository.DeleteAllPerson();
        }


        public void Dispose()
        {
            _repository?.Dispose();
        }

        [Theory]
        [InlineData("John", 30)]
        [InlineData("Jane", 25)]
        [InlineData("Bob", 40)]
        [InlineData("Alice", 25)]
        [InlineData("Eve", 3)]
        public void CreatePerson(string name, int age)
        {
            _repository.Create(name, age);
            var persons = _repository.GetAll();
            Assert.Contains(persons, u => u.Name == name && u.Age == age);
        }
    }
}
