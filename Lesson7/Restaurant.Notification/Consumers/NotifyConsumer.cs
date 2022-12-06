using MassTransit;
using Microsoft.Extensions.Logging;
using Restaurant.Messages;
using Restaurant.Messages.InMemoryDb;

namespace Restaurant.Notification.Consumers
{
	public class NotifyConsumer : IConsumer<INotify>
	{
		private readonly Notifier _notifier;
		private readonly ProcessedMessageRepository _repository;
		private readonly ILogger _logger;

		public NotifyConsumer(Notifier notifier,
			ILogger<NotifyConsumer> logger)
		{
			_notifier = notifier;
			_repository = new(logger);
			_logger = logger;
		}

		public Task Consume(ConsumeContext<INotify> context)
		{
			var transaction = new DatabaseTransaction(_logger);
			try
			{
				if (!_repository.TryAddMessage(context.MessageId.ToString()))
					throw new Exception("Дублирующее сообщение "+context.MessageId.ToString());
				_notifier.Notify(context.Message.OrderId, context.Message.ClientId, context.Message.Message);
				transaction.Commit();
			} catch (Exception e)
			{
				_logger.LogWarning("Ошибка: {ErrorMessage}", e.Message);
				transaction.Rollback();
			}
			return context.ConsumeCompleted;
		}
	}
}
