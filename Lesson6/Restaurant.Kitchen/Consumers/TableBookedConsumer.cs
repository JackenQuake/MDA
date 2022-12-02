using MassTransit;
using Restaurant.Messages;
using Restaurant.Messages.InMemoryDb;

namespace Restaurant.Kitchen.Consumers
{
	internal class KitchenBookingRequestedConsumer : IConsumer<IBookingRequest>
	{
		private readonly Manager _manager;
		private readonly IProcessedMessageRepository _repository;

		private static readonly Random random = new Random();

		public KitchenBookingRequestedConsumer(Manager manager, IProcessedMessageRepository repository)
		{
			_manager = manager;
			_repository = repository;
		}

		public async Task Consume(ConsumeContext<IBookingRequest> context)
		{
			var transaction = new DatabaseTransaction();
			try
			{
				if (!_repository.TryAddMessage(context.MessageId.ToString()))
					throw new Exception("Дублирующее сообщение "+context.MessageId.ToString());

				Console.WriteLine($"[OrderId: {context.Message.OrderId} CreationDate: {context.Message.CreationDate}]");

				if (context.Message.PreOrder == Dish.Lasagna)
					throw new Exception("Простите, сегодня лазанья недоступна.");

				var rnd = random.Next(1000, 10000);
				Console.WriteLine($"Kitchen check will take {rnd} at {DateTime.Now}");
				await Task.Delay(rnd);

				if (_manager.CheckKitchenReady(context.Message.OrderId, context.Message.PreOrder))
					await context.Publish<IKitchenReady>(new KitchenReady(context.Message.OrderId, true));

				transaction.Commit();
			} catch (Exception e)
			{
				Console.WriteLine("Ошибка: "+e.Message);
				transaction.Rollback();
			}
		}
	}
}