using System.Text;
using RabbitMQ.Client;

namespace Messaging
{
	public class Producer
	{
		private readonly string _hostName;

		public Producer(string hostName)
		{
			_hostName = hostName;
		}

		public void Send(string message, string routingKey)
		{
			var factory = new ConnectionFactory() { HostName = _hostName };
			using var connection = factory.CreateConnection();
			using var channel = connection.CreateModel();
			channel.ExchangeDeclare("restaurant_notifications", "topic");

			var body = Encoding.UTF8.GetBytes(message); // формируем тело сообщения для отправки
			channel.BasicPublish(exchange: "restaurant_notifications",
				routingKey: routingKey,
				basicProperties: null,
				body: body); //отправляем сообщение
		}
	}
}