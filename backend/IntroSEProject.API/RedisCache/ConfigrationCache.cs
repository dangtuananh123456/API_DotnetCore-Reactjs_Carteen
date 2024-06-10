using Microsoft.Extensions.Configuration;
using System.IO;

namespace Layer.Presentation.RedisCache
{
	static class ConfigrationCache
	{
		public static IConfiguration AppSetting
		{
			get;
		}
		static ConfigrationCache()
		{
			AppSetting = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json").Build();
		}
	}
}
