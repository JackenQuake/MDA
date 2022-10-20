using System.Diagnostics;
using System.Timers;

namespace Restaurant
{
	internal class Program
	{
		public static Random rnd = new Random();

		public enum State
		{
			/// <summary>
			/// Стол свободен
			/// </summary>
			Free = 0,

			/// <summary>
			/// Стол занят
			/// </summary>
			Booked = 1
		}

		public class Table
		{
			public State State { get; private set; }

			public int SeatsCount { get; }

			public int Id { get; }

			public Table(int id)
			{
				Id = id; // в учебном примере просто присвоим id при вызове
				State = State.Free; // новый стол всегда свободен
				SeatsCount = rnd.Next(2, 5); // пусть количество мест за каждым столом будет случайным, от 2х до 5ти
			}

			public bool SetState(State state)
			{
				if (state == State)
					return false;
				State = state;
				return true;
			}
		}

		public class ClientNotifier
		{
			public async Task SendNotificationAsync(string message)
			{
				await Task.Delay(1000 * 3); // имитируем задержку создания сообщения
				Console.WriteLine("УВЕДОМЛЕНИЕ: " + message);
			}
		}

		public class Restaurant
		{
			private readonly List<Table> _tables = new ();
			private readonly ClientNotifier notifier = new();
			private readonly System.Timers.Timer tableReleaseTimer;

			public Restaurant()
			{
				for (ushort i = 1; i <= 10; i++)
				{
					_tables.Add(new Table(i));
				}
				tableReleaseTimer = new System.Timers.Timer(1000 * 20);
				tableReleaseTimer.Elapsed += ReleaseAllTables;
				tableReleaseTimer.AutoReset = true;
				tableReleaseTimer.Enabled = true;
			}

			public void BookFreeTable(int countOfPersons)
			{
				Console.WriteLine("Добрый день! Подождите секунду, я подберу столик и подтвержу вашу бронь, оставайтесь на линии");
				var table = _tables.FirstOrDefault(t => t.SeatsCount > countOfPersons && t.State == State.Free);
				Thread.Sleep(1000 * 5); // у нас нерасторопные менеджеры, 5 секунд они находятся в поисках стола
				table?.SetState(State.Booked);

				Console.WriteLine(table is null
					? "К сожалению, сейчас все столики заняты"
					: $"Готово! Ваш столик номер {table.Id}");
			}

			public void BookFreeTableAsync(int countOfPersons)
			{
				Console.WriteLine("Добрый день! Подождите секунду, я подберу столик и подтвержу вашу бронь, вам придет уведомление");
				Task.Run(async () =>
				{
					var table = _tables.FirstOrDefault(t => t.SeatsCount > countOfPersons && t.State == State.Free);
					await Task.Delay(1000 * 5); // у нас нерасторопные менеджеры, 5 секунд они находятся в поисках стола
					table?.SetState(State.Booked);

					await notifier.SendNotificationAsync(table is null
						? "К сожалению, сейчас все столики заняты"
						: $"Готово! Ваш столик номер {table.Id}");
				});
			}


			public void ReleaseTable(int id)
			{
				Console.WriteLine("Добрый день! Подождите секунду, я сниму вашу бронь, оставайтесь на линии");
				var table = _tables.FirstOrDefault(t => t.Id == id);
				Thread.Sleep(1000 * 5);
				if (table is null)
				{
					Console.WriteLine("К сожалению, указанный столик не найден.");
					return;
				}
				if (table.State == State.Free)
				{
					Console.WriteLine("Указаный столик не был забронирован.");
					return;
				}
				table.SetState(State.Free);
				Console.WriteLine("Бронь снята.");
			}

			public void ReleaseTableAsync(int id)
			{
				Console.WriteLine("Добрый день! Подождите секунду, я сниму вашу бронь, вам придет уведомление");
				Task.Run(async () =>
				{
					var table = _tables.FirstOrDefault(t => t.Id == id);
					await Task.Delay(1000 * 5);
					if (table is null)
					{
						await notifier.SendNotificationAsync("К сожалению, указанный столик не найден.");
						return;
					}
					if (table.State == State.Free)
					{
						await notifier.SendNotificationAsync("Указаный столик не был забронирован.");
						return;
					}
					table.SetState(State.Free);
					await notifier.SendNotificationAsync("Бронь снята.");
				});
			}

			private void ReleaseAllTables(Object source, ElapsedEventArgs e)
			{
				Console.WriteLine("Запущено автоматическое освобождение забронированных столиков...");
				foreach (Table table in _tables)
					if (table.State == State.Booked)
					{
						table.SetState(State.Free);
						Console.WriteLine($"Бронь для столика {table.Id} снята.");
					}
			}
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
