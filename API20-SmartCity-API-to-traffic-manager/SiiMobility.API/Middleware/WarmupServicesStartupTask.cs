using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace SiiMobility.API.Middleware
{
	public class WarmupServicesStartupTask : IStartupTask
	{
		private readonly IServiceCollection _services;
		private readonly IServiceProvider _provider;
		public WarmupServicesStartupTask(IServiceCollection services, IServiceProvider provider)
		{
			_services = services;
			_provider = provider;
		}

		public Task ExecuteAsync(CancellationToken cancellationToken)
		{
			using (var scope = _provider.CreateScope())
			{
				foreach (var service in GetServices(_services))
				{
					scope.ServiceProvider.GetServices(service);
				}
			}

			return Task.CompletedTask;
		}

		public Task ShutdownAsync(CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		static IEnumerable<Type> GetServices(IServiceCollection services)
		{
			return services
				.Where(descriptor => descriptor.ImplementationType != typeof(WarmupServicesStartupTask))
				.Where(descriptor => descriptor.ServiceType.ContainsGenericParameters == false)
				.Select(descriptor => descriptor.ServiceType)
				.Distinct();
		}
	}
}