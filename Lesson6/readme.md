This is JackenQuake repository.
Synchronized with GitHub.

### Tasks description for Lesson 6:

Lesson 6 task done.

#### Комментарии:

Задание к уроку 6 выполнено.

Для реализации второго типа хранилища сообщений было сделано ProcessedMessageRepository, хранящее в простом ConcurrentDictionary пары (ИД-сообщения, время-сообщения). При этом асинхронный метод Cleanup каждую секунду просматривает хранилище и удаляет все записи старше 30 секунд. Для имитации транзакций сделан класс DatabaseTransaction, имитирующий Commit и Rollback (просто сообщающий о том, что они случились).