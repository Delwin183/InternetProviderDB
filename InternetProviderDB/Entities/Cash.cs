using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace InternetProviderDB.Entities
{
    public class Cash
    {
        [BsonId]
        private ObjectId Id { get; set; }

        [BsonElement("CashNumber")]
        public string? CashNumber { get; set; }

        [BsonElement("CashDescription")]
        public string? CashDescription { get; set; }

        [BsonElement("AssignedTo")]
        public List<User> AssignedTo { get; set; } = new List<User>();

       
    }
}
