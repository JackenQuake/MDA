using MassTransit;
using Microsoft.Extensions.Logging;
using Restaurant.Messages;
using Restaurant.Messages.InMemoryDb;

namespace Restaurant.Kitchen.Consumers
{
	public class KitchenBookingRequestedConsumer : IConsumer<IBookingRequest>
	{
		private readonly Manager _manager;
		private readonly ProcessedMessageRepository _repository;
		private readonly ILogger _logger;

		private static readonly Random random = new();

		public KitchenBookingRequestedConsumer(Manager manager,
			ILogger<KitchenBookingRequestedConsumer> logger)
		{
			_manager = manager;
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

				_logger.LogInformation("[OrderId: {OrderId} CreationDate: {CreationDate}]", context.Message.OrderId, context.Message.CreationDate);

				if (context.Message.PreOrder == Dish.Lasagna)
					throw new Exception("Простите, сегодня лазанья недоступна.");

				var rnd = random.Next(1000, 10000);
				_logger.LogDebug("Kitchen check will take {KitchenCheckDuration} at {KitchenCheckTime}", rnd, DateTime.Now);
				await Task.Delay(rnd);

				if (_manager.CheckKitchenReady(context.Message.OrderId, context.Message.PreOrder))
					await context.Publish<IKitchenReady>(new KitchenReady(context.Message.OrderId, true));

				transaction.Commit();
			} catch (Exception e)
			{
				_logger.LogWarning("Ошибка: {ErrorMessage}", e.Message);
				transaction.Rollback();
			}
		}
	}
}