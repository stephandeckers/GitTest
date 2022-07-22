/**
 * @Name OData.cs
 * @Purpose OData example
 * @Date 01 June 2021, 22:01:14
 * @Author S.Deckers
 * @url https://devblogs.microsoft.com/odata/asp-net-core-odata-now-available/
 * @Description 
 * - https://localhost:44321/odata/$metadata to query metadata
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
	using Microsoft.Extensions.Logging;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;
	using Microsoft.AspNet.OData;
	using d=System.Diagnostics.Debug;
	using Microsoft.AspNetCore.OData.Routing.Controllers;
	#endregion

	//[ ApiController]
	//[ Route( "[controller]" )]
	//public class SomeApiController : ControllerBase
	//{
	//}

	[ ApiController	( )]
    [ Route			("api/foo")]
	public class FooController : ODataController
	{
		[ Microsoft.AspNet.OData.EnableQuery( )]
		public IActionResult Get()
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

			Person p = new Person( ) { Name = string.Format( "Pietje de {0}e Puk", System.DateTime.Now.Second), Age = System.DateTime.Now.Minute };
			return Ok( p);
		}

        [ HttpPost	( )]
        [ Route		("")]
        public IActionResult Post( [FromBody]string s)
        {
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

            return Created( s);
        }
	}

    //[ ApiVersion("1.0")]
    [ ApiController	( )]
    [ Route			("api/books")]
	public class BooksController : ODataController
	{
		private BookStoreContext _db;

		public BooksController( BookStoreContext context)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

			_db = context;

			if( context.Books.Count() == 0)
			{
				foreach (var b in DataSource.GetBooks())
				{
					context.Books.Add(b);
					context.Presses.Add(b.Press);
				}
				context.SaveChanges();
			}
		}
		
		[ Microsoft.AspNet.OData.EnableQuery( )]
		public IActionResult Get()
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

			return Ok( _db.Books);
		}

		[ Microsoft.AspNet.OData.EnableQuery( )]
		[ Route("{key:int}")]
		public IActionResult Get(int key)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

			Book book = _db.Books.FirstOrDefault(c => c.Id == key);

			if( book == null)
			{
				NotFoundObjectResult notFoundObjectResult = new NotFoundObjectResult( string.Format( "Book {0} not found", key) );
				return( notFoundObjectResult);
			}

			book.Dump( );

			return Ok( book);
		}

        [ HttpPost	( )]
        [ Route		("")]
        public IActionResult Post( [FromBody]Book book)
        {
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

            _db.Books.Add(book);
            _db.SaveChanges();
            return Created(book);
        }

		/// <summary date="02-06-2021, 15:47:50" author="S.Deckers">
		/// [FromBody] geeft aan dat de argumenten uit de body moeten worden gehaald, als dat niet zo is kun je [FromUri] gebruiken
		/// 
		/// url : https://stackoverflow.com/questions/24625303/why-do-we-have-to-specify-frombody-and-fromuri 
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
        [ HttpDelete ( )]
        [ Route		 ("{id:int}")]
        public IActionResult Delete([FromBody]int key)
        {
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

            Book b = _db.Books.FirstOrDefault(c => c.Id == key);
            if (b == null)
            {
                return NotFound();
            }

            _db.Books.Remove(b);
            _db.SaveChanges();
            return Ok();
        }
	}
}

namespace Whoua.Core.Api.Model
{
	#region -- Using directives --
	using System;
	using System.Text;
	using System.Collections.Generic;
	using Microsoft.EntityFrameworkCore;
	using d=System.Diagnostics.Debug;
	#endregion

    public class Person
    {
		public int		Id		{ get; set; }
        public string	Name	{ get; set; }
        public int		Age		{ get; set; }
    }

    public class Address
    {
        public string	City		{ get; set; }
        public string	Street		{ get; set; }
    }

    public class Press
    {
        public int		Id			{ get; set; }
        public string	Name		{ get; set; }
        public string	Email		{ get; set; }
        public Category Category	{ get; set; }
    }

    public class Book
    {
        public int		Id			{ get; set; }
        public string	ISBN		{ get; set; }
        public string	Title		{ get; set; }
        public string	Author		{ get; set; }
        public decimal	Price		{ get; set; }
        public Address	Location	{ get; set; }
        public Press	Press		{ get; set; }

		public void Dump( )
		{
			d.WriteLine( this.ToString( ) );
		}

		public override string ToString( )
		{
			StringBuilder sb = new StringBuilder( );
			sb.AppendFormat( "Id={0}",			this.Id);
			sb.AppendFormat( ", ISBN={0}",		this.ISBN);
			sb.AppendFormat( ", Title={0}",		this.Title);
			sb.AppendFormat( ", Author={0}",	this.Author);
			sb.AppendFormat( ", Price={0}",		this.Price);

			return( sb.ToString( ) );
		}
	}

    public enum Category
    {
        Book,
        Magazine,
        EBook
    }

	public class BookStoreContext : DbContext
	{
	   public BookStoreContext(DbContextOptions<BookStoreContext> options)
		 : base( options)
	   {
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));
	   }

	   public DbSet<Book>	Books	{ get; set; }
	   public DbSet<Press>	Presses { get; set; }

	   protected override void OnModelCreating(ModelBuilder modelBuilder)
	   {
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

			modelBuilder.Entity<Book>().OwnsOne(c => c.Location);

			//modelBuilder.Entity<TestEntity>().HasKey(t => t.EntityID);

			//modelBuilder.Entity<Person>().HasKey(t => t.Name);
	   }
	}

    public static class DataSource
    {
        private static IList<Book> _books { get; set; }

        public static IList<Book> GetBooks()
        {
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

            if (_books != null)
            {
                return _books;
            }

            _books = new List<Book>();

            // book #1
            Book book = new Book
            {
                Id			= 1,
                ISBN		= "978-0-321-87758-1",
                Title		= "Essential C#5.0",
                Author		= "Mark Michaelis",
                Price		= 59.99m,
                Location	= new Address { City = "Redmond", Street = "156TH AVE NE" },
                Press = new Press
                {
                    Id			= 1,
                    Name		= "Addison-Wesley",
                    Category	= Category.Book
                }
            };
            _books.Add( book);

            // book #2
            book = new Book
            {
                Id			= 2,
                ISBN		= "063-6-920-02371-5",
                Title		= "Enterprise Games",
                Author		= "Michael Hugos",
                Price		= 49.99m,
                Location	= new Address { City = "Bellevue", Street = "Main ST" },
                Press = new Press
                {
                    Id = 2,
                    Name = "O'Reilly",
                    Category = Category.EBook,
                }
            };

            _books.Add( book);

            return( _books);
        }
    }
}

namespace Whoua.Core.Api
{
	#region -- Using directives --
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.AspNetCore.HttpsPolicy;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Hosting;
	using Microsoft.Extensions.Logging;
	using Whoua.Core.Api.Model;

	using Microsoft.AspNet.OData.Builder;
	using Microsoft.AspNet.OData.Extensions;
	using Microsoft.OData.Edm;

	using Microsoft.EntityFrameworkCore;

	using d=System.Diagnostics.Debug;
	#endregion

	public class Startup
	{
		public Startup( IConfiguration configuration)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		public void ConfigureServices( IServiceCollection services )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));
			services.AddControllersWithViews( );

			//services.AddOData	( );
			services.AddMvc		( );

			services.AddDbContext<BookStoreContext>(opt => opt.UseInMemoryDatabase("BookLists"));
			services.AddOData();
			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0); // using Microsoft.EntityFrameworkCore;
		}

		public void Configure( IApplicationBuilder app, IWebHostEnvironment env )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

			if( env.IsDevelopment( ))
			{
				app.UseDeveloperExceptionPage( );
			}
			else
			{
				app.UseExceptionHandler( "/Home/Error" );
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts( );
			}

			// --- Specify default files to use :
			//     app.UseDefaultFiles(new DefaultFilesOptions { DefaultFileNames = new    List<string> { "index.html" } });

			// --- Use mvc with routes:

			//app.UseMvc(routes =>
			//   {
			//	   routes.MapRoute(
			//		   name: "default",
			//		   template: "{controller=Home}/{action=Index}");
			//   });

			//app.UseDefaultFiles	( );
			app.UseStaticFiles	( );

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.Select().Expand().Filter().OrderBy().MaxTop(100).Count();
                endpoints.MapODataRoute("odata", "odata", GetEdmModel());

				//endpoints.MapODataRoute( routeName:"foodata", routePrefix:"xxxx", model:GetFooModel( ));
				//endpoints.MapODataRoute(
            });
			
		}

		/// <summary>
		/// EDM = Entity Data Model
		/// </summary>
		/// <returns></returns>
        private static IEdmModel GetFooModel()
        {
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

            //ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            //builder.EntitySet<Book>		("Books");
           // builder.EntitySet<Press>	("Presses");
            //return builder.GetEdmModel();

			ODataConventionModelBuilder builder = new ODataConventionModelBuilder( );

			builder.EntitySet<Person>		("Person");
			return( builder.GetEdmModel());
        }

        private static IEdmModel GetEdmModel()
        {
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            builder.EntitySet<Book>		( "Books");
			//builder.EntitySet<Book>		( "Boeken");
            builder.EntitySet<Press>	( "Presses");
            return builder.GetEdmModel();
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