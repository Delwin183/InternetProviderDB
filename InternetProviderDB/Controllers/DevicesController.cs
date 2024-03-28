using InternetProviderDB.Data;
using InternetProviderDB.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace InternetProviderDB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DevicesController : ControllerBase
    {
        private readonly IMongoCollection<Devices>? _devices;

        public DevicesController(MongoDbService mongoDbService)
        {
            _devices = mongoDbService.Database?.GetCollection<Devices>("Devices");
        }

        [HttpGet]
        public async Task<IEnumerable<Devices>> Get()
        {
            return await _devices.Find(FilterDefinition<Devices>.Empty).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Devices?>> GetById(string id)
        {
            var filter = Builders<Devices>.Filter.Eq(x => x.DeviceId, id);
            var result = await _devices.Find(filter).FirstOrDefaultAsync();
            return result != null ? Ok(result) : NotFound();
        }

        [HttpPost]
        public async Task<ActionResult> Create(Devices device)
        {
            try
            {
                // Insertar dispositivo en la base de datos
                await _devices.InsertOneAsync(device);

                // Devolver una respuesta 201 Created junto con la ubicación del nuevo recurso creado
                return CreatedAtAction(nameof(GetById), new { id = device.DeviceId }, device);
            }
            catch (Exception error)
            {
                // En caso de cualquier error durante la inserción, devolver un error interno del servidor (500)
                return StatusCode(500, $"Internal Server Error: {error.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, [FromBody] Devices device)
        {
            var filter = Builders<Devices>.Filter.Eq(x => x.DeviceId, id);
            var updateDefinition = Builders<Devices>.Update
                .Set(x => x.DeviceName, device.DeviceName)
                .Set(x => x.DeviceDescription, device.DeviceDescription);

            var result = await _devices.UpdateOneAsync(filter, updateDefinition);

            return result.IsAcknowledged ? Ok() : StatusCode(StatusCodes.Status500InternalServerError);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var filter = Builders<Devices>.Filter.Eq(x => x.DeviceId, id);
            var result = await _devices.DeleteOneAsync(filter);
            return result.IsAcknowledged ? Ok() : StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}