using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SiiMobility.Infrastructure.Contracts;

namespace SiiMobility.Infrastructure
{
	public sealed class ApplicationDbContext : DbContext, IApplicationDbContext
	{
		private readonly ILoggerFactory _factory;

		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ILoggerFactory factory) :
			base(options)
		{
			_factory = factory;
			Database.SetCommandTimeout(5000);
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);
			optionsBuilder.UseLoggerFactory(_factory);
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			// DATA ENTITY
		}

	}
}