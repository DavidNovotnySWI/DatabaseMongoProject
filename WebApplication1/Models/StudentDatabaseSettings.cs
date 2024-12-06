namespace WebApplication1.Models
{
    public class StudentDatabaseSettings
    {
        public string? ConnectionString { get; set; } = null;
        public string? DatabaseName { get; set; } = null;
        public string? StudentsCollectionName { get; set; } = null;

        public string? BooksCollectionName { get; set; } = null;

    }
}
