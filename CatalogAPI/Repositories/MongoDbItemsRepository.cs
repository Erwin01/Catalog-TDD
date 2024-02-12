using CatalogAPI.Entities;
using CatalogAPI.Settings;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CatalogAPI.Repositories
{
    public class MongoDbItemsRepository : IItemsRepository
    {

        internal MongoDbSettings _repository = new MongoDbSettings();
        private readonly IMongoCollection<Item> Collection;


        public MongoDbItemsRepository()
        {
            Collection = _repository.mongoDatabase.GetCollection<Item>("Items");
        }

        public async Task<Item> GetItemByIdAsync(string id)
        {
            return await Collection.FindAsync(new BsonDocument { { "_id", new ObjectId(id) } }).Result.FirstAsync();
        }

        public async Task<IEnumerable<Item>> GetItemsAsync()
        {
            return await Collection.FindAsync(new BsonDocument()).Result.ToListAsync();
        }

        public async Task CreateItemAsync(Item item)
        {
            await Collection.InsertOneAsync(item);
        }

        public async Task UpdateItemAsync(Item item)
        {
            var filter = Builders<Item>.Filter.Eq(i => i.Id, item.Id);
            await Collection.ReplaceOneAsync(filter, item);
        }

        public async Task DeleItemAsync(string id)
        {
            var filter = Builders<Item>.Filter.Eq(i => i.Id, new ObjectId(id));
            await Collection.DeleteOneAsync(filter);
        }
    }
}
