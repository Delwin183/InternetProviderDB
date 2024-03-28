using MongoDB.Driver;

namespace InternetProviderDB.Data
{
    public class MongoDbService
    {
        private readonly IConfiguration _configuration;
        private readonly IMongoDatabase? database;
        public MongoDbService(IConfiguration configuration)
        {
            _configuration = configuration;

            var connectionString = _configuration.GetConnectionString("MongoDBConnection");
            var mongoUrl= MongoUrl.Create(connectionString);
            var mongoClient = new MongoClient(mongoUrl);
            database = mongoClient.GetDatabase(mongoUrl.DatabaseName);
        }

        public IMongoDatabase? Database => database;
    }
}