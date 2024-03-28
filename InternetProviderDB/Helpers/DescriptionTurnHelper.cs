using InternetProviderDB.Entities;

namespace InternetProviderDB.Helpers
{
    public class DescriptionTurnHelper
    {
        private static Dictionary<Turn.AttentionType, string> AttentionTypeDescriptions = new Dictionary<Turn.AttentionType, string>
        {
            { Turn.AttentionType.CustomerSupport, "CS" }, // Atención al cliente
            { Turn.AttentionType.ServicesPay, "PS" }, // Pago de servicio
            { Turn.AttentionType.TechnicalService, "TS" } // Servicio técnico
        };

        private static Dictionary<Turn.AttentionType, int> AttentionTypeCounters = new Dictionary<Turn.AttentionType, int>
        {
            { Turn.AttentionType.CustomerSupport, 0 },
            { Turn.AttentionType.ServicesPay, 0 },
            { Turn.AttentionType.TechnicalService, 0 }
        };

        public static string GenerateDescription(Turn.AttentionType attentionType)
        {
            int counter = ++AttentionTypeCounters[attentionType];
            return $"{AttentionTypeDescriptions[attentionType]}{counter:D4}";
        }
    }
}

//---------------- VALIDACION CRUD TURNS Y GENERACION DE TURN DE ACUERDO A AL TIPO DE ATENCION -----------------