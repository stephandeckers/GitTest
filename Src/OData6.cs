/**
 * @Name OData6.cs
 * @Purpose 
 * @Date 23 August 2021, 07:12:59
 * @Author S.Deckers
 * @Description 
 * @url
 * https://www.learmoreseekmore.com/2021/07/intro-on-odata-v8-in-dotnet5-api.html
 * https://devblogs.microsoft.com/odata/asp-net-core-odata-now-available/
 * https://devblogs.microsoft.com/odata/asp-net-odata-8-0-preview-for-net-5/
 
 - 2do: Als je met OData filtert op een item wat niet standaard met linq wordt opgehaald, werkt de filter dan ? 
 
CREATE TABLE [dbo].[Gadgets](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProductName] [varchar](max) NULL,
	[Brand] [varchar](max) NULL,
	[Cost] [decimal](18, 0) NOT NULL,
	[ImageName] [varchar](1024) NULL,
	[Type] [varchar](128) NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];

insert into Gadgets( ProductName, Brand, Cost) values ( 'Samsung Galaxy M30', 'Samsung', 17000);
insert into Gadgets( ProductName, Brand, Cost) values ( 'One Plus 7', 'Google', 52000);
insert into Gadgets( ProductName, Brand, Cost) values ( 'Iphone X', 'Apple', 77000);
insert into Gadgets( ProductName, Brand, Cost) values ( 'Think pad', 'IBM', 100000);

OData queries:
--------------
query single book	 : http://localhost:5000/odata/Books(1)
multiple expressions : http://localhost:5000/odata/Books?$filter=Price le 50&$expand=Press($select=Name)&$select=Location($select=City)

$select : request a specific set of properties for each entity or complex type. The set of properties must be comma-separated while requesting.
$filter : filter data based on a boolean condition. The following are conditional operators that have to be used in URLs.
	eq - equals to.
	ne - not equals to
	gt - greater than
	ge - greater than or equal 
	lt - less than
	le - less than or equal
$orderby : sort the data using 'asc' and 'desc' keywords. We can do sorting on multiple properties using comma separation
$top 	 : fetch specified the count of top records in the collection
$skip 	 : skip the specified number of records and fetches the remaining data.
$expand	 : query the internal or navigation property object.

Examples
---------
$select  :	https://localhost:5001/gadget/Get?$select=ProductName,Cost
			https://localhost:5001/odata/GadgetsOdata?$select=ProductName,Cost
$filter
	  eq :	https://localhost:5001/gadget/Get?$filter=ProductName eq 'Think Pad'
			https://localhost:5001/odata/GadgetsOdata?$filter=ProductName eq 'Think Pad'
	  ne :	https://localhost:5001/gadget/Get?$filter=ProductName ne 'Think Pad'
			https://localhost:5001/odata/GadgetsOdata?$filter=ProductName ne 'Think Pad'
	  gt :	https://localhost:5001/gadget/Get?$filter=Cost gt 17000
			https://localhost:5001/odata/GadgetsOdata?$filter=Cost gt 17000
	  ge :	https://localhost:5001/gadget/Get?$filter=Cost ge 17000
			https://localhost:5001/odata/GadgetsOdata?$filter=Cost ge 17000
	  lt :	https://localhost:5001/gadget/Get?$filter=Cost lt 17000
			https://localhost:5001/odata/GadgetsOdata?$filter=Cost lt 17000
	  le :	https://localhost:5001/gadget/Get?$filter=Cost ge 17000
			https://localhost:5001/odata/GadgetsOdata?$filter=Cost ge 17000

$orderby :	https://localhost:5001/gadget/Get?$orderby=Id desc
			https://localhost:5001/gadget/Get?$orderby=Id asc 
			https://localhost:5001/gadget/Get?$orderby=Id desc, Brand asc
			https://localhost:5001/gadget/Get?$orderby=Id asc, Brand asc

			https://localhost:5001/odata/GadgetsOdata?$orderby=Id desc

$top	 :	https://localhost:5001/gadget/Get?$top=2
			https://localhost:5001/odata/GadgetsOdata?$top=2
$skip	 :	https://localhost:5001/gadget/Get?$skip=2
			https://localhost:5001/odata/GadgetsOdata?$skip=2
$expand	 :	https://localhost:5001/gadget/person?$expand=BankAccounts			
			https://localhost:5001/gadget/person?$expand=BankAccounts($select=AccountId)
			https://localhost:5001/gadget/person?$expand=BankAccounts($select=BankName,AccountId)

			https://localhost:5001/odata/GadgetsOdata/person?$expand=BankAccounts

$count	:   https://localhost:5001/odata/GadgetsOdata/$count
 */

namespace Whoua.Core.Api.Controllers
{
	#region -- Using directives --
	using System;
	using System.Collections.Generic;
	using Microsoft.AspNetCore.Mvc;

	using Microsoft.AspNetCore.OData.Query;
	using Microsoft.AspNetCore.OData.Routing.Controllers;
	using Microsoft.AspNetCore.OData.Routing.Attributes;

	using Whoua.Core.Data;
	using Whoua.Core.Data.Entities;

	using d = System.Diagnostics.Debug;
	#endregion

	[ Route("gadget")]
	[ ApiController( )]
	public class GadgetsController : ControllerBase
	{
		private readonly MyWorldDbContext _myWorldDbContext;

		public GadgetsController(MyWorldDbContext myWorldDbContext)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			_myWorldDbContext = myWorldDbContext;
		}

		[ Microsoft.AspNetCore.OData.Query.EnableQuery( )]
		[ HttpGet("Get")]
		public IActionResult Get()
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			return Ok(_myWorldDbContext.Gadgets.AsQueryable());
		}

		[ HttpGet("person")]
		[ EnableQuery]
		public IActionResult GetPerson()
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			var person = new Person
			{
				Name = "Naveen",
				Id = 1,
				BankAccounts = new List<BankAccount>
				{
					new BankAccount
					{
						AccountId = 1111,
						BankName = "Bank 1"
					},
					new BankAccount
					{
						AccountId = 2222,
						BankName = "Bank 2"
					}
				}
			};
			var result = new List<Person>();
			result.Add(person);
			return Ok(result);
		}
	}

	/// <summary date="25-08-2021, 22:27:23" author="S.Deckers">
	/// Implement this
	/// </summary>
	public class GadgetsOdataController:ControllerBase
	{
		private readonly MyWorldDbContext _myWorldDbContext;
		public GadgetsOdataController(MyWorldDbContext myWorldDbContext)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			_myWorldDbContext = myWorldDbContext;
		}
 
		[ EnableQuery( )]
		public IActionResult Get()
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			return Ok(_myWorldDbContext.Gadgets.AsQueryable());
		}

		[ HttpGet("/odata/GadgetsOdata/person")]
		[ EnableQuery]
		public IActionResult GetPerson()
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			var person = new Person
			{
				Name = "Naveen",
				Id = 1,
				BankAccounts = new List<BankAccount>
				{
					new BankAccount
					{
						AccountId = 1111,
						BankName = "Bank 1"
					},
					new BankAccount
					{
						AccountId = 2222,
						BankName = "Bank 2"
					}
				}
			};
			var result = new List<Person>();
			result.Add(person);
			return Ok(result);
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

	public class Person
	{
		public int					Id				{ get; set; }
		public string				Name			{ get; set; }
		public List<BankAccount>	BankAccounts	{ get; set; }
	}
 
	public class BankAccount
	{
		public int					AccountId	{ get; set; }
		public string				BankName	{ get; set; }
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

	public class MyWorldDbContext : DbContext
	{
		public MyWorldDbContext(DbContextOptions<MyWorldDbContext> options) : base(options)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );
		}

		public DbSet<Gadgets> Gadgets { get; set; }
 
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			// below line to watch the ef core sql quiries generation
			// not at all recomonded for the production code
			optionsBuilder.LogTo(Console.WriteLine);
			optionsBuilder.EnableSensitiveDataLogging();
		}
	}
}

namespace Whoua.Core.Api
{
	#region -- Using directives --
	using System;
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
	
	using Whoua.Core.Data;
	using Whoua.Core.Data.Entities;

	using d = System.Diagnostics.Debug;
using Microsoft.EntityFrameworkCore;


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
					.AddRouteComponents( routePrefix:"odata", model:GetEdmModel())
				);

			services.AddSwaggerGen	( c => c.SwaggerDoc( "v1", new OpenApiInfo { Title = "Foo.WebApi", Version = "v1" } ) );			
			
			string connectionString = Configuration.GetConnectionString( "DefaultConnection");
			connectionString = "Server=LP08,1433;Database=xx01;User ID=sa;Password=Gvv17213#;";

			services.AddDbContext<MyWorldDbContext>(options =>
			{
				options.UseSqlServer( connectionString);
			});
		}

		public void Configure( IApplicationBuilder app, IWebHostEnvironment env )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			app.UseSwagger			( );
			app.UseSwaggerUI		( c => c.SwaggerEndpoint( url:"/swagger/v1/swagger.json", name:"Foo.WebApi v1" ) );
			app.UseHttpsRedirection	( );
			app.UseRouting			( );

			app.UseAuthorization	( );

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapDefaultControllerRoute();
			});
		}

		private static IEdmModel GetEdmModel()
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
			//builder.EntitySet<Book>		( "Books");
			//builder.EntitySet<Press>	( "Presses");

			// --- Name of the entity should match with our controller(eg: GadgetsOdataController) because this name
			//	   will be used as part of our URL path by OData.

			builder.EntitySet<Gadgets>("GadgetsOdata");
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