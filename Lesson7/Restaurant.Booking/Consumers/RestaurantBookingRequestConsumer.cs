using MassTransit;
using Microsoft.Extensions.Logging;
using Restaurant.Messages;
using Restaurant.Messages.InMemoryDb;

namespace Restaurant.Booking.Consumers
{
	public class RestaurantBookingRequestConsumer : IConsumer<IBookingRequest>
	{
		private readonly Restaurant _restaurant;
		private readonly ProcessedMessageRepository _repository;
		private readonly ILogger _logger;

		public RestaurantBookingRequestConsumer(Restaurant restaurant,
			ILogger<RestaurantBookingRequestConsumer> logger)
		{
			_restaurant = restaurant;
			_repository = new(logger);
			_logger = logger;
		}

		public async Task Consume(ConsumeContext<IBookingRequest> context)
		{
			var transaction = new DatabaseTransaction(_logger);
			try
			{
				if (!_repository.TryAddMessage(context.MessageId.ToString()))
					throw new Exception("Дублирующее сообщение "+context.MessageId.ToString());
				var result = await _restaurant.BookFreeTableAsync(1, context.Message.OrderId);
				_logger.LogInformation("Booking attempt [OrderId: {OrderId}, result: {BookingResult}]",
					context.Message.OrderId, result);
				await context.Publish<ITableBooked>(new TableBooked(context.Message.OrderId, result));
				transaction.Commit();
			} catch (Exception e)
			{
				_logger.LogWarning("Ошибка: {ErrorMessage}", e.Message);
				transaction.Rollback();
			}
		}
	}
}