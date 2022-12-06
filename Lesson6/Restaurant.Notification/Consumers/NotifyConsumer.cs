using MassTransit;
using Restaurant.Messages;
using Restaurant.Messages.InMemoryDb;

namespace Restaurant.Notification.Consumers
{
	public class NotifyConsumer : IConsumer<INotify>
	{
		private readonly Notifier _notifier;
		private readonly IProcessedMessageRepository _repository;

		public NotifyConsumer(Notifier notifier, IProcessedMessageRepository repository)
		{
			_notifier = notifier;
			_repository = repository;
		}

		public Task Consume(ConsumeContext<INotify> context)
		{
			var transaction = new DatabaseTransaction();
			try
			{
				if (!_repository.TryAddMessage(context.MessageId.ToString()))
					throw new Exception("Дублирующее сообщение "+context.MessageId.ToString());
				_notifier.Notify(context.Message.OrderId, context.Message.ClientId, context.Message.Message);
				transaction.Commit();
			} catch (Exception e)
			{
				Console.WriteLine("Ошибка: "+e.Message);
				transaction.Rollback();
			}
			return context.ConsumeCompleted;
		}
	}
}
