using MassTransit;
using Microsoft.Extensions.Hosting;
using Restaurant.Messages;
using System.Diagnostics;

namespace Restaurant.Booking
{
	public class Worker : BackgroundService
	{
		private readonly IBus _bus;
		private readonly Restaurant _restaurant;

		public Worker(IBus bus, Restaurant restaurant)
		{
			_bus = bus;
			_restaurant = restaurant;
		}

		static int GetNumTable()
		{
			Console.Write("Пожалуйста, укажите номер столика: ");
			while (true)
			{
				if (int.TryParse(Console.ReadLine(), out var id))
					return id;
				Console.Write("Пожалуйста, введите корректный номер столика: ");
			}
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			Console.OutputEncoding = System.Text.Encoding.UTF8;
			while (!stoppingToken.IsCancellationRequested)
			{
				Console.WriteLine("Привет! Желаете ли вы..." +
					"\n1 - забронировать столик (с асинхронным уведомлением)" +
					"\n2 - снять бронь (с асинхронным уведомлением)" +
					"\n3 - занять ранее забронированный столик");  // приглашаем ко вводу
				if (!int.TryParse(Console.ReadLine(), out var choice) && choice is not (1 or 2 or 3))
				{
					Console.WriteLine("Введите, пожалуйста, 1, 2, 3"); // всегда нужно защититься от невалидного ввода
					continue;
				}
				var stopWatch = new Stopwatch();
				stopWatch.Start(); // замерим потраченное нами время на бронирование, ведь наше время - самое дорогое, что у нас есть
				switch (choice)
				{
					case 1:
						Console.WriteLine("Добрый день! Подождите секунду, я подберу столик и подтвержу вашу бронь, вам придет уведомление");
					    await _bus.Publish((IBookingRequest)new BookingRequest(NewId.NextGuid(), NewId.NextGuid(), null, DateTime.Now), stoppingToken);
						break;
					case 2:
						var ReleaseOrderId = _restaurant.FindOrderId(GetNumTable());
						if (ReleaseOrderId is null)
						{
							Console.WriteLine("Прошу прощения, ваш заказ не найден.");
						} else
						{
							Console.WriteLine("Добрый день! Подождите секунду, я сниму вашу бронь, вам придет уведомление");
							Console.WriteLine(ReleaseOrderId);
							await _bus.Publish((IBookingCancellationRequest)new BookingCancellationRequest(ReleaseOrderId.Value), stoppingToken);
						}
						break;
					case 3:
						var ClaimOrderId = _restaurant.FindOrderId(GetNumTable());
						if (ClaimOrderId is null)
						{
							Console.WriteLine("Прошу прощения, ваш заказ не найден.");
						} else
						{
							Console.WriteLine("Добрый день! Подождите секунду, я сниму вашу бронь, вам придет уведомление");
							Console.WriteLine(ClaimOrderId);
							await _bus.Publish((IGuestArrived)new GuestArrived(ClaimOrderId.Value), stoppingToken);
						}
						break;
				}
				Console.WriteLine("Спасибо за ваше обращение!"); // клиента всегда нужно порадовать благодарностью
				stopWatch.Stop();
				var ts = stopWatch.Elapsed;
				Console.WriteLine($"{ts.Seconds:00}:{ts.Milliseconds:00}"); // выведем потраченное нами время
			}
		}
	}
}