using InternetProviderDB.Data;
using InternetProviderDB.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace InternetProviderDB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly IMongoCollection<Service>? _services;

        public ServiceController(MongoDbService mongoDbService)
        {
            _services = mongoDbService.Database?.GetCollection<Service>("Services");
        }

        [HttpGet]
        public async Task<IEnumerable<Service>> Get()
        {
            return await _services.Find(FilterDefinition<Service>.Empty).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Service?>> GetById(string id)
        {
            var filter = Builders<Service>.Filter.Eq(x => x.ServiceId, id);
            var result = await _services.Find(filter).FirstOrDefaultAsync();
            return result != null ? Ok(result) : NotFound();
        }

        [HttpPost]
        public async Task<ActionResult> Create(Service service)
        {
            try
            {
                // Insertar dispositivo en la base de datos
                await _services.InsertOneAsync(service);

                // Devolver una respuesta 201 Created junto con la ubicación del nuevo recurso creado
                return CreatedAtAction(nameof(GetById), new { id = service.ServiceId }, service);
            }
            catch (Exception error)
            {
                // En caso de cualquier error durante la inserción, devolver un error interno del servidor (500)
                return StatusCode(500, $"Internal Server Error: {error.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, [FromBody] Service service)
        {
            var filter = Builders<Service>.Filter.Eq(x => x.ServiceId, id);
            var updateDefinition = Builders<Service>.Update
                .Set(x => x.ServiceName, service.ServiceName)
                .Set(x => x.ServiceDescription, service.ServiceDescription)
                .Set(x => x.Price, service.Price);
            var result = await _services.UpdateOneAsync(filter, updateDefinition);

            return result.IsAcknowledged ? Ok() : StatusCode(StatusCodes.Status500InternalServerError);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var filter = Builders<Service>.Filter.Eq(x => x.ServiceId, id);
            var result = await _services.DeleteOneAsync(filter);
            return result.IsAcknowledged ? Ok() : StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}

