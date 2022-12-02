namespace Restaurant.Messages.InMemoryDb
{
	public class DatabaseTransaction
	{
		private Guid guid;

		public DatabaseTransaction()
		{
			guid = Guid.NewGuid();
			Console.WriteLine($"Транзакция {guid} начата.");
		}

		public void Commit()
		{
			Console.WriteLine($"Транзакция {guid} успешно завершена.");
		}

		public void Rollback()
		{
			Console.WriteLine($"Транзакция {guid} отменена.");
		}
	}
}
