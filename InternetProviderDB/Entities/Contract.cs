using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Text.Json.Serialization;

namespace InternetProviderDB.Entities
{
    public class Contract
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("StartDate")]
        public DateTime StartDate { get; set; }

        [BsonElement("EndDate")]
        public DateTime EndDate { get; set; }

        [BsonElement("ServiceId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ServiceId { get; set; }

        [BsonElement("ClientId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ClientId { get; set; }

        [BsonElement("DeviceId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string DeviceId { get; set; }

        [BsonIgnoreIfNull]
        
        public string Client { get; set; }

        [BsonIgnoreIfNull]
        
        public string Service { get; set; }

        [BsonIgnoreIfNull]
        
        public string Device { get; set; }

        [BsonElement("MethodPayment")]
        [BsonRepresentation(BsonType.String)]
        public Payment MethodPayment { get; set; }

        [BsonElement("StatusContract")]
        [BsonRepresentation(BsonType.String)]
        public Status StatusContract { get; set; }

        public Contract()
        {
            StartDate = DateTime.UtcNow;
            EndDate = StartDate.AddMonths(1);
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum Status
        {
            Active,
            Cancelled
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum Payment
        {
            Cash,
            Card
        }
    }
}
