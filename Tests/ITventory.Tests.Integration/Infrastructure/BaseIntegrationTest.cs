using ITventory.Infrastructure.EF.Contexts;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ITventory.Tests.Integration.Infrastructure
{
    public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebApplicationFactory<Program>>, IAsyncLifetime
    {
        protected readonly IntegrationTestWebApplicationFactory<Program> Factory;
        protected readonly IServiceScope Scope;
        internal readonly WriteDbContext WriteDbContext;
        internal readonly ReadDbContext ReadDbContext;

        protected BaseIntegrationTest(IntegrationTestWebApplicationFactory<Program> factory)
        {
            Factory = factory;
            Scope = factory.Services.CreateScope();
            WriteDbContext = Scope.ServiceProvider.GetRequiredService<WriteDbContext>();
            ReadDbContext = Scope.ServiceProvider.GetRequiredService<ReadDbContext>();
        }

        public virtual Task InitializeAsync() => Task.CompletedTask;

        public virtual async Task DisposeAsync()
        {
            // Clean up the database after each test
            await WriteDbContext.Database.EnsureDeletedAsync();
            await WriteDbContext.Database.EnsureCreatedAsync();
            
            Scope.Dispose();
        }
    }
}
