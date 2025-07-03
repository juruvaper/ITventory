using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ITventory.Infrastructure.EF.Contexts;
using Testcontainers.PostgreSql;
using Xunit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace ITventory.Tests.Integration.Infrastructure
{
    public class IntegrationTestWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>, IAsyncLifetime
        where TProgram : class
    {
        private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder()
            .WithImage("postgres:15-alpine")
            .WithDatabase("itventory_test")
            .WithUsername("testuser")
            .WithPassword("testpass")
            .Build();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove existing DbContext registrations
                services.RemoveAll(typeof(DbContextOptions<WriteDbContext>));
                services.RemoveAll(typeof(DbContextOptions<ReadDbContext>));

                // Add test database contexts
                services.AddDbContext<WriteDbContext>(options =>
                    options.UseNpgsql(_postgreSqlContainer.GetConnectionString()));

                services.AddDbContext<ReadDbContext>(options =>
                    options.UseNpgsql(_postgreSqlContainer.GetConnectionString()));

                // Remove the SystemTextJsonOutputFormatter and related services to avoid PipeWriter issue
                services.PostConfigure<Microsoft.AspNetCore.Mvc.MvcOptions>(options =>
                {
                    // Remove System.Text.Json formatters
                    var systemTextJsonFormatter = options.OutputFormatters
                        .OfType<Microsoft.AspNetCore.Mvc.Formatters.SystemTextJsonOutputFormatter>()
                        .FirstOrDefault();
                    if (systemTextJsonFormatter != null)
                    {
                        options.OutputFormatters.Remove(systemTextJsonFormatter);
                    }

                    // Add a simple string formatter for testing
                    options.OutputFormatters.Insert(0, new TestJsonOutputFormatter());
                });

                // Add a test authentication scheme to bypass authentication issues
                services.AddAuthentication("Test")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>("Test", options => { });
                
                services.AddAuthorization(options =>
                {
                    options.DefaultPolicy = new AuthorizationPolicyBuilder("Test")
                        .RequireAssertion(_ => true)
                        .Build();
                });
            });

            builder.UseEnvironment("Testing");
        }

        public async Task InitializeAsync()
        {
            await _postgreSqlContainer.StartAsync();
            
            // Apply migrations
            using var scope = Services.CreateScope();
            var writeDbContext = scope.ServiceProvider.GetRequiredService<WriteDbContext>();
            await writeDbContext.Database.MigrateAsync();
        }

        public new async Task DisposeAsync()
        {
            await _postgreSqlContainer.StopAsync();
            await base.DisposeAsync();
        }
    }

    public class TestAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public TestAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger, UrlEncoder encoder) : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "TestUser"),
                new Claim(ClaimTypes.NameIdentifier, "123"),
            };

            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "Test");

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }

    public class TestJsonOutputFormatter : TextOutputFormatter
    {
        public TestJsonOutputFormatter()
        {
            SupportedMediaTypes.Add("application/json");
            SupportedEncodings.Add(Encoding.UTF8);
        }

        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            var httpContext = context.HttpContext;
            
            string json;
            if (context.Object == null)
            {
                json = "null";
            }
            else
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = false
                };
                options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
                
                json = JsonSerializer.Serialize(context.Object, context.Object.GetType(), options);
            }

            await httpContext.Response.WriteAsync(json, selectedEncoding);
        }
    }
}
