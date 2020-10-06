using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using ScramNet.Ally.AssignVictimClient.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ScramNet.Ally.AssignVictimClient
{
    public class Repository : IRepository<VictimClient>
    {
        private readonly ILogger<Repository> _logger;
        private readonly IMongoCollection<VictimClient> _collection;

        public Repository(DatabaseSettings settings, ILogger<Repository> logger)
        {
            _logger = logger;
            _collection = new MongoClient(settings.ConnectionString).GetDatabase(settings.DatabaseName).GetCollection<VictimClient>(settings.CollectionName);
        }
        public async Task InsertRecord(VictimClient record)
        {
            try
            {
                var filter = Builders<VictimClient>.Filter
                    .Where(x => x.VictimId == record.VictimId);
                await _collection.DeleteManyAsync(filter);
                await _collection.InsertOneAsync(record);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception("Unable to Insert Client");
            }
        }
    }
}
