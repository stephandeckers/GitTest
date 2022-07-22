/**
 * @Name SwaggerVersions.cs
 * @Purpose Ondersteuning voor multiple versions in Swagger dropown
 * @Date 27 May 2022, 10:14:47
 * @Author S.Deckers
 * @url https://github.com/rgudkov-uss/aspnetcore-net6-odata-swagger-versioned
 * @Description startpage https://localhost:5001/index.html
 */

namespace Whoua.Core.Api.Supporting
{
	public static class RouteNames
	{
		public const string Route1 = "api/v1";
		public const string Route2 = "api/v2";
	}
}

namespace Whoua.Core.Api.Controllers.V1
{
	#region -- Using directives --
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.Extensions.Logging;
	using Microsoft.AspNetCore.OData.Query;
	using Microsoft.AspNetCore.OData.Routing.Attributes;
	using Microsoft.AspNetCore.OData.Routing.Controllers;

	using Whoua.Core.Api.Supporting;
	using Whoua.Core.Data.Entities.V1;
	using Swashbuckle.AspNetCore.Annotations;

	using d=System.Diagnostics.Debug;
	#endregion

	[ Produces				( "application/json")]
	[ ODataRouteComponent	( RouteNames.Route1)]
	public class WeatherForecastController : ODataController
	{
		private const string swaggerControllerDescription = "WeatherForecast V1";

		private static readonly string[] Summaries = new[]
		{
			"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
		};

		private readonly ILogger<WeatherForecastController> _logger;

		public WeatherForecastController(ILogger<WeatherForecastController> logger)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			_logger = logger;
		}

		[ HttpGet		( RouteNames.Route1 + "/{id:long}" )]
		[ EnableQuery	( )]
		[ SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "Get single forecast" )]
		public ActionResult<IQueryable<WeatherForecast>> GetSingle(int id)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			var item = Summaries[ id];
			return Ok(item);
		}

		[ HttpGet		( )]
		[ EnableQuery	( )]
		[ SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "Get multiple forecasts" )]
		public ActionResult<IEnumerable<WeatherForecast>> Get()
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			var items = Enumerable.Range(1, 5)
				.Select(index => new WeatherForecast
				{
					Date = DateTime.Now.AddDays(index),
					TemperatureC = 45,
					Summary = Summaries[ 0],
				});
			return Ok(items);
		}
	}
}

namespace Whoua.Core.Api.Controllers.V2
{
	#region -- Using directives --
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.Extensions.Logging;
	using Microsoft.AspNetCore.OData.Query;
	using Microsoft.AspNetCore.OData.Routing.Attributes;
	using Microsoft.AspNetCore.OData.Routing.Controllers;

	using Whoua.Core.Api.Supporting;
	using Whoua.Core.Data.Entities.V2;

	using d=System.Diagnostics.Debug;
	using Swashbuckle.AspNetCore.Annotations;
	#endregion

	[ Produces				( "application/json")]
	[ ODataRouteComponent	( RouteNames.Route2)]
	public class WeatherForecastController : ODataController
	{
		private const string swaggerControllerDescription = "WeatherForecast V2";

		private static readonly string[] Summaries = new[]
		{
			"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
		};

		private readonly ILogger<WeatherForecastController> _logger;

		public WeatherForecastController(ILogger<WeatherForecastController> logger)
		{
			_logger = logger;
		}

		/// <summary>
		/// Get weather forecast for N days
		/// </summary>
		/// <param name="days">Days to get forecast for</param>
		/// <returns></returns>
		[ HttpGet		( )]
		[ EnableQuery	( )]
		[ SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "Get single forecast" )]
		public ActionResult<IQueryable<WeatherForecast>> Get(int days = 5)
		{
			var items = Enumerable.Range(1, days)
				.Select(index => new WeatherForecast
				{
					Date = DateTime.Now.AddDays(index),
					TemperatureC = days,
					Summary = Summaries[ days],
					Source = new Source { Name1 = "Test name 1" }
				})
				.AsQueryable();
			return Ok(items);
		}
	}
}

namespace Whoua.Core.Data.Entities.V1
{
	#region -- Using directives --
	using System;
	using System.ComponentModel.DataAnnotations;
	#endregion

	public class WeatherForecast
	{
		[ Key ()]
		public DateTime Date			{ get; set; }
		public int		TemperatureC	{ get; set; }
		public int		TemperatureF => 32 + (int)(TemperatureC / 0.5556);
		public string	Summary			{ get; set; }
	}
}

namespace Whoua.Core.Data.Entities.V2
{
	#region -- Using directives --
	using System;
	using System.ComponentModel.DataAnnotations;
	#endregion

	public class WeatherForecast
	{
		[ Key ()]
		public DateTime Date			{ get; set; }
		public int		TemperatureC	{ get; set; }
		public int		TemperatureF => 32 + (int)(TemperatureC / 0.5556);
		public string	Summary			{ get; set; }
		public Source	Source			{ get; set; }
	}

	public class Source
	{
		public string	Name1			{ get; set; }
		public string	Name2			{ get; set; }
		public string	Name3			{ get; set; }
	}
}

namespace Whoua.Core.Api
{
	#region -- Using directives --
	using System;
	using System.IO;
	using System.Linq;	
	using System.Reflection;
	using System.Collections.Generic;

	using Microsoft.EntityFrameworkCore;
	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Hosting;
	using Microsoft.AspNetCore.Mvc.Controllers;

	#region -- Swagger --
	using Swashbuckle.AspNetCore.SwaggerGen;
	#endregion

	#region -- OData --
	using Microsoft.AspNetCore.OData;
	using Microsoft.OpenApi.Models;
	using Microsoft.OData.Edm;
	using Microsoft.OData.ModelBuilder;
	//using Microsoft.AspNetCore.OData.NewtonsoftJson; // --- install-package Microsoft.AspNetCore.OData.NewtonsoftJson
	using Microsoft.AspNetCore.OData.Batch;
	using Microsoft.AspNetCore.OData.Routing.Attributes;
	using Microsoft.AspNetCore.OData.Query;
	#endregion
	
	#region -- Whoua --
	using Whoua.Core.Data;
	using Whoua.Core.Data.Entities;
	using Whoua.Core.Data.Entities.V1;
	using Whoua.Core.Data.Entities.V2;
	using d = System.Diagnostics.Debug;	
	#endregion

	#endregion

    public static class EdmBuilder
    {
        public static IEdmModel BuildV1()
        {
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

            var builder = new ODataConventionModelBuilder();

            builder.EntitySet<Whoua.Core.Data.Entities.V1.WeatherForecast>("WeatherForecast");

            return builder.GetEdmModel();
        }

        public static IEdmModel BuildV2()
        {
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

            var builder = new ODataConventionModelBuilder();

            builder.EntitySet<Whoua.Core.Data.Entities.V2.WeatherForecast>("WeatherForecast");

            return builder.GetEdmModel();
        }
    }

	public class ODataOperationFilter : IOperationFilter
	{
		public void Apply(OpenApiOperation operation, OperationFilterContext context)
		{
			if (operation.Parameters == null) operation.Parameters = new List<OpenApiParameter>();

			var descriptor = context.ApiDescription.ActionDescriptor as ControllerActionDescriptor;
			var queryAttr = descriptor?.FilterDescriptors
				.Where(fd => fd.Filter is EnableQueryAttribute)
				.Select(fd => fd.Filter as EnableQueryAttribute)
				.FirstOrDefault();

			if (queryAttr == null)
				return;

			if (queryAttr.AllowedQueryOptions.HasFlag(AllowedQueryOptions.Select))
				operation.Parameters.Add(new OpenApiParameter()
				{
					Name		= "$select"
				,	In			= ParameterLocation.Query
				,	Schema		= new OpenApiSchema	{ Type = "string" }
				,	Required	= false
				});

			if (queryAttr.AllowedQueryOptions.HasFlag(AllowedQueryOptions.Expand))
				operation.Parameters.Add(new OpenApiParameter()
				{
					Name		= "$expand"
				,	In			= ParameterLocation.Query
				,	Schema		= new OpenApiSchema	{ Type = "string" }
				,	Required	= false
				});


			if (queryAttr.AllowedQueryOptions.HasFlag(AllowedQueryOptions.Filter))
				operation.Parameters.Add(new OpenApiParameter()
				{
					Name		= "$filter"
				,	In			= ParameterLocation.Query
				,	Schema		= new OpenApiSchema	{ Type = "string" }
				,	Required	= false
				});


			if (queryAttr.AllowedQueryOptions.HasFlag(AllowedQueryOptions.Top))
				operation.Parameters.Add(new OpenApiParameter()
				{
					Name		= "$top"
				,	In			= ParameterLocation.Query
				,	Schema		= new OpenApiSchema { Type = "string" }
				,	Required	= false
				});

			if (queryAttr.AllowedQueryOptions.HasFlag(AllowedQueryOptions.Skip))
				operation.Parameters.Add(new OpenApiParameter()
				{
					Name		= "$skip"
				,	In			= ParameterLocation.Query
				,	Schema		= new OpenApiSchema { Type = "string" }
				,	Required	= false
				});


			if (queryAttr.AllowedQueryOptions.HasFlag(AllowedQueryOptions.Skip))
				operation.Parameters.Add(new OpenApiParameter()
				{
					Name		= "$orderby"
				,	In			= ParameterLocation.Query
				,	Schema		= new OpenApiSchema { Type = "string" }
				,	Required	= false
				});

			if (queryAttr.AllowedQueryOptions.HasFlag(AllowedQueryOptions.Count))
				operation.Parameters.Add(new OpenApiParameter()
				{
					Name		= "$count"
				,	In			= ParameterLocation.Query
				,	Schema		= new OpenApiSchema	{ Type = "boolean" }
				,	Required	= false,
				});
		}
	}

	public class Startup
	{
		public Startup( IConfiguration configuration )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		public void ConfigureServices( IServiceCollection services )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			var model1 = EdmBuilder.BuildV1();
			var model2 = EdmBuilder.BuildV2();

			services
				.AddControllers			( )
				//.AddODataNewtonsoftJson	( )
				.AddOData(options =>
				{
					//var defaultBatchHandler = new DefaultODataBatchHandler();
					//defaultBatchHandler.MessageQuotas.MaxNestingDepth			= 2;
					//defaultBatchHandler.MessageQuotas.MaxOperationsPerChangeset = 10;
					//defaultBatchHandler.MessageQuotas.MaxReceivedMessageSize	= 100;

					options.Select().Filter().OrderBy().Count().SetMaxTop(5000).Expand();
					//options.AddRouteComponents("api/v1", model1, defaultBatchHandler);
					options.AddRouteComponents("api/v1", model1);
					options.AddRouteComponents("api/v2", model2);
				});

			//services.AddEndpointsApiExplorer();

			services.AddSwaggerGen(options =>
			{
				options.EnableAnnotations( );

				options.SwaggerDoc("v1", new OpenApiInfo
				{
					Version		= "v1",
					Title		= "Weather API",
					Description = "A simple example ASP.NET Core Web API",
				});
				options.SwaggerDoc("v2", new OpenApiInfo
				{
					Version		= "v2",
					Title		= "Weather API",
					Description = "A simple example ASP.NET Core Web API",
				});

				// --- org maps api/v1 to v1. skips /$metadata, /$count
				options.DocInclusionPredicate((docName, apiDesc) =>
				{
					if (apiDesc.RelativePath.Contains('$')) return false;
					if (!apiDesc.TryGetMethodInfo(out MethodInfo methodInfo)) return false;

					var routeAttrs = methodInfo.DeclaringType
						.GetCustomAttributes(true)
						.OfType<ODataRouteComponentAttribute>();
					var versions = routeAttrs.Select(i => i.RoutePrefix.Split('/').Last());
					return versions.Any(v => string.Equals(v, docName, StringComparison.CurrentCultureIgnoreCase));
				});

				/*
				options.DocInclusionPredicate((docName, apiDesc) =>
				{
					 string relativePath    = apiDesc.RelativePath;
					 int    pos             = relativePath.IndexOf( docName);

					 d.WriteLine( $"docName=[{docName}], relativePath=[{relativePath}], pos={pos}" );

					if( pos > 0)
					{
						 return( true);
					}
					return( false);
				});
				*/

				options.CustomSchemaIds(type => type.FullName);
				options.OperationFilter<ODataOperationFilter>();
			});
		}

		public void Configure( IApplicationBuilder app, IWebHostEnvironment env )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			app.UseDeveloperExceptionPage	( );
			app.UseODataRouteDebug			( );
			app.UseSwagger					( );
			app.UseSwaggerUI(options =>
			{
				options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
				options.SwaggerEndpoint("/swagger/v2/swagger.json", "v2");
				//options.RoutePrefix = string.Empty;
				options.RoutePrefix = "swagger";

				options.DefaultModelsExpandDepth(-1); // hides schemas dropdown
				//options.EnableTryItOutByDefault();
				options.ShowExtensions( );
			});

			app.UseODataBatching	( );
			app.UseRouting			( );
			app.UseHttpsRedirection	( );
			app.UseAuthorization	( );

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});

			//endpoints.MapODataRoute("api", "api", GetEdmModel());
		}
	}

	public static class Global
	{
		public static int CallCount = 0;
	}

	public class Program
	{
		public static void Main( string[] args )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			CreateHostBuilder( args ).Build( ).Run( );
		}

		public static IHostBuilder CreateHostBuilder1( string[] args ) =>
			Host.CreateDefaultBuilder( args )
				.ConfigureWebHostDefaults( w => w.UseStartup<Startup>( ) );

		/// <summary>
		/// Hoe ziet de ConfigureWebHostDefaults eruit als je hem basic schrijft ?
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
		public static IHostBuilder CreateHostBuilder( string[] args)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			IHostBuilder hostBuilder = Host.CreateDefaultBuilder( args);
			hostBuilder.ConfigureWebHostDefaults( w => w.UseStartup<Startup>( ));

			return( hostBuilder);
		}
	}
}

