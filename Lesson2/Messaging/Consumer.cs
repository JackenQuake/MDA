using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Messaging
{
	public class Consumer : IDisposable
	{
		private readonly string _hostName;
		private readonly string [] _keyTemplates;
		private readonly IConnection _connection;
		private readonly IModel _channel;

		public Consumer(string hostName, string [] keyTemplates)
		{
			_hostName = hostName;
			_keyTemplates = keyTemplates;
			var factory = new ConnectionFactory() { HostName = _hostName };
			_connection = factory.CreateConnection(); //создаем подключение
			_channel = _connection.CreateModel();
		}

		public void Receive(EventHandler<BasicDeliverEventArgs> receiveCallback)
		{
			_channel.ExchangeDeclare("restaurant_notifications", "topic"); // объявляем обменник
			var queueName = _channel.QueueDeclare().QueueName; // объявляем очередь
			foreach (var key in _keyTemplates)
				_channel.QueueBind(
					queue: queueName,
					exchange: "restaurant_notifications",
					routingKey: key); // биндим

			var consumer = new EventingBasicConsumer(_channel); // создаем consumer для канала
			consumer.Received += receiveCallback; // добавляем обработчик события приема сообщения

			_channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer); //стартуем!
		}

		public void Dispose()
		{
			_connection?.Dispose();
			_channel?.Dispose();
		}
	}
}