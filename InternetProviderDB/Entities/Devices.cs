using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace InternetProviderDB.Entities
{
    public class Devices
    {
        [BsonId]
        private ObjectId Id { get; set; }

        [BsonElement("DeviceId")]
        public string DeviceId { get; set; }

        [BsonElement("DeviceName")]
        public string DeviceName { get; set; }

        [BsonElement("DeviceDescription")]
        public string DeviceDescription { get; set; }

        [JsonIgnore]
        [BsonElement("Contracts")]
        public List<Contract> Contracts { get; set; } = new List<Contract>();
    }
}
