using Npgsql;

namespace SQL_Proba
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            PersonRepository person = new PersonRepository("Host=192.168.0.138;Port=5432;Username=myuser;Password=mypassword;Database=mydb");
            await person.DeleteAllPersonAsync();
            await person.CreateAsync("Alice", 30);
            List<Person> persons = await person.GetAllAsync();
            foreach (var item in persons)
            {
                Console.WriteLine($"Id = {item.Id}\t Name = {item.Name}\t Age = {item.Age}");
            }
        }
    }
}
