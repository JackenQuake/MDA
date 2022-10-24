using Messaging;

namespace Restaurant.Booking
{
	public class ClientNotifier
	{
		private readonly Producer _producer = new("localhost");

		private readonly Random rnd = new Random();

		private string GetRandomKey(int KeyLength)
		{
			char[] key = new char[KeyLength];
			for (int i = 0; i < KeyLength; i++)
				key[i] = (char) rnd.Next(65, 91);
			return new string(key);
		}

		public async Task SendNotificationAsync(string message)
		{
			await Task.Delay(1000 * 3); // имитируем задержку создания сообщения
			//Console.WriteLine("УВЕДОМЛЕНИЕ: " + message);
			_producer.Send("УВЕДОМЛЕНИЕ: " + message, "sms."+ GetRandomKey(10));
		}
	}
}
