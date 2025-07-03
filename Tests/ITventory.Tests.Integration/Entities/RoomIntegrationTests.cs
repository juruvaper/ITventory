using ITventory.Tests.Integration.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;
using System.Net.Http.Json;
using System.Net;
using ITventory.Application.Services.RoomService.Add_room;
using ITventory.Application.Services.RoomService.Add_iventory;
using ITventory.Application.Services.RoomService.Reduce_inventory;
using ITventory.Infrastructure.EF.DTO;
using ITventory.Infrastructure.EF.Queries.Room;
using ITventory.Infrastructure.EF.Queries;
using ITventory.Domain.Enums;
using ITventory.Domain.ValueObjects;
using ITventory.Domain;
using ITventory.Application.Services.ProductService.Add_product;

namespace ITventory.Tests.Integration.Controllers
{
    public class RoomIntegrationTests : BaseIntegrationTest
    {
        private readonly HttpClient _httpClient;

        public RoomIntegrationTests(IntegrationTestWebApplicationFactory<Program> factory) : base(factory)
        {
            _httpClient = factory.CreateClient();
        }

        private async Task<(Country country, Location location, Office office, Employee employee)> CreateRequiredEntitiesAsync()
        {
            // Create Country
            var country = new Country("United States", "US", Region.NorthAmerica);
            WriteDbContext.Countries.Add(country);
            await WriteDbContext.SaveChangesAsync();

            // Create Location
            var location = new Location(
                "Main Campus",
                country.Id,
                new ZipCode("12345"),
                "New York",
                new Latitude(40.7128),
                new Longitude(-74.0060),
                TypeOfPlant.Factory
            );
            WriteDbContext.Locations.Add(location);
            await WriteDbContext.SaveChangesAsync();

            // Create Office
            var office = Office.Create(
                "Main Street",
                "100",
                location.Id,
                new Latitude(40.7128),
                new Longitude(-74.0060)
            );
            WriteDbContext.Office.Add(office);
            await WriteDbContext.SaveChangesAsync();

            // Create Employee
            var employee = Employee.CreateMinimal(new Username("testuser"), "identity123");
            WriteDbContext.Employees.Add(employee);
            await WriteDbContext.SaveChangesAsync();

            return (country, location, office, employee);
        }

        [Fact]
        public async Task Should_create_room_through_api()
        {
            // Arrange
            var (country, location, office, employee) = await CreateRequiredEntitiesAsync();
            
            var addRoomCommand = new AddRoom(
                office.Id,
                floor: 1,
                area: 50.0,
                capacity: 10,
                personResponsible: employee.Id,
                name: "API Test Room"
            );

            // Act
            var response = await _httpClient.PostAsJsonAsync("/itventory/room", addRoomCommand);

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
        }

        [Fact]
        public async Task Should_get_rooms_through_api()
        {
            // Arrange
            var (country, location, office, employee) = await CreateRequiredEntitiesAsync();
            
            // Create a room first
            var addRoomCommand = new AddRoom(
                office.Id,
                floor: 2,
                area: 75.0,
                capacity: 15,
                personResponsible: employee.Id,
                name: "Get Test Room"
            );
            
            await _httpClient.PostAsJsonAsync("/itventory/room", addRoomCommand);

            // Act
            var response = await _httpClient.GetAsync("/itventory/room");
            var content = await response.Content.ReadAsStringAsync();
            var rooms = await response.Content.ReadFromJsonAsync<ICollection<RoomDTO>>();

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            response.Content.Headers.ContentType?.MediaType.ShouldBe("application/json");
            content.ShouldNotBeNullOrEmpty();
            rooms.ShouldNotBeNull();
            rooms.ShouldNotBeEmpty();
            rooms.Any(r => r.RoomName == "Get Test Room").ShouldBeTrue();
        }

        [Fact]
        public async Task Should_get_room_by_id_through_api()
        {
            // Arrange
            var (country, location, office, employee) = await CreateRequiredEntitiesAsync();
            
            // Create a room first
            var addRoomCommand = new AddRoom(
                office.Id,
                floor: 3,
                area: 100.0,
                capacity: 20,
                personResponsible: employee.Id,
                name: "GetById Test Room"
            );
            
            await _httpClient.PostAsJsonAsync("/itventory/room", addRoomCommand);

            // Get the created room to find its ID
            var getRoomsResponse = await _httpClient.GetAsync($"/itventory/room?roomName=GetById Test Room");
            var rooms = await getRoomsResponse.Content.ReadFromJsonAsync<ICollection<RoomDTO>>();
            var createdRoom = rooms!.First();

            // Act
            var response = await _httpClient.GetAsync($"/itventory/room/{createdRoom.Id}");
            var room = await response.Content.ReadFromJsonAsync<RoomDTO>();

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            room.ShouldNotBeNull();
            room.RoomName.ShouldBe("GetById Test Room");
            room.Floor.ShouldBe(3);
            room.Area.ShouldBe(100.0);
            room.Capacity.ShouldBe(20);
            room.PersonResponsibleId.ShouldBe(employee.Id);
        }

        [Fact]
        public async Task Should_add_inventory_to_room_through_api()
        {
            // Arrange
            var (country, location, office, employee) = await CreateRequiredEntitiesAsync();
            
            // Create a room first
            var addRoomCommand = new AddRoom(
                office.Id,
                floor: 1,
                area: 50.0,
                capacity: 10,
                personResponsible: employee.Id,
                name: "Inventory Test Room"
            );
            
            await _httpClient.PostAsJsonAsync("/itventory/room", addRoomCommand);

            // Get the created room ID
            var getRoomsResponse = await _httpClient.GetAsync($"/itventory/room?roomName=Inventory Test Room");
            var rooms = await getRoomsResponse.Content.ReadFromJsonAsync<ICollection<RoomDTO>>();
            var createdRoom = rooms!.First();

            // Create a product
            var addProductCommand = new AddProduct("Test Product API", ProductType.Stationery, 50, 100.0);
            await _httpClient.PostAsJsonAsync("/itventory/product", addProductCommand);

            // Get the created product ID
            var getProductsResponse = await _httpClient.GetAsync($"/itventory/product?description=Test Product API");
            var products = await getProductsResponse.Content.ReadFromJsonAsync<ICollection<ProductDTO>>();
            var createdProduct = products!.First();

            // Add inventory
            var addInventoryCommand = new AddInventory(createdRoom.Id, createdProduct.Id, 25);

            // Act
            var response = await _httpClient.PutAsJsonAsync("/itventory/room/inventory-add", addInventoryCommand);

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

            // Verify inventory was added
            var inventoryResponse = await _httpClient.GetAsync($"/itventory/room/inventory?roomId={createdRoom.Id}");
            var inventory = await inventoryResponse.Content.ReadFromJsonAsync<ICollection<ProductInventoryDTO>>();
            
            inventoryResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
            inventory.ShouldNotBeNull();
            inventory.ShouldNotBeEmpty();
            inventory.Count.ShouldBe(1);
            inventory.First().SKU.ShouldBe(25);
            inventory.First().Product.Description.ShouldBe("Test Product API");
        }

        [Fact]
        public async Task Should_update_inventory_when_adding_existing_product_through_api()
        {
            // Arrange
            var (country, location, office, employee) = await CreateRequiredEntitiesAsync();
            
            // Create a room and product
            var addRoomCommand = new AddRoom(
                office.Id,
                floor: 1,
                area: 50.0,
                capacity: 10,
                personResponsible: employee.Id,
                name: "Update Inventory Room"
            );
            
            await _httpClient.PostAsJsonAsync("/itventory/room", addRoomCommand);

            var getRoomsResponse = await _httpClient.GetAsync($"/itventory/room?roomName=Update Inventory Room");
            var rooms = await getRoomsResponse.Content.ReadFromJsonAsync<ICollection<RoomDTO>>();
            var createdRoom = rooms!.First();

            var addProductCommand = new AddProduct("Update Test Product", ProductType.EHS, 100, 150.0);
            await _httpClient.PostAsJsonAsync("/itventory/product", addProductCommand);

            var getProductsResponse = await _httpClient.GetAsync($"/itventory/product?description=Update Test Product");
            var products = await getProductsResponse.Content.ReadFromJsonAsync<ICollection<ProductDTO>>();
            var createdProduct = products!.First();

            // Add initial inventory
            var addInventoryCommand1 = new AddInventory(createdRoom.Id, createdProduct.Id, 30);
            await _httpClient.PutAsJsonAsync("/itventory/room/inventory-add", addInventoryCommand1);

            // Act - Add more inventory of the same product
            var addInventoryCommand2 = new AddInventory(createdRoom.Id, createdProduct.Id, 20);
            var response = await _httpClient.PutAsJsonAsync("/itventory/room/inventory-add", addInventoryCommand2);

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

            // Verify inventory was updated (should be 50 total)
            var inventoryResponse = await _httpClient.GetAsync($"/itventory/room/inventory?roomId={createdRoom.Id}");
            var inventory = await inventoryResponse.Content.ReadFromJsonAsync<ICollection<ProductInventoryDTO>>();
            
            inventory.ShouldNotBeNull();
            inventory.Count.ShouldBe(1); // Still only one inventory entry
            inventory.First().SKU.ShouldBe(50); // 30 + 20
        }

        [Fact]
        public async Task Should_reduce_inventory_through_api()
        {
            // Arrange
            var (country, location, office, employee) = await CreateRequiredEntitiesAsync();
            
            // Create room, product, and initial inventory
            var addRoomCommand = new AddRoom(
                office.Id,
                floor: 1,
                area: 50.0,
                capacity: 10,
                personResponsible: employee.Id,
                name: "Reduce Inventory Room"
            );
            
            await _httpClient.PostAsJsonAsync("/itventory/room", addRoomCommand);

            var getRoomsResponse = await _httpClient.GetAsync($"/itventory/room?roomName=Reduce Inventory Room");
            var rooms = await getRoomsResponse.Content.ReadFromJsonAsync<ICollection<RoomDTO>>();
            var createdRoom = rooms!.First();

            var addProductCommand = new AddProduct("Reduce Test Product", ProductType.Fuel, 200, 75.0);
            await _httpClient.PostAsJsonAsync("/itventory/product", addProductCommand);

            var getProductsResponse = await _httpClient.GetAsync($"/itventory/product?description=Reduce Test Product");
            var products = await getProductsResponse.Content.ReadFromJsonAsync<ICollection<ProductDTO>>();
            var createdProduct = products!.First();

            // Add initial inventory
            var addInventoryCommand = new AddInventory(createdRoom.Id, createdProduct.Id, 50);
            await _httpClient.PutAsJsonAsync("/itventory/room/inventory-add", addInventoryCommand);

            // Act - Reduce inventory
            var reduceInventoryCommand = new ReduceInventory(createdRoom.Id, createdProduct.Id, 20);
            var response = await _httpClient.PutAsJsonAsync("/itventory/room/inventory-reduce", reduceInventoryCommand);

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

            // Verify inventory was reduced
            var inventoryResponse = await _httpClient.GetAsync($"/itventory/room/inventory?roomId={createdRoom.Id}");
            var inventory = await inventoryResponse.Content.ReadFromJsonAsync<ICollection<ProductInventoryDTO>>();
            
            inventory.ShouldNotBeNull();
            inventory.Count.ShouldBe(1);
            inventory.First().SKU.ShouldBe(30); // 50 - 20
        }

        [Fact]
        public async Task Should_handle_multiple_products_in_room_inventory_through_api()
        {
            // Arrange
            var (country, location, office, employee) = await CreateRequiredEntitiesAsync();
            
            // Create room
            var addRoomCommand = new AddRoom(
                office.Id,
                floor: 2,
                area: 75.0,
                capacity: 15,
                personResponsible: employee.Id,
                name: "Multi Product Room"
            );
            
            await _httpClient.PostAsJsonAsync("/itventory/room", addRoomCommand);

            var getRoomsResponse = await _httpClient.GetAsync($"/itventory/room?roomName=Multi Product Room");
            var rooms = await getRoomsResponse.Content.ReadFromJsonAsync<ICollection<RoomDTO>>();
            var createdRoom = rooms!.First();

            // Create multiple products
            var products = new[]
            {
                new AddProduct("Multi Product 1", ProductType.Stationery, 100, 50.0),
                new AddProduct("Multi Product 2", ProductType.EHS, 200, 75.0),
                new AddProduct("Multi Product 3", ProductType.Fuel, 150, 25.0)
            };

            foreach (var product in products)
            {
                await _httpClient.PostAsJsonAsync("/itventory/product", product);
            }

            // Get product IDs
            var product1Response = await _httpClient.GetAsync($"/itventory/product?description=Multi Product 1");
            var product1List = await product1Response.Content.ReadFromJsonAsync<ICollection<ProductDTO>>();
            var product1 = product1List!.First();

            var product2Response = await _httpClient.GetAsync($"/itventory/product?description=Multi Product 2");
            var product2List = await product2Response.Content.ReadFromJsonAsync<ICollection<ProductDTO>>();
            var product2 = product2List!.First();

            var product3Response = await _httpClient.GetAsync($"/itventory/product?description=Multi Product 3");
            var product3List = await product3Response.Content.ReadFromJsonAsync<ICollection<ProductDTO>>();
            var product3 = product3List!.First();

            // Act - Add inventory for all products
            await _httpClient.PutAsJsonAsync("/itventory/room/inventory-add", new AddInventory(createdRoom.Id, product1.Id, 10));
            await _httpClient.PutAsJsonAsync("/itventory/room/inventory-add", new AddInventory(createdRoom.Id, product2.Id, 20));
            await _httpClient.PutAsJsonAsync("/itventory/room/inventory-add", new AddInventory(createdRoom.Id, product3.Id, 15));

            // Assert
            var inventoryResponse = await _httpClient.GetAsync($"/itventory/room/inventory?roomId={createdRoom.Id}");
            var inventory = await inventoryResponse.Content.ReadFromJsonAsync<ICollection<ProductInventoryDTO>>();
            
            inventoryResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
            inventory.ShouldNotBeNull();
            inventory.Count.ShouldBe(3);
            
            var inventory1 = inventory.First(i => i.Product.Description == "Multi Product 1");
            var inventory2 = inventory.First(i => i.Product.Description == "Multi Product 2");
            var inventory3 = inventory.First(i => i.Product.Description == "Multi Product 3");

            inventory1.SKU.ShouldBe(10);
            inventory2.SKU.ShouldBe(20);
            inventory3.SKU.ShouldBe(15);
        }

        public override async Task DisposeAsync()
        {
            _httpClient.Dispose();
            await base.DisposeAsync();
        }
    }
}
