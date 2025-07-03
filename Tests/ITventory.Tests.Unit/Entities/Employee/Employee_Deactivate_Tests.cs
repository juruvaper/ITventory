using System;
using ITventory.Domain;
using ITventory.Domain.Enums;
using ITventory.Domain.ValueObjects;
using Shouldly;
using Xunit;

namespace ITventory.Tests.Unit.Entities
{
    public class Employee_Deactivate_Tests
    {
        [Fact]
        public void given_active_employee_should_deactivate()
        {
            var employee = CreateActiveEmployee();

            employee.Deactivate();

            employee.IsActive.ShouldBeFalse();
        }

        [Fact]
        public void given_inactive_employee_should_throw_argument_exception()
        {
            var employee = CreateInactiveEmployee();

            var exception = Record.Exception(() => employee.Deactivate());

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<ArgumentException>();
            exception.Message.ShouldBe("User alerady inactive");
        }

        [Fact]
        public void given_multiple_activate_deactivate_cycles_should_work_correctly()
        {
            var employee = CreateActiveEmployee();

            // Deactivate
            employee.Deactivate();
            employee.IsActive.ShouldBeFalse();

            // Activate
            employee.Activate();
            employee.IsActive.ShouldBeTrue();

            // Deactivate again
            employee.Deactivate();
            employee.IsActive.ShouldBeFalse();
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
