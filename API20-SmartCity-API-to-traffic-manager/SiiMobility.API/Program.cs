using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using SiiMobility.API.ApplicationService.Extensions;
using NLog.Web;

namespace SiiMobility.API
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			await CreateWebHostBuilder(args)
				.Build()
				//.MigrateDbContext<ApplicationDbContext>((context, services) =>
				//{
				//    var env = services.GetRequiredService<IHostingEnvironment>();
				//    var logger = services.GetService<ILogger<ApplicationDbContext>>();

				//    new ApplicationContextSeed()
				//        .SeedAsync(context, env, logger)
				//        .Wait();
				//})
				.RunWithTasksAsync();
		}

		private static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.UseStartup<Startup>()
				.UseNLog();

		//.UseIISIntegration()
		//.CaptureStartupErrors(true);
	}
}
