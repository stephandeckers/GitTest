/**
 * @Name Generics.cs
 * @Purpose 
 * @Date 17 July 2021, 06:29:55
 * @Author S.Deckers
 * @Description Voorbeeld hoe je met behulp van Generics een structuur kunt opzetten waarbij meerdere controllers
 * dezelfde interface implementeren, maar de implementatie middels generics wordt gedaan en je dus niet elke
 * keer weer opnieuw de code hoeft te schrijven
 */

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

	[ ApiController	( )]
	[ Route			( "[controller]" )]
	public class AController 
	{
		private readonly IA				_a;

		public AController( IA a)
		{			
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			this._a				= a;
		}

		[ HttpGet( )]
		[ SwaggerOperation( Tags = new[] { "A in control" }, Summary = "Retrieve single instance" )]
		public ActionResult<string> Get( )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			string s=string.Format( "Pietje Puk a {0}e", System.DateTime.Now.Second);
			this._a.DoItA( );

			if( this._a is IServiceable svc)
			{
				svc.VeryComplicatedMethod( );
			}

			return( s);
		}
	}

	[ ApiController	( )]
	[ Route			( "[controller]" )]
	public class BController
	{
		private readonly IB				_b;

		public BController( IB b)
		{			
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );
			this._b				= b;
		}

		[ HttpGet( )]
		[ SwaggerOperation( Tags = new[] { "B in control" }, Summary = "Retrieve single instance" )]
		public ActionResult<string> Get( )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			string s=string.Format( "Pietje Puk b {0}e", System.DateTime.Now.Second);
			this._b.DoItB( );

			if( this._b is IServiceable svc)
			{
				svc.VeryComplicatedMethod( );
			}
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

	public interface IServiceable
	{
		void Foo();
		void VeryComplicatedMethod();
	}

	public class Servicable<T> : IServiceable
	{
		public void Foo( )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));
			d.WriteLine( string.Format( "The type={0}", this.GetType().ToString( ) ));
		}

		public void VeryComplicatedMethod( )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));
			d.WriteLine( string.Format( "The type={0}", this.GetType().ToString( ) ));
		}
	}

	public interface IA : IServiceable
	{
		void DoItA();
	}

	public class A : Servicable<A>,IA
	{
		public void DoItA( )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

			Foo( );
		}
	}

	public interface IB: IServiceable
	{
		void DoItB();
	}

	public class B: Servicable<B>, IB
	{
		public void DoItB( )
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
				c.SwaggerDoc		( name: "v1", info: new OpenApiInfo { Title = "Generics", Version = "v1" } );
				c.EnableAnnotations	( );
			});

			services.AddScoped<IA,		A>();
			services.AddScoped<IB,		B>();
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