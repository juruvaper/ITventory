using System;
using ITventory.Domain;
using ITventory.Domain.Enums;
using ITventory.Domain.ValueObjects;
using Shouldly;
using Xunit;

namespace ITventory.Tests.Unit.Entities
{
    public class Employee_SetManager_Tests
    {
        private readonly Employee _employee;

        public Employee_SetManager_Tests()
        {
            var username = new Username("test.user");
            var identityId = "test123";
            _employee = Employee.CreateMinimal(username, identityId);
        }

        [Fact]
        public void given_valid_manager_id_should_set_manager()
        {
            var managerId = Guid.NewGuid();

            _employee.SetManager(managerId);

            _employee.ManagerId.ShouldBe(managerId);
        }

        [Fact]
        public void given_empty_manager_id_should_throw_argument_exception()
        {
            var exception = Record.Exception(() => _employee.SetManager(Guid.Empty));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<ArgumentException>();
            exception.Message.ShouldBe("Manager ID cannot be empty or the same as the current user");
        }

        [Fact]
        public void given_same_id_as_employee_should_throw_argument_exception()
        {
            var exception = Record.Exception(() => _employee.SetManager(_employee.Id));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<ArgumentException>();
            exception.Message.ShouldBe("Manager ID cannot be empty or the same as the current user");
        }

        [Fact]
        public void given_different_manager_id_should_update_manager()
        {
            var firstManagerId = Guid.NewGuid();
            var secondManagerId = Guid.NewGuid();

            _employee.SetManager(firstManagerId);
            _employee.SetManager(secondManagerId);

            _employee.ManagerId.ShouldBe(secondManagerId);
        }
    }
}
