using Messaging;
using Microsoft.Extensions.Hosting;
using System.Text;

namespace Restaurant.Notification
{
    public class Worker : BackgroundService
    {
        private readonly Consumer _consumer;

        public Worker()
        {
            string [] keys = { "sms.*", "email.*" };
            // важно чтобы имя очереди совпадало
            _consumer = new Consumer("localhost", keys); 
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _consumer.Receive((sender, args) => 
            {
                var body = args.Body.ToArray();
                var message = Encoding.UTF8.GetString(body); // декодируем
                Console.WriteLine(" [x] Received {0}", message);
            });
        }
    }
}
