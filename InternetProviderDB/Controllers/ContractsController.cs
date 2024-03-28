using InternetProviderDB.Data;
using InternetProviderDB.Entities;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;





namespace InternetProviderDB.Controllers

{
    [Route("api/[controller]")]
    [ApiController]
    public class ContractsController : ControllerBase
    {
        private readonly IMongoCollection<Contract> _contracts;
        private readonly IMongoCollection<Client> _clients;
        private readonly IMongoCollection<Service> _services;
        private readonly IMongoCollection<Devices> _devices;

        public ContractsController(MongoDbService mongoDbService)
        {
            _contracts = mongoDbService.Database.GetCollection<Contract>("Contracts");
            _clients = mongoDbService.Database.GetCollection<Client>("Client");
            _services = mongoDbService.Database.GetCollection<Service>("Services");
            _devices = mongoDbService.Database.GetCollection<Devices>("Devices");
        }



        // POST api/<ContractsController>

        [HttpPost("{Identification}/{serviceId}/{deviceId}")]
        public async Task<ActionResult> CreateContract(string Identification, string serviceId, string deviceId)
        {
            try
            {
                // Verificar si el cliente existe
                var filter = Builders<Client>.Filter.Eq(c => c.Identification, Identification);
                var client = await _clients.Find(filter).FirstOrDefaultAsync();
                if (client == null)
                    return NotFound("Client not found.");

                // Crear un nuevo contrato
                var contract = new Contract
                {
                    MethodPayment = Contract.Payment.Cash, // Por defecto, el método de pago es en efectivo
                    StatusContract = Contract.Status.Active // Estado del contrato por defecto
                };

                // Asignar la identificación del cliente al contrato
                contract.Client = client.Identification;

                // Asignar el servicio al contrato
                var service = await _services.Find(s => s.ServiceId == serviceId).FirstOrDefaultAsync();
                if (service == null)
                    return NotFound("Service not found.");
                contract.Service = service.ServiceName;

                // Asignar el dispositivo al contrato
                var device = await _devices.Find(d => d.DeviceId == deviceId).FirstOrDefaultAsync();
                if (device == null)
                    return NotFound("Device not found.");
                contract.Device = device.DeviceName;

                // Guardar el contrato en la base de datos
                await _contracts.InsertOneAsync(contract);

                // Agregar el contrato a la lista de contratos asociados en el cliente
                client.Contracts.Add(contract);
                await _clients.ReplaceOneAsync(c => c.Identification == client.Identification, client);

                return Ok($"Contract created successfully for client {Identification}.");
            }
            catch (Exception error)
            {
                return StatusCode(500, $"Internal Server Error: {error.Message}");
            }
        }

        [HttpGet("{Identification}")]
            public async Task<ActionResult<Contract>> GetContractById(string Identification)
            {
                var contract = await _contracts.Find(c => c.Client == Identification).FirstOrDefaultAsync();
            if (contract == null)
                {
                    return NotFound();
                }
                return contract;
            }
        }
}

//using InternetProviderDB.Data;
//using InternetProviderDB.Entities;
//using Microsoft.AspNetCore.Mvc;
//using MongoDB.Driver;
//using System;

//namespace InternetProviderDB.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class ContractsController : ControllerBase
//    {
//        private readonly IMongoCollection<Contract> _contracts;
//        private readonly IMongoCollection<Client> _clients;
//        private readonly IMongoCollection<Service> _services;
//        private readonly IMongoCollection<Devices> _devices;

//        public ContractsController(MongoDbService mongoDbService)
//        {
//            _contracts = mongoDbService.Database.GetCollection<Contract>("Contracts");
//            _clients = mongoDbService.Database.GetCollection<Client>("Client");
//            _services = mongoDbService.Database.GetCollection<Service>("Services");
//            _devices = mongoDbService.Database.GetCollection<Devices>("Devices");
//        }

//        [HttpPost("{Identification}/{serviceId}/{deviceId}")]
//        public async Task<ActionResult> CreateContract(string Identification, string serviceId, string deviceId)
//        {
//            try
//            {
//                // Verificar si el cliente existe
//                var filter = Builders<Client>.Filter.Eq(c => c.Identification, Identification);
//                var client = await _clients.Find(filter).FirstOrDefaultAsync();
//                if (client == null)
//                    return NotFound("Client not found.");

//                // Crear un nuevo contrato
//                var contract = new Contract
//                {

//                    MethodPayment = Contract.Payment.Cash, // Por defecto, el método de pago es en efectivo
//                    StatusContract = Contract.Status.Active // Estado del contrato por defecto
//                };

//                // Asignar el ID generado automáticamente por MongoDB a ClientId
//                var generatedId = contract.Id.ToString(); // Convertir el ObjectId a string
//                contract.ClientId = generatedId;


//                // Asignar el servicio al contrato
//                var service = await _services.Find(s => s.ServiceId == serviceId).FirstOrDefaultAsync();
//                if (service == null)
//                    return NotFound("Service not found.");
//                contract.Service = service;
//                contract.ServiceId = service.ServiceId;



//                // Asignar el dispositivo al contrato
//                var device = await _devices.Find(d => d.DeviceId == deviceId).FirstOrDefaultAsync();
//                if (device == null)
//                    return NotFound("Device not found.");
//                contract.Device = device;
//                contract.DeviceId = device.DeviceId;

//                // Guardar el contrato en la base de datos
//                await _contracts.InsertOneAsync(contract);

//                // Agregar el contrato a la lista de contratos asociados en el cliente
//                client.Contracts.Add(contract);
//                await _clients.ReplaceOneAsync(c => c.Identification == client.Identification, client);

//                //// Agregar el contrato a la lista de contratos asociados en el servicio
//                //service.Contracts.Add(contract);
//                //await _services.ReplaceOneAsync(s => s.ServiceId == serviceId, service);

//                //// Agregar el contrato a la lista de contratos asociados en el dispositivo
//                //device.Contracts.Add(contract);
//                //await _devices.ReplaceOneAsync(d => d.DeviceId == deviceId, device);

//                return Ok($"Contract created successfully for client {Identification}.");
//            }
//            catch (Exception error)
//            {
//                return StatusCode(500, $"Internal Server Error: {error.Message}");
//            }
//        }

//        [HttpGet("{id}")]
//        public async Task<ActionResult<Contract>> GetContractById(string id)
//        {
//            var contract = await _contracts.Find(c => c.Id == id).FirstOrDefaultAsync();
//            if (contract == null)
//            {
//                return NotFound();
//            }
//            return contract;
//        }
//    }
//}
