using MassTransit;
using Restaurant.Messages;
using Restaurant.Messages.InMemoryDb;

namespace Restaurant.Kitchen.Consumers
{
	public class KitchenBookingRequestFaultConsumer : IConsumer<Fault<IBookingRequest>>
	{
		private readonly IProcessedMessageRepository _repository;

		public KitchenBookingRequestFaultConsumer(IProcessedMessageRepository repository)
		{
			_repository=repository;
		}

		public Task Consume(ConsumeContext<Fault<IBookingRequest>> context)
		{
			var transaction = new DatabaseTransaction();
			try
			{
				if (!_repository.TryAddMessage(context.MessageId.ToString()))
					throw new Exception("Дублирующее сообщение "+context.MessageId.ToString());
				//Console.WriteLine($"[OrderId {context.Message.Message.OrderId}] Отмена на кухне");
				transaction.Commit();
			} catch (Exception e)
			{
				Console.WriteLine("Ошибка: "+e.Message);
				transaction.Rollback();
			}
			return Task.CompletedTask;
		}
	}
}