namespace Restaurant.Messages
{
	public interface IBookingCancellationRequest
	{
		public Guid OrderId { get; }
	}

	public class BookingCancellationRequest : IBookingCancellationRequest
	{
		public BookingCancellationRequest(Guid orderId)
		{
			OrderId = orderId;
		}

		public Guid OrderId { get; }
	}
}