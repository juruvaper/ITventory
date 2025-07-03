using System;
using ITventory.Domain;
using ITventory.Domain.Enums;
using ITventory.Domain.ValueObjects;
using Shouldly;
using Xunit;

namespace ITventory.Tests.Unit.Entities
{
    public class EmployeeTests
    {
        [Fact]
        public void given_valid_parameters_should_create_minimal_employee()
        {
            var username = new Username("john.doe");
            var identityId = "identity123";

            var employee = Employee.CreateMinimal(username, identityId);

            employee.Id.ShouldNotBe(Guid.Empty);
            employee.Username.ShouldBe(username);
            employee.IdentityId.ShouldBe(identityId);
            employee.IsActive.ShouldBeTrue();
            employee.Name.ShouldBeNull();
            employee.LastName.ShouldBeNull();
            employee.Area.ShouldBeNull();
            employee.PositionName.ShouldBeNull();
            employee.Seniority.ShouldBeNull();
            employee.ManagerId.ShouldBeNull();
            employee.DepartmentId.ShouldBeNull();
            employee.HireDate.ShouldBeNull();
            employee.BirthDate.ShouldBeNull();
            employee.RoomId.ShouldBeNull();
            employee.Experience.ShouldBe(0);
        }

        [Fact]
        public void given_same_parameters_should_generate_unique_ids()
        {
            var username = new Username("john.doe");
            var identityId = "identity123";

            var employee1 = Employee.CreateMinimal(username, identityId);
            var employee2 = Employee.CreateMinimal(username, identityId);

            employee1.Id.ShouldNotBe(employee2.Id);
        }

        [Fact]
        public void given_null_username_should_throw_null_reference_exception()
        {
            Username username = null!;
            var identityId = "identity123";

            var exception = Record.Exception(() => Employee.CreateMinimal(username, identityId));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<NullReferenceException>();
        }

        [Fact]
        public void given_valid_identity_id_should_not_throw_exception()
        {
            var username = new Username("john.doe");
            var identityId = "";

            var exception = Record.Exception(() => Employee.CreateMinimal(username, identityId));

            exception.ShouldBeNull();
        }

        [Fact]
        public void given_null_identity_id_should_not_throw_exception()
        {
            var username = new Username("john.doe");
            string identityId = null!;

            var exception = Record.Exception(() => Employee.CreateMinimal(username, identityId));

            exception.ShouldBeNull();
        }

        [Fact]
        public void given_hire_date_should_calculate_experience()
        {
            var username = new Username("john.doe");
            var identityId = "identity123";
            var employee = Employee.CreateMinimal(username, identityId);
            var hireDate = DateOnly.FromDateTime(DateTime.Today.AddYears(-5));

            employee.SetDetails(
                "John", "Doe", Area.IT, "Developer", Seniority.Senior,
                Guid.NewGuid(), Guid.NewGuid(), hireDate,
                DateOnly.FromDateTime(DateTime.Today.AddYears(-30)), Guid.NewGuid());

            employee.Experience.ShouldBe(5);
        }

        [Fact]
        public void given_no_hire_date_should_return_zero_experience()
        {
            var username = new Username("john.doe");
            var identityId = "identity123";
            var employee = Employee.CreateMinimal(username, identityId);

            employee.Experience.ShouldBe(0);
        }
    }
}
