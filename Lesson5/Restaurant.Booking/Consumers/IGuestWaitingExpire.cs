namespace Restaurant.Booking.Consumers
{
	public interface IGuestWaitingExpire
	{
		public Guid OrderId { get; }
	}

	public class GuestWaitingExpire : IGuestWaitingExpire
	{
		private readonly RestaurantBooking _instance;

		public GuestWaitingExpire(RestaurantBooking instance)
		{
			_instance = instance;
		}

		public Guid OrderId => _instance.OrderId;
	}
}