using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class Book
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }  

        public string Title { get; set; }
        public int PublisheDate { get; set; } = 0;

        public string? Genre { get; set; } = null;

        public string? Description { get; set; } = null;

        public string? Picture { get; set; } = null;
        public int Pages { get; set; } = 0;

        public int Price { get; set; } = 0;

        [BsonRepresentation(BsonType.ObjectId)]
        public string AuthorId { get; set; } = string.Empty; // Reference na autora
    }
}
