using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Restaurant.Notification.Consumers;

namespace Restaurant.Notification
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Console.OutputEncoding = System.Text.Encoding.UTF8;
			CreateHostBuilder(args).Build().Run();
		}

		private static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureServices((hostContext, services) =>
				{
					services.AddMassTransit(x =>
					{
						x.AddConsumer<NotifyConsumer>().Endpoint(e => { e.Temporary = true; });
						x.UsingRabbitMq((context, cfg) =>
						{
							cfg.ConfigureEndpoints(context);
						});
					});
					services.AddSingleton<Notifier>();
					services.AddMassTransitHostedService(true);
				});
	}
}