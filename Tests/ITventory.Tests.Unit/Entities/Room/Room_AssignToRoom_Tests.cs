using System;
using System.Linq;
using ITventory.Domain;
using ITventory.Domain.ValueObjects;
using Shouldly;
using Xunit;

namespace ITventory.Tests.Unit.Entities
{
    public class Room_AssignToRoom_Tests
    {
        private readonly Room _room;
        private readonly Employee _employee;

        public Room_AssignToRoom_Tests()
        {
            var officeId = Guid.NewGuid();
            var roomName = "Test Room";
            var floor = 1;
            var area = 50.0;
            var capacity = 10;
            var personResponsibleId = Guid.NewGuid();
            
            _room = Room.Create(officeId, roomName, floor, area, capacity, personResponsibleId);
            
            var username = new Username("test.user");
            var identityId = "test123";
            _employee = Employee.CreateMinimal(username, identityId);
        }

        [Fact]
        public void given_valid_employee_should_assign_to_room()
        {
            _room.AssignToRoom(_employee);

            _room.Employees.ShouldContain(_employee);
            _room.Employees.Count.ShouldBe(1);
        }

        [Fact]
        public void given_null_employee_should_throw_argument_null_exception()
        {
            Employee nullEmployee = null!;

            var exception = Record.Exception(() => _room.AssignToRoom(nullEmployee));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<ArgumentNullException>();
            exception.Message.ShouldBe("Value cannot be null. (Parameter 'Invalid employee id')");
        }

        [Fact]
        public void given_room_at_capacity_should_throw_invalid_operation_exception()
        {
            // Create a room with capacity of 2
            var officeId = Guid.NewGuid();
            var roomName = "Small Room";
            var floor = 1;
            var area = 20.0;
            var capacity = 2;
            var personResponsibleId = Guid.NewGuid();
            var smallRoom = Room.Create(officeId, roomName, floor, area, capacity, personResponsibleId);

            // Fill the room to capacity
            var employee1 = Employee.CreateMinimal(new Username("user1"), "id1");
            var employee2 = Employee.CreateMinimal(new Username("user2"), "id2");
            smallRoom.AssignToRoom(employee1);
            smallRoom.AssignToRoom(employee2);

            // Try to assign one more employee
            var employee3 = Employee.CreateMinimal(new Username("user3"), "id3");

            var exception = Record.Exception(() => smallRoom.AssignToRoom(employee3));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<InvalidOperationException>();
            exception.Message.ShouldBe("Room capacity limit has been met");
        }

        [Fact]
        public void given_multiple_employees_should_assign_all_within_capacity()
        {
            var employee1 = Employee.CreateMinimal(new Username("user1"), "id1");
            var employee2 = Employee.CreateMinimal(new Username("user2"), "id2");
            var employee3 = Employee.CreateMinimal(new Username("user3"), "id3");

            _room.AssignToRoom(employee1);
            _room.AssignToRoom(employee2);
            _room.AssignToRoom(employee3);

            _room.Employees.ShouldContain(employee1);
            _room.Employees.ShouldContain(employee2);
            _room.Employees.ShouldContain(employee3);
            _room.Employees.Count.ShouldBe(3);
        }

        [Fact]
        public void given_same_employee_twice_should_add_twice()
        {
            _room.AssignToRoom(_employee);
            _room.AssignToRoom(_employee);

            _room.Employees.Count.ShouldBe(2);
            _room.Employees.Count(e => e.Id == _employee.Id).ShouldBe(2);
        }
    }
}
