using System;
using Npgsql;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

public class Person
{
    public int? Id { get; set; }
    public string Name { get; set; } = String.Empty;
    public string Email { get; set; } = String.Empty;
}

public interface IPersonRepository
{
    Task AddAsync(Person person);
    Task<List<Person>> GetAllAsync();
}

public class PersonRepository : IPersonRepository
{
    private readonly string? _connectionString;

    public PersonRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("MyDatabaseConnection");
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

    public async Task AddAsync(Person person)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var insertCommand = "INSERT INTO person (name, email) VALUES (@name, @email);";

        await using (var command = new NpgsqlCommand(insertCommand, connection))
        {
            command.Parameters.AddWithValue("name", person.Name);
            command.Parameters.AddWithValue("email", person.Email);

            await command.ExecuteNonQueryAsync();
        }
    }
}

