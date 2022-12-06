using MassTransit;
using Microsoft.Extensions.Logging;
using Restaurant.Messages;
using Restaurant.Messages.InMemoryDb;

namespace Restaurant.Booking.Consumers
{
	public class BookingCancellationConsumer : IConsumer<IBookingCancellation>
	{
		private readonly Restaurant _restaurant;
		private readonly ProcessedMessageRepository _repository;
		private readonly ILogger _logger;

		public BookingCancellationConsumer(Restaurant restaurant,
			ILogger<BookingCancellationConsumer> logger)
		{
			_restaurant = restaurant;
			_repository = new(logger);
			_logger = logger;
		}

		public async Task Consume(ConsumeContext<IBookingCancellation> context)
		{
			var transaction = new DatabaseTransaction(_logger);
			try
			{
				if (!_repository.TryAddMessage(context.MessageId.ToString()))
					throw new Exception("Дублирующее сообщение "+context.MessageId.ToString());
				_logger.LogInformation("[OrderId {OrderId}] Отмена в зале", context.Message.OrderId);
				await _restaurant.ReleaseTableAsync(context.Message.OrderId);
				transaction.Commit();
			} catch (Exception e)
			{
				_logger.LogWarning("Ошибка: {ErrorMessage}", e.Message);
				transaction.Rollback();
			}
		}
	}
}