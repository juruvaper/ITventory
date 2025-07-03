using System;
using ITventory.Domain;
using ITventory.Domain.Enums;
using ITventory.Domain.ValueObjects;
using Shouldly;
using Xunit;

namespace ITventory.Tests.Unit.Entities
{
    public class Employee_ChangeDepartment_Tests
    {
        private readonly Employee _employee;
        private readonly Guid _initialDepartmentId;

        public Employee_ChangeDepartment_Tests()
        {
            var username = new Username("test.user");
            var identityId = "test123";
            _employee = Employee.CreateMinimal(username, identityId);
            _initialDepartmentId = Guid.NewGuid();
            
            // Set initial department through SetDetails
            _employee.SetDetails("John", "Doe", Area.IT, "Developer", Seniority.Senior,
                Guid.NewGuid(), _initialDepartmentId, DateOnly.FromDateTime(DateTime.Today.AddYears(-2)),
                DateOnly.FromDateTime(DateTime.Today.AddYears(-30)), Guid.NewGuid());
        }

        [Fact]
        public void given_different_department_id_should_change_department()
        {
            var newDepartmentId = Guid.NewGuid();

            _employee.ChangeDepartment(newDepartmentId);

            _employee.DepartmentId.ShouldBe(newDepartmentId);
        }

        [Fact]
        public void given_same_department_id_should_throw_argument_exception()
        {
            var exception = Record.Exception(() => _employee.ChangeDepartment(_initialDepartmentId));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<ArgumentException>();
            exception.Message.ShouldBe("Cannot set department to the same one");
        }

        [Fact]
        public void given_multiple_department_changes_should_update_correctly()
        {
            var firstNewDepartmentId = Guid.NewGuid();
            var secondNewDepartmentId = Guid.NewGuid();

            _employee.ChangeDepartment(firstNewDepartmentId);
            _employee.ChangeDepartment(secondNewDepartmentId);

            _employee.DepartmentId.ShouldBe(secondNewDepartmentId);
        }
    }
}
