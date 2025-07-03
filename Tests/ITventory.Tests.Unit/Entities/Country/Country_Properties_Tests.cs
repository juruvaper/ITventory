using System;
using ITventory.Domain;
using ITventory.Domain.Enums;
using Shouldly;
using Xunit;

namespace ITventory.Tests.Unit.Entities
{
    public class Country_Properties_Tests
    {
        [Fact]
        public void given_new_country_code_should_update_property()
        {
            var country = new Country("Test Country", "TC", Region.Europe);
            var newCountryCode = "NEW";

            country.CountryCode = newCountryCode;

            country.CountryCode.ShouldBe(newCountryCode);
        }

        [Fact]
        public void given_null_country_code_should_update_property()
        {
            var country = new Country("Test Country", "TC", Region.Europe);

            country.CountryCode = null;

            country.CountryCode.ShouldBeNull();
        }
    }
}
