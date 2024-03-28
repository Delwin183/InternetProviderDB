using InternetProviderDB.Data;
using InternetProviderDB.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.RegularExpressions;

namespace InternetProviderDB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMongoCollection<User>? _usersCollection;
        private readonly IMongoCollection<Cash>? _cashCollection;



        public UserController(MongoDbService mongoDbService)
        {
            _usersCollection = mongoDbService.Database?.GetCollection<User>("User");
            _cashCollection = mongoDbService.Database?.GetCollection<Cash>("Cash");


        }

        // Método para validar el formato del nombre de usuario
        private bool IsValidUsername(string username)
        {
            // El nombre de usuario debe tener 8 caracteres mínimo y 20 como máximo
            if (username.Length < 8 || username.Length > 20)
                return false;

            // El nombre de usuario debe tener letras y al menos un número. Sin caracteres especiales.
            return Regex.IsMatch(username, @"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]*$");
        }

        // Método para validar el formato de la contraseña
        private bool IsValidPassword(string password)
        {
            // La contraseña debe tener al menos un número, al menos una letra mayúscula, mínimo 8 caracteres y máximo 30 caracteres.
            return Regex.IsMatch(password, @"^(?=.*\d)(?=.*[A-Z]).{8,30}$");
        }

        [HttpGet]
        public async Task<IEnumerable<User>> Get()
        {
            return await _usersCollection.Find(FilterDefinition<User>.Empty).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User?>> GetById(string id)
        {
            var filter = Builders<User>.Filter.Eq(x => x.UserId, id);
            var User = await _usersCollection.Find(filter).FirstOrDefaultAsync();
            return User != null ? Ok(User) : NotFound();
        }

        [HttpPost]
        public async Task<ActionResult> Create(User user)
        {
            try
            {
                // Validar el formato del nombre de usuario
                if (!IsValidUsername(user.UserName))
                    return BadRequest("The username must be between 8 and 20 characters, and contain at least one letter and one number.");

                // Verificar si el nombre de usuario está duplicado
                var existingUser = await _usersCollection.Find(u => u.UserName == user.UserName).FirstOrDefaultAsync();
                if (existingUser != null)
                    return Conflict("Username is already in use.");

                // Validar el formato de la contraseña
                if (!IsValidPassword(user.Password))
                    return BadRequest("The password must be at least 8 characters, including at least one number and one uppercase letter.");

                // Insertar el usuario en la base de datos
                await _usersCollection.InsertOneAsync(user);

                // Devolver una respuesta 201 Created junto con la ubicación del nuevo recurso creado
                return CreatedAtAction(nameof(GetById), new { id = user.UserId }, user);
            }
            catch (Exception error)
            {
                // En caso de cualquier error durante la inserción, devolver un error interno del servidor (500)
                return StatusCode(500, $"Internal Server Error: {error.Message}");
            }
        }


        // Nuevo método para asignar caja a usuario "Cashier"
        [HttpPost("{managerId}/assignCash/{cashierId}")]
        public async Task<ActionResult> AssignCashToCashier(string managerId, string cashierId, [FromBody] RequestCashId request)
        {
            try
            {
                var managerFilter = Builders<User>.Filter.And(
                    Builders<User>.Filter.Eq(u => u.UserId, managerId),
                    Builders<User>.Filter.Eq(u => u.UserRole, Entities.User.Rol.Manager)
                );

                // Verificar si el usuario que realiza la asignación es de tipo "Manager"
                var manager = await _usersCollection.Find(managerFilter).FirstOrDefaultAsync();
                if (manager == null)
                    return NotFound("Manager not found or not authorized.");

                // Verificar si el usuario "Cashier" existe
                var cashier = await _usersCollection.Find(u => u.UserId == cashierId && u.UserRole == Entities.User.Rol.Cashier).FirstOrDefaultAsync();
                if (cashier == null)
                    return NotFound("Cashier not found.");

                // Obtener la información de la caja
                var cash = await _cashCollection.Find(c => c.CashNumber == request.CashId).FirstOrDefaultAsync();
                if (cash == null)
                    return NotFound("Cash not found.");

                // Asignar la caja al usuario "Cashier" en la colección de cajas
                cash.AssignedTo.Add(cashier);
                await _cashCollection.ReplaceOneAsync(c => c.CashNumber == request.CashId, cash);

                // Asignar la caja al usuario "Cashier" en la colección de usuarios
                //cashier.AssignedCash = cash;
               // await _usersCollection.ReplaceOneAsync(u => u.UserId == cashierId, cashier);

                return Ok($"Cash {cash.CashNumber} assigned to Cashier {cashierId} successfully by Manager {managerId}.");
            }
            catch (Exception error)
            {
                return StatusCode(500, $"Internal Server Error: {error.Message}");
            }


        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var filter = Builders<User>.Filter.Eq(x => x.UserId, id);
            var result = await _usersCollection.DeleteOneAsync(filter);
            return result.IsAcknowledged ? Ok() : StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
    public class RequestCashId
    {
        public string CashId { get; set; }
        
    }
}
