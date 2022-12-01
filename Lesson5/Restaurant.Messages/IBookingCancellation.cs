namespace Restaurant.Messages
{
	public interface IBookingCancellation
	{
		public Guid OrderId { get; }
	}

	public class BookingCancellation : IBookingCancellation
	{
		public BookingCancellation(Guid orderId)
		{
			OrderId = orderId;
		}

		public Guid OrderId { get; }
	}
}