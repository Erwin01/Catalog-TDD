using CatalogAPI.Entities;
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

        private const string databaseName = "catalog";
        private const string collectionName = "items";

        private readonly IMongoCollection<Item> _itemsCollection;
        private readonly FilterDefinitionBuilder<Item> _filterDefinitionBuilder = Builders<Item>.Filter;
        public MongoDbItemsRepository(IMongoClient mongoClient)
        {
            IMongoDatabase database = mongoClient.GetDatabase(databaseName);
            _itemsCollection = database.GetCollection<Item>(collectionName);
        }

        //private readonly List<Item> items = new()
        //{
        //    new Item { Id = Guid.NewGuid(), Name = "Potion", Price = 9, CreatedDate = DateTimeOffset.UtcNow },
        //    new Item { Id = Guid.NewGuid(), Name = "Iron Sword", Price = 20, CreatedDate = DateTimeOffset.UtcNow },
        //    new Item { Id = Guid.NewGuid(), Name = "Bronze Shield", Price = 19, CreatedDate = DateTimeOffset.UtcNow },
        //};

        //public async Task CreateItem(Item item)
        //{
        //    await Task.CompletedTask;
        //    items.Add(item);

        //}

        //public async Task DeleItem(Guid id)
        //{
        //    await Task.CompletedTask;
        //    var index = items.FindIndex(existingItem => existingItem.Id == id);
        //    items.RemoveAt(index);

        //}

        //public async Task<Item> GetItem(Guid id)
        //{
        //    await Task.CompletedTask;
        //    var index = items.Where(item => item.Id == id).SingleOrDefault();
        //    return index;
        //}

        //public async Task<IEnumerable<Item>> GetItems()
        //{
        //    await Task.CompletedTask;
        //    return items;
        //}

        //public async Task UpdateItem(Item item)
        //{
        //    await Task.CompletedTask;
        //    var index = items.FindIndex(existingItem => existingItem.Id == item.Id);
        //    items[index] = item;


        //}


        public async Task CreateItemAsync(Item item)
        {
            await _itemsCollection.InsertOneAsync(item);
        }

        public async Task DeleItemAsync(Guid id)
        {
            var filter = _filterDefinitionBuilder.Eq(item => item.Id, id);
            await _itemsCollection.DeleteOneAsync(filter);
        }

        public async Task<Item> GetItemAsync(Guid id)
        {
            var filter = _filterDefinitionBuilder.Eq(item => item.Id, id);
            return await _itemsCollection.Find(filter).SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<Item>> GetItemsAsync()
        {
            return await _itemsCollection.Find(new BsonDocument()).ToListAsync();
        }

        public async Task UpdateItemAsync(Item item)
        {
            var filter = _filterDefinitionBuilder.Eq(existsItem => existsItem.Id, item.Id);
            await _itemsCollection.ReplaceOneAsync(filter, item);
        }
    }
}
