using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using NSwag.Annotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;



namespace InternetProviderDB.Entities
{
    public class User
    {
        [BsonId]
        private ObjectId Id { get; set; }

        
        [BsonElement("UserID")]
        public required String UserId { get; set; }


        [BsonElement("UserName")]
        public required string UserName { get; set; }


        [BsonElement("UserEmail")]
      
        public required string Email { get; set; }


        [BsonElement("UserPass")]
      
        public required string Password { get; set; }

        [BsonElement("UserRole")]
        [BsonRepresentation(BsonType.String)] // Representar el enum como string en la base de datos
        public Rol UserRole { get; set; }

        [JsonIgnore]
        [BsonElement("AssignedCash")]
        public Cash? AssignedCash { get; set; } // Referencia a la caja asignada

        [JsonIgnore]
        [BsonElement("CreationDate")]
        
        public DateTime? CreationDate { get; set; }

       


        public User()
        {
            CreationDate = DateTime.UtcNow; // Establece la fecha de creación como la fecha y hora actual en formato UTC
           
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum Rol
        {
            Manager,
            Cashier
        }

    }
}