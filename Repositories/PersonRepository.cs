using System;
using Npgsql;
using System.Collections.Generic;
using System.Threading.Tasks;

public class Person
{
    public int? Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
}

public interface IPersonRepository
{
    Task<List<Person>> GetAllAsync();
}

public class PersonRepository : IPersonRepository
{
    private string _connectionString;

    public PersonRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<List<Person>> GetAllAsync()
    {
        var persons = new List<Person>();

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using (var command = new NpgsqlCommand("SELECT * FROM person", connection))
        await using (var reader = await command.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                var person = new Person
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Email = reader.GetString(2)
                };

                persons.Add(person);
            }
        }

        return persons;
    }
}

