using MassTransit;
using Restaurant.Messages;

namespace Restaurant.Kitchen
{
	public class Manager
	{
		private readonly IBus _bus;

		public Manager(IBus bus)
		{
			_bus = bus;
		}

		public bool CheckKitchenReady(Guid orderId, Dish? dish)
		{
			return true;
		}
	}
}