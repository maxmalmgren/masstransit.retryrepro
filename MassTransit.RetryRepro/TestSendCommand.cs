using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace MassTransit.RetryRepro
{
    public class TestSendCommand
    {
        private ScenarioTestRunner TestRunner { get; set; } = null!;

        [OneTimeSetUp]
        public async Task OneTimeSetup()
        {
            TestRunner = await new ScenarioTestRunner().Build();
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            await TestRunner.DisposeAsync();
        }

        [Test]
        public async Task Test_send_command()
        {
            var bus = TestRunner.Provider.GetService<IBus>();

            var hostAuthority = bus.Address.GetLeftPart(UriPartial.Authority);
            var uri = new Uri(hostAuthority + "/TestConsumer");

            var sendpoint = await bus.GetSendEndpoint(uri);

            await sendpoint.Send(new TestCommand { Throw = false });
            await sendpoint.Send(new TestCommand { Throw = false });
            await sendpoint.Send(new TestCommand { Throw = false });
            await sendpoint.Send(new TestCommand { Throw = true });            

            var start = DateTime.Now;

            await Task.Delay(15000);

            var hashcodes = new List<string>();

            while (TestConsumer.HashCodes.TryDequeue(out var data))
            {
                hashcodes.Add(data.Item1);
                Console.WriteLine($"Hashcode during execution: {data.Item1}, command throw was: {data.Item2.Throw}");
            }

            foreach(var hashcode in hashcodes)
            {
                Assert.AreEqual(1, hashcodes.Count(x => x == hashcode));
            }
        }
    }
}
