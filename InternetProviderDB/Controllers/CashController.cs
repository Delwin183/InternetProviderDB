using InternetProviderDB.Data;
using InternetProviderDB.Entities;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace InternetProviderDB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CashController : ControllerBase
    {
        private readonly IMongoCollection<Cash>? _cash;

        public CashController(MongoDbService mongoDbService)
        {
            _cash = mongoDbService.Database?.GetCollection<Cash>("Cash");
        }

        [HttpGet]
        public async Task<IEnumerable<Cash>> Get()
        {
            return await _cash.Find(FilterDefinition<Cash>.Empty).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Cash?>> GetById(string id)
        {
            var filter = Builders<Cash>.Filter.Eq(x => x.CashNumber, id);
            var result = await _cash.Find(filter).FirstOrDefaultAsync();
            return result != null ? Ok(result) : NotFound();
        }

        [HttpPost]
        public async Task<ActionResult> Create(Cash cash)
        {
           

            try
            {
                // Insertar cash en la base de datos
                await _cash.InsertOneAsync(cash);

                // Devolver una respuesta 201 Created junto con la ubicación del nuevo recurso creado
                return CreatedAtAction(nameof(GetById), new { id = cash.CashNumber }, cash);
            }
            catch (Exception error)
            {
                // En caso de cualquier error durante la inserción, devolver un error interno del servidor (500)
                return StatusCode(500, $"Internal Server Error: {error.Message}");
            }
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, [FromBody] Cash cash)
        {
          
            var filter = Builders<Cash>.Filter.Eq(x => x.CashNumber, id);
            var updateDefinition = Builders<Cash>.Update
                .Set(x => x.CashDescription, cash.CashDescription);
               

            var result = await _cash.UpdateOneAsync(filter, updateDefinition);

            return result.IsAcknowledged ? Ok() : StatusCode(StatusCodes.Status500InternalServerError);
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var filter = Builders<Cash>.Filter.Eq(x => x.CashNumber, id);
            var result = await _cash.DeleteOneAsync(filter);
            return result.IsAcknowledged ? Ok() : StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
