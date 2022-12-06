using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Restaurant.Messages.InMemoryDb
{
	public class ProcessedMessageRepository
	{
		private readonly ConcurrentDictionary<string, DateTime> repo = new ();
		private readonly ILogger _logger;

		public ProcessedMessageRepository(ILogger logger)
		{
			_logger = logger;
			Cleanup();  // Запускаем службу очистки
		}

		public bool TryAddMessage(string MessageId)
		{
			return repo.TryAdd(MessageId, DateTime.Now);
		}

		public async void Cleanup()
		{
			while (true)
			{
				await Task.Delay(1000);
				// Каждую секунду просматриваем хранилище
				foreach (var message in repo)
					if ((DateTime.Now - message.Value).TotalSeconds > 30)  // Если с создания объекта до текущего момента прошло больше 30 секунд
						if (repo.TryRemove(message))  // - пытаемся удалить и сообщаем
							_logger.LogDebug("Сообщение {MessageId} устарело и удалено из хранилища", message.Key);
			}
		}
	}
}