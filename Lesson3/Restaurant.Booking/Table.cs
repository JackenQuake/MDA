namespace Restaurant.Booking
{
	public class Table
	{
		private static readonly Random rnd = new Random();

		public State State { get; private set; }

		public int SeatsCount { get; }

		public int Id { get; }

		public Table(int id)
		{
			Id = id; // в учебном примере просто присвоим id при вызове
			State = State.Free; // новый стол всегда свободен
			SeatsCount = rnd.Next(2, 5); // пусть количество мест за каждым столом будет случайным, от 2х до 5ти
		}

		public bool SetState(State state)
		{
			if (state == State)
				return false;
			State = state;
			return true;
		}
	}
}
