using System;
using ITventory.Domain;
using ITventory.Domain.Enums;
using ITventory.Domain.ValueObjects;
using Shouldly;
using Xunit;

namespace ITventory.Tests.Unit.Entities
{
    public class Employee_Promote_Tests
    {
        private readonly Employee _employee;

        public Employee_Promote_Tests()
        {
            var username = new Username("test.user");
            var identityId = "test123";
            _employee = Employee.CreateMinimal(username, identityId);
            
            // Set initial seniority through SetDetails
            _employee.SetDetails("John", "Doe", Area.IT, "Developer", Seniority.Junior,
                Guid.NewGuid(), Guid.NewGuid(), DateOnly.FromDateTime(DateTime.Today.AddYears(-2)),
                DateOnly.FromDateTime(DateTime.Today.AddYears(-30)), Guid.NewGuid());
        }

        [Fact]
        public void given_different_seniority_should_promote()
        {
            var newSeniority = Seniority.Senior;

            _employee.Promote(newSeniority);

            _employee.Seniority.ShouldBe(newSeniority);
        }

        [Fact]
        public void given_same_seniority_should_throw_argument_exception()
        {
            var exception = Record.Exception(() => _employee.Promote(Seniority.Junior));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<ArgumentException>();
            exception.Message.ShouldBe("Cannot set seniority to the same one");
        }

        [Fact]
        public void given_multiple_promotions_should_update_correctly()
        {
            _employee.Promote(Seniority.Regular);
            _employee.Promote(Seniority.Senior);
            _employee.Promote(Seniority.Architect);

            _employee.Seniority.ShouldBe(Seniority.Architect);
        }

        [Theory]
        [InlineData(Seniority.Manager)]
        [InlineData(Seniority.Junior)]
        [InlineData(Seniority.Regular)]
        [InlineData(Seniority.Senior)]
        [InlineData(Seniority.Architect)]
        public void given_all_valid_seniorities_should_promote(Seniority targetSeniority)
        {
            // Skip if it's the same as current seniority
            if (targetSeniority == Seniority.Junior) return;

            _employee.Promote(targetSeniority);

            _employee.Seniority.ShouldBe(targetSeniority);
        }
    }
}
