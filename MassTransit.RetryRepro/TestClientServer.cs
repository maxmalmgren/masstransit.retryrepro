using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MassTransit.RetryRepro;

internal class TestClientServer : IAsyncDisposable
{
    private readonly IHost _host;

    public TestClientServer(Action<IServiceCollection>? configureServices = null)
    {
        _host = Build(configureServices);
        _host.Start();
    }

    private static IHost Build(Action<IServiceCollection>? configureServices)
    {
        return Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(builder =>
            {
                builder.UseTestServer()
                    .UseStartup<ScenarioStartup>()
                    .ConfigureServices(configureServices ?? ((services) => { }));
            })
            .Build();
    }

    public IServiceProvider Services()
    {
        return _host.Services;
    }

    public async ValueTask DisposeAsync()
    {
        await _host.StopAsync();
        _host.Dispose();
    }
}