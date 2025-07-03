using System;
using ITventory.Domain;
using Shouldly;
using Xunit;

namespace ITventory.Tests.Unit.Entities
{
    public class Room_Create_Tests
    {
        [Fact]
        public void given_valid_parameters_should_create_room()
        {
            var officeId = Guid.NewGuid();
            var roomName = "Conference Room A";
            var floor = 1;
            var area = 50.0;
            var capacity = 10;
            var personResponsibleId = Guid.NewGuid();

            var room = Room.Create(officeId, roomName, floor, area, capacity, personResponsibleId);

            room.Id.ShouldNotBe(Guid.Empty);
            room.OfficeId.ShouldBe(officeId);
            room.RoomName.ShouldBe(roomName);
            room.Floor.ShouldBe(floor);
            room.Area.ShouldBe(area);
            room.Capacity.ShouldBe(capacity);
            room.PersonResponsibleId.ShouldBe(personResponsibleId);
            room.Employees.ShouldBeEmpty();
            room.RoomInventory.ShouldBeEmpty();
        }

        [Fact]
        public void given_same_parameters_should_generate_unique_ids()
        {
            var officeId = Guid.NewGuid();
            var roomName = "Conference Room A";
            var floor = 1;
            var area = 50.0;
            var capacity = 10;
            var personResponsibleId = Guid.NewGuid();

            var room1 = Room.Create(officeId, roomName, floor, area, capacity, personResponsibleId);
            var room2 = Room.Create(officeId, roomName, floor, area, capacity, personResponsibleId);

            room1.Id.ShouldNotBe(room2.Id);
        }

        [Theory]
        [InlineData(-2)]
        [InlineData(11)]
        [InlineData(15)]
        public void given_invalid_floor_should_throw_argument_exception(int invalidFloor)
        {
            var officeId = Guid.NewGuid();
            var roomName = "Conference Room A";
            var area = 50.0;
            var capacity = 10;
            var personResponsibleId = Guid.NewGuid();

            var exception = Record.Exception(() => Room.Create(officeId, roomName, invalidFloor, area, capacity, personResponsibleId));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<ArgumentException>();
            exception.Message.ShouldBe("Invalid floor number - it must vary between -1 and 10");
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(10)]
        public void given_valid_floor_should_create_room(int validFloor)
        {
            var officeId = Guid.NewGuid();
            var roomName = "Conference Room A";
            var area = 50.0;
            var capacity = 10;
            var personResponsibleId = Guid.NewGuid();

            var exception = Record.Exception(() => Room.Create(officeId, roomName, validFloor, area, capacity, personResponsibleId));

            exception.ShouldBeNull();
        }

        [Theory]
        [InlineData(4.9)]
        [InlineData(2001)]
        [InlineData(-1)]
        [InlineData(0)]
        public void given_invalid_area_should_throw_argument_exception(double invalidArea)
        {
            var officeId = Guid.NewGuid();
            var roomName = "Conference Room A";
            var floor = 1;
            var capacity = 10;
            var personResponsibleId = Guid.NewGuid();

            var exception = Record.Exception(() => Room.Create(officeId, roomName, floor, invalidArea, capacity, personResponsibleId));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<ArgumentException>();
            exception.Message.ShouldBe("Area must be between 5 and 2000");
        }

        [Theory]
        [InlineData(5)]
        [InlineData(100)]
        [InlineData(500)]
        [InlineData(2000)]
        public void given_valid_area_should_create_room(double validArea)
        {
            var officeId = Guid.NewGuid();
            var roomName = "Conference Room A";
            var floor = 1;
            var capacity = 10;
            var personResponsibleId = Guid.NewGuid();

            var exception = Record.Exception(() => Room.Create(officeId, roomName, floor, validArea, capacity, personResponsibleId));

            exception.ShouldBeNull();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(101)]
        public void given_invalid_capacity_should_throw_argument_exception(int invalidCapacity)
        {
            var officeId = Guid.NewGuid();
            var roomName = "Conference Room A";
            var floor = 1;
            var area = 50.0;
            var personResponsibleId = Guid.NewGuid();

            var exception = Record.Exception(() => Room.Create(officeId, roomName, floor, area, invalidCapacity, personResponsibleId));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<ArgumentException>();
            exception.Message.ShouldBe("Capacity must be between 2 and 100");
        }

        [Theory]
        [InlineData(2)]
        [InlineData(50)]
        [InlineData(100)]
        public void given_valid_capacity_should_create_room(int validCapacity)
        {
            var officeId = Guid.NewGuid();
            var roomName = "Conference Room A";
            var floor = 1;
            var area = 50.0;
            var personResponsibleId = Guid.NewGuid();

            var exception = Record.Exception(() => Room.Create(officeId, roomName, floor, area, validCapacity, personResponsibleId));

            exception.ShouldBeNull();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void given_invalid_room_name_should_throw_argument_exception(string invalidRoomName)
        {
            var officeId = Guid.NewGuid();
            var floor = 1;
            var area = 50.0;
            var capacity = 10;
            var personResponsibleId = Guid.NewGuid();

            var exception = Record.Exception(() => Room.Create(officeId, invalidRoomName, floor, area, capacity, personResponsibleId));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<ArgumentException>();
            exception.Message.ShouldBe("Room name cannot be empty");
        }

        [Fact]
        public void given_valid_room_name_should_create_room()
        {
            var officeId = Guid.NewGuid();
            var roomName = "Conference Room A";
            var floor = 1;
            var area = 50.0;
            var capacity = 10;
            var personResponsibleId = Guid.NewGuid();

            var exception = Record.Exception(() => Room.Create(officeId, roomName, floor, area, capacity, personResponsibleId));

            exception.ShouldBeNull();
        }
    }
}
