using System;
using ITventory.Domain;
using ITventory.Domain.Enums;
using ITventory.Domain.ValueObjects;
using Shouldly;
using Xunit;

namespace ITventory.Tests.Unit.Entities
{
    public class Employee_SetDetails_Tests
    {
        private readonly Employee _employee;

        public Employee_SetDetails_Tests()
        {
            var username = new Username("test.user");
            var identityId = "test123";
            _employee = Employee.CreateMinimal(username, identityId);
        }

        [Fact]
        public void given_valid_details_should_set_all_properties()
        {
            var name = "John";
            var lastName = "Doe";
            var area = Area.IT;
            var positionName = "Software Developer";
            var seniority = Seniority.Senior;
            var managerId = Guid.NewGuid();
            var departmentId = Guid.NewGuid();
            var hireDate = DateOnly.FromDateTime(DateTime.Today.AddYears(-2));
            var birthDate = DateOnly.FromDateTime(DateTime.Today.AddYears(-30));
            var roomId = Guid.NewGuid();

            _employee.SetDetails(name, lastName, area, positionName, seniority, 
                managerId, departmentId, hireDate, birthDate, roomId);

            _employee.Name.ShouldBe(name);
            _employee.LastName.ShouldBe(lastName);
            _employee.Area.ShouldBe(area);
            _employee.PositionName.ShouldBe(positionName);
            _employee.Seniority.ShouldBe(seniority);
            _employee.ManagerId.ShouldBe(managerId);
            _employee.DepartmentId.ShouldBe(departmentId);
            _employee.HireDate.ShouldBe(hireDate);
            _employee.BirthDate.ShouldBe(birthDate);
            _employee.RoomId.ShouldBe(roomId);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void given_invalid_name_should_throw_argument_exception(string invalidName)
        {
            var exception = Record.Exception(() => _employee.SetDetails(
                invalidName, "Doe", Area.IT, "Developer", Seniority.Senior,
                Guid.NewGuid(), Guid.NewGuid(), DateOnly.FromDateTime(DateTime.Today.AddYears(-2)),
                DateOnly.FromDateTime(DateTime.Today.AddYears(-30)), Guid.NewGuid()));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<ArgumentException>();
            exception.Message.ShouldBe("Name cannot be null or empty. (Parameter 'name')");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void given_invalid_last_name_should_throw_argument_exception(string invalidLastName)
        {
            var exception = Record.Exception(() => _employee.SetDetails(
                "John", invalidLastName, Area.IT, "Developer", Seniority.Senior,
                Guid.NewGuid(), Guid.NewGuid(), DateOnly.FromDateTime(DateTime.Today.AddYears(-2)),
                DateOnly.FromDateTime(DateTime.Today.AddYears(-30)), Guid.NewGuid()));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<ArgumentException>();
            exception.Message.ShouldBe("LastName cannot be null or empty. (Parameter 'lastName')");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void given_invalid_position_name_should_throw_argument_exception(string invalidPositionName)
        {
            var exception = Record.Exception(() => _employee.SetDetails(
                "John", "Doe", Area.IT, invalidPositionName, Seniority.Senior,
                Guid.NewGuid(), Guid.NewGuid(), DateOnly.FromDateTime(DateTime.Today.AddYears(-2)),
                DateOnly.FromDateTime(DateTime.Today.AddYears(-30)), Guid.NewGuid()));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<ArgumentException>();
            exception.Message.ShouldBe("PositionName cannot be null or empty. (Parameter 'positionName')");
        }

        [Fact]
        public void given_invalid_area_should_throw_argument_exception()
        {
            var invalidArea = (Area)999;

            var exception = Record.Exception(() => _employee.SetDetails(
                "John", "Doe", invalidArea, "Developer", Seniority.Senior,
                Guid.NewGuid(), Guid.NewGuid(), DateOnly.FromDateTime(DateTime.Today.AddYears(-2)),
                DateOnly.FromDateTime(DateTime.Today.AddYears(-30)), Guid.NewGuid()));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<ArgumentException>();
            exception.Message.ShouldBe("Invalid Area value. (Parameter 'area')");
        }

        [Fact]
        public void given_invalid_seniority_should_throw_argument_exception()
        {
            var invalidSeniority = (Seniority)999;

            var exception = Record.Exception(() => _employee.SetDetails(
                "John", "Doe", Area.IT, "Developer", invalidSeniority,
                Guid.NewGuid(), Guid.NewGuid(), DateOnly.FromDateTime(DateTime.Today.AddYears(-2)),
                DateOnly.FromDateTime(DateTime.Today.AddYears(-30)), Guid.NewGuid()));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<ArgumentException>();
            exception.Message.ShouldBe("Invalid Seniority value. (Parameter 'seniority')");
        }

        [Fact]
        public void given_empty_manager_id_should_throw_argument_exception()
        {
            var exception = Record.Exception(() => _employee.SetDetails(
                "John", "Doe", Area.IT, "Developer", Seniority.Senior,
                Guid.Empty, Guid.NewGuid(), DateOnly.FromDateTime(DateTime.Today.AddYears(-2)),
                DateOnly.FromDateTime(DateTime.Today.AddYears(-30)), Guid.NewGuid()));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<ArgumentException>();
            exception.Message.ShouldBe("ManagerId cannot be empty. (Parameter 'managerId')");
        }

        [Fact]
        public void given_empty_department_id_should_throw_argument_exception()
        {
            var exception = Record.Exception(() => _employee.SetDetails(
                "John", "Doe", Area.IT, "Developer", Seniority.Senior,
                Guid.NewGuid(), Guid.Empty, DateOnly.FromDateTime(DateTime.Today.AddYears(-2)),
                DateOnly.FromDateTime(DateTime.Today.AddYears(-30)), Guid.NewGuid()));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<ArgumentException>();
            exception.Message.ShouldBe("DepartmentId cannot be empty. (Parameter 'departmentId')");
        }

        [Fact]
        public void given_empty_room_id_should_throw_argument_exception()
        {
            var exception = Record.Exception(() => _employee.SetDetails(
                "John", "Doe", Area.IT, "Developer", Seniority.Senior,
                Guid.NewGuid(), Guid.NewGuid(), DateOnly.FromDateTime(DateTime.Today.AddYears(-2)),
                DateOnly.FromDateTime(DateTime.Today.AddYears(-30)), Guid.Empty));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<ArgumentException>();
            exception.Message.ShouldBe("RoomId cannot be empty. (Parameter 'roomId')");
        }

        [Fact]
        public void given_future_birth_date_should_throw_argument_exception()
        {
            var futureBirthDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));

            var exception = Record.Exception(() => _employee.SetDetails(
                "John", "Doe", Area.IT, "Developer", Seniority.Senior,
                Guid.NewGuid(), Guid.NewGuid(), DateOnly.FromDateTime(DateTime.Today.AddYears(-2)),
                futureBirthDate, Guid.NewGuid()));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<ArgumentException>();
            exception.Message.ShouldBe("BirthDate cannot be in the future. (Parameter 'birthDate')");
        }

        [Fact]
        public void given_future_hire_date_should_throw_argument_exception()
        {
            var futureHireDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));

            var exception = Record.Exception(() => _employee.SetDetails(
                "John", "Doe", Area.IT, "Developer", Seniority.Senior,
                Guid.NewGuid(), Guid.NewGuid(), futureHireDate,
                DateOnly.FromDateTime(DateTime.Today.AddYears(-30)), Guid.NewGuid()));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<ArgumentException>();
            exception.Message.ShouldBe("HireDate cannot be in the future. (Parameter 'hireDate')");
        }

        [Fact]
        public void given_birth_date_after_hire_date_should_throw_argument_exception()
        {
            var hireDate = DateOnly.FromDateTime(DateTime.Today.AddYears(-10));
            var birthDate = DateOnly.FromDateTime(DateTime.Today.AddYears(-5));

            var exception = Record.Exception(() => _employee.SetDetails(
                "John", "Doe", Area.IT, "Developer", Seniority.Senior,
                Guid.NewGuid(), Guid.NewGuid(), hireDate, birthDate, Guid.NewGuid()));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<ArgumentException>();
            exception.Message.ShouldBe("BirthDate cannot be after HireDate.");
        }

        [Theory]
        [InlineData(Area.Accountings)]
        [InlineData(Area.IT)]
        [InlineData(Area.SupplyChain)]
        [InlineData(Area.PMO)]
        [InlineData(Area.Manufacturing)]
        public void given_all_valid_areas_should_set_area(Area area)
        {
            _employee.SetDetails("John", "Doe", area, "Developer", Seniority.Senior,
                Guid.NewGuid(), Guid.NewGuid(), DateOnly.FromDateTime(DateTime.Today.AddYears(-2)),
                DateOnly.FromDateTime(DateTime.Today.AddYears(-30)), Guid.NewGuid());

            _employee.Area.ShouldBe(area);
        }

        [Theory]
        [InlineData(Seniority.Manager)]
        [InlineData(Seniority.Junior)]
        [InlineData(Seniority.Regular)]
        [InlineData(Seniority.Senior)]
        [InlineData(Seniority.Architect)]
        public void given_all_valid_seniorities_should_set_seniority(Seniority seniority)
        {
            _employee.SetDetails("John", "Doe", Area.IT, "Developer", seniority,
                Guid.NewGuid(), Guid.NewGuid(), DateOnly.FromDateTime(DateTime.Today.AddYears(-2)),
                DateOnly.FromDateTime(DateTime.Today.AddYears(-30)), Guid.NewGuid());

            _employee.Seniority.ShouldBe(seniority);
        }
    }
}
