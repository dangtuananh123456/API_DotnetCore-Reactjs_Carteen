//using Microsoft.EntityFrameworkCore.Storage;
//using Org.BouncyCastle.Asn1.Crmf;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;

namespace Layer.Presentation.RedisCache
{
	public class RedisCacheService: ICacheService
	{
		private IDatabase database;
		public RedisCacheService()
		{
			ConfigureRedis();
		}

		private void ConfigureRedis()
		{
			//ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost:6379");
			database = ConnectionCacheHelper.Connection.GetDatabase();
		}

		public T GetData<T>(string key)
		{
			var value = database.StringGet(key);
			if(!string.IsNullOrEmpty(value))
			{
				var tem = JsonConvert.DeserializeObject<string>(value);
				return JsonConvert.DeserializeObject<T>(tem);
			}
			return default;
		}

		public bool SetData<T>(string key, T value, DateTimeOffset expiredTime)
		{
			TimeSpan _expiredTime = expiredTime.DateTime.Subtract(DateTime.Now); 
			var isSet = database.StringSet(key, JsonConvert.SerializeObject(value), _expiredTime);
			return isSet;
		}
		public object RemoveData(string key)
		{
			bool isKeyExist = database.KeyExists(key);
			if(isKeyExist)
			{
				return database.KeyDelete(key);
			}
				return false;
		}

	}
}
