using WebApplication1.Service;
using WebApplication1.Controllers;
using WebApplication1.Models;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<StudentDatabaseSettings>(
    builder.Configuration.GetSection("StudentDatabase"));

builder.Services.AddSingleton<StudentService>();
builder.Services.AddSingleton<BookService>();

builder.Services.AddControllers()
    .AddJsonOptions(
        options => options.JsonSerializerOptions.PropertyNamingPolicy = null
    );

// Add services to the container.
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

// **Database setting**
using (var scope = app.Services.CreateScope())
{
    var settings = builder.Configuration.GetSection("StudentDatabase").Get<StudentDatabaseSettings>();
    var mongoClient = new MongoClient(settings.ConnectionString);
    var database = mongoClient.GetDatabase(settings.DatabaseName);
    var booksCollection = database.GetCollection<Book>(settings.BooksCollectionName);

    // **Inicialize search index**
    var indexKeys = Builders<Book>.IndexKeys.Text(b => b.Title).Text(b => b.Genre);
    await booksCollection.Indexes.CreateOneAsync(new CreateIndexModel<Book>(indexKeys));


}

app.Run();
