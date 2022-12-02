namespace Restaurant.Messages.InMemoryDb
{
	public class DatabaseTransaction
	{
		private Guid guid;

		public DatabaseTransaction()
		{
			guid = Guid.NewGuid();
			Console.WriteLine($"���������� {guid} ������.");
		}

		public void Commit()
		{
			Console.WriteLine($"���������� {guid} ������� ���������.");
		}

		public void Rollback()
		{
			Console.WriteLine($"���������� {guid} ��������.");
		}
	}
}
