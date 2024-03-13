using API;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Testcontainers.PostgreSql;

namespace IntegrationTests;

public class DatabaseFixture : IAsyncLifetime
{
    private IHost _host;
    private PostgreSqlContainer _postgreSqlContainer;

    public IDocumentStore Store;

    public async Task InitializeAsync()
    {
        _postgreSqlContainer = new PostgreSqlBuilder().Build();
        await _postgreSqlContainer.StartAsync();

        var builder = Host.CreateDefaultBuilder()
                          .ConfigureDefaults(Array.Empty<string>())
                          .UseEnvironment("Tests")
                          .ConfigureServices((context, services) =>
                                             {
                                                 Setup.Marten(context.HostingEnvironment,
                                                              services, 
                                                              _postgreSqlContainer.GetConnectionString());
                                             });

        _host = builder.Build();

        Store = _host.Services.GetRequiredService<IDocumentStore>();

        await Store.Advanced.ResetAllData();
    }

    public Task DisposeAsync()
    {
        _host.Dispose();
        return _postgreSqlContainer.DisposeAsync().AsTask();
    }
}