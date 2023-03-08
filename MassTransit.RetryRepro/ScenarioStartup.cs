using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace MassTransit.RetryRepro;

public class ScenarioStartup
{
    public virtual void ConfigureServices(IServiceCollection services)
    {
        services.AddMassTransit(configurator =>
        {
            configurator.AddConsumer<TestConsumer>().Endpoint(x => x.Name = "TestConsumer");

            configurator.UsingInMemory((context, cfg) =>
            {
                cfg.UseRetry(c =>
                {
                    c.Intervals(TimeSpan.Zero, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2));
                });

                cfg.UseMessageScope(context);
                cfg.UseInMemoryOutbox();
                cfg.ReceiveEndpoint("TestConsumer", e => e.ConfigureConsumer<TestConsumer>(context));
            });
        });
    }

    public void Configure(IApplicationBuilder app)
    { }
}