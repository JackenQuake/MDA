namespace Restaurant.Messages.InMemoryDb
{
	public interface IInMemoryRepository<T> where T : class
	{
		public void AddOrUpdate(T entity);

		public IEnumerable<T> Get();
	}
}