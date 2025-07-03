using System;
using System.Linq;
using ITventory.Domain;
using ITventory.Domain.Enums;
using Shouldly;
using Xunit;

namespace ITventory.Tests.Unit.Entities
{
    public class Room_ReduceInventory_Tests
    {
        private readonly Room _room;
        private readonly Product _product;

        public Room_ReduceInventory_Tests()
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
        public void given_existing_inventory_should_reduce_sku()
        {
            var initialSku = 20;
            var reduceBy = 5;
            _room.AddInventory(_product, initialSku);

            _room.ReduceInventory(_product, reduceBy);

            var inventory = _room.RoomInventory.First();
            inventory.SKU.ShouldBe(initialSku - reduceBy);
        }

        [Fact]
        public void given_non_existing_product_should_throw_argument_exception()
        {
            var nonExistingProduct = Product.Create("Non-existing Product", ProductType.Fuel, 50.0, 25);
            var reduceBy = 5;

            var exception = Record.Exception(() => _room.ReduceInventory(nonExistingProduct, reduceBy));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<ArgumentException>();
            exception.Message.ShouldBe("Cannot reduce inventory: inventory doesn't exist");
        }

        [Fact]
        public void given_zero_sku_reduction_should_not_change_inventory()
        {
            var initialSku = 20;
            var reduceBy = 0;
            _room.AddInventory(_product, initialSku);

            _room.ReduceInventory(_product, reduceBy);

            var inventory = _room.RoomInventory.First();
            inventory.SKU.ShouldBe(initialSku);
        }

        [Fact]
        public void given_negative_sku_reduction_should_not_throw_exception_but_result_in_negative_check()
        {
            var initialSku = 20;
            var reduceBy = -5; // Negative value
            _room.AddInventory(_product, initialSku);

            // The ReduceSku method doesn't validate negative input, only the result
            var exception = Record.Exception(() => _room.ReduceInventory(_product, reduceBy));

            exception.ShouldBeNull(); // No exception should be thrown for negative input
            var inventory = _room.RoomInventory.First();
            inventory.SKU.ShouldBe(initialSku - reduceBy); // Should actually increase SKU
        }

        [Fact]
        public void given_sku_reduction_exceeding_current_sku_should_throw_argument_exception()
        {
            var initialSku = 10;
            var reduceBy = 15; // More than available
            _room.AddInventory(_product, initialSku);

            var exception = Record.Exception(() => _room.ReduceInventory(_product, reduceBy));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<ArgumentException>();
        }

        [Fact]
        public void given_sku_reduction_equal_to_current_sku_should_result_in_zero()
        {
            var initialSku = 10;
            var reduceBy = 10;
            _room.AddInventory(_product, initialSku);

            _room.ReduceInventory(_product, reduceBy);

            var inventory = _room.RoomInventory.First();
            inventory.SKU.ShouldBe(0);
        }

        [Fact]
        public void given_multiple_products_should_reduce_only_specified_product()
        {
            var product1 = Product.Create("Product 1", ProductType.Stationery, 100.0, 50);
            var product2 = Product.Create("Product 2", ProductType.EHS, 200.0, 30);
            var initialSku1 = 20;
            var initialSku2 = 15;
            var reduceBy = 5;

            _room.AddInventory(product1, initialSku1);
            _room.AddInventory(product2, initialSku2);

            _room.ReduceInventory(product1, reduceBy);

            var inventory1 = _room.RoomInventory.First(i => i.Product == product1);
            var inventory2 = _room.RoomInventory.First(i => i.Product == product2);
            
            inventory1.SKU.ShouldBe(initialSku1 - reduceBy);
            inventory2.SKU.ShouldBe(initialSku2); // Should remain unchanged
        }

        [Fact]
        public void given_multiple_reductions_should_accumulate()
        {
            var initialSku = 30;
            var firstReduction = 5;
            var secondReduction = 8;
            _room.AddInventory(_product, initialSku);

            _room.ReduceInventory(_product, firstReduction);
            _room.ReduceInventory(_product, secondReduction);

            var inventory = _room.RoomInventory.First();
            inventory.SKU.ShouldBe(initialSku - firstReduction - secondReduction);
        }
    }
}
