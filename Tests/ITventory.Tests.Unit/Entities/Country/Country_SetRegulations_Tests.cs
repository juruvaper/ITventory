using System;
using ITventory.Domain;
using ITventory.Domain.Enums;
using Shouldly;
using Xunit;

namespace ITventory.Tests.Unit.Entities
{
    public class Country_SetRegulations_Tests
    {
        private readonly Country _country;

        public Country_SetRegulations_Tests()
        {
            _country = new Country("Test Country", "TC", Region.Europe);
        }

        [Fact]
        public void given_valid_regulations_should_set_regulations()
        {
            var regulations = "GDPR compliant data handling required";

            _country.SetRegulations(regulations);

            _country.Regulations.ShouldBe(regulations);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void given_invalid_regulations_should_throw_argument_null_exception(string invalidRegulations)
        {
            var exception = Record.Exception(() => _country.SetRegulations(invalidRegulations));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<ArgumentNullException>();
            exception.Message.ShouldBe("Value cannot be null. (Parameter 'Regulations cannot be empty')");
        }

        [Fact]
        public void given_existing_regulations_should_overwrite()
        {
            var initialRegulations = "Initial regulations";
            var newRegulations = "Updated regulations";

            _country.SetRegulations(initialRegulations);
            _country.SetRegulations(newRegulations);

            _country.Regulations.ShouldBe(newRegulations);
        }
    }
}
