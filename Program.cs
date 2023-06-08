var connectionString = "Host=localhost;Username=kevin;Password=1234;Database=test";
var personRepository = new PersonRepository(connectionString);

var persons = await personRepository.GetAllAsync();

foreach(var person in persons)
{
    Console.WriteLine($"ID: {person.Id}, Name: {person.Name}, Email: {person.Email}");
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IPersonRepository>(provider => new PersonRepository(connectionString));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

