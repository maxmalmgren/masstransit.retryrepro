using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MassTransit.RetryRepro;

internal class ScenarioTestRunner : IAsyncDisposable
{
    private TestClientServer? _server;

    internal async Task<ScenarioTestRunner> Build()
    {

        _server = new TestClientServer(
            services =>
            {
            });

        await Task.WhenAll(_server!.Services().GetServices<IHostedService>().Select(x => x.StartAsync(new CancellationTokenSource().Token)));

        return this;
    }

    public IServiceProvider Provider => _server!.Services();

    public async ValueTask DisposeAsync()
    {
        await _server!.DisposeAsync();
    }
}