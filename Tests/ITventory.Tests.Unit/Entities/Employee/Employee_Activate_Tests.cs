using System;
using ITventory.Domain;
using ITventory.Domain.Enums;
using ITventory.Domain.ValueObjects;
using Shouldly;
using Xunit;

namespace ITventory.Tests.Unit.Entities
{
    public class Employee_Activate_Tests
    {
        [Fact]
        public void given_inactive_employee_should_activate()
        {
            var employee = CreateInactiveEmployee();

            employee.Activate();

            employee.IsActive.ShouldBeTrue();
        }

        [Fact]
        public void given_active_employee_should_throw_argument_exception()
        {
            var employee = CreateActiveEmployee();

            var exception = Record.Exception(() => employee.Activate());

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<ArgumentException>();
            exception.Message.ShouldBe("User alerady active");
        }

        private Employee CreateActiveEmployee()
        {
            var username = new Username("test.user");
            var identityId = "test123";
            return Employee.CreateMinimal(username, identityId); // Creates active employee by default
        }

        private Employee CreateInactiveEmployee()
        {
            var employee = CreateActiveEmployee();
            employee.Deactivate(); // Make it inactive first
            return employee;
        }
    }
}
