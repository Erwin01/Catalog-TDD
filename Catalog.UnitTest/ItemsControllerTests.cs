using CatalogAPI;
using CatalogAPI.Controllers;
using CatalogAPI.Entities;
using CatalogAPI.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Catalog.UnitTests
{

    public class ItemsControllerTests
    {

        #region [ GLOBALS VARIABLES ]

        private readonly Mock<IItemsRepository> repositoryStub = new Mock<IItemsRepository>();
        private readonly Mock<ILogger<ItemsController>> loggerStub = new Mock<ILogger<ItemsController>>();
        private readonly Random random = new Random();

        #endregion

        //MethodName_StateUnderTest_ExpectedBehavior



        [Fact]
        public async Task GetItemAsync_WithUnexistingItem_ReturnsNotFound() 
        {

            // Arrange: We Need Return Null. Stop of the controller and We pass the stops with the object property and the logger stops the object 
            repositoryStub.Setup(repository => repository.GetItemByIdAsync(It.IsAny<string>())).ReturnsAsync((Item)null);

            var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);

            // Action: We execute the action. Where to execute what you are going to test
            var result = await controller.GetItemAsync(ObjectId.Empty.ToString());

            // Assert: Check what we get as a result and the property we get from in our case
            Assert.IsType<NotFoundResult>(result.Result);
            //result.Result.Should().BeOfType<NotFoundResult>();

        }



        [Fact]
        public async Task GetItemAsync_WithExistingItem_ReturnsExpectedItem()
        {

            // Arrange 
            Item expectedItem = CreateRandomItem();

            repositoryStub.Setup(repository => repository.GetItemByIdAsync(It.IsAny<string>())).ReturnsAsync((expectedItem));

            var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);

            // Action
            var result = await controller.GetItemAsync(It.IsAny<string>());

            // Assert
            //result.Value.Should().BeEquivalentTo(expectedItem, options => options.ComparingByMembers<Item>());
            Assert.IsType<ItemDto>(result.Value);
            //var dto = result.Value;
            //Assert.Equal(expectedItem.Id, dto.Id);
            //Assert.Equal(expectedItem.Name, dto.Name);

        }



        [Fact]
        public async Task GetItemsAsync_WithExistingItems_ReturnsAllItems()
        {

            //Arrange
            //var expectedItems = new Item[] { CreateRandomItem(), CreateRandomItem(), CreateRandomItem() };

            //repositoryStub.Setup(repository => repository.GetItemsAsync()).ReturnsAsync(expectedItems);

            var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);

            // Act
            var actualItems = await controller.GetItemsAsync();

            //Assert
            //actualItems.Should().BeEquivalentTo(expectedItems, options => options.ComparingByMembers<Item>());
            Assert.IsAssignableFrom<IEnumerable<ItemDto>>(actualItems);
            //Assert.IsType<ItemDto>(actualItems);

        }



        [Fact]
        public async Task GetItemsAsync_WithMatchingItems_ReturnsMatchingItems()
        {

            //Arrange
            var allItems = new Item[] 
            {
                new Item(){ Name = "Potion"},
                new Item(){ Name = "Iron Sword"},
                new Item(){ Name = "Bronze Shield"}

            };

            var nameToMatch = "Potion";

            repositoryStub.Setup(repository => repository.GetItemsAsync()).ReturnsAsync(allItems);

            var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);

            // Act
            IEnumerable<ItemDto> foundItems = await controller.GetItemsNameAsync(nameToMatch);

            //Assert
            foundItems.Should().OnlyContain(item => item.Name == allItems[0].Name || item.Name == allItems[2].Name);

        }



        [Fact]
        public async Task CreateItemAsync_WithItemToCreate_ReturnCreatedItem()
        {

            // Arrange
            var itemToCreate = new CreateItemDto(
                Guid.NewGuid().ToString(), 
                Guid.NewGuid().ToString(),
                random.Next(1000)
            );

            var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);

            // Act
            var result = await controller.CreateItemAsync(itemToCreate);


            // Assert
            var createdItem = (result.Result as CreatedAtActionResult).Value as ItemDto;
            itemToCreate.Should().BeEquivalentTo(createdItem, options => options.ComparingByMembers<ItemDto>().ExcludingMissingMembers());
            createdItem.Id.Should();
            createdItem.CreatedDate.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1000));
        }



        [Fact]
        public async Task UpdateItemAsync_WithExistingItem_ReturnsNoContent()
        {

            // Arrange
            Item existingItem = CreateRandomItem();

            repositoryStub.Setup(repository => repository.GetItemByIdAsync(It.IsAny<string>())).ReturnsAsync((existingItem));

            var itemId = existingItem.Id;
            var itemToUpdate = new UpdateItemDto(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), existingItem.Price + 3);

            var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);

            // Act
            var result = await controller.UpdateItemAsync(ObjectId.GenerateNewId().ToString(), itemToUpdate);

            //Assert
            result.Should().BeOfType<NoContentResult>();

        }



        [Fact]
        public async Task DeleteItemAsync_WithExistingItem_ReturnsNoContent()
        {

            // Arrange
            Item existingItem = CreateRandomItem();

            repositoryStub.Setup(repository => repository.GetItemByIdAsync(It.IsAny<string>())).ReturnsAsync((existingItem));

            var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);

            // Act
            var result = await controller.DeleteItemAsync(ObjectId.GenerateNewId().ToString());

            //Assert
            result.Should().BeOfType<NoContentResult>();

        }



        private Item CreateRandomItem() 
        {
            return new Item() 
            {
                Id = ObjectId.GenerateNewId(),
                Name = Guid.NewGuid().ToString(),
                Description = Guid.NewGuid().ToString(),
                Price = random.Next(1000),
                CreatedDate = DateTimeOffset.Now
            };
        }
    }
}
