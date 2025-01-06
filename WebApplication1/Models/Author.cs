using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace WebApplication1.Models
{
    public class Author
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int Age { get; set; } = 0;
        public string? Image { get; set; } = string.Empty;

        [BsonIgnore] // ignore in database save
        public List<Book> Books { get; set; } = new(); // Book colection of authors

    }
}
