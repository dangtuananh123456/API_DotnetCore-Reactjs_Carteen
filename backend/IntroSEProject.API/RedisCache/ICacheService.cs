namespace Layer.Presentation.RedisCache
{
	public interface ICacheService
	{
		T GetData<T>(string key);
		bool SetData<T>(string key, T value, DateTimeOffset date);
		object RemoveData(string key);
	}
}
