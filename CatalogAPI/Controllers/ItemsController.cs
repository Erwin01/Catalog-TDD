using CatalogAPI.Entities;
using CatalogAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CatalogAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class ItemsController : ControllerBase
    {

        private readonly IItemsRepository _repository;
        private readonly ILogger<ItemsController> _logger;


        public ItemsController(IItemsRepository repository, ILogger<ItemsController> logger)
        {
            _repository = repository;
            _logger = logger;
        }



        // GET /items
        [HttpGet]
        public async Task<IEnumerable<ItemDto>> GetItemsAsync()
        {
            var items = (await _repository.GetItemsAsync()).Select(item => item.AsDto());
            _logger.LogInformation($"{DateTime.Now:hh:mm:ss}: Retrieved {items.Count()} items");

            return items;
        }



        // GET /items
        // Method Filter Of Elements
        [HttpGet]
        [Route("/items/name")]
        public async Task<IEnumerable<ItemDto>> GetItemsNameAsync([FromQuery] string name = null)
        {

            var items = (await _repository.GetItemsAsync()).Select(item => item.AsDto());

            if (!string.IsNullOrWhiteSpace(name))
            {
                items = items.Where(item => item.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
            }

            _logger.LogInformation($"{DateTime.Now:hh:mm:ss}: Retrieved {items.Count()} items");

            return items;

        }



        // GET /items/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetItemAsync(string id)
        {
            var item = await _repository.GetItemByIdAsync(id);

            if (item is null)
            {
                return NotFound();
            }

            _logger.LogInformation($"{DateTime.Now:hh:mm:ss}: Retrieved Id: {item.Id}");

            return item.AsDto();
        }



        // POST /items
        [HttpPost]
        public async Task<ActionResult<ItemDto>> CreateItemAsync(CreateItemDto createItemDto)
        {
            Item item = new() 
            {
                Id = ObjectId.GenerateNewId(),
                Name = createItemDto.Name,
                Price = createItemDto.Price,
                Description = createItemDto.Description,
                CreatedDate = DateTimeOffset.Now
            };

            await _repository.CreateItemAsync(item);

            _logger.LogInformation($"{DateTime.Now:hh:mm:ss}: Created Item: {item.Id} {item.Name} {item.Price}");

            return CreatedAtAction(nameof(GetItemAsync), new { id = item.Id }, item.AsDto());
        }



        // PUT /items/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateItemAsync(string id, UpdateItemDto updateItemDto)
        {

            var existingItem = await _repository.GetItemByIdAsync(id);

            if (existingItem is null)
            {

                return NotFound();
            }

            existingItem.Name = updateItemDto.Name;
            existingItem.Description = updateItemDto.Description;
            existingItem.Price = updateItemDto.Price;

            await _repository.UpdateItemAsync(existingItem);

            _logger.LogInformation($"{DateTime.Now:hh:mm:ss}: Updated Item: {existingItem.Id}");

            return NoContent();
        }



        // DELETE /items/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteItemAsync(string id)
        {

            var existingItem = await _repository.GetItemByIdAsync(id);

            if (existingItem is null)
            {
                return NotFound();
            }

            await _repository.DeleItemAsync(id);

            _logger.LogInformation($"{DateTime.Now:hh:mm:ss}: Deleted Item: {existingItem.Id}");

            return NoContent();
        }

    }
}
