namespace Restaurant.Booking
{
	public class Table
	{
		private static readonly Random rnd = new Random();

		public TableState State { get; private set; }

		public int SeatsCount { get; }

		public int Id { get; }
		
		public Guid OrderId { get; private set; }

		public Table(int id)
		{
			Id = id; // в учебном примере просто присвоим id при вызове
			State = TableState.Free; // новый стол всегда свободен
			SeatsCount = rnd.Next(2, 5); // пусть количество мест за каждым столом будет случайным, от 2х до 5ти
		}

		public bool SetState(TableState state)
		{
			if (state == State)
				return false;
			State = state;
			return true;
		}

		public bool BookTable(Guid orderId)
		{
			if (!SetState(TableState.Booked))
				return false;
			OrderId = orderId;
			return true;
		}
	}
}
