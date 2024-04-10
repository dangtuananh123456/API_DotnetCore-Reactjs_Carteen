using Microsoft.Extensions.Configuration;
using System.IO;

namespace IntroSEProject.API.RedisCache
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
