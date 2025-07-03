using System;
using System.Linq;
using ITventory.Domain;
using ITventory.Domain.Enums;
using Shouldly;
using Xunit;

namespace ITventory.Tests.Unit.Entities
{
    public class Room_AddInventory_Tests
    {
        private readonly Room _room;
        private readonly Product _product;

        public Room_AddInventory_Tests()
        {
            var officeId = Guid.NewGuid();
            var roomName = "Test Room";
            var floor = 1;
            var area = 50.0;
            var capacity = 10;
            var personResponsibleId = Guid.NewGuid();
            
            _room = Room.Create(officeId, roomName, floor, area, capacity, personResponsibleId);
            _product = Product.Create("Test Product", ProductType.Stationery, 100.0, 50);
        }

        [Fact]
        public void given_new_product_should_add_inventory()
        {
            var sku = 10;

            _room.AddInventory(_product, sku);

            _room.RoomInventory.Count.ShouldBe(1);
            var inventory = _room.RoomInventory.First();
            inventory.Product.ShouldBe(_product);
            inventory.SKU.ShouldBe(sku);
            inventory.RoomId.ShouldBe(_room.Id);
        }

        [Fact]
        public void given_existing_product_should_increase_sku()
        {
            var initialSku = 10;
            var additionalSku = 5;
            
            _room.AddInventory(_product, initialSku);
            _room.AddInventory(_product, additionalSku);

            _room.RoomInventory.Count.ShouldBe(1);
            var inventory = _room.RoomInventory.First();
            inventory.SKU.ShouldBe(initialSku + additionalSku);
        }

        [Fact]
        public void given_multiple_different_products_should_add_separate_inventories()
        {
            var product1 = Product.Create("Product 1", ProductType.Stationery, 100.0, 50);
            var product2 = Product.Create("Product 2", ProductType.EHS, 200.0, 30);
            var sku1 = 10;
            var sku2 = 5;

            _room.AddInventory(product1, sku1);
            _room.AddInventory(product2, sku2);

            _room.RoomInventory.Count.ShouldBe(2);
            var inventory1 = _room.RoomInventory.First(i => i.Product == product1);
            var inventory2 = _room.RoomInventory.First(i => i.Product == product2);
            
            inventory1.SKU.ShouldBe(sku1);
            inventory2.SKU.ShouldBe(sku2);
        }

        [Fact]
        public void given_zero_sku_should_create_inventory_with_zero()
        {
            var sku = 0;

            _room.AddInventory(_product, sku);

            _room.RoomInventory.Count.ShouldBe(1);
            var inventory = _room.RoomInventory.First();
            inventory.SKU.ShouldBe(0);
        }

        [Fact]
        public void given_negative_sku_should_throw_argument_exception()
        {
            var negativeSku = -5;

            var exception = Record.Exception(() => _room.AddInventory(_product, negativeSku));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<ArgumentException>();
        }

        [Fact]
        public void given_sku_exceeding_max_should_throw_argument_exception()
        {
            var excessiveSku = _product.MaxSKU + 1;

            var exception = Record.Exception(() => _room.AddInventory(_product, excessiveSku));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<ArgumentException>();
        }

        [Fact]
        public void given_existing_product_exceeding_max_sku_should_throw_argument_exception()
        {
            var initialSku = _product.MaxSKU - 5;
            var additionalSku = 10; // This will exceed max

            _room.AddInventory(_product, initialSku);
            
            var exception = Record.Exception(() => _room.AddInventory(_product, additionalSku));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<ArgumentException>();
        }
    }
}
