/**
 * @Name SwaggerPublicRoles.cs
 * @Purpose Tonen bepaalde gedeeltes Swagger-document gebaseerd op rollen
 * @Date 04 June 2022, 07:32:52
 * @Author S.Deckers
 * @url https://medium.com/@alexandre.malavasi/show-only-specific-apis-on-swagger-asp-net-core-aff91f6b9834
 * @Description 
 */

namespace Whoua.Core.Api.Controllers
{
	#region -- Using directives --

    #region -- System --
	using System;
    using System.Text;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
    using System.IdentityModel.Tokens.Jwt;
	using Whoua.Core.Api.Model;
	using Swashbuckle.AspNetCore.Annotations;
    #endregion

    #region -- Microsoft --
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.Extensions.Logging;
    #endregion

	using d=System.Diagnostics.Debug;
	#endregion

	[ ApiController	( )]
	[ Route			( "api/[controller]" )]
	[ Produces		( "application/json")]
	public class WeatherForecastController : Controller
	{
		private static readonly string[] Summaries = new[]
		{
			"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
		};

		[ HttpGet	(  )]
		[ Route		( "Get")]
		public IEnumerable<WeatherForecast> Get( )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			return Enumerable.Range( 1, 5 ).Select( index => new WeatherForecast
			{
				Id				= index
			,	Date			= DateTime.Now.AddDays( index )
			,	TemperatureC	= Random.Shared.Next( -20, 55 )
			,	Summary			= Summaries[Random.Shared.Next( Summaries.Length )]
			} )
			.ToArray( );
		}

		[ HttpGet	(  )]
		[ Route		( "PublicGet")]
		public WeatherForecast PublicGet( int initialRange, int finalRange)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			return Enumerable.Range( initialRange, finalRange).Select( index => new WeatherForecast
			{
				Id				= index
			,	Date			= DateTime.Now.AddDays( index )
			,	TemperatureC	= Random.Shared.Next( -20, 55 )
			,	Summary			= Summaries[Random.Shared.Next( Summaries.Length )]
			} )
			.FirstOrDefault( );
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

	#region -- System --
	using System;
	using System.Text;
	using System.IO;
	using System.Linq;	
	using System.Reflection;
	using System.Collections.Generic;
	#endregion

	#region -- Microsoft --
	using Microsoft.AspNetCore.Authentication.JwtBearer; // --- PM>install-package Microsoft.AspNetCore.Authentication.JwtBearer
	using Microsoft.EntityFrameworkCore;
	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Hosting;
	using Microsoft.AspNetCore.Mvc.Controllers;
	using Microsoft.IdentityModel.Tokens;
	#endregion

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
		public static int	 CallCount		= 0;
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

			services.AddControllers( );

			string srcTitle		= "Show only specific APIs on Swagger — Asp.Net Core";
			string src			= "https://medium.com/@alexandre.malavasi/show-only-specific-apis-on-swagger-asp-net-core-aff91f6b9834";
			string theUrl		= @$"<a href={src} target=_blank>{srcTitle}</a>";
			string description	= $"Only show public methods Swagger document<br/><br/>{theUrl}";

			services.AddSwaggerGen( c =>
			{
				c.SwaggerDoc							( name: "v1", info: new OpenApiInfo { Title = "SwaggerRoles", Version = "v1", Description = description } );
				c.EnableAnnotations						( );
				c.ResolveConflictingActions				( apiDescription => apiDescription.First());
				c.DocumentFilter<CustomSwaggerFilter>	( ); // --- Uncomment to view all methods (20220604 SDE)
			});
		}

		public void Configure( IApplicationBuilder app, IWebHostEnvironment env )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			app.UseDeveloperExceptionPage	( );
            app.UseHttpsRedirection			( );
            app.UseRouting					( );
            app.UseAuthorization			( );
            app.UseEndpoints				( e => { e.MapControllers(); });
            app.UseAuthentication			( );            
            app.UseSwagger					( );
            app.UseSwaggerUI				( c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Foo API v1"); });
		}
	}
}

namespace Whoua.Core.Api.Supporting
{
	#region -- Using directives --

	#region -- System --
	using System;
	using System.Linq;	
	using System.Reflection;
	using System.Collections.Generic;
	#endregion

	#region -- Microsoft --
	using Microsoft.OpenApi.Models;
	#endregion

	#region -- Swagger --	
	using Swashbuckle.AspNetCore.SwaggerGen;
	#endregion

	#region -- Whoua --
	using d = System.Diagnostics.Debug;	
	#endregion

	#endregion

    public class CustomSwaggerFilter : IDocumentFilter
    {
        public void Apply( OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

            var routes = swaggerDoc.Paths
                .Where(x => !x.Key.ToLower().Contains("public"))
                .ToList();

			foreach( var item in routes)
			{
				d.WriteLine( $"key=[{item.Key}], value={item.Value.Description}");
			}
            routes.ForEach(x => { swaggerDoc.Paths.Remove(x.Key); });
        }
    }
}