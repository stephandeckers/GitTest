/**
 * @Name ODataOutput.v1.cs
 * @Purpose 
 * @Date 01 June 2022, 09:14:11
 * @Author S.Deckers
 * @Notes Swagger pakt veranderingen soms niet goed op
 * @url https://devblogs.microsoft.com/odata/asp-net-odata-8-0-preview-for-net-5/#routing
   In deze url zie je dat de json output ook in een value-member zit
 * @Description Soms zie je de output van OData in een values element, dat zou een breaking change zijn voor PXS
 * 
 * dat is standaard OData serialization, als elementen niet in een value element zitten heet dat JSON serialisation. Je 
 * krijgt standaard serialisation (vanilla JSON) als je controller derived van Microsoft.AspNetCore.Mvc.ControllerBase,
 * OData serialization als je derived van een ODataController of een ODataFormatting attribuut kent.
 * 
 * Als je OData endpoint mappings hebt, dan krijg je het formaat "value":[], anders niet
 * 
 * zie https://github.com/OData/WebApi/issues/1873
 * Dat is het verschil tussen een OData-payload en regular formatting https://devblogs.microsoft.com/odata/attribute-routing-in-asp-net-core-odata-8-0-rc/
 */

/* --- json2
[
  {
    "date": "2022-06-02T09:14:25.848493+02:00",
    "temperatureC": -20,
    "temperatureF": -3,
    "summary": "Freezing"
  },
  {
    "date": "2022-06-03T09:14:25.8488186+02:00",
    "temperatureC": 37,
    "temperatureF": 98,
    "summary": "Scorching"
  },
  {
    "date": "2022-06-04T09:14:25.8488202+02:00",
    "temperatureC": 17,
    "temperatureF": 62,
    "summary": "Cool"
  },
  {
    "date": "2022-06-05T09:14:25.8488204+02:00",
    "temperatureC": 1,
    "temperatureF": 33,
    "summary": "Sweltering"
  },
  {
    "date": "2022-06-06T09:14:25.8488207+02:00",
    "temperatureC": -19,
    "temperatureF": -2,
    "summary": "Hot"
  }
]
*/

/* --- json2
{
  "@odata.context": "https://localhost:5001/api/v1/$metadata#WeatherForecast",
  "value": [
    {
      "Id": 0,
      "Date": "2022-06-02T12:01:04.1394411+02:00",
      "TemperatureC": -10,
      "Summary": "Scorching"
    },
    {
      "Id": 0,
      "Date": "2022-06-03T12:01:04.1918603+02:00",
      "TemperatureC": -17,
      "Summary": "Cool"
    },
    {
      "Id": 0,
      "Date": "2022-06-04T12:01:04.1925458+02:00",
      "TemperatureC": 48,
      "Summary": "Balmy"
    },
    {
      "Id": 0,
      "Date": "2022-06-05T12:01:04.1926243+02:00",
      "TemperatureC": 0,
      "Summary": "Mild"
    },
    {
      "Id": 0,
      "Date": "2022-06-06T12:01:04.1927025+02:00",
      "TemperatureC": -6,
      "Summary": "Cool"
    }
  ]
}
*/
namespace Whoua.Core.Api.Controllers
{
	#region -- Using directives --
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Microsoft.AspNetCore.Mvc;
	using Whoua.Core.Api.Model;
	using Swashbuckle.AspNetCore.Annotations;
	using Microsoft.Extensions.Logging;
	using d=System.Diagnostics.Debug;
	#endregion

	#region -- OData --	
	using Microsoft.AspNetCore.OData.Query;
	using Microsoft.AspNetCore.OData.Results;
	using Microsoft.AspNetCore.OData.Routing.Attributes;
	using Microsoft.AspNetCore.OData.Routing.Controllers;
	#endregion

	using Whoua.Core.Api.Supporting;

	[ ApiController	( )]
	[ Route			( "[controller]" )]
	public class WeatherForecast1Controller : ControllerBase
	{
		private const string swaggerControllerDescription = "OData json output-v1";

		private static readonly string[] Summaries = new[]
		{
			"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
		};

		[ HttpGet( )]
		[ SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "json-v1" )]
		public IEnumerable<WeatherForecast> Getv1( )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));			

			var rng = new Random( );
			return Enumerable.Range( 1, 5 ).Select( index => new WeatherForecast
			{
				Id				= System.DateTime.Now.Second,
				Date			= DateTime.Now.AddDays( index ),
				TemperatureC	= rng.Next( -20, 55 ),
				Summary			= Summaries[rng.Next( Summaries.Length )]
			} )
			.ToArray( );
		}
	}

	//[ ApiController			( )]
	//[ Route					( "[controller]" )]
	[ Produces				( "application/json")]
	//[ ODataAttributeRouting( )]
	[ ODataRouteComponent	( RouteNames_v1.RoutePrefix)]
	public class WeatherForecast2Controller : ODataController
	{
		private const string swaggerControllerDescription = "OData json output-v2";

		private static readonly string[] Summaries = new[]
		{
			"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
		};

/*
{
  "@odata.context": "https://localhost:5001/api/v1/$metadata#WeatherForecast2",
  "value": [
    {
      "Id": 1,
      "Date": "2022-06-17T14:26:41.4785704+02:00",
      "TemperatureC": 24,
      "Summary": "Nice Weather-1"
    },
    {
      "Id": 2,
      "Date": "2022-06-17T14:26:41.4786159+02:00",
      "TemperatureC": 25,
      "Summary": "Nice Weather-2"
    }
  ]
}
    }
*/
		[ HttpGet		( )]
		[ EnableQuery	( )]
		[ SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "Get values1" )]
		public ActionResult<IEnumerable<WeatherForecast>> Get()
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			int i = 0;
			List<WeatherForecast> items = new List<WeatherForecast>( )
			{
				new WeatherForecast( ) { Id = ++i, Summary = $"Nice Weather-{i}", TemperatureC = 23+i, Date = System.DateTime.Now }
			,	new WeatherForecast( ) { Id = ++i, Summary = $"Nice Weather-{i}", TemperatureC = 23+i, Date = System.DateTime.Now }
			};

			return Ok(items.AsEnumerable<WeatherForecast>());
		}

/* --- Zodra je niet meer de standaard route gebruikt krijg je geen 'value'-element meer
[
  {
    "id": 1,
    "date": "2022-06-17T14:27:04.9412227+02:00",
    "temperatureC": 24,
    "temperatureF": 75,
    "summary": "Nice Weather-1"
  },
  {
    "id": 2,
    "date": "2022-06-17T14:27:04.9412269+02:00",
    "temperatureC": 25,
    "temperatureF": 76,
    "summary": "Nice Weather-2"
  }
]
*/

		//[ ODataRouting( )]
		[ HttpGet		( RouteNames_v1.RoutePrefix + "/o/[controller]" )]
		//[ HttpGet		( )]
		//[ ODataRoute("AnotherGet")]
		[ EnableQuery	( )]
		[ SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "Get values2" )]
		public ActionResult<IEnumerable<WeatherForecast>> GetSingle2(  )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			int i = 0;
			List<WeatherForecast> items = new List<WeatherForecast>( )
			{
				new WeatherForecast( ) { Id = ++i, Summary = $"Nice Weather-{i}", TemperatureC = 23+i, Date = System.DateTime.Now }
			,	new WeatherForecast( ) { Id = ++i, Summary = $"Nice Weather-{i}", TemperatureC = 23+i, Date = System.DateTime.Now }
			};

			return Ok(items.AsEnumerable<WeatherForecast>());

			//WeatherForecast item = new WeatherForecast( ) { Id = 4, Summary = "Nice Weather", TemperatureC = 23, Date = System.DateTime.Now };
			//return Ok( item );
		}
		
/*
{
  "id": 4,
  "date": "2022-06-17T14:12:43.8106291+02:00",
  "temperatureC": 23,
  "temperatureF": 73,
  "summary": "Nice Weather"
}
*/
		[ HttpGet		( RouteNames_v1.RoutePrefix + "/[controller]/{id:long}" )]
		[ EnableQuery	( )]
		[ SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "Get single" )]
		public ActionResult<IQueryable<WeatherForecast>> GetSingle( int id )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			WeatherForecast item = new WeatherForecast( ) { Id = 4, Summary = "Nice Weather", TemperatureC = 23, Date = System.DateTime.Now };
			return Ok( item );
		}
	}
}

namespace Whoua.Core.Api.Model
{
	#region -- Using directives --
	using System;
	using System.ComponentModel.DataAnnotations;
	using d=System.Diagnostics.Debug;
	#endregion

	public class WeatherForecast
	{
		[ Key ()]
		public int		Id					{ get; set; }
		public DateTime Date				{ get; set; }
		public int		TemperatureC		{ get; set; }
		public int		TemperatureF => 32 + (int)(TemperatureC / 0.5556);
		public string	Summary				{ get; set; }
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
	using Whoua.Core.Api.Supporting;
	using d = System.Diagnostics.Debug;	
	#endregion

	#endregion

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

		public static IHostBuilder CreateHostBuilder( string[] args ) =>
			Host.CreateDefaultBuilder( args )
				.ConfigureWebHostDefaults( webBuilder =>
				 {
					 webBuilder.UseStartup<Startup>( );
				 } );
	}

	public class Startup
	{
		public void ConfigureServices( IServiceCollection services )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			//var generalModel	= EdmBuilder.GeneralBuild	( );
			var model1			= EdmBuilder.Buildv1		( );

			services
				.AddControllers			( )
				.AddODataNewtonsoftJson	( )
				.AddOData(options =>
				{
					options
						.Select		( )
						.Filter		( )
						.OrderBy	( )
						.Count		( )
						.SetMaxTop	( 5000)
						.Expand		( );

					//options.AddRouteComponents( model:generalModel); // --- enable schemadata elements. We could have used model1/model2 for this as well (20220529 SDE)
					options.AddRouteComponents( routePrefix:RouteNames_v1.RoutePrefix, model:model1);
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

				//options.DocInclusionPredicate((docName, apiDesc) =>
				//{
				//	d.WriteLine( $"docName=[{docName}], apiDesc=[{apiDesc}]");

				//	if( !apiDesc.TryGetMethodInfo(out MethodInfo methodInfo)) 
				//	{
				//		d.WriteLine( $"unable to fetch methodInfo");
				//		return false;
				//	}

				//	var routeAttrs = methodInfo.DeclaringType
				//		.GetCustomAttributes(true)
				//		.OfType<ODataRouteComponentAttribute>();
				//	var versions = routeAttrs.Select(i => i.RoutePrefix.Split('/').Last());

				//	int i = 0;
				//	foreach( var version in versions)
				//	{
				//		d.WriteLine( $"version # {i++}={version}");
				//	}

				//	if (apiDesc.RelativePath == "$metadata")
				//	{
				//		return (true);
				//	}

				//	bool result = versions.Any(v => string.Equals(v, docName, StringComparison.CurrentCultureIgnoreCase));

				//	d.WriteLine( $"result={result}");
				//	return( result);
				//});
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

			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint( url:"/swagger/v1/swagger.json", name:RouteNames_v1.Version);
				c.RoutePrefix = "swagger";

				c.ShowCommonExtensions();
				c.ShowExtensions( );
			});

			app.UseRouting			( );
			app.UseHttpsRedirection	( );
			app.UseAuthorization	( );

			app.UseEndpoints(endpoints => { endpoints.MapControllers();	});
		}
	}

	public static class EdmBuilder
    {
        public static IEdmModel Buildv1()
        {
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

            var builder = new ODataConventionModelBuilder();

			builder.EntitySet<Whoua.Core.Api.Model.WeatherForecast>( name: "WeatherForecast1" );
			builder.EntitySet<Whoua.Core.Api.Model.WeatherForecast>( name: "WeatherForecast2" );
			return builder.GetEdmModel();
        }
    }
}

namespace Whoua.Core.Api.Supporting
{
	public static class RouteNames_v1
	{
		public const string Version			= "v1";
		public const string RoutePrefix		= $"api/{Version}";
		public const string Description		= "Weather Forecase - v1 : First release";
	}
}