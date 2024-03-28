using InternetProviderDB.Data;
using InternetProviderDB.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Text.RegularExpressions;

namespace InternetProviderDB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IMongoCollection<Client>? _clients;

        public ClientController(MongoDbService mongoDbService)
        {
            _clients = mongoDbService.Database?.GetCollection<Client>("Client");
        }



        // Método para validar la identificación del cliente
        private bool IsValidIdentification(string identification)
        {
            // La identificación debe tener mínimo 10 dígitos y máximo 13, y solo números
            return Regex.IsMatch(identification, @"^\d{10,13}$");
        }

        // Método para validar la dirección y referencia de dirección
        private bool IsValidAddress(string address)
        {
            // La dirección debe tener al menos 20 caracteres y máximo 100
            return address.Length >= 20 && address.Length <= 100;
        }

        // Método para validar el número de teléfono
        private bool IsValidPhoneNumber(string phoneNumber)
        {
            // El número de teléfono debe tener al menos 10 dígitos y solo empezar con 09
            return Regex.IsMatch(phoneNumber, @"^09\d{8}$");
        }



        //----------  ROUTES  ----------

        [HttpGet]
        public async Task<IEnumerable<Client>> Get()
        {
            return await _clients.Find(FilterDefinition<Client>.Empty).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Client?>> GetById(string id)
        {
            var filter = Builders<Client>.Filter.Eq(x => x.Identification, id);
            var client = await _clients.Find(filter).FirstOrDefaultAsync();
            return client != null ? Ok(client) : NotFound();
        }

        [HttpPost]
        public async Task<ActionResult> Create(Client client)
        {

            try
            {
                // Validar la identificación del cliente
                if (!IsValidIdentification(client.Identification))
                    return BadRequest("The ID must be between 10 and 13 digits and contain only numbers.");

                // Validar la dirección y referencia de dirección
                if (!IsValidAddress(client.Address) || !IsValidAddress(client.ReferenceAddress))
                    return BadRequest("The address and address reference must be between 20 and 100 characters.");

                // Validar el número de teléfono
                if (!IsValidPhoneNumber(client.Phone))
                    return BadRequest("The phone number must be 10 digits long and start with '09'.");

                // Insertar el cliente en la base de datos
                await _clients.InsertOneAsync(client);

                // Devolver una respuesta 201 Created junto con la ubicación del nuevo recurso creado
                return CreatedAtAction(nameof(GetById), new { id = client.Identification }, client);
            }
            catch (Exception error)
            {
                // En caso de cualquier error durante la inserción, devolver un error interno del servidor (500)
                return StatusCode(500, $"Internal Server Error: {error.Message}");
            }
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, [FromBody] Client client)
        {
          

            var filter = Builders<Client>.Filter.Eq(x => x.Identification, id);
            var updateDefinition = Builders<Client>.Update
                .Set(x => x.Name, client.Name)
                .Set(x => x.LastName, client.LastName)
                .Set(x => x.Email, client.Email)
                .Set(x => x.Phone, client.Phone)
                .Set(x => x.Address, client.Address)
                .Set(x => x.ReferenceAddress, client.ReferenceAddress);

            var result = await _clients.UpdateOneAsync(filter, updateDefinition);

            return result.IsAcknowledged ? Ok() : StatusCode(StatusCodes.Status500InternalServerError);
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var filter = Builders<Client>.Filter.Eq(x => x.Identification, id);
            var result = await _clients.DeleteOneAsync(filter);
            return result.IsAcknowledged ? Ok() : StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
