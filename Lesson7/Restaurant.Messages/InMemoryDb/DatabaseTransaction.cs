using Microsoft.Extensions.Logging;

namespace Restaurant.Messages.InMemoryDb
{
	public class DatabaseTransaction
	{
		private readonly Guid guid;
		private readonly ILogger _logger;

		public DatabaseTransaction(ILogger logger)
		{
			_logger = logger;
			guid = Guid.NewGuid();
			_logger.LogTrace("Транзакция {Transaction} начата.", guid);
		}

		public void Commit()
		{
			_logger.LogTrace("Транзакция {Transaction} успешно завершена.", guid);
		}

		public void Rollback()
		{
			_logger.LogWarning("Транзакция {Transaction} отменена.", guid);
		}
	}
}
