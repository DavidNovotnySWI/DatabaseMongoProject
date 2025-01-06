using Microsoft.Extensions.Options;
using MongoDB.Driver;
using WebApplication1.Models;

namespace WebApplication1.Service
{
    public class AuthorService
    {
        private readonly IMongoCollection<Author> authorCollection;
        private readonly IMongoCollection<Book> booksCollection;

        public AuthorService(IOptions<AuthorDatabaseSettings> authorDbSettings)
        {
            var mongoClient = new MongoClient(
                authorDbSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                authorDbSettings.Value.DatabaseName);

            authorCollection = mongoDatabase.GetCollection<Author>(
                authorDbSettings.Value.AuthorsCollectionName);

            booksCollection = mongoDatabase.GetCollection<Book>(
                authorDbSettings.Value.BooksCollectionName);


        }

        public async Task<List<Author>> GetAsync() =>
            await authorCollection.Find(_ => true).ToListAsync();

        public async Task<Author> GetAsync(string id) =>
            await authorCollection.Find(x => x.Id == id).FirstOrDefaultAsync();


        public async Task CreateAsync(Author newAuthor) =>
            await authorCollection.InsertOneAsync(newAuthor);

        public async Task UpdateAsync(string id, Author updatedAuthor) =>
            await authorCollection.ReplaceOneAsync(x => x.Id == id, updatedAuthor);

        public async Task RemoveAsync(string id)
        {
            // Najdeme všechny knihy tohoto autora (AuthorId)
            var authorBooks = await booksCollection.Find(x => x.AuthorId == id).ToListAsync();

            // Pokud má autor nějaké knihy, tak je odstraníme
            if (authorBooks.Count > 0)
            {
                await booksCollection.DeleteManyAsync(x => x.AuthorId == id);
            }

            await authorCollection.DeleteOneAsync(x => x.Id == id);
        }


        public async Task AddMultipleAsync(List<Author> authors) =>
            await authorCollection.InsertManyAsync(authors);

        public async Task<List<Author>> FilterByLastNameAsync(string lastName) =>
            await authorCollection.Find(author => author.LastName == lastName).ToListAsync();

        public async Task<List<Author>> GetYoungerThanAsync(int age) =>
            await authorCollection.Find(author => author.Age < age).ToListAsync();

        public async Task<List<Author>> GetOlderThanAsync(int age) =>
              await authorCollection.Find(author => author.Age > age).ToListAsync();

    }

}
