using MassTransit;
using Restaurant.Messages;
using Restaurant.Messages.InMemoryDb;

namespace Restaurant.Booking.Consumers
{
	public class BookingCancellationConsumer : IConsumer<IBookingCancellation>
	{
		private readonly Restaurant _restaurant;
		private readonly IProcessedMessageRepository _repository;

		public BookingCancellationConsumer(Restaurant restaurant, IProcessedMessageRepository repository)
		{
			_restaurant = restaurant;
			_repository = repository;
		}

		public async Task Consume(ConsumeContext<IBookingCancellation> context)
		{
			var transaction = new DatabaseTransaction();
			try
			{
				if (!_repository.TryAddMessage(context.MessageId.ToString()))
					throw new Exception("Дублирующее сообщение "+context.MessageId.ToString());
				Console.WriteLine($"[OrderId {context.Message.OrderId}] Отмена в зале");
				await _restaurant.ReleaseTableAsync(context.Message.OrderId);
				transaction.Commit();
			} catch (Exception e)
			{
				Console.WriteLine("Ошибка: "+e.Message);
				transaction.Rollback();
			}
		}
	}
}