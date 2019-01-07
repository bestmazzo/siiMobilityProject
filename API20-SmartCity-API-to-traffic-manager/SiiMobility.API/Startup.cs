using CommitSoftware.NetCore.Framework.Core.Providers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SiiMobility.API.ApplicationService.Extensions;
using SiiMobility.API.Middleware;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;

namespace SiiMobility.API
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.Configure<FormOptions>(options =>
			{
				options.ValueCountLimit = 10;
				options.MultipartBodyLengthLimit = 10000000;
			});
			// Adds services required for using options.
			services.AddOptions();
			// Add authentication
			var apiKey = Configuration["publicKey"];
			services.AddAuthentication(options =>
				{
					options.DefaultAuthenticateScheme = CustomAuthOptions.DefaultScheme;
					options.DefaultChallengeScheme = CustomAuthOptions.DefaultScheme;
				})
				// Call custom authentication extension method
				.AddCustomAuth(options =>
				{
					// Configure single or multiple passwords for authentication
					options.AuthKey = apiKey;
				});

			services
				.AddMvc(options =>
				{
					options.ModelBinderProviders.Insert(0, new EntityBinderProvider(new JsonSerializer(), services));
					// All endpoints need authentication
					options.Filters.Add(new AuthorizeFilter(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build()));
				})
				.AddJsonOptions(options =>
				{
					options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
					options.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
					options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
					options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
					options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
					options.SerializerSettings.MaxDepth = 10;
				})
				.AddControllersAsServices()
				.SetCompatibilityVersion(CompatibilityVersion.Latest);

			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new Info { Title = "API", Version = "v1" });
				c.IncludeXmlComments($@"{System.AppDomain.CurrentDomain.BaseDirectory}\SiiMobility.API.xml");
			});

			services.AddServices(Configuration);
			services.AddDbContext(Configuration);
			//services.AddHealthChecks();
			services.AddStartupTask<WarmupServicesStartupTask>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory factory)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseForwardedHeaders(new ForwardedHeadersOptions
			{
				ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
			});

			// Enable middleware to serve generated Swagger as a JSON endpoint.
			app.UseSwagger(c =>
			{
				c.PreSerializeFilters.Add((swagger, httpReq) => swagger.Host = httpReq.Host.Value);
			});

			app.UseStaticFiles();
			app.UseCors(builder =>
				builder
					.AllowAnyHeader()
					.AllowAnyMethod()
					.AllowAnyOrigin()
			);

			app.MapWhen(x => !x.Request.Path.Value.StartsWith("/api-docs"), builder =>
			{
				//app.UseWhiteListMiddleware(Configuration["WhiteList"]);
				app.UseHttpStatusCodeExceptionMiddleware();
				if (!env.IsDevelopment())
				{
					app.UseExceptionHandler();
				}
				app.UseAuthentication();
				builder.UseMvc();
			});

			app.UseSwaggerUI(c =>
			{
				c.RoutePrefix = "api-docs";
				c.SwaggerEndpoint("../swagger/v1/swagger.json", "My API V1");
				c.ShowExtensions();
			});
		}
	}
}
