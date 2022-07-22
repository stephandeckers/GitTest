/**
 * @Name GenericCrudDemo.cs
 * @Purpose 
 * @Date 22 September 2021, 09:44:41
 * @Author S.Deckers
 * @Notes Zie Patch6.cs voor de echter GenericCrud-demo
 * @url
 * implementing patch : https://docs.microsoft.com/en-us/aspnet/core/web-api/jsonpatch?view=aspnetcore-5.0
 * 
 * @Description 
 * 
 * - How to implement patch (krijg nu een rare api)
 * - api method naming
 * 
 * Install packages:
 * -----------------
 * PM> Install-Package Microsoft.EntityFrameworkCore
 * PM> Install-Package Microsoft.EntityFrameworkCore.SqlServer
 * PM> Install-Package Microsoft.EntityFrameworkCore.Tools
 * PM> Install-Package Swashbuckle.AspNetCore
 * PM> Install-Package Swashbuckle.AspNetCore.Annotations
 * PM> Install-Package Microsoft.AspNetCore.OData
 * PM> Install-Package Microsoft.AspNetCore.Mvc.NewtonsoftJson (support for patch)
 * 
 * First migration:
 * ---------------
 * PM> add-migration GenericCRUD -Namespace Src\SqlServerMigration -o Src\SqlServerMigration
 * PM> Update-Database 
 * 
 * Operations:
 * -----------
 * Create	: curl -X POST "https://localhost:5001/Person" -H  "accept: application/json;odata.metadata=minimal;odata.streaming=true" -H  "Content-Type: application/json;odata.metadata=minimal;odata.streaming=true" -d "{\"name\":\"Willem Puk\",\"city\":\"Lintelo\",\"dob\":\"2021-09-22T14:43:38.560Z\"}"
 * Delete	: curl -X DELETE "https://localhost:5001/Person/10"
 * GetItems	: curl -X GET "https://localhost:5001/Person" -H  "accept: application/json"
 * GetItem	: curl -X GET "https://localhost:5001/Person/6" -H  "accept: application/json"
 * Patch	: curl -X PATCH "https://localhost:5001/Person?id=5" -H  "Content-Type: application/json;odata.metadata=minimal;odata.streaming=true" -d "{\"name\":\"Tinus Puk\"}"
*/

namespace Whoua.Core.Api
{
	#region -- Using directives --
	using System;
	using System.Linq;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNet.OData.Builder;
	using Microsoft.AspNet.OData.Extensions;
	using Microsoft.AspNet.OData;
	//using Microsoft.AspNetCore.OData.Deltas;
	//using Microsoft.AspNetCore.OData.Query;
	//using Microsoft.AspNetCore.OData.Results;
	using Swashbuckle.AspNetCore.Annotations;
	using Whoua.Core.Data;
	using d = System.Diagnostics.Debug;
	#endregion

	public static class Info
	{
		public const string GenericSwaggerTitle		= "Generic Crud service";
		public const string SwaggerDropDownTitle	= "V1";
	}

	public class CrudController : ControllerBase
	{
		protected readonly ModelContext _ModelContext;

		public CrudController( ModelContext modelContext)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));
			this._ModelContext = modelContext;
		}
	}

	[ ApiController	( )]
	[ Route			( "[controller]" )]
	[ Produces		( "application/json")]
	public class PersonController : CrudController
	{
		private const string swaggerControllerDescription = "Person";

		public PersonController( ModelContext modelContext)
		: base( modelContext)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));
		}

		[ HttpPost()]
		[ SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Create item")]
		public ActionResult<Person> Create([FromBody] Person item)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

			this._ModelContext.Add( item);
			this._ModelContext.SaveChanges( );
			return Ok( item);
		}

		[ EnableQuery	( )]
		[ HttpGet("{id:long}")]
		[ SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Retrieve single item")]
		public ActionResult<Person> Get( long id)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

			var item = this._ModelContext.PersonCollection.Where( x => x.Id == id);
			
			if( item?.Count() == 0)
			{
				return Ok($"Item with id {id} not found");
			}

			var itm = item.FirstOrDefault();

			d.WriteLine( $"id={ itm.Id }, Name={ itm.Name }, City={ itm.City }, DOB={ itm.DOB }");

			itm.Dump();

			return Ok( SingleResult.Create(item));
		}

		[ EnableQuery	( )]
		[ HttpGet		( )]
		[ SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Retrieve list")]
		public IQueryable<Person> GetItems()
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

			var items = _ModelContext.PersonCollection;
			return( items);
		}

		[ HttpPatch()]
		[ SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Modify item")]
		public ActionResult Patch(long id, [FromBody] Delta<Person> delta)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

			Person dbItem = this._ModelContext.PersonCollection.SingleOrDefault( x => x.Id == id);
			dbItem.Dump();

			//delta.CopyChangedValues( dbItem);
			delta.Patch( dbItem);

			dbItem.Dump();

			this._ModelContext.SaveChanges();
			return Ok( dbItem);
		}

		[ HttpGet		( )]
		[ Route			( "InternalPatch")]
		[ SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Internal patch sample")]
		public void InternalPatch()
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

			Person o = new Person( )
			{
				Id				= 1
			,	City			= "Dinxperlo"
			,	DOB				= new System.DateTime( year:1980, month:12, day:30)
			,	Name			= "Tinus Puk"
			,	Foo				= "a666"
			,	IntProperty		= 11
			,	LongProperty	= 1001
			,	DoubleProperty	= 12.33
			};

			o.Dump( ); // --- Id=1, Name=Tinus Puk, City=Dinxperlo, Foo=a666, IntProperty=11, LongProperty=1001, DoubleProperty=12,33, DOB=30/12/1980 00:00:00

			Delta<Person> delta = new Delta<Person>( );

			string propName	= string.Empty;
			object propValue= string.Empty;
			
			propName	= "City";
			propValue	= "Velp";
			if( ! delta.TrySetPropertyValue( propName, propValue))	d.WriteLine( $"Error setting { propName } to { propValue}");

			propName	= "Name";
			propValue	= "Willem Puk";
			if( ! delta.TrySetPropertyValue( propName, propValue))	d.WriteLine( $"Error setting { propName } to { propValue}");

			propName	= "DOB";
			propValue	= new System.DateTime( year:1981, month:11, day:28);
			if( ! delta.TrySetPropertyValue( propName, propValue))	d.WriteLine( $"Error setting { propName } to { propValue}");

			propName	= "Foo";
			propValue	= "c777";
			if( ! delta.TrySetPropertyValue( propName, propValue))	d.WriteLine( $"Error setting { propName } to { propValue}");

			propName	= "IntProperty";
			propValue	= (int) 12;
			if( ! delta.TrySetPropertyValue( propName, propValue))	d.WriteLine( $"Error setting { propName } to { propValue}");

			propName	= "LongProperty";
			propValue	= (long) 1002;
			if( ! delta.TrySetPropertyValue( propName, propValue))	d.WriteLine( $"Error setting { propName } to { propValue}");

			propName	= "DoubleProperty";
			propValue	= (double) 12.44;
			if( ! delta.TrySetPropertyValue( propName, propValue))	d.WriteLine( $"Error setting { propName } to { propValue}");

			delta.Patch( original:o);

			o.Dump( ); // --- Id=1, Name=Willem Puk, City=Velp, Foo=c777, IntProperty=12, LongProperty=1002, DoubleProperty=12,44, DOB=28/11/1981 00:00:00

			var items = delta.GetChangedPropertyNames();

			foreach( var item in items)
			{
				d.WriteLine( item); // City
			}
		}

		[ HttpDelete("{id:int}")]
		[ SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Delete item")]
		public IActionResult Delete(long id)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

			var item = this._ModelContext.PersonCollection.SingleOrDefault( x => x.Id == id);

			if( item == null )
			{
				return Ok($"Error deleting Item with id {id}");
			}

			this._ModelContext.Remove( item);
			this._ModelContext.SaveChanges( );
			return Ok();
		}
	}
}

namespace Whoua.Core.Data
{
	#region -- Using directives --
	using System;
	using System.Text;
	using Microsoft.EntityFrameworkCore;
	using Swashbuckle.AspNetCore.Annotations;
	using Whoua.Core.Api;
	using d = System.Diagnostics.Debug;
	#endregion;

	public static class ConnectionStrings
	{
		public static string SqlServer	= "Server=LP07,1433;Database=xx01;User ID=sa;Password=hilversum";
	}

	public class Person
	{
		public Person()
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));
		}

		[ SwaggerSchema( ReadOnly = true)]
		public int		Id				{ get; set; }
		public string	Name			{ get; set; }
		public string	City			{ get; set; }
		public string	Foo				{ get; set; }
		public int		IntProperty		{ get; set; }
		public long		LongProperty	{ get; set; }
		public double	DoubleProperty	{ get; set; }
		public DateTime	DOB				{ get; set; }

		public void Dump()	{	d.WriteLine( this.ToString( )); }

		public override string ToString( )
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat( "Id={0}",					this.Id);
			sb.AppendFormat( ", Name={0}",				this.Name);
			sb.AppendFormat( ", City={0}",				this.City);
			sb.AppendFormat( ", Foo={0}",				this.Foo);
			sb.AppendFormat( ", IntProperty={0}",		this.IntProperty);
			sb.AppendFormat( ", LongProperty={0}",		this.LongProperty);
			sb.AppendFormat( ", DoubleProperty={0:N2}",	this.DoubleProperty);
			sb.AppendFormat( ", DOB={0}",				this.DOB);
			return( sb.ToString( ));
		}
	}

	public class Company
	{
		public int		Id		{ get; set; }
		public string	Name	{ get; set; }
		public long		Revenue	{ get; set; }
	}

	public class ModelContext : DbContext
	{
		public ModelContext(DbContextOptions<ModelContext> options) : base(options) 
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));
		}

		public DbSet<Person>	PersonCollection	{ get; set; }
		public DbSet<Company>	CompanyCollection	{ get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{ 
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));
			optionsBuilder.UseSqlServer	( ConnectionStrings.SqlServer);
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

			base.OnModelCreating( modelBuilder);

			modelBuilder.Entity<Person>	().ToTable	( "Person");
			modelBuilder.Entity<Company>().ToTable	( "Company");
		}
	}
}

namespace Whoua.Core.Api
{
	#region -- Using directives --
	using System;
	using System.IO;
	using Microsoft.AspNet.OData.Extensions;
	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Hosting;
	using Microsoft.OpenApi.Models;
	using Microsoft.AspNet.OData.Builder;
	using Whoua.Core.Data;
	using Swashbuckle.AspNetCore.SwaggerUI;
	using Microsoft.AspNetCore.Mvc.Formatters;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.Extensions.Options;
	using Microsoft.OData.Edm;
	using Microsoft.AspNet.OData.Formatter;
	using Microsoft.Net.Http.Headers;
	using System.Linq;
	using d = System.Diagnostics.Debug;
	#endregion

	public class Startup
	{
		public Startup( IConfiguration configuration)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		/// <summary>
		/// ConfigureServices
		/// </summary>
		/// <param name="services"></param>
		public void ConfigureServices( IServiceCollection services )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

			//services.AddControllers( ).AddOData(option => option.Select().Filter().Count().OrderBy().Expand());
			/*
			services.AddControllers( ).AddOData( o =>
			{
				o.Select().Filter().Count().OrderBy().Expand();
				o.AddRouteComponents( "odata", GetEdmModel());
			});
			*/

/*
			services.AddControllers()
				.AddNewtonsoftJson(
				options =>
				{
					options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
					options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
					options.SerializerSettings.ContractResolver = ShouldSerializeContractResolver.Instance;
					//options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Ignore;

				})
				.AddJsonOptions(
				options =>
				{
					options.JsonSerializerOptions.PropertyNamingPolicy = null;
					options.JsonSerializerOptions.IgnoreNullValues = true;
				}
				);
*/
			/*
			services.AddControllers
				( 
					o => o.InputFormatters.Insert( 0, GetJsonPatchInputFormatter())
				)
				.AddOData( o =>
				{
					o.Select().Filter().Count().OrderBy().Expand();
					o.AddRouteComponents( "odata", GetEdmModel());
				})
				.AddNewtonsoftJson( );
			*/

			services.AddControllers()
				.AddNewtonsoftJson(
				options =>
				{
					options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
					options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
				//	options.SerializerSettings.ContractResolver = Newtonsoft.Json.ShouldSerializeContractResolver.Instance;
				})
				.AddJsonOptions(
				options =>
				{
					options.JsonSerializerOptions.PropertyNamingPolicy = null;
					options.JsonSerializerOptions.IgnoreNullValues = true;
				}
				);

			services.AddOData();

			services.AddSwaggerGen( c =>
			{
				c.SwaggerDoc( name: "v1", info: new OpenApiInfo { Title = Info.GenericSwaggerTitle, Version = "v1" } );
				c.EnableAnnotations();
				c.OrderActionsBy((apiDesc) => $"{apiDesc.ActionDescriptor.RouteValues["controller"]}_{apiDesc.HttpMethod}");
			});

			//services.AddControllersWithViews(options =>
			//{
			//	options.InputFormatters.Insert(0, GetJsonPatchInputFormatter());
			//});

        services.AddMvcCore(options =>
        {
            foreach (var outputFormatter in options.OutputFormatters.OfType<ODataOutputFormatter>().Where(_ => _.SupportedMediaTypes.Count == 0))
            {
                outputFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/prs.odatatestxx-odata"));
            }
            foreach (var inputFormatter in options.InputFormatters.OfType<ODataInputFormatter>().Where(_ => _.SupportedMediaTypes.Count == 0))
            {
                inputFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/prs.odatatestxx-odata"));
            }
        });


			services.AddDbContext<ModelContext>( o => o.UseSqlServer( ConnectionStrings.SqlServer));
		}

		private static NewtonsoftJsonPatchInputFormatter GetJsonPatchInputFormatter( )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty ) );

			var builder = new ServiceCollection( )
				.AddLogging( )
				.AddMvc( )
				.AddNewtonsoftJson( )
				.Services.BuildServiceProvider( );

			return builder
				.GetRequiredService<IOptions<MvcOptions>>( )
				.Value
				.InputFormatters
				.OfType<NewtonsoftJsonPatchInputFormatter>( )
				.First( );
		}

		private static IEdmModel GetEdmModel() 
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			ODataConventionModelBuilder builder = new Microsoft.AspNet.OData.Builder.ODataConventionModelBuilder(); 
			builder.EntitySet<Person>	("PersonsCollection"); 
			//builder.EntitySet<Company>	("Companies"); 
			return builder.GetEdmModel(); 
		}

		public void Configure( IApplicationBuilder app, IWebHostEnvironment env)
		{
			app.UseDeveloperExceptionPage	( );
			app.UseHttpsRedirection			( );
			app.UseStaticFiles				( );
			app.UseRouting					( );
			app.UseAuthorization			( );

			//app.UseEndpoints( endpoints =>	{ endpoints.MapControllerRoute( name: "default", pattern: "{controller=Home}/{action=Index}/{id?}"); });

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
				//endpoints.EnableDependencyInjection();
				//endpoints.Select().Filter().OrderBy().Count().Expand().MaxTop(100);
				//endpoints.MapODataRoute("api", "api", GetEdmModel());
			});

			app.UseSwagger	( );

			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint(url: "/swagger/v1/swagger.json", name: Info.SwaggerDropDownTitle);
				//c.DefaultModelsExpandDepth(1);
				//c.DefaultModelExpandDepth(1);
				//c.DefaultModelRendering(ModelRendering.Example);
				c.DisplayOperationId();
			});

		}
	}

	public static class Global
	{
		public static int CallCount = 0;
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