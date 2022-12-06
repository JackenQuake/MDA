namespace Restaurant.Booking
{
	public class Restaurant
	{
		private readonly List<Table> _tables = new ();

		public Restaurant()
		{
			for (ushort i = 1; i <= 10; i++)
			{
				_tables.Add(new Table(i));
			}
		}

		public async Task<bool> BookFreeTableAsync(int countOfPersons, Guid OrderId)
		{
			var table = _tables.FirstOrDefault(t => t.SeatsCount > countOfPersons && t.State == TableState.Free);
			if (table is null)
				return false;
			await Task.Delay(100 * 5); // у нас нерасторопные менеджеры, 5 секунд они находятся в поисках стола
			return table.BookTable(OrderId);
		}

		public Guid? FindOrderId(int id)
		{
			var table = _tables.FirstOrDefault(t => t.Id == id);
			return table?.OrderId;
		}

		public async Task<bool?> ReleaseTableAsync(Guid orderId)
		{
			var table = _tables.FirstOrDefault(t => t.OrderId == orderId);
			await Task.Delay(1000 * 5);
			return table?.SetState(TableState.Free);
		}
	}
}
