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

        public PersonRepositoryTests()
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
            _repository.DeleteAllPerson();
            _repository.Create(name, age);
            var persons = _repository.GetAll();
            Assert.Contains(persons, u => u.Name == name && u.Age == age);
        }

        [Theory]
        [InlineData("John", 30, "John_Updated", 31)]
        [InlineData("Jane", 25, "Jane_Modified", 26)]
        public void UpdatePerson_ExistingId_ShouldUpdatePerson(
                    string originalName,
                    int originalAge,
                    string updatedName,
                    int updatedAge)
        {
            //// Arrange - создаем пользователя
            //_repository.DeleteAllPerson();
            //_repository.Create(originalName, originalAge);
            //var personId = _repository.GetAll().Single().Id;

            //// Act
            //_repository.Update(personId, updatedName, updatedAge);

            //// Assert
            //var person = _repository.GetAll().Single();
            //Assert.Equal(updatedName, person.Name);
            //Assert.Equal(updatedAge, person.Age);
            //Assert.Equal(personId, person.Id);

            _repository.DeleteAllPerson();// 1. Очищаем
            _repository.Create(originalName, originalAge);// 2. Создаем пользователя
            var allPersons = _repository.GetAll();// 3. Получаем всех пользователей ДО Single()
            var personId = allPersons.Single().Id;// 4. Получаем ID
            _repository.Update(personId, updatedName, updatedAge);// 5. Обновляем
            var updatedPersons = _repository.GetAll();// 6. Проверяем результат
            var person = updatedPersons.Single();
            // Assert
            Assert.Equal(updatedName, person.Name);
            Assert.Equal(updatedAge, person.Age);
            Assert.Equal(personId, person.Id);
        }

        [Theory]
        [InlineData(new string[] { "All", "Nik", "Victoria", "Gerda", "Francheska" }, new int[]{1250, 26, 29, 20, 23})]
        [InlineData(new string[] { "Bill", "Anna", "Victoria", "Gerda", "Francheska" }, new int[] { 45, 7, 29, 20, 23 })]
        [InlineData(new string[] { "Vladimir", "Nika" }, new int[] { 40, 33 })]
        [InlineData(new string[] { "Vladimir", "Nika", "Max" }, new int[] { 40, 33, 1 })]
        public void UpdatePerson_ExistingId_ShouldUpdatePerson2(string[] Name, int[] age)
        {
            _repository.DeleteAllPerson();// 1. Очищаем
            for (int i = 0; i < Name.Length; i++)// 2. Создаем пользователей
            {
                _repository.Create(Name[i], age[i]);
            }
            var allPersons = _repository.GetAll();// Получаем всех пользователей

            int[] personIds = _repository.GetAll().Select(p => p.Id).ToArray();// 3. Получаем все ID
            for (int i = 0; i < Name.Length; i++)// 4. Обновляем всех пользователей
            {
                _repository.Update(personIds[i], Name[i] + "_Updated", age[i] + 1);
            }

            var updatedPersons = _repository.GetAll();// 5. Проверяем результат
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
