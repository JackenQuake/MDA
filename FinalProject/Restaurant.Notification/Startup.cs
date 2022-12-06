using MassTransit;
using MassTransit.Audit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Restaurant.Notification.Consumers;
using Prometheus;

namespace Restaurant.Notification
{
	public class Startup
	{
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers();

			services.AddMassTransit(x =>
			{
				services.AddSingleton<IMessageAuditStore, AuditStore>();

				var serviceProvider = services.BuildServiceProvider();
				var auditStore = serviceProvider.GetService<IMessageAuditStore>();

				x.AddConsumer<NotifyConsumer>().Endpoint(e => { e.Temporary = true; });
				x.UsingRabbitMq((context, cfg) =>
				{
					cfg.UsePrometheusMetrics(serviceName: "booking_service");

					cfg.ConfigureEndpoints(context);

					cfg.ConnectSendAuditObservers(auditStore);
					cfg.ConnectConsumeAuditObserver(auditStore);
				});
			});
			services.AddSingleton<Notifier>();
			//services.AddMassTransitHostedService(true);
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			app.UseRouting();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapMetrics();
				endpoints.MapControllers();
			});
		}
	}
}

