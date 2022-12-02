using System.Collections.Concurrent;

namespace Restaurant.Messages.InMemoryDb
{
	public class InMemoryRepository<T> : IInMemoryRepository<T> where T : class
	{
		private readonly ConcurrentBag<T> _repo = new ();

		public void AddOrUpdate(T entity)
		{
			_repo.Add(entity);
		}

		public IEnumerable<T> Get()
		{
			return _repo;
		}
	}
}