using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace InternetProviderDB.Entities
{
    public class Service
    {
        [BsonId]
        private ObjectId Id { get; set; }

        [BsonElement("ServiceId")]
        public string ServiceId { get; set; }

        [BsonElement("ServiceName")]
        public string ServiceName { get; set; }

        [BsonElement("ServiceDescription")]
        public string ServiceDescription { get; set; }

        [BsonElement("Price")]
        public string Price { get; set; }

        [JsonIgnore]
        [BsonElement("Contracts")]
        public List<Contract> Contracts { get; set; } = new List<Contract>();

        [JsonIgnore]
        [BsonElement("Devices")]
        public List<Devices> Devices { get; set; } = new List<Devices>();
    }
}
