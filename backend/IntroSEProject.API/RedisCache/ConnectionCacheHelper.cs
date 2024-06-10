using StackExchange.Redis;

namespace Layer.Presentation.RedisCache
{
	public class ConnectionCacheHelper
	{
		private static Lazy<ConnectionMultiplexer> lazyConnection;

		static ConnectionCacheHelper()
		{
			lazyConnection = new Lazy<ConnectionMultiplexer>(() => {
				return ConnectionMultiplexer.Connect(ConfigrationCache.AppSetting["RedisURL"]);
			});
		}

		public static ConnectionMultiplexer Connection
		{
			get
			{
				return lazyConnection.Value;
			}
		}
	}
}
