using System.Timers;

namespace Restaurant.Booking
{
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
}
