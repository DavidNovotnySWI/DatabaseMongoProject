using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace WebApplication1.Models
{
    public class Student
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string FirstName { get; set; } = null;
        public string LastName { get; set; } = null;
        public int Age { get; set; } = 0;

        [BsonIgnore] // ignore in database save
        public List<Book> Books { get; set; } = new(); // Book colection of authors

    }
}
