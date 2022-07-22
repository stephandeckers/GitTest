/**
 * @Name SwaggerVersions.cs
 * @Purpose Ondersteuning voor multiple versions in Swagger drodpown
 * @Date 27 May 2022, 10:14:47
 * @Author S.Deckers
 * @url https://github.com/rgudkov-uss/aspnetcore-net6-odata-swagger-versioned
 * @Description startpage https://localhost:5001/index.html
 * - eenvoudigere return waarden
 * - schemas
 */

namespace Whoua.Core.Api.Supporting
{
	public static class RouteNames_v1
	{
		public const string Version			= "v1";
		public const string RoutePrefix		= $"api/{Version}";
		public const string Description		= "Weather Forecase - v1 : First release";
	}

	public static class RouteNames_v2
	{
		public const string Version			= "v2";
		public const string RoutePrefix		= $"api/{Version}";
		public const string Description		= "Weather Forecase - v2 - Heavy change";
	}
}

namespace Whoua.Core.Api.Controllers.v1
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

	using Swashbuckle.AspNetCore.Annotations;

	using Whoua.Core.Api.Supporting;
	using Whoua.Core.Data.Entities.v1;

	using d=System.Diagnostics.Debug;
	#endregion

	[ Produces				( "application/json")]
	[ ODataRouteComponent	( RouteNames_v1.RoutePrefix)]
	public class WeatherForecastController : ODataController
	{
		private const string swaggerControllerDescription = "WeatherForecast";

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

		[ HttpGet		( RouteNames_v1.RoutePrefix + "/[controller]/{id:long}" )]
		[ EnableQuery	( )]
		[ SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "wfc - Get single forecast" )]
		public ActionResult<IQueryable<WeatherForecast>> GetSingle( int id )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			var item = Summaries[id];
			return Ok( item );
		}

		[ HttpGet		( )]
		[ EnableQuery	( )]
		[ SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "wfc - Get all forecasts" )]
		public ActionResult<IEnumerable<WeatherForecast>> Get()
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			var items = Enumerable.Range(1, 5)
				.Select(index => new WeatherForecast
				{
					Date = DateTime.Now.AddDays(index),
					TemperatureC = Random.Shared.Next(-20, 55),
					Summary = Summaries[Random.Shared.Next(Summaries.Length)],
				});
			return Ok(items);
		}
	}

	[ Produces				( "application/json")]
	[ ODataRouteComponent	( RouteNames_v1.RoutePrefix)]
	public class CategoryController : ODataController
	{
		private const string swaggerControllerDescription = "Categories";

		private static readonly string[] Summaries = new[]
		{
			"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
		};

		private static List<Category> _CategoryCollection = new List<Category>( )
		{ 
			new Category( ) { Id = 1, Name = "Category 1", Description = "Description 1" }
		,	new Category( ) { Id = 2, Name = "Category 2", Description = "Description 2" }
		,	new Category( ) { Id = 3, Name = "Category 3", Description = "Description 3" }
		};

		private readonly ILogger<CategoryController> _logger;

		public CategoryController( ILogger<CategoryController> logger )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			_logger = logger;
		}

		[ HttpGet		( RouteNames_v1.RoutePrefix + "/[controller]/{id:long}" )]
		//[HttpGet("{id}", Name = "GetProductById")]
		[ EnableQuery	( )]
		[ SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "cg - Get single forecast" )]
		public ActionResult<IQueryable<WeatherForecast>> GetSingle( int id )
		//public IQueryable<WeatherForecast> GetSingle( int id )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			var item = Summaries[id];
			return Ok( item );
		}

		[ HttpGet( RouteNames_v1.RoutePrefix + "/c/[controller]/{id:long}" )]
		[ EnableQuery( )]
		[ SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "cg - Get single category" )]
		public ActionResult<IQueryable<Category>> GetSingleCategory( int id )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			var item = _CategoryCollection.FirstOrDefault( );
			return Ok( item );
		}

		//[ HttpGet		( RouteNames_v1.RoutePrefix + "/c/[controller]/{id:long}" )]
		//[ EnableQuery	( )]
		//[ SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "cg - Get single category" )]
		//public IQueryable<Category> GetSingleCategory2( int id )
		//{
		//	d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

		//	var item = _CategoryCollection.FirstOrDefault();
		//	return (IQueryable<Category>)item;
		//}

		[ HttpGet		( )]
		[ EnableQuery	( )]
		[ SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "cg - Get all categories" )]
		public ActionResult<IEnumerable<Category>> Get()
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			return( Ok(_CategoryCollection.AsQueryable()));
		}
	}
}

namespace Whoua.Core.Api.Controllers.v2
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
	using Whoua.Core.Data.Entities.v2;

	using Swashbuckle.AspNetCore.Annotations;

	using d=System.Diagnostics.Debug;
	#endregion

	[ Produces				( "application/json")]
	[ ODataRouteComponent	( RouteNames_v2.RoutePrefix)]	
	public class WeatherForecastController : ODataController
	{
		private const string swaggerControllerDescription = "WeatherForecast extended";

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
					TemperatureC = Random.Shared.Next(-20, 55),
					Summary = Summaries[Random.Shared.Next(Summaries.Length)],
					Source = new Source { Name1 = "Test name 1" }
				})
				.AsQueryable();
			return Ok(items);
		}
	}
}

namespace Whoua.Core.Data.Entities.v1
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

	public class Category
	{
        [ Key			( )]
        public int Id				{ get; set; }

        [ Required		( )]
        [ StringLength	(400)]
        public string Name			{ get; set; }
        public string Description	{ get; set; }
	}
}

namespace Whoua.Core.Data.Entities.v2
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
	using Microsoft.AspNetCore.OData.NewtonsoftJson; // --- install-package Microsoft.AspNetCore.OData.NewtonsoftJson
	using Microsoft.AspNetCore.OData.Batch;
	using Microsoft.AspNetCore.OData.Routing.Attributes;
	using Microsoft.AspNetCore.OData.Query;
	#endregion
	
	#region -- Whoua --
	using Whoua.Core.Data;
	using Whoua.Core.Data.Entities;
	using Whoua.Core.Api.Supporting;
	using Whoua.Core.Data.Entities.v1;
	using Whoua.Core.Data.Entities.v2;
	using d = System.Diagnostics.Debug;	
	#endregion

	#endregion

    public static class EdmBuilder
    {
        public static IEdmModel Buildv1()
        {
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

            var builder = new ODataConventionModelBuilder();

            builder.EntitySet<Whoua.Core.Data.Entities.v1.WeatherForecast>	( name:"WeatherForecast");
			//builder.EntitySet<Whoua.Core.Data.Entities.v1.WeatherForecast>	( name:"Category"); // --- Name must match otherwise methods don't show
			builder.EntitySet<Whoua.Core.Data.Entities.v1.Category>			( name:"Category"); // --- Name must match otherwise methods don't show
			
            return builder.GetEdmModel();
        }

        public static IEdmModel Buildv2()
        {
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

            var builder = new ODataConventionModelBuilder();

            builder.EntitySet<Whoua.Core.Data.Entities.v2.WeatherForecast>("WeatherForecast");

            return builder.GetEdmModel();
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

			var model1 = EdmBuilder.Buildv1();
			var model2 = EdmBuilder.Buildv2();

			services
				.AddControllers			( )
				.AddODataNewtonsoftJson	( )
				.AddOData(options =>
				{
					var defaultBatchHandler = new DefaultODataBatchHandler();
					defaultBatchHandler.MessageQuotas.MaxNestingDepth			= 2;
					defaultBatchHandler.MessageQuotas.MaxOperationsPerChangeset = 10;
					defaultBatchHandler.MessageQuotas.MaxReceivedMessageSize	= 100;
					//defaultBatchHandler.PrefixName = "x1";

					options
						.Select		( )
						.Filter		( )
						.OrderBy	( )
						.Count		( )
						.SetMaxTop	( 5000)
						.Expand		( );
					//options.AddRouteComponents("api/v1", model1, defaultBatchHandler);
					//options.AddRouteComponents("api/v2", model2);

					options.AddRouteComponents( routePrefix:RouteNames_v1.RoutePrefix, model:model1, batchHandler:defaultBatchHandler);				
					options.AddRouteComponents( routePrefix:RouteNames_v2.RoutePrefix, model:model2);
				});

			services.AddEndpointsApiExplorer();

			services.AddSwaggerGen(options =>
			{
				options.EnableAnnotations( );

				options.SwaggerDoc( name:RouteNames_v1.Version, new OpenApiInfo
				{
					Version		= RouteNames_v1.Version,
					Title		= "Weather API",
					Description = RouteNames_v1.Description,
				});
				options.SwaggerDoc( name:RouteNames_v2.Version, new OpenApiInfo
				{
					Version		= RouteNames_v2.Version,
					Title		= "Weather API",
					Description = RouteNames_v2.Description,
				});

				// maps api/v1 to v1. skips /$metadata, /$count

				options.DocInclusionPredicate((docName, apiDesc) =>
				{
					d.WriteLine( $"docName=[{docName}], apiDesc=[{apiDesc}]");

					//if( apiDesc.RelativePath.Contains('$'))
					//{
					//	d.WriteLine( $"apiDesc= containt $");
					//	return false;
					//}

					if( !apiDesc.TryGetMethodInfo(out MethodInfo methodInfo)) 
					{
						d.WriteLine( $"unable to fetch methodInfo");
						return false;
					}

					var routeAttrs = methodInfo.DeclaringType
						.GetCustomAttributes(true)
						.OfType<ODataRouteComponentAttribute>();
					var versions = routeAttrs.Select(i => i.RoutePrefix.Split('/').Last());
					foreach( var version in versions)
					{
						d.WriteLine( $"version={version}");
					}

					return versions.Any(v => string.Equals(v, docName, StringComparison.CurrentCultureIgnoreCase));
				});

				options.CustomSchemaIds(type => type.FullName);
				options.OperationFilter<ODataOperationFilter>();
			});
		}

		/// <summary date="28-05-2022, 19:27:43" author="S.Deckers">
		/// Configure
		/// </summary>
		/// <param name="app"></param>
		/// <param name="env"></param>
		public void Configure( IApplicationBuilder app, IWebHostEnvironment env )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			app.UseDeveloperExceptionPage	( );
			app.UseODataRouteDebug			( );
			app.UseSwagger					( );

			app.UseSwaggerUI(options =>
			{
				options.SwaggerEndpoint( url:"/swagger/v1/swagger.json", name:RouteNames_v1.Version);
				options.SwaggerEndpoint( url:"/swagger/v2/swagger.json", name:RouteNames_v2.Version);
				options.RoutePrefix = "swagger";

				options.DefaultModelsExpandDepth(-1); // hides schemas dropdown
				//options.EnableTryItOutByDefault	( );

				options.ShowCommonExtensions();
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
}