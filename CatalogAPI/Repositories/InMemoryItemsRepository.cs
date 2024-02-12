using CatalogAPI.Entities;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CatalogAPI.Repositories
{

    public class InMemoryItemsRepository : IItemsRepository
    {


        private readonly List<Item> items = new()
        {
            new Item { Id = ObjectId.GenerateNewId(), Name = "Potion", Description = "Restores a small amount of HG", Price = 9, CreatedDate = DateTimeOffset.Now },
            new Item { Id = ObjectId.GenerateNewId(), Name = "Iron Sword", Description = "Iron", Price = 20, CreatedDate = DateTimeOffset.Now },
            new Item { Id = ObjectId.GenerateNewId(), Name = "Bronze Shield", Description = "Armor +100", Price = 19, CreatedDate = DateTimeOffset.Now },
        };


        public async Task CreateItemAsync(Item item)
        {
            items.Add(item);
            await Task.CompletedTask;
        }

        public async Task DeleItemAsync(string id)
        {
            var index = items.FindIndex(existingItem => existingItem.Id == new ObjectId(id));
            items.RemoveAt(index);
            await Task.CompletedTask;
        }

        public async Task<Item> GetItemByIdAsync(string id)
        {
            var item = items.Where(item => item.Id == new ObjectId(id)).SingleOrDefault();
            return await Task.FromResult(item);
        }

        public async Task<IEnumerable<Item>> GetItemsAsync()
        {
            return await Task.FromResult(items);
        }

        public async Task UpdateItemAsync(Item item)
        {
            var index = items.FindIndex(existingItem => new BsonObjectId(existingItem.Id) == new BsonObjectId(item.Id));
            items[index] = item;
            await Task.CompletedTask;
        }
    }
}
