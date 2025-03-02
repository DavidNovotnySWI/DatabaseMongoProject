﻿using Microsoft.Extensions.Options;
using MongoDB.Driver;
using WebApplication1.Models;

namespace WebApplication1.Service
{
    public class BookService
    {
        private readonly IMongoCollection<Author> studentCollection;
        private readonly IMongoCollection<Book> booksCollection;

        public BookService(IOptions<AuthorDatabaseSettings> studentDbSettings)
        {
            var mongoClient = new MongoClient(
                studentDbSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                studentDbSettings.Value.DatabaseName);

            studentCollection = mongoDatabase.GetCollection<Author>(
                studentDbSettings.Value.AuthorsCollectionName);

            booksCollection = mongoDatabase.GetCollection<Book>(
                studentDbSettings.Value.BooksCollectionName);


        }

        public async Task<List<Book>> GetAsync() =>
            await booksCollection.Find(_ => true).ToListAsync();

        public async Task<Book> GetAsync(string id) =>
            await booksCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task<Book> GetBookAsync(string title) =>
            await booksCollection.Find(x => x.Title == title).FirstOrDefaultAsync();

        public async Task<Book> GetBookByGenreAsync(string genre) =>
            await booksCollection.Find(x => x.Genre == genre).FirstOrDefaultAsync();

        public async Task<Book> GetBookByAuthorAsync(string authorId) =>
            await booksCollection.Find(x => x.AuthorId == authorId).FirstOrDefaultAsync();

        public async Task CreateAsync(Book newBook) =>
            await booksCollection.InsertOneAsync(newBook);

        public async Task UpdateAsync(string id, Book updatedBook) =>
             await booksCollection.ReplaceOneAsync(x => x.Id == id, updatedBook);


        public async Task RemoveAsync(string id) =>
            await booksCollection.DeleteOneAsync(x => x.Id == id);


        public async Task AddMultipleAsync(List<Book> books) =>
               await booksCollection.InsertManyAsync(books);

        public async Task<List<Book>> FilterByTitleAsync(string title) =>
            await booksCollection.Find(book => book.Title == title).ToListAsync();

        public async Task<List<Book>> SearchBooksAsync(string searchTerm, string? category = null)
        {
            var textFilter = Builders<Book>.Filter.Text(searchTerm);

            var filters = new List<FilterDefinition<Book>> { textFilter };

            // Přidání filtru pro kategorii, pokud je poskytnuta
            if (!string.IsNullOrEmpty(category))
            {
                filters.Add(Builders<Book>.Filter.Eq(b => b.Genre, category));
            }

            var combinedFilter = Builders<Book>.Filter.And(filters);

            // Řazení podle relevance (pokud MongoDB ukládá textScore)
            var options = new FindOptions<Book>
            {
                Sort = Builders<Book>.Sort.MetaTextScore("textScore")
            };

            return await booksCollection.Find(combinedFilter).ToListAsync();
        }

        public async Task<Author> GetAuthorByBookIdAsync(string bookId)
        {
            // Najdeme knihu podle ID
            var book = await booksCollection.Find(x => x.Id == bookId).FirstOrDefaultAsync();

            if (book == null)
            {
                return null; // Kniha nenalezena
            }

            // Najdeme autora knihy (studenta) na základě AuthorId
            var student = await studentCollection.Find(x => x.Id == book.AuthorId).FirstOrDefaultAsync();

            if (student == null)
            {
                return null; // Student nenalezen
            }

            // Najdeme všechny knihy tohoto autora (AuthorId)
            var authorBooks = await booksCollection.Find(x => x.AuthorId == book.AuthorId).ToListAsync();

            // Přidáme všechny nalezené knihy do seznamu knih studenta
            student.Books = authorBooks;

            return student;
        }

    }

}
