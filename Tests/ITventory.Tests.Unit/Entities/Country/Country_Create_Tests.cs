using System;
using ITventory.Domain;
using ITventory.Domain.Enums;
using Shouldly;
using Xunit;

namespace ITventory.Tests.Unit.Entities
{
    public class Country_Create_Tests
    {
        [Fact]
        public void given_valid_parameters_should_return_country_instance()
        {
            var name = "Japan";
            var countryCode = "JP";
            var region = Region.Asia;

            var country = Country.Create(name, countryCode, region);

            country.ShouldNotBeNull();
            country.Name.ShouldBe(name);
            country.CountryCode.ShouldBe(countryCode);
            country.Region.ShouldBe(region);
            country.Id.ShouldNotBe(Guid.Empty);
        }

        [Fact]
        public void given_null_country_code_should_return_country_instance()
        {
            var name = "Brazil";
            string? countryCode = null;
            var region = Region.SouthAmerica;

            var country = Country.Create(name, countryCode, region);

            country.ShouldNotBeNull();
            country.Name.ShouldBe(name);
            country.CountryCode.ShouldBeNull();
            country.Region.ShouldBe(region);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void given_invalid_name_should_throw_argument_null_exception(string invalidName)
        {
            var countryCode = "AU";
            var region = Region.Australia;

            var exception = Record.Exception(() => Country.Create(invalidName, countryCode, region));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<ArgumentNullException>();
            ((ArgumentNullException)exception).ParamName.ShouldBe("name");
        }

        [Fact]
        public void given_invalid_region_should_throw_argument_out_of_range_exception()
        {
            var name = "Test Country";
            var countryCode = "TC";
            var invalidRegion = (Region)(-1);

            var exception = Record.Exception(() => Country.Create(name, countryCode, invalidRegion));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<ArgumentOutOfRangeException>();
            ((ArgumentOutOfRangeException)exception).ParamName.ShouldBe("region");
        }
    }
}
