using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace InternetProviderDB.Entities
{
    public class Turn
    {
        [JsonIgnore]
        [BsonId]
        public ObjectId TurnId { get; set; }
        public string? Description { get; set; }
        public DateTime Date { get; set; }
        public string? CashId { get; set; } // Referencia al ID de la Caja asignada
        public string? ClientId { get; set; } // Referencia al ID del usuario que gestionó el turno
        public string? ClientName { get; set; } // Referencia al Nombre del usuario que gestionó el turno

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AttentionType? Attentiontype { get; set; } // Referencia al Nombre del usuario que gestionó el turno
        public enum AttentionType
        {
            CustomerSupport,
            ServicesPay,
            TechnicalService
        }

    }
}
