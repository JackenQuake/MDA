using MassTransit;
using Microsoft.Extensions.Logging;
using Restaurant.Messages;
using Restaurant.Messages.InMemoryDb;

namespace Restaurant.Booking.Consumers
{
	public class BookingRequestFaultConsumer : IConsumer<Fault<IBookingRequest>>
	{
		private readonly ProcessedMessageRepository _repository;
		private readonly ILogger _logger;

		public BookingRequestFaultConsumer(ILogger<BookingRequestFaultConsumer> logger)
		{
			_repository = new(logger);
			_logger = logger;
		}

		public Task Consume(ConsumeContext<Fault<IBookingRequest>> context)
		{
			var transaction = new DatabaseTransaction(_logger);
			try
			{
				if (!_repository.TryAddMessage(context.MessageId.ToString()))
					throw new Exception("Дублирующее сообщение "+context.MessageId.ToString());
				_logger.LogInformation("[OrderId {OrderId}] Отмена в зале", context.Message.Message.OrderId);
				transaction.Commit();
			} catch (Exception e)
			{
				_logger.LogWarning("Ошибка: {ErrorMessage}", e.Message);
				transaction.Rollback();
			}
			return Task.CompletedTask;
		}
	}
}