using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SiiMobility.API.ApplicationService.Extensions
{
	public static class WebHostExtensions
	{
		public static IWebHost MigrateDbContext<TContext>(this IWebHost webHost,
			Action<TContext, IServiceProvider> seeder) where TContext : DbContext
		{
			using (var scope = webHost.Services.CreateScope())
			{
				var services = scope.ServiceProvider;
				var context = services.GetService<TContext>();
				var factory = services.GetRequiredService<ILoggerFactory>();
				var logger = factory.CreateLogger("Migration");

				try
				{
					logger.LogInformation($"Migrating database associated with context {typeof(TContext).Name}");
					context.Database.Migrate();
					seeder(context, services);
					logger.LogInformation($"Migrated database associated with context {typeof(TContext).Name}");
				}
				catch (Exception ex)
				{
					logger.LogError(ex,
						$"An error occurred while migrating the database used on context {typeof(TContext).Name}");
				}
			}

			return webHost;
		}
	}
}