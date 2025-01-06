namespace WebApplication1.Models
{
    public class AuthorDatabaseSettings
    {
        public string? ConnectionString { get; set; } = null;
        public string? DatabaseName { get; set; } = null;
        public string? AuthorsCollectionName { get; set; } = null;

        public string? BooksCollectionName { get; set; } = null;

    }
}
