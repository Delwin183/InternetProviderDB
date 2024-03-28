using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using InternetProviderDB.Entities;
using InternetProviderDB.Data;
using InternetProviderDB.Helpers;
using System.Text.Json.Serialization;

namespace InternetProviderDB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TurnsController : ControllerBase
    {
        
        private readonly IMongoCollection<Turn> _turns;
        private readonly IMongoCollection<Cash> _cash;
        private readonly IMongoCollection<Client> _clients;


        public TurnsController(MongoDbService mongoDbService)
        {
            _turns = mongoDbService.Database.GetCollection<Turn>("Turns");
            _cash = mongoDbService.Database.GetCollection<Cash>("Cash");
            _clients = mongoDbService.Database.GetCollection<Client>("Client");


        }

        [HttpGet]
        public async Task<IEnumerable<Turn>> Get()
        {
            return await _turns.Find(FilterDefinition<Turn>.Empty).ToListAsync();
        }


        [HttpGet("{Description}")]
        public async Task<ActionResult<Turn>> GetTurnById(string Description)
        {
            try
            {
                var turn = await _turns.Find(turn => turn.Description == Description).FirstOrDefaultAsync();
                if (turn == null)
                    return NotFound("Turno no encontrado.");
                return Ok(turn);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpPost("generate-turn/{Identification}/{AttentionType}")]
        public async Task<ActionResult<Turn>> GenerateTurn(string Identification, Turn.AttentionType AttentionType)
        {
            try
            {
                var client = await _clients.Find(client => client.Identification == Identification).FirstOrDefaultAsync();
                if (client == null)
                    return NotFound("Cliente no encontrado.");

                // Generar la descripción del turno
                var descriptionTurn = DescriptionTurnHelper.GenerateDescription(AttentionType);

                // Obtener una caja disponible para el tipo de atención
                var cash = await GetCashAvailable(AttentionType);
                if (cash == null)
                    return NotFound("No hay cajas disponibles para este tipo de atención.");

                // Crear el turno
                var turno = new Turn
                {
                    Description = descriptionTurn,
                    Date = DateTime.UtcNow,
                    CashId = cash.CashNumber, // Asignar el ID de la caja
                    ClientId = client.Identification, // Aquí puedes asignar el ID del usuario que generó el turno si lo deseas
                    ClientName = $"{client.Name} {client.LastName}",
                    Attentiontype = AttentionType
                };

                // Guardar el turno en la base de datos
                await _turns.InsertOneAsync(turno);

                return Ok(turno);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        private async Task<Cash> GetCashAvailable(Turn.AttentionType AttentionType)
        {
            return await _cash.Find(cash => cash.CashDescription == AttentionType.ToString() && cash.AssignedTo.Count == 0).FirstOrDefaultAsync();
        }
    }
}
