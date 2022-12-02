using MassTransit;
using Restaurant.Messages;
using Restaurant.Messages.InMemoryDb;

namespace Restaurant.Booking.Consumers
{
	public class RestaurantBookingRequestConsumer : IConsumer<IBookingRequest>
	{
		private readonly Restaurant _restaurant;
		private readonly IProcessedMessageRepository _repository;

		public RestaurantBookingRequestConsumer(Restaurant restaurant, IProcessedMessageRepository repository)
		{
			_restaurant = restaurant;
			_repository = repository;
		}

		public async Task Consume(ConsumeContext<IBookingRequest> context)
		{
			var transaction = new DatabaseTransaction();
			try
			{
				if (!_repository.TryAddMessage(context.MessageId.ToString()))
					throw new Exception("Дублирующее сообщение "+context.MessageId.ToString());
				var result = await _restaurant.BookFreeTableAsync(1, context.Message.OrderId);
				Console.WriteLine($"Booking attempt [OrderId: {context.Message.OrderId}, result: {result}]");
				await context.Publish<ITableBooked>(new TableBooked(context.Message.OrderId, result));
				transaction.Commit();
			} catch (Exception e)
			{
				Console.WriteLine("Ошибка: "+e.Message);
				transaction.Rollback();
			}
		}
	}
}