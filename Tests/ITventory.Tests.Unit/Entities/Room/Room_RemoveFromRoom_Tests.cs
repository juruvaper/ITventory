using System;
using ITventory.Domain;
using ITventory.Domain.ValueObjects;
using Shouldly;
using Xunit;

namespace ITventory.Tests.Unit.Entities
{
    public class Room_RemoveFromRoom_Tests
    {
        private readonly Room _room;
        private readonly Employee _employee;

        public Room_RemoveFromRoom_Tests()
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
        public void given_employee_in_room_should_remove_from_room()
        {
            _room.AssignToRoom(_employee);
            _room.Employees.Count.ShouldBe(1);

            _room.RemoveFromRoom(_employee);

            _room.Employees.ShouldNotContain(_employee);
            _room.Employees.Count.ShouldBe(0);
        }

        [Fact]
        public void given_null_employee_should_throw_argument_null_exception()
        {
            Employee nullEmployee = null!;

            var exception = Record.Exception(() => _room.RemoveFromRoom(nullEmployee));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<ArgumentNullException>();
            exception.Message.ShouldBe("Value cannot be null. (Parameter 'Invalid employee id')");
        }

        [Fact]
        public void given_employee_not_in_room_should_throw_invalid_operation_exception()
        {
            var exception = Record.Exception(() => _room.RemoveFromRoom(_employee));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<InvalidOperationException>();
            exception.Message.ShouldBe("User not in the room");
        }

        [Fact]
        public void given_multiple_employees_should_remove_specific_employee()
        {
            var employee1 = Employee.CreateMinimal(new Username("user1"), "id1");
            var employee2 = Employee.CreateMinimal(new Username("user2"), "id2");
            var employee3 = Employee.CreateMinimal(new Username("user3"), "id3");

            _room.AssignToRoom(employee1);
            _room.AssignToRoom(employee2);
            _room.AssignToRoom(employee3);
            _room.Employees.Count.ShouldBe(3);

            _room.RemoveFromRoom(employee2);

            _room.Employees.ShouldContain(employee1);
            _room.Employees.ShouldNotContain(employee2);
            _room.Employees.ShouldContain(employee3);
            _room.Employees.Count.ShouldBe(2);
        }

        [Fact]
        public void given_same_employee_added_twice_should_remove_one_instance()
        {
            _room.AssignToRoom(_employee);
            _room.AssignToRoom(_employee);
            _room.Employees.Count.ShouldBe(2);

            _room.RemoveFromRoom(_employee);

            _room.Employees.Count.ShouldBe(1);
            _room.Employees.ShouldContain(_employee);
        }

        [Fact]
        public void given_employee_with_same_id_should_remove_correctly()
        {
            var employee1 = Employee.CreateMinimal(new Username("user1"), "id1");
            var employee2 = Employee.CreateMinimal(new Username("user2"), "id2");
            
            _room.AssignToRoom(employee1);
            _room.AssignToRoom(employee2);

            _room.RemoveFromRoom(employee1);

            _room.Employees.ShouldNotContain(employee1);
            _room.Employees.ShouldContain(employee2);
            _room.Employees.Count.ShouldBe(1);
        }
    }
}
