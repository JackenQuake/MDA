using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Restaurant.Notification.Consumers;
using Restaurant.Messages;
using Restaurant.Messages.InMemoryDb;

namespace Restaurant.Tests;

[TestFixture]
public class NotifyConsumerTests
{
	private ServiceProvider _provider;
	private ITestHarness _harness;

	[OneTimeSetUp]
	public async Task Init()
	{
		_provider = new ServiceCollection()
			.AddMassTransitTestHarness(cfg =>
			{
				cfg.AddConsumer<NotifyConsumer>();
			})
			.AddLogging()
			.AddTransient<Notification.Notifier>()
			.BuildServiceProvider(true);

		_harness = _provider.GetTestHarness();

		await _harness.Start();
	}

	[OneTimeTearDown]
	public async Task TearDown()
	{
		await _harness.OutputTimeline(TestContext.Out, options => options.Now().IncludeAddress());
		await _provider.DisposeAsync();
	}

	[Test]
	public async Task RequestConsumedTest()
	{
		await _harness.Bus.Publish((INotify)new Notify(Guid.NewGuid(), Guid.NewGuid(), null));
		Assert.That(await _harness.Consumed.Any<INotify>());
	}
}
