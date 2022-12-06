using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Restaurant.Booking.Consumers;
using Restaurant.Messages;
using Restaurant.Messages.InMemoryDb;

namespace Restaurant.Tests;

[TestFixture]
public class BookingCancellationConsumerTests
{
	private ServiceProvider _provider;
	private ITestHarness _harness;

	[OneTimeSetUp]
	public async Task Init()
	{
		_provider = new ServiceCollection()
			.AddMassTransitTestHarness(cfg =>
			{
				cfg.AddConsumer<BookingCancellationConsumer>();
			})
			.AddLogging()
			.AddTransient<Booking.Restaurant>()
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
		await _harness.Bus.Publish((IBookingCancellation) new BookingCancellation(Guid.NewGuid()));
		Assert.That(await _harness.Consumed.Any<IBookingCancellation>());
	}
}
