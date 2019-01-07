using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SiiMobility.API.Middleware;
using System.Threading;
using System.Threading.Tasks;

namespace SiiMobility.API.ApplicationService.Extensions
{
	public static class StartupTaskWebHostExtensions
	{
		public static async Task RunWithTasksAsync(this IWebHost webHost, CancellationToken cancellationToken = default)
		{
			try
			{
				// Load all tasks from DI
				var startupTasks = webHost.Services.GetServices<IStartupTask>();

				// Execute all the tasks
				foreach (var startupTask in startupTasks)
				{
					await startupTask.ExecuteAsync(cancellationToken);
				}
			}
			finally
			{
				// Start the tasks as normal
				await webHost.RunAsync(cancellationToken);
			}
		}
	}
}
