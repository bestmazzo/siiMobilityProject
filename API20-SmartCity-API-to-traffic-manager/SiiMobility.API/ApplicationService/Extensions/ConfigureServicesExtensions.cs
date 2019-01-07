using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using CommitSoftware.NetCore.Framework.Core.Contracts;
using CommitSoftware.NetCore.Framework.Core.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using SiiMobility.API.Middleware;
using SiiMobility.Infrastructure;

namespace SiiMobility.API.ApplicationService.Extensions
{
	public static class ConfigureServicesExtensions
	{
		public static IServiceCollection AddStartupTask<TStartupTask>(this IServiceCollection services)
			where TStartupTask : class, IStartupTask
			=> services
				.AddTaskExecutingServer()
				.AddScoped<IStartupTask, TStartupTask>();

		public static void AddServices(this IServiceCollection services, IConfiguration configuration)
		{
			services
				.AddScoped(typeof(DbContext), x => x.GetService(typeof(ApplicationDbContext)))
				.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));

			services.AddSingleton(configuration);
			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			services.AddScoped<IFileManager, UploadManager>();
			services.AddSingleton<IFileProvider, PhysicalFileProvider>();
		}

		public static void AddDbContext(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddDbContext<ApplicationDbContext>(options =>
			{
				options.UseSqlServer(configuration.GetValue<string>("DbConnectionString"),
					builder => builder.UseRowNumberForPaging());
			});
		}

		//public static void AddApplicationIdentity(this IServiceCollection services, IConfiguration configuration)
		//{
		//    // ===== Add Identity ========
		//    services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
		//        {
		//            // Password settings
		//            options.Password.RequireDigit = true;
		//            options.Password.RequiredLength = 8;
		//            options.Password.RequireNonAlphanumeric = true;
		//            options.Password.RequireUppercase = true;
		//            options.Password.RequireLowercase = true;
		//            options.User.RequireUniqueEmail = true;
		//        })
		//        .AddEntityFrameworkStores<ApplicationDbContext>()
		//        .AddDefaultTokenProviders();
		//}

		public static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
		{
			JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // => remove default claims
			services
				.AddAuthentication(options =>
				{
					options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
					options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
					options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				})
				.AddJwtBearer(cfg =>
				{
					cfg.RequireHttpsMetadata = false;
					cfg.SaveToken = true;

					cfg.TokenValidationParameters = new TokenValidationParameters
					{
						ValidIssuer = configuration.GetValue<string>("publicKey"),
						ValidAudience = configuration.GetValue<string>("JwtIssuer"),
						IssuerSigningKey =
							new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("publicKey"))),
						ClockSkew = TimeSpan.Zero // remove delay of token when expire
					};
				});
		}

		private static IServiceCollection AddTaskExecutingServer(this IServiceCollection services)
		{
			var decoratorType = typeof(TaskExecutingServer);
			if (services.Any(service => service.ImplementationType == decoratorType))
			{
				// We've already decorated the IServer (make this call idempotent)
				return services;
			}

			var serverDescriptor = GetIServerDescriptor(services);
			if (serverDescriptor is null)
			{
				// We don't have an IServer!
				throw new Exception("Could not find any registered services for type IServer. IStartupTask requires using an IServer");
			}

			var decoratorDescriptor = CreateDecoratorDescriptor(serverDescriptor, decoratorType);

			var index = services.IndexOf(serverDescriptor);

			// To avoid reordering descriptors, in case a specific order is expected.
			services.Insert(index, decoratorDescriptor);

			services.Remove(serverDescriptor);

			return services;
		}

		private static ServiceDescriptor GetIServerDescriptor(IServiceCollection services)
		{
			Type server = typeof(IServer);
			return services.FirstOrDefault(service => service.ServiceType == server);
		}

		private static ServiceDescriptor CreateDecoratorDescriptor(this ServiceDescriptor innerDescriptor, Type decoratorType)
		{
			Func<IServiceProvider, object> factory = provider => provider.CreateInstance(decoratorType, provider.GetInstance(innerDescriptor));
			return ServiceDescriptor.Describe(innerDescriptor.ServiceType, factory, innerDescriptor.Lifetime);
		}

		private static object GetInstance(this IServiceProvider provider, ServiceDescriptor descriptor)
		{
			if (descriptor.ImplementationInstance != null)
			{
				return descriptor.ImplementationInstance;
			}

			if (descriptor.ImplementationType != null)
			{
				return provider.GetServiceOrCreateInstance(descriptor.ImplementationType);
			}

			return descriptor.ImplementationFactory(provider);
		}

		private static object GetServiceOrCreateInstance(this IServiceProvider provider, Type type)
		{
			return ActivatorUtilities.GetServiceOrCreateInstance(provider, type);
		}

		private static object CreateInstance(this IServiceProvider provider, Type type, params object[] arguments)
		{
			return ActivatorUtilities.CreateInstance(provider, type, arguments);
		}
	}
}