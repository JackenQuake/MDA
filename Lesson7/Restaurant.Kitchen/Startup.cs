using GreenPipes;
using MassTransit;
using MassTransit.Audit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Restaurant.Kitchen.Consumers;
using Prometheus;

namespace Restaurant.Kitchen
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

				x.AddConsumer<KitchenBookingRequestedConsumer>(
					configurator =>
					{
						configurator.UseScheduledRedelivery(r => { r.Intervals(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(20), TimeSpan.FromSeconds(30)); });
						configurator.UseMessageRetry(r => { r.Incremental(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2)); });
					}
				).Endpoint(e => { e.Temporary = true; });
				x.AddConsumer<KitchenBookingRequestFaultConsumer>().Endpoint(e => { e.Temporary = true; });
				x.AddDelayedMessageScheduler();

				x.UsingRabbitMq((context, cfg) =>
				{
					cfg.UsePrometheusMetrics(serviceName: "booking_service");

					cfg.UseDelayedMessageScheduler();
					cfg.UseInMemoryOutbox();
					cfg.ConfigureEndpoints(context);

					cfg.ConnectSendAuditObservers(auditStore);
					cfg.ConnectConsumeAuditObserver(auditStore);
				});
			});
			services.AddSingleton<Manager>();
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

