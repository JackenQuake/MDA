using System.Diagnostics;

namespace Restaurant.Booking
{
	class Program
	{
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

		static void Main(string[] args)
		{
			Console.OutputEncoding = System.Text.Encoding.UTF8;
			var rest = new Restaurant();
			while (true)
			{
				Console.WriteLine("Привет! Желаете ли вы..." +
					"\n1 - забронировать столик с уведомлением по смс (асинхронно)" +
					"\n2 - забронировать столик с ожиданием на линии (синхронно)" +
					"\n3 - снять бронь с уведомлением по смс (асинхронно)" +
					"\n4 - снять бронь с ожиданием на линии (синхронно)"); // приглашаем ко вводу
				if (!int.TryParse(Console.ReadLine(), out var choice) && choice is not (1 or 2 or 3 or 4))
				{
					Console.WriteLine("Введите, пожалуйста, 1 или 2"); //всегда нужно защититься от невалидного ввода
					continue;
				}

				var stopWatch = new Stopwatch();
				stopWatch.Start(); // замерим потраченное нами время на бронирование, ведь наше время - самое дорогое, что у нас есть
				switch (choice)
				{
					case 1:
						rest.BookFreeTableAsync(1); // забронируем с ответом по смс
						break;
					case 2:
						rest.BookFreeTable(1); // забронируем по звонку
						break;
					case 3:
						rest.ReleaseTableAsync(GetNumTable()); // снимаем бронь с ответом по смс
						break;
					case 4:
						rest.ReleaseTable(GetNumTable()); // снимаем бронь по звонку
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