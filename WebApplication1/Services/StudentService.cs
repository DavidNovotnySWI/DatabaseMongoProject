using MongoDB.Driver;
using Microsoft.Extensions.Options;
using WebApplication1.Models;
using System.Net;

namespace WebApplication1.Service
{
    public class StudentService
    {
        private readonly IMongoCollection<Student> studentCollection;
        private readonly IMongoCollection<Book> booksCollection;

        public StudentService(IOptions<StudentDatabaseSettings> studentDbSettings) 
        {
            var mongoClient = new MongoClient(
                studentDbSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                studentDbSettings.Value.DatabaseName);

            studentCollection = mongoDatabase.GetCollection<Student>(
                studentDbSettings.Value.StudentsCollectionName);

            booksCollection = mongoDatabase.GetCollection<Book>(
                studentDbSettings.Value.BooksCollectionName);


        }

        public async Task<List<Student>> GetAsync() =>
            await studentCollection.Find(_ => true).ToListAsync();

        public async Task<Student> GetAsync(string id) =>
            await studentCollection.Find(x => x.Id == id).FirstOrDefaultAsync();


        public async Task CreateAsync (Student newStudent) =>
            await studentCollection.InsertOneAsync(newStudent);

        public async Task UpdateAsync(string id,Student updatedStudent) =>
            await studentCollection.ReplaceOneAsync(x => x.Id == id, updatedStudent);

        public async Task RemoveAsync(string id)
        {
            // Najdeme všechny knihy tohoto autora (AuthorId)
            var authorBooks = await booksCollection.Find(x => x.AuthorId == id).ToListAsync();

            // Pokud má autor nějaké knihy, tak je odstraníme
            if (authorBooks.Count > 0)
            {
                await booksCollection.DeleteManyAsync(x => x.AuthorId == id);
            }

            await studentCollection.DeleteOneAsync(x => x.Id == id);
        }
          

        public async Task AddMultipleAsync(List<Student> students) =>
            await studentCollection.InsertManyAsync(students);

        public async Task<List<Student>> FilterByLastNameAsync(string lastName) =>
            await studentCollection.Find(student => student.LastName == lastName).ToListAsync();

        public async Task<List<Student>> GetYoungerThanAsync(int age) =>
            await studentCollection.Find(student => student.Age < age).ToListAsync();

        public async Task<List<Student>> GetOlderThanAsync(int age) =>
              await studentCollection.Find(student => student.Age > age).ToListAsync();

    }

}
