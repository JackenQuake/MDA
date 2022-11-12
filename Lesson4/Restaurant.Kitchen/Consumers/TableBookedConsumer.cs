using MassTransit;
using Restaurant.Messages;

namespace Restaurant.Kitchen.Consumers
{
	internal class KitchenBookingRequestedConsumer : IConsumer<IBookingRequest>
	{
		private readonly Manager _manager;
		private static readonly Random random = new Random();

		public KitchenBookingRequestedConsumer(Manager manager)
		{
			_manager = manager;
		}

		public async Task Consume(ConsumeContext<IBookingRequest> context)
		{
			Console.WriteLine($"[OrderId: {context.Message.OrderId} CreationDate: {context.Message.CreationDate}]");
			var rnd = random.Next(1000, 10000);
			Console.WriteLine($"Kitchen check will take {rnd} at {DateTime.Now}");
			await Task.Delay(rnd);

			if (_manager.CheckKitchenReady(context.Message.OrderId, context.Message.PreOrder))
				await context.Publish<IKitchenReady>(new KitchenReady(context.Message.OrderId, true));
		}
	}
}