using InternetProviderDB.Data;
using InternetProviderDB.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace InternetProviderDB.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMongoCollection<User> _usersCollection;

        public AuthController(MongoDbService mongoDbService)
        {
            _usersCollection = mongoDbService.Database.GetCollection<User>("User");
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var user = await _usersCollection.Find(u => u.UserName == request.UserName && u.Password == request.Password).FirstOrDefaultAsync();

                if (user == null)
                {
                    return NotFound("User not found or invalid credentials.");
                }

                // Verificar el tipo de usuario
                if (user.UserRole == Entities.User.Rol.Manager)
                {
                    // Usuario Manager, 
                    return Ok(user);
                }
                else if (user.UserRole == Entities.User.Rol.Cashier)
                {
                    // Usuario Cashier, 
                    return Ok(user);
                }
                else
                {
                    // Otro tipo de usuario no soportado
                    return StatusCode(StatusCodes.Status403Forbidden, "Unsupported user role.");
                }
            }
            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal Server Error: {error.Message}");
            }
        }
    }

    public class LoginRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
