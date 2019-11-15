using CodeFest.Api.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace CodeFest.Api
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }
		
		public void ConfigureServices(IServiceCollection services)
		{
			services
				.AddCustomServices(Configuration)
				.AddSwagger()				
				.AddCustomMVC();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseCors("CorsPolicy");

			app.UseSwagger()
				.UseSwaggerUI(c =>
				{
					c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{Program.AppName} v1.0.0");
				});

			app.UseHttpsRedirection();
			app.UseMvc();
		}
	}

	internal static class CustomExtensionMethods
	{
		public static IServiceCollection AddCustomMVC(this IServiceCollection services)
		{
			services.AddMvc(options =>
			{
			})
			.SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
			.AddControllersAsServices();

			services.AddCors(options =>
			{
				options.AddPolicy("CorsPolicy",
					builder => builder
					.SetIsOriginAllowed((host) => true)
					.WithMethods(
						"GET",
						"POST",
						"PUT",
						"DELETE",
						"OPTIONS")
					.AllowAnyHeader()
					.AllowCredentials());
			});

			return services;
		}

		public static IServiceCollection AddCustomServices(this IServiceCollection services, IConfiguration configuration)
		{			
			services.AddDbContext<CodeFestDBContext>(options => options.UseSqlServer(configuration.GetConnectionString("CodeFestDB")));
			// services.AddDbContext<CodeFestDBContext>(options => options.UseInMemoryDatabase("CodeFestDB"));


			// services.AddScoped<IUtilService, UtilServiceMock>();

			return services;
		}

		public static IServiceCollection AddSwagger(this IServiceCollection services)
		{
			services.AddSwaggerGen(options =>
			{
				options.DescribeAllEnumsAsStrings();
				options.EnableAnnotations();
				options.SwaggerDoc("v1", new Info
				{
					Version = "v1.0.0",
					Title = $"{Program.AppName}",
					Description = $"API to expose logic for {Program.AppName}",
					TermsOfService = ""
				});
				// var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
				// var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
				// options.IncludeXmlComments(xmlPath);
			});

			return services;
		}
	}
}
