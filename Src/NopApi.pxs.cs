/**
 * @Name NopApi.cs
 * @Purpose NopApi prototyping
 * @Date 25 May 2022, 06:51:38
 * @Author S.Deckers
 * @Description Pxs approach included

curl -X "GET" "https://localhost:5001/gadget/person/$count" -H "accept: application/json"
curl -X "GET" "https://localhost:5001/odata/GadgetsOdata/$count" -H "accept: application/json"
curl -X "GET" "https://localhost:5001/odata/GadgetsOdata?$select=ProductName,Cost" -H "accept: application/json"
curl -X "GET" "https://localhost:5001/api/GadgetsOdata?$select=ProductName,Cost" -H "accept: application/json"
curl -X "GET" "https://localhost:5001/api/GadgetsOdata?$filter=ProductName eq 'Think Pad'" -H "accept: application/json"

curl -X "GET" https://localhost:5001/api/Category/$count -H "accept: application/json"
curl -X "GET" "https://localhost:5001/api/Category?$select=Id,Name" -H "accept: application/json"
curl -X "GET" "https://localhost:5001/api/Category?$filter=Id%20eq%203" -H "accept: application/json"
 */

namespace Whoua.Core.Api.Supporting
{
	public static class RouteNames
	{
		public const string RoutePrefix			= "api";
		public const string ControllerPrefix	= $"{RouteNames.RoutePrefix}/[controller]";
	}
}

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

	using Whoua.Core.Data;
	using Whoua.Core.Data.Entities;
	using Whoua.Core.Services;
	using Whoua.Core.Api.Supporting;

	using d = System.Diagnostics.Debug;
	using Swashbuckle.AspNetCore.Annotations;
	using Microsoft.AspNetCore.OData.Results;
	#endregion

	[ ApiController			( )]
	[ ApiExplorerSettings	( IgnoreApi = false)]
	[ Route					( "api/[controller]")]
	[ Produces				( "application/json", "application/json;odata.metadata=none")]
	public class GenericBaseController : Microsoft.AspNetCore.OData.Routing.Controllers.ODataController
	{
	}

	[ ApiExplorerSettings	( IgnoreApi = false)]
	[ Produces				( "application/json", "application/json;odata.metadata=none")]
	public class GenericBase2Controller : Microsoft.AspNetCore.OData.Routing.Controllers.ODataController
	{
	}

	/// <summary>
	/// Dit is de aanpak die ik voor nopapi nodig heb
	/// </summary>
	public class CategoryController : GenericBase2Controller
	{
		private const string swaggerControllerDescription = "Category information";

		private readonly ModelContext _modelContext;
		public CategoryController( ModelContext modelContext)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			_modelContext = modelContext;
		}

		[ HttpGet( )] // --- is niet nodig
		[ EnableQuery( )]
		[ SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "Retrieve all Items" )]
		public IActionResult Get( )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			return Ok( this._modelContext.CategoryCollection );
		}

		//[ HttpGet		( RouteNames.RoutePrefix + "/[controller]" + "/{id:long}" )]
		[ HttpGet		( RouteNames.ControllerPrefix + "/{id:long}" )]
		[ EnableQuery	( )]
		[ SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "Retrieve single Item" )]
		public ActionResult<Category> GetSingle( long id )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			var item = _modelContext.CategoryCollection.Where( x => x.Id == id );
			return (Ok( item ));
		}
	}

	/// <summary date="25-08-2021, 22:27:23" author="S.Deckers">
	/// Implement this
	/// </summary>
	public class GadgetsOdataController : ControllerBase
	{
		private readonly ModelContext _modelContext;
		public GadgetsOdataController( ModelContext modelContext )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			_modelContext = modelContext;
		}

		[EnableQuery( )]
		public IActionResult Get( )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			return Ok( _modelContext.Gadgets.AsQueryable( ) );
		}
	}

	/// <summary>
	/// Oude PXS-aanpak. Op deze maniet werkt de '$count' option niet maar wordt een route gedefinieerd in de base controller
	/// wel voor een getter geplakt "/api/PxsCategory")]
	/// </summary>
	public class PxsCategoryController : GenericBaseController
	{
		private const string swaggerControllerDescription = "Category information - PXS approach (no $count)";

		private readonly ModelContext _modelContext;
		public PxsCategoryController(ModelContext modelContext)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			_modelContext = modelContext;
		}

        [ HttpGet		( )] // --- route:/api/PxsCategory
		[ EnableQuery	( )]
        [ SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Retrieve list of Categories")]
        public IQueryable<Category> GetItems()
        {
            var localItems = _modelContext.CategoryCollection;
            return (localItems);
        }

		[ HttpGet( "{id:long}" )] // --- route:/api/PxsCategory/23
		[ EnableQuery( )]
		[ SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "Retrieve single Item" )]
		public ActionResult<Category> GetSingle( long id )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			var item = _modelContext.CategoryCollection.Where( x => x.Id == id );
			return (Ok( item ));
		}
	}

	/// <summary>
	/// Use a service
	/// </summary>
	public class ServicedCategoryController : ODataController
	{
		private const string swaggerControllerDescription = "Category information Serviced";

		private readonly ICategoryService	_categoryService;

		public ServicedCategoryController( ICategoryService categoryService)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			_categoryService = categoryService;
		}

		[ HttpGet( )] // --- is niet nodig
		[ EnableQuery( )]
		[ SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "Retrieve all Items" )]
		public IActionResult Get( )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			return Ok( this._categoryService.GetAll( ) );
		}

		[ HttpGet( "/api/ServicedCategory/{id:long}" )]
		[ EnableQuery( )]
		[ SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "Retrieve single Item" )]
		public ActionResult<Category> GetSingle( long id )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			var item = this._categoryService.Get( id);
			return (Ok( item ));
		}
	}
}

namespace Whoua.Core.Data.Entities
{
	#region -- Using directives --
	using System.Collections.Generic;
	#endregion

	public class Gadgets
	{
		public int		Id			{ get; set; }
		public string	ProductName { get; set; }
		public string	Brand		{ get; set; }
		public decimal	Cost		{ get; set; }
		public string	Type		{ get; set; }
	}

/*
CREATE TABLE [dbo].[Category](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](400) NOT NULL,
	[Description] [nvarchar](max) NULL
);

insert into Category( name, Description) select name, description from kevelam_430.dbo.Category;
*/
	[ System.ComponentModel.DataAnnotations.Schema.Table("Category")]
	public partial class Category
    {
        public int		Id			{ get; set; }
        public string	Name		{ get; set; } = null!;
        public string	Description { get; set; }
    }
}

namespace Whoua.Core.Data
{
	#region -- Using directives --
	using System;

	using Microsoft.EntityFrameworkCore;

	using Whoua.Core.Api;
	using Whoua.Core.Data.Entities;
	using d = System.Diagnostics.Debug;
	#endregion

	public class ModelContext : DbContext
	{
		public ModelContext(DbContextOptions<ModelContext> options) : base(options)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );
		}

		public DbSet<Gadgets>	Gadgets				{ get; set; }
		public DbSet<Category>	CategoryCollection	{ get; set; }
 
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			optionsBuilder.LogTo(Console.WriteLine);
			optionsBuilder.EnableSensitiveDataLogging();
		}
	}
}

namespace Whoua.Core.Services
{
	#region -- Using directives --
	using System.Linq;

	using Microsoft.AspNetCore.OData.Deltas;
	using Microsoft.AspNetCore.OData.Results;

	using Whoua.Core.Api;
	using Whoua.Core.Data;
	using Whoua.Core.Data.Entities;

	using d=System.Diagnostics.Debug;
	#endregion

	public interface ICategoryService
	{
		SingleResult<Category>	Get		( long id);
		IQueryable<Category>	GetAll	( );
	}

	public class CategoryService : ICategoryService
	{
		protected readonly ModelContext _ModelContext;

		public CategoryService
		(
			ModelContext modelContext
		)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			this._ModelContext = modelContext;
		}

		public SingleResult<Category> Get(long id)
		{
			return SingleResult.Create( this._ModelContext.CategoryCollection.Where( a => a.Id == id));
		}

		public IQueryable<Category>	GetAll( )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			return( this._ModelContext.CategoryCollection);
		}
	}
}

namespace Whoua.Core.Api
{
	#region -- Using directives --
	using System;

	using Microsoft.EntityFrameworkCore;
	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Hosting;

	#region -- Swagger --
	using Swashbuckle.AspNetCore.SwaggerGen;
	#endregion

	#region -- OData --
	using Microsoft.AspNetCore.OData;
	using Microsoft.OpenApi.Models;
	using Microsoft.OData.Edm;
	using Microsoft.OData.ModelBuilder;
	#endregion
	
	#region -- Whoua --
	using Whoua.Core.Data;
	using Whoua.Core.Data.Entities;
	using Whoua.Core.Api.Supporting;
	using Whoua.Core.Services;
	using d = System.Diagnostics.Debug;	
	#endregion

	#endregion

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

			const string routePrefix = Whoua.Core.Api.Supporting.RouteNames.RoutePrefix;
			services.AddControllers	( )
				.AddOData(
					opt => opt
					.Count		( )
					.Filter		( )
					.Expand		( )
					.Select		( )
					.OrderBy	( )
					.Expand		( )
					.SetMaxTop	( 100) 
					.AddRouteComponents( routePrefix:routePrefix, model:GetEdmModel())
				);

			services.AddSwaggerGen(c =>
			{
				c.OperationFilter<SwaggerQueryParameter>( );
				c.SwaggerDoc		( name: "v1", info: new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Nop.Mobile Proto", Version = "v1" });
				c.OrderActionsBy	( (apiDesc) => $"{apiDesc.ActionDescriptor.RouteValues["controller"]}_{apiDesc.HttpMethod}");
				c.EnableAnnotations	( );
			});
			
			string connectionString = Configuration.GetConnectionString( "DefaultConnection");
			connectionString = "Server=LP08,1433;Database=xx01;User ID=sa;Password=Gvv17213#;";

			services.AddDbContext<ModelContext>(options =>
			{
				options.UseSqlServer( connectionString);
			});

			services.AddScoped<ICategoryService,	CategoryService>();
		}

		public void Configure( IApplicationBuilder app, IWebHostEnvironment env )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			app.UseSwagger			( );
			app.UseSwaggerUI		( c => 
				{ 
					c.SwaggerEndpoint( url:"/swagger/v1/swagger.json", name:"Foo.WebApi v1" );
					c.DisplayOperationId();
				}
			);
			app.UseHttpsRedirection	( );
			app.UseRouting			( );

			app.UseAuthorization	( );

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapDefaultControllerRoute();
			});
		}

		//routeBuilder.Expand().Select().Count()

		private static IEdmModel GetEdmModel()
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			ODataConventionModelBuilder builder = new ODataConventionModelBuilder();

			// --- Name of the entity should match with our controller(eg: GadgetsOdataController) because this name
			//	   will be used as part of our URL path by OData.

			// --- Als deze mapping niet goed is verschijnen je controllers niet in je swagger contract 
			builder.EntitySet<Gadgets>	( name:"GadgetsOdata");
			builder.EntitySet<Category>	( name:"PxsCategory");
			builder.EntitySet<Category>	( name:"ServicedCategory");
			builder.EntitySet<Category>	( name:"Category");
			return builder.GetEdmModel();
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

namespace Whoua.Core.Api.Supporting
{
	#region -- Using directives --
	using System;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using System.Collections.Generic;
	using Microsoft.Net.Http.Headers;
	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.Extensions.Hosting;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.DependencyInjection;

	using OData.Swagger.Services;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.AspNetCore.Rewrite;
	using Microsoft.AspNetCore.OData;

	using Microsoft.OData.Edm;
	using Microsoft.OData.ModelBuilder;

	using Microsoft.OpenApi.Models;

	using Swashbuckle.AspNetCore.SwaggerUI;
	using Swashbuckle.AspNetCore.SwaggerGen;	
	
	using Newtonsoft.Json;
	using Newtonsoft.Json.Serialization;
		
	using d=System.Diagnostics.Debug;
	#endregion

	public class SwaggerQueryParameter : IOperationFilter
	{
		public void Apply( OpenApiOperation operation, OperationFilterContext context)
		{
			//d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			// --- Do not display parameters with the '$count'-operator since this is a route itself in OData 8 (20220525 SDE)

			if (operation?.OperationId?.IndexOf( "$count" ) > 0)
			{
				return;
			}

			if (operation.Parameters == null) operation.Parameters = new List<OpenApiParameter>();

			var attr = context.MethodInfo.CustomAttributes.ToList();

			if (attr.Any(a => a.AttributeType == typeof(Microsoft.AspNetCore.OData.Query.EnableQueryAttribute)))
			{
				OpenApiSchema stringSchema = new OpenApiSchema { Type = "string" };

				operation.Parameters.Add(new OpenApiParameter()
				{
					Name			=  "$select",
					In				= ParameterLocation.Query,
					Description		= "Select specific fields, comma separated, e.g. $select=name,type",
					Required		= false,
					Schema			= stringSchema,
					AllowReserved	= true
				});

				operation.Parameters.Add(new OpenApiParameter()
				{
					Name			= "$orderby",
					In				= ParameterLocation.Query,
					Description		= "Order records by a field asc or descending e.g. $orderby=name,type desc",
					Required		= false,
					Schema			= stringSchema,
					AllowReserved	= true
				});

				operation.Parameters.Add(new OpenApiParameter()
				{
					Name			= "$top",
					In				= ParameterLocation.Query,
					Description		= "only return the n top records. e.g. $top=5",
					Required		= false,
					Schema			= stringSchema,
					AllowReserved	= true
				});

				operation.Parameters.Add(new OpenApiParameter()
				{
					Name			= "$skip",
					In				= ParameterLocation.Query,
					Description		= "Skip the first n records. e.g $skip=10",
					Required		= false,
					Schema			= stringSchema,
					AllowReserved	= true
				});

				operation.Parameters.Add(new OpenApiParameter()
				{
					Name			= "$filter",
					In				= ParameterLocation.Query,
					Description		= "Advanced filtering options. e.g. $filter=indexof(toupper(RfAreaCode),'AR') eq 0",
					Required		= false,
					Schema			= stringSchema,
					AllowReserved	= true
				});

				operation.Parameters.Add(new OpenApiParameter()
				{
					Name			= "$search",
					In				= ParameterLocation.Query,
					Description		= "Only return records matching a specific search expression. e.g. $search=sometext",
					Required		= false,
					Schema			= stringSchema,
					AllowReserved	= true
				});

				operation.Parameters.Add(new OpenApiParameter()
				{
					Name			= "$expand",
					In				= ParameterLocation.Query,
					Description		= "Expands related entities. Generally those names with end with 'Ref'. e.g. $expand=OwnerRef,ZoneRef",
					Required		= false,
					Schema			= stringSchema,
					AllowReserved	= true
				});
			}
		}
	}
}
