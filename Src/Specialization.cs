/**
 * @Name Generics.cs
 * @Purpose 
 * @Date 17 July 2021, 06:29:55
 * @Author S.Deckers
 * @Description 
 */

namespace Whoua.Core.Api
{
	internal sealed partial class AssemblyProperties
	{
		public const string Author		= "SDE Computing";
		public const string Description = "Foo Rulez";
		public const string Name		= "whatever zTuff";
		public const string Version		= "1.2";
	}
}

namespace Whoua.Core.Api.Controllers
{
	#region -- Using directives --
	using System;
	using System.Text;
	using System.Collections.Generic;
	using System.Linq;
	using Microsoft.AspNetCore.Mvc;
	
	using Swashbuckle.AspNetCore.Annotations;

	using Whoua.Core.Services;

	using d = System.Diagnostics.Debug;
	#endregion

	public class GenericController : ControllerBase
	{
		protected readonly IBla _bla;

		public GenericController( IBla bla)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );
			this._bla = bla;
		}
	}

	[ ApiController	( )]
	[ Route			( "[controller]" )]
	public class AController : GenericController
	{
		public AController( IBla bla)
			: base( bla)
		{			
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );
		}

		[ HttpGet( )]
		[ SwaggerOperation( Tags = new[] { "A in control" }, Summary = "Retrieve single instance" )]
		public ActionResult<string> Get( )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			string s=string.Format( "Pietje Puk a {0}e", System.DateTime.Now.Second);
			this._bla.DoIt( );
			return( s);
		}
	}

	[ ApiController	( )]
	[ Route			( "[controller]" )]
	public class BController : GenericController
	{
		public BController( IBla bla)
			: base( bla)
		{			
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );
		}

		[ HttpGet( )]
		[ SwaggerOperation( Tags = new[] { "B in control" }, Summary = "Retrieve single instance" )]
		public ActionResult<string> Get( )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			string s=string.Format( "Pietje Puk b {0}e", System.DateTime.Now.Second);
			this._bla.DoIt( );
			return( s);
		}
	}
}

namespace Whoua.Core.Services
{
	#region -- Using directives --
	using System;
	using Whoua.Core.Api;
	using d=System.Diagnostics.Debug;
	#endregion

	public interface IBla
	{
		void DoIt();
	}

	public class Bla : IBla
	{
		public void DoIt( )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));
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
    using Microsoft.OpenApi.Models;

	using Whoua.Core.Services;

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

		/// <summary>
		/// ConfigureServices
		/// </summary>
		/// <param name="services"></param>
		public void ConfigureServices( IServiceCollection services )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

			services.AddControllers( );

			services.AddSwaggerGen( c =>
			{
				c.SwaggerDoc( name: "v1", info: new OpenApiInfo { Title = "Generics", Version = "v1" } );

				c.EnableAnnotations();
			});

			services.AddScoped<IBla,			Bla>();
		}

		/// <summary>
		/// Configure
		/// </summary>
		/// <param name="app"></param>
		/// <param name="env"></param>
		public void Configure( IApplicationBuilder app, IWebHostEnvironment env )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

			app.UseDeveloperExceptionPage( );
			app.UseSwagger( );
			app.UseSwaggerUI( c => c.SwaggerEndpoint( url:"/swagger/v1/swagger.json", name:"Generics v1" ) );

			app.UseHttpsRedirection	( );
			app.UseRouting			( );
			app.UseAuthorization	( );

			app.UseEndpoints( endpoints =>
			{
				 endpoints.MapControllers( );
			} );
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