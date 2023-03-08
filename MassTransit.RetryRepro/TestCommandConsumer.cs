using MassTransit;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.Mime;

namespace MassTransit.RetryRepro;

public class TestCommand
{
    public bool Throw { get; set; }
}



public class TestConsumer : IConsumer<TestCommand>
{
    public static ConcurrentQueue<(string, TestCommand)> HashCodes = new ConcurrentQueue<(string, TestCommand)>();

    public Task Consume(ConsumeContext<TestCommand> context)
    {        
        HashCodes.Enqueue((HashCode, context.Message));

        if(context.Message.Throw)
        {
            throw new Exception("Test");
        }
        
        return Task.CompletedTask;
    }

    public string HashCode => GetHashCode().ToString();
}
