using Xunit;
using System;
using System.Linq;
using System.Reflection.Metadata;

namespace SQL_Proba.Tests
{
    public class PersonRepositoryTests
    {
        private readonly PersonRepository _repository;
        private readonly string _connectionString;

        public PersonRepositoryTests()
        {
            // Настройте свою строку подключения к тестовой БД
            _connectionString = "Host=192.168.0.138;Port=5432;Username=myuser;Password=mypassword;Database=mydb";
            _repository = new PersonRepository(_connectionString);

        }

        private async Task ClearTable()
        {
            try
            {
                await _repository.DeleteAllPersonAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Could not clear table: {ex.Message}");
            }
        }

        [Fact]
        public async Task CreatePerson_ShouldCreateAndRetrievePerson()
        {
            // Arrange
            await ClearTable();
            string name = "TestUser";
            int age = 25;

            // Act
            await _repository.CreateAsync(name, age);
            var persons = await _repository.GetAllAsync();

            // Assert
            Assert.Single(persons);
            var person = persons.First();
            Assert.Equal(name, person.Name);
            Assert.Equal(age, person.Age);
        }

        [Theory]
        [InlineData("John", 30)]
        [InlineData("Jane", 25)]
        [InlineData("Bob", 40)]
        [InlineData("Alice", 25)]
        [InlineData("Eve", 3)]
        public async Task CreatePerson_Theory(string name, int age)
        {
            // Arrange
            await ClearTable();
            // Act
            await _repository.CreateAsync(name, age);
            var persons = await _repository.GetAllAsync();
            // Assert
            //Assert.Contains(persons, u => u.Name == name && u.Age == age);
            var person = persons.FirstOrDefault(persons => persons.Name == name && persons.Age == age);
            Assert.NotNull(person);
            Assert.Equal(name, person.Name);
            Assert.Equal(age, person.Age);
        }

        [Fact]
        public async Task GetAll_WhenNoUsers_ShouldReturnEmptyList()
        {
            // Arrange
            await ClearTable();

            // Act
            var persons = await _repository.GetAllAsync();

            // Assert
            Assert.Empty(persons);
        }

        [Theory]
        [InlineData("John", 30, "John_Updated", 31)]
        [InlineData("Jane", 25, "Jane_Modified", 26)]
        public async Task UpdatePerson_ExistingId_ShouldUpdatePerson(
                    string originalName,
                    int originalAge,
                    string updatedName,
                    int updatedAge)
        {
            // Arrange
            await ClearTable();// 1. Очищаем
            await _repository.CreateAsync(originalName, originalAge);// 2. Создаем пользователя
            var personId = (await _repository.GetAllAsync()).First().Id;

            // Act
            await _repository.UpdateAsync(personId, updatedName, updatedAge);// 5. Обновляем
            var updatedPersons = (await _repository.GetAllAsync()).First();// 6. Проверяем результат

            // Assert
            Assert.Equal(updatedName, updatedPersons.Name);
            Assert.Equal(updatedAge, updatedPersons.Age);
            Assert.Equal(personId, updatedPersons.Id);
        }

        [Theory]
        [InlineData(new string[] { "All", "Nik", "Victoria", "Gerda", "Francheska" }, new int[] { 1250, 26, 29, 20, 23 })]
        [InlineData(new string[] { "Bill", "Anna", "Victoria", "Gerda", "Francheska" }, new int[] { 45, 7, 29, 20, 23 })]
        [InlineData(new string[] { "Vladimir", "Nika" }, new int[] { 40, 33 })]
        [InlineData(new string[] { "Vladimir", "Nika", "Max" }, new int[] { 40, 33, 1 })]
        public async Task UpdatePerson_ExistingId_ShouldUpdatePerson2(string[] Name, int[] age)
        {
            await ClearTable();// 1. Очищаем
            for (int i = 0; i < Name.Length; i++)// 2. Создаем пользователей
            {
                await _repository.CreateAsync(Name[i], age[i]);
            }
            var allPersons = await _repository.GetAllAsync();// Получаем всех пользователей

            //int[] personIds = await _repository.GetAllAsync().Select(p => p.Id).ToArray();// 3. Получаем все ID
            int[] personIds = allPersons.Select(p => p.Id).ToArray();
            for (int i = 0; i < Name.Length; i++)// 4. Обновляем всех пользователей
            {
                await _repository.UpdateAsync(personIds[i], Name[i] + "_Updated", age[i] + 1);
            }

            var updatedPersons = await _repository.GetAllAsync();// 5. Проверяем результат
            for (int i = 0; i < Name.Length; i++)
            {
                var person = updatedPersons.Single(p => p.Id == personIds[i]);
                // Assert
                Assert.Equal(Name[i] + "_Updated", person.Name);
                Assert.Equal(age[i] + 1, person.Age);
                Assert.Equal(personIds[i], person.Id);
            }
        }
    }
}
