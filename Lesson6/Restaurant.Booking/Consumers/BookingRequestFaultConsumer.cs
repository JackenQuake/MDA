using MassTransit;
using Restaurant.Messages;
using Restaurant.Messages.InMemoryDb;

namespace Restaurant.Booking.Consumers
{
	public class BookingRequestFaultConsumer : IConsumer<Fault<IBookingRequest>>
	{
		private readonly IProcessedMessageRepository _repository;

		public BookingRequestFaultConsumer(IProcessedMessageRepository repository)
		{
			_repository = repository;
		}

		public Task Consume(ConsumeContext<Fault<IBookingRequest>> context)
		{
			var transaction = new DatabaseTransaction();
			try
			{
				if (!_repository.TryAddMessage(context.MessageId.ToString()))
					throw new Exception("Дублирующее сообщение "+context.MessageId.ToString());
				Console.WriteLine($"[OrderId {context.Message.Message.OrderId}] Отмена в зале");
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