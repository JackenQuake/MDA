using MassTransit;
using Restaurant.Messages;

namespace Restaurant.Booking.Consumers
{
	public class BookingCancellationConsumer : IConsumer<IBookingCancellation>
	{
		private readonly Restaurant _restaurant;

		public BookingCancellationConsumer(Restaurant restaurant)
		{
			_restaurant = restaurant;
		}

		public async Task Consume(ConsumeContext<IBookingCancellation> context)
		{
			Console.WriteLine($"[OrderId {context.Message.OrderId}] Отмена в зале");
			await _restaurant.ReleaseTableAsync(context.Message.OrderId);
		}
	}
}