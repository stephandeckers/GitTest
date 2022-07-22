/**
 * @Name OData.context.cs
 * @Purpose 
 * o Het is van belang dat je nodes element odata.context terug krijgt, dan maak je namelijk gebruik van een "OData route"
 * en anders van een convenional route
 * 
 * "@odata.context": "https://localhost:5001/odata/$metadata#Product",
 * "@odata.count": 10,
 * value
 * [..]
 * 
 * server driven paging
 * --------------------
 * De server bepaalt de grootte van een pagina en geeft een 'odata.nextLink' terug
 * 
 * @odata.nextLink": "https://localhost:5001/odata/Product?$top=2&$count=true&$skip=1
 * Alleen bij gebruik van [EnableQuery( PageSize = 2 )]
 * 
 *   je OData routes niet
 * o Wat betekent public IQueryable<Product> Get( [FromODataUri]  string s)
 * @Date 13 July 2022, 19:41:59
 * @Author S.Deckers
 * @Description 
 * PM> install-package Microsoft.AspNetCore.OData -version 8.0.10
 * PM> install-package OData.Swagger -version 1.0.0
 * 
 	Controller			Controller Route	OData route EntitySet  #  Routes								OData Endpoint
 	-----------------	------------------	-----------	--------- -- -----------------------------------	-------------
 	ProductController	odata/[controller]	odata		Product	  01 /odata/Product/Products	
 																  02 /odata/Product/odata/Products/$count
 																  03 /odata/Product							X
 																  04 /odata/Product/$count					X
 
 	ProductController	api/[controller]	odata		Product	  01 /api/Product/Products	
 																  02 /api/Product/odata/Products/$count
 																  03 /odata/Product							X
 																  04 /odata/Product/$count					X
 																  

 * - entity set used is crucial
 * OData routing
 * OData debug
 * 
 * Conventional routing
 * --------------------
 *   takes the controller name and action name into consideration and build endpoint for any actions meet the 
 *   conventional rule. Your "UsersController" and "Get()" method meets the entity set convention routing. 
 *   So, entity set convention routing will build two endpoint for "Get()" method.

    ~/api/odata/Users
    ~/api/odata/Users/$count

  Attribute routing
  -----------------
   It takes the routing attributes into consideration. The attributes have:

    [ODataRoutingAttribute]
    [RouteAttribute]
    [HttpGetAttribute]
    [HttpPostAttribute]
    ....

  Conventional routes
  -------------------

   # Name								Order
  -- -------------------------------	-----
  01 MetadataRoutingConvention			(0)
  02 EntitySetRoutingConvention			(100)
  03 SingletonRoutingConvention			(200)
  04 EntityRoutingConvention			(300)
  05 PropertyRoutingConvention			(400)
  06 NavigationRoutignConvention		(500)
  07 FunctionRoutingConvention			(600)
  08 ActionRoutingConvention			(700)
  09 OperationImportRoutingConvention	(900)
  10 RefRoutingConvention				(1000)

 * Routing conventions
 *   https://docs.microsoft.com/en-us/odata/webapi/built-in-routing-conventions
 *   https://devblogs.microsoft.com/odata/attribute-routing-in-asp-net-core-odata-8-0-rc/
 *   
 * Open
 * - [ ApiExplorerSettings	( IgnoreApi = false)]
 * - Waarom zit Product niet in mijn EntitySet
 */

namespace Whoua.Core.Api.Controllers
{
	#region -- Using directives --
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using Microsoft.AspNetCore.Mvc;

	using Microsoft.AspNetCore.OData.Query;
	using Microsoft.AspNetCore.OData.Routing.Controllers;
	using Microsoft.AspNetCore.OData.Routing.Attributes;

	using Whoua.Core.Data.Entities;
	using Whoua.Core.Api.Supporting;

	using d = System.Diagnostics.Debug;
	using Swashbuckle.AspNetCore.Annotations;
	using Microsoft.AspNetCore.OData.Results;
	using Microsoft.AspNetCore.OData.Formatter;
	#endregion

	/* --- SwaggerException (20220716 SDE)
	 * 
	/// <summary>
	/// Waarom krijgen we hier een SwaggerException die aangeeft dat de 'Get'-method niet uniek is ?
	/// </summary>
	[ ApiController( )]
	[ Route("api/[controller]")]	// --- ok
	[ Route("odata/[controller]")]	// --- exception
	[ Produces				( "application/json", "application/json;odata.metadata=none")]	
	public class ProductController : ODataController
	{
		private static List<Product> _products = null;

		private const string swaggerControllerDescription = "Products SwaggerGeneratorException";

		static ProductController( )
		{
			static void init( )
			{
				d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

				_products = new List<Product>( );

				int top = 10;
				for (int i = 0; i < top; i++)
				{
					_products.Add( new Product( ) { Id = i + 1, Name = $"Product {i + 1}", Description = $"Description {i + 1}" } );
				}
			}

			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			init( );
		}

		// --- Swashbuckle.AspNetCore.SwaggerGen.SwaggerGeneratorException: Conflicting method/path combination "GET odata/Product" for actions - 
		[HttpGet( )]
		[EnableQuery( )]
		[SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "Get Products" )]
		public IQueryable<Product> Get( )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			return _products.AsQueryable( );
		}
	}
	*/

	// --- Hier krijgen we 4 operations 
	[ApiController( )]
	[ApiExplorerSettings( IgnoreApi = false )]
	//[Route( "odata/[controller]" )]
	[Route( "api/[controller]" )]
	public class ProductController : ODataController
	{
		private static List<Product> _products = null;

		private const string swaggerControllerDescription = "4 Product operations";

		static ProductController( )
		{
			static void init( )
			{
				d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty ) );

				_products = new List<Product>( );

				int top = 10;
				for (int i = 0; i < top; i++)
				{
					_products.Add( new Product( ) { Id = i + 1, Name = $"Product {i + 1}", Description = $"Description {i + 1}" } );
				}
			}

			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty ) );

			init( );
		}

		[EnableQuery( PageSize = 2 )]
		[HttpGet( "Products" )]
		[HttpGet( "odata/Products/$count" )]
		[SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "Get Items" )]
		public IActionResult Get( )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			return Ok( _products );
		}
	}

	//[ ApiController( )]
	//[ ApiExplorerSettings( IgnoreApi = false )]
	//[ Route( "odata/[controller]" )]
	//public class ProductsController : ODataController
	//{
	//	private static List<Product> _products = null;

	//	private const string swaggerControllerDescription = "Product Sample 4";

	//	static ProductsController( )
	//	{
	//		static void init( )
	//		{
	//			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty ) );

	//			_products = new List<Product>( );

	//			int top = 10;
	//			for (int i = 0; i < top; i++)
	//			{
	//				_products.Add( new Product( ) { Id = i + 1, Name = $"Product {i + 1}", Description = $"Description {i + 1}" } );
	//			}
	//		}

	//		d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty ) );

	//		init( );
	//	}

	//	[ Microsoft.AspNetCore.OData.Query.EnableQuery( PageSize = 1 )]
	//	[ HttpGet( template: "Products" )]
	//	[ HttpGet( template: "odata/Products/$count" )]
	//	[ SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "Get Items" )]
	//	public IActionResult GetItems( )
	//	{
	//		d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

	//		return Ok( _products );
	//	}
	//}
}

namespace Whoua.Core.Data.Entities
{
	#region -- Using directives --
	using System.Collections.Generic;
	using System.Linq;
	using System.Web;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.ComponentModel.DataAnnotations;
	#endregion

	public partial class Product
    {
		[ Key()]
        public int		Id			{ get; set; }
        public string	Name		{ get; set; }
        public string	Description { get; set; }
    }
}

namespace Whoua.Core.Api
{
	#region -- Using directives --
	using System;
	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.AspNetCore.OData;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Hosting;
	using Microsoft.OData.Edm;
	using Microsoft.OData.ModelBuilder;
	using Microsoft.OpenApi.Models;
	using Swashbuckle.AspNetCore.SwaggerUI;

	using Whoua.Core;
	using Whoua.Core.Api.Supporting;
	using Whoua.Core.Data.Entities;

	using d=System.Diagnostics.Debug;
	#endregion

	public class Startup
	{
		/// <summary date="04-06-2021, 12:11:46" author="S.Deckers">
		/// Configures the services.
		/// </summary>
		/// <param name="services">The services.</param>
		public void ConfigureServices( IServiceCollection services)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

			services.AddSwaggerGen(c =>
			{
				c.OperationFilter<SwaggerQueryParameter>( );
				c.SwaggerDoc		( name: "v1", info: new Microsoft.OpenApi.Models.OpenApiInfo { Title = "OData context", Version = "v1" });
				c.OrderActionsBy	( (apiDesc) => $"{apiDesc.ActionDescriptor.RouteValues["controller"]}_{apiDesc.HttpMethod}");
				c.EnableAnnotations	( );
				//c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
			});

			string routename = "odata"; // ---  needs to map routename in controller (20220716 SDE)

			services
				.AddControllers( )
				.AddOData( options =>
				 {
					 options
						 .Select( )
						 .Filter( )
						 .OrderBy( )
						 .Count( )
						 .SetMaxTop( 250 )
						 .Expand( );

					 options.AddRouteComponents( routePrefix:routename, model: GetEdmModel( ) );
				 } );
		}

        private static IEdmModel GetEdmModel()
        {
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

            var builder = new ODataConventionModelBuilder();
			//var builder = new ODataModelBuilder();

			//builder.AddEntityType( typeof( Product ) );

			string controllerName = "Product";
			builder.EntitySet<Whoua.Core.Data.Entities.Product>	( name: "Product" );
			return builder.GetEdmModel();
        }

		public void Configure( IApplicationBuilder app, IWebHostEnvironment env )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			app.UseDeveloperExceptionPage	( );
            app.UseHttpsRedirection			( );
            app.UseRouting					( );
            app.UseEndpoints				( e => { e.MapControllers(); });

            app.UseSwagger					( );
            app.UseSwaggerUI				( c => 
			{ 
				c.SwaggerEndpoint			( "/swagger/v1/swagger.json", "Foo API v1"); 
				c.DisplayOperationId		( );
				c.EnableTryItOutByDefault	( );
			});

			app.UseODataRouteDebug(); // https://localhost:5001/$odata

			//app.UseODataQueryRequest(); // --- wat doet dit ??
		}
	}

	public partial class Program
	{
		public static void Main( string[] args )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", typeof( Program).Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

			var host = CreateHostBuilder(args).Build();
			host.Run();
		}

		public static IHostBuilder CreateHostBuilder( string[] args ) =>
			Host.CreateDefaultBuilder( args )
				.ConfigureWebHostDefaults( webBuilder =>
				 {
					 webBuilder.UseStartup<Startup>( );
				 } );
	}
}

namespace Whoua.Core
{
	public static class Global
	{
		public static int CallCount = 0;
	}
}

namespace Whoua.Core.Api.Supporting
{
	#region -- Using directives --
	using System;
	using System.IO;
	using System.Linq;	
	using System.Reflection;
	using System.Collections.Generic;
	using Microsoft.AspNetCore.Mvc.Controllers;
	using Swashbuckle.AspNetCore.SwaggerGen;
	using Microsoft.OpenApi.Models;
	using Microsoft.AspNetCore.OData.Query;
	using Whoua.Core.Data;
	using Whoua.Core.Data.Entities;
	using d = System.Diagnostics.Debug;	
	#endregion

	public class SwaggerQueryParameter : IOperationFilter
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

			if (queryAttr.AllowedQueryOptions.HasFlag(AllowedQueryOptions.Format))
				operation.Parameters.Add(new OpenApiParameter()
				{
					Name		= "$format"
				,	In			= ParameterLocation.Query
				,	Schema		= new OpenApiSchema	{ Type = "string" }
				,	Required	= false
				});

			if (queryAttr.AllowedQueryOptions.HasFlag(AllowedQueryOptions.SkipToken))
				operation.Parameters.Add(new OpenApiParameter()
				{
					Name		= "$skiptoken"
				,	In			= ParameterLocation.Query
				,	Schema		= new OpenApiSchema	{ Type = "string" }
				,	Required	= false
				});

			if (queryAttr.AllowedQueryOptions.HasFlag(AllowedQueryOptions.Apply))
				operation.Parameters.Add(new OpenApiParameter()
				{
					Name		= "$apply"
				,	In			= ParameterLocation.Query
				,	Schema		= new OpenApiSchema	{ Type = "string" }
				,	Required	= false
				});

			if (queryAttr.AllowedQueryOptions.HasFlag(AllowedQueryOptions.Compute))
				operation.Parameters.Add(new OpenApiParameter()
				{
					Name		= "$compute"
				,	In			= ParameterLocation.Query
				,	Schema		= new OpenApiSchema	{ Type = "string" }
				,	Required	= false
				});

			if (queryAttr.AllowedQueryOptions.HasFlag(AllowedQueryOptions.Search))
				operation.Parameters.Add(new OpenApiParameter()
				{
					Name		= "$search"
				,	In			= ParameterLocation.Query
				,	Schema		= new OpenApiSchema	{ Type = "string" }
				,	Required	= false
				});

			if (queryAttr.AllowedQueryOptions.HasFlag(AllowedQueryOptions.Supported))
				operation.Parameters.Add(new OpenApiParameter()
				{
					Name		= "$supported"
				,	In			= ParameterLocation.Query
				,	Schema		= new OpenApiSchema	{ Type = "string" }
				,	Required	= false
				});
		}
	}
}