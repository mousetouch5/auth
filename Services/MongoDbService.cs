// MongoDbService.cs
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using auth.Models;
using auth.Controllers;

namespace auth.Services
{
    public class MongoDbService
    {
        private readonly IMongoDatabase _database;

        public MongoDbService(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("MongoDB");
            var client = new MongoClient(connectionString);
            var databaseName = configuration.GetSection("MongoDB:DatabaseName").Value;
            _database = client.GetDatabase(databaseName);
        }

        public IMongoCollection<ChatMessage> GetChatCollection() => _database.GetCollection<ChatMessage>("Chats");
    }
}
