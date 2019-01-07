using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace SiiMobility.Infrastructure.Seed
{
	public interface IContextSeed<in TContext>
	{
		Task SeedAsync(TContext context, IHostingEnvironment env, ILogger logger);
	}
}