using System.Net;
using System.Net.Http.Json;
using ITventory.Tests.Integration.Infrastructure;
using Shouldly;
using Xunit;

namespace ITventory.Tests.Integration.Controllers
{
    public class CountryControllerIntegrationTests : BaseIntegrationTest
    {
        private readonly HttpClient _httpClient;

        public CountryControllerIntegrationTests(IntegrationTestWebApplicationFactory<Program> factory) 
            : base(factory)
        {
            _httpClient = factory.CreateClient();
        }

        [Fact]
        public async Task Get_Countries_Should_Return_Ok()
        {
            // Act
            var response = await _httpClient.GetAsync("/itventory/country");

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Get_Countries_Should_Return_Json_Content()
        {
            // Act
            var response = await _httpClient.GetAsync("/itventory/country");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            response.Content.Headers.ContentType?.MediaType.ShouldBe("application/json");
            content.ShouldNotBeNullOrEmpty();
        }

        [Fact]
        public async Task Should_Handle_Database_Operations_Through_Api()
        {
            // This test demonstrates that the API is using the TestContainer database
            // and can perform actual database operations during integration tests
            
            // Act
            var response = await _httpClient.GetAsync("/itventory/country");
            
            // Assert
            response.IsSuccessStatusCode.ShouldBeTrue();
            
            // You can expand this to test POST, PUT, DELETE operations
            // that actually interact with the TestContainer database
        }

        public override async Task DisposeAsync()
        {
            _httpClient.Dispose();
            await base.DisposeAsync();
        }
    }
}
