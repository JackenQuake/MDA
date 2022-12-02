namespace Restaurant.Messages.InMemoryDb
{
	public interface IProcessedMessageRepository
	{
		public bool TryAddMessage(string MessageId);
	}
}