using MassTransit;
using Restaurant.Messages;

namespace Restaurant.Booking.Consumers
{
	public class RestaurantBookingRequestConsumer : IConsumer<IBookingRequest>
	{
		private readonly Restaurant _restaurant;

		public RestaurantBookingRequestConsumer(Restaurant restaurant)
		{
			_restaurant = restaurant;
		}

		public async Task Consume(ConsumeContext<IBookingRequest> context)
		{
			var result = await _restaurant.BookFreeTableAsync(1, context.Message.OrderId);
			Console.WriteLine($"Booking attempt [OrderId: {context.Message.OrderId}, result: {result}]");
			await context.Publish<ITableBooked>(new TableBooked(context.Message.OrderId, result));
		}
	}
}