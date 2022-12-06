using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Restaurant.Kitchen.Consumers;
using Restaurant.Messages;
using Restaurant.Messages.InMemoryDb;

namespace Restaurant.Tests;

[TestFixture]
public class KitchenBookingRequestConsumerTests
{
	private ServiceProvider _provider;
	private ITestHarness _harness;

	[OneTimeSetUp]
	public async Task Init()
	{
		_provider = new ServiceCollection()
			.AddMassTransitTestHarness(cfg =>
			{
				cfg.AddConsumer<KitchenBookingRequestedConsumer>();
			})
			.AddLogging()
			.AddTransient<Kitchen.Manager>()
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
		await _harness.Bus.Publish((IBookingRequest)new BookingRequest(Guid.NewGuid(), Guid.NewGuid(), null, DateTime.Now));
		Assert.That(await _harness.Consumed.Any<IBookingRequest>());
	}

	[Test]
	public async Task ResponseSentTest()
	{
		var consumer = _harness.GetConsumerHarness<KitchenBookingRequestedConsumer>();
		var orderId = NewId.NextGuid();

		await _harness.Bus.Publish((IBookingRequest) new BookingRequest(orderId, orderId, null, DateTime.Now));
		Assert.That(consumer.Consumed.Select<IBookingRequest>().Any(x => x.Context.Message.OrderId == orderId), Is.True);
		Assert.That(_harness.Published.Select<IKitchenReady>().Any(x => x.Context.Message.OrderId == orderId), Is.True);
	}
}
