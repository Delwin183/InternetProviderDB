using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace InternetProviderDB.Entities
{
    public class Client
    {
        [BsonId]
        private ObjectId Id { get; set; }

        [BsonElement("Name")]
       
        public required string Name { get; set; }

        [BsonElement("LastName")] 
   
        public  required string LastName { get; set; }

        [BsonElement("Identification")]
    
        public  required string Identification { get; set; }

        [BsonElement("Email")]
    
        public required string Email { get; set; }

        [BsonElement("Phone")]
     
        public string? Phone { get; set; }

        [BsonElement("Address")]
   
        public string? Address { get; set; }

        [BsonElement("RefAddress")]
     
        public string? ReferenceAddress { get; set; }

        [BsonElement("Contracts")]
        [JsonIgnore]
        // Nueva propiedad para almacenar los contratos asociados
        public ICollection<Contract> Contracts { get; set; } = new List<Contract>();
    }
}
