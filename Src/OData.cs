/**
 * @Name OData.cs
 * @Purpose 
 * @Date 03 June 2021, 18:54:51
 * @Author S.Deckers
 * @Description 
 * @notes Go to http://localhost:14875/odata/students?$select=name to see OData
 * 
 * PM> install-package microsoft.aspnetcore.odata
 * 
 * @url https://devblogs.microsoft.com/odata/experimenting-with-odata-in-asp-net-core-3-1/
 * 
 * model annotations swagger  :  https://blog.georgekosmidis.net/2020/07/11/swagger-in-asp-net-core-tips-and-tricks/
 */

namespace Whoua.Core.Api.Controllers
{
	#region -- Using directives --
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Microsoft.AspNetCore.Http;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.Extensions.Logging;
	using Microsoft.Extensions.Hosting;
	using Microsoft.Net.Http.Headers;

	using Microsoft.AspNet.OData.Builder;
	using Microsoft.AspNet.OData.Extensions;
	using Microsoft.AspNet.OData.Formatter;
	using Microsoft.OData.Edm;
	using Microsoft.OpenApi.Models;

	using Swashbuckle.AspNetCore.Annotations;

	using Whoua.Core.Api.Model;

	using d = System.Diagnostics.Debug;
	
	#endregion

    [ Microsoft.AspNetCore.Mvc.Route				( "odata/[controller]")]
    [ Microsoft.AspNetCore.Mvc.ApiExplorerSettings	( IgnoreApi = false)]
	[ SwaggerTag( description:"Thoug need this controller", externalDocsUrl:"http://www.xs4all.nl")]
	public class FooController : Microsoft.AspNet.OData.ODataController
	{
		[ HttpGet( )]
		[ SwaggerResponse( statusCode:200, description:"Everyting OK", type:typeof(string))]
		[ SwaggerResponse( 400, "BAD request")]
		[ SwaggerOperation( Tags = new [] { "Complicated Controller" })] // --- Er komt een aparte sectie in Swagger UI met als title "Complicated Controller"
		public ActionResult<string> YouCanGiveAnyNameToThisMethod( )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			string s=string.Format( "Pietje Puk de {0}e", System.DateTime.Now.Second);
			return( s);
		}

		[ HttpPost( )]
		[ Swashbuckle.AspNetCore.Annotations.SwaggerOperation( Summary = "Very complicated post", Description = "Though shall post")]
		[ SwaggerResponse( 200, "Everyting OK")]
		[ SwaggerResponse( 400, "BAD request")]
		[ ProducesResponseType( StatusCodes.Status201Created)]  
		public IActionResult SamplePost( string s1)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			string s= string.Format( "[{0}] was posted", s1);
			d.WriteLine( s);

			return( Ok( s));
		}

		[ HttpDelete( )]
		[ ApiExplorerSettings(GroupName="XYZ - A collection of XYZ APIs")]
		public IActionResult SampleDelete( string arg)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			string s= string.Format( "[{0}] was deleted", arg);
			d.WriteLine( s);

			return( Ok( s));
		}

		/// <summary>
		/// Put werkt via de uri : http://localhost:14875/Foo?s=aaa
		/// </summary>
		/// <param name="s"></param>
		[ HttpPut( )]
		public IActionResult SamplePut( string arg)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			string s= string.Format( "[{0}] was put", arg);
			d.WriteLine( s);

			return( Ok( s));
		}

		/// <summary>
		/// Patch werkt via de body
		/// </summary>
		/// <param name="s"></param>
		[ HttpPatch( )]
		public IActionResult SamplePatch( string p)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			string s= string.Format( "[{0}] was patched", p);
			d.WriteLine( s);

			return( Ok( s));
		}
	}

	[ Microsoft.AspNetCore.Mvc.Route				( "odata/[controller]")]
    [ Microsoft.AspNetCore.Mvc.ApiExplorerSettings	( IgnoreApi = false)]
	public class WeatherForecastController : Microsoft.AspNet.OData.ODataController
	{
		[ HttpGet( )]
		public ActionResult<WeatherForecast> Get( )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			//string s=string.Format( "Pietje Puk de {0}e", System.DateTime.Now.Second);
			WeatherForecast w = new WeatherForecast( );
			return( w);
		}

		[ HttpPost( )]		
		public IActionResult Post( WeatherForecast w)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			string s= string.Format( "[{0}] was posted", w.ToString( ));
			d.WriteLine( s);

			return( Ok( s));
		}

		[ HttpDelete( )]		
		public IActionResult Delete( int id)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			string s= string.Format( "[{0}] was deleted", id);
			d.WriteLine( s);

			return( Ok( s));
		}

		/// <summary>
		/// Put werkt via de uri : http://localhost:14875/Foo?s=aaa
		/// </summary>
		/// <param name="s"></param>
		[ HttpPut( )]
		public IActionResult Put( WeatherForecast w)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			string s= string.Format( "[{0}] was put", w.ToString( ));
			d.WriteLine( s);

			return( Ok( s));
		}

		/// <summary>
		/// Patch werkt via de body
		/// </summary>
		/// <param name="s"></param>
		[ HttpPatch( )]
		public IActionResult Patch( WeatherForecast w)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			string s= string.Format( "[{0}] was patched", w.ToString( ));
			d.WriteLine( s);

			return( Ok( s));
		}
	}

	public class Startup
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Startup"/> class.
		/// </summary>
		/// <param name="configuration">The configuration.</param>
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
           
            services.AddControllers	( );

			services.AddOData		( );

			// --- Beschikbare pages die je krijg na selectie Swagger DropDown

			services.AddSwaggerGen( c =>
			{
				c.SwaggerDoc( "v1", new OpenApiInfo { Title = "My API1", Version = "v1" } );
				c.SwaggerDoc( "v2", new OpenApiInfo { Title = "My API2", Version = "v2" } );
				c.SwaggerDoc( "matchingVersion", new OpenApiInfo { Title = "Title appearing when page is selected", Version = "keyNeeds2Match" } );
				c.SwaggerDoc( "v666", new OpenApiInfo { Title = "My API V666", Version = "v666" } );
				c.SwaggerDoc( name:swaggerVersion, info:new OpenApiInfo { Title = swaggerKey, Version = "v8 (any version)" } );
				c.EnableAnnotations( ); //install-package Swashbuckle.AspNetCore.Annotations

				c.SwaggerDoc( "v10", info:new OpenApiInfo( )
				{ 
					Title			= "ZE TITLE"
				,	Version			= "v10" 
				,	Description		= "This is to complicated for though"
				,	TermsOfService	= new Uri("https://example.com/terms")
				,	Contact			= new OpenApiContact { Name = "Your Name XYZ",  Email = "xyz@gmail.com",  Url = new Uri("https://example.com")  }
				,	License			= new OpenApiLicense { Name = "Use under OpenApiLicense",  Url = new Uri("https://example.com/license")	}  
				});

			} );


			//install-package Swashbuckle.AspNetCore.Annotations
			SetOutputFormatters(services);
		}

		private const string swaggerKey			= "Very Cool App";
		private const string swaggerVersion		= "v8";

        private static void SetOutputFormatters(IServiceCollection services)
        {
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", typeof( Program).Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

            services.AddMvcCore(options =>
            {
                IEnumerable<ODataOutputFormatter> outputFormatters = options.OutputFormatters.OfType<ODataOutputFormatter>()
                        .Where(formatter => formatter.SupportedMediaTypes.Count == 0);

                foreach (var outputFormatter in outputFormatters)
                {
                    outputFormatter.SupportedMediaTypes.Add( new Microsoft.Net.Http.Headers.MediaTypeHeaderValue("application/odata"));
                }
            });
        }

		/// <summary>
		/// Configure
		/// </summary>
		/// <param name="app"></param>
		/// <param name="env"></param>
		public void Configure( IApplicationBuilder app, IWebHostEnvironment env )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection	( );
            app.UseRouting			( );
            app.UseAuthorization	( );

			//app.UseEndpoints( );

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers			( );
                endpoints.EnableDependencyInjection	( );
                //endpoints.Select().Filter().Expand().MaxTop(10);
                endpoints.MapODataRoute( routeName:"odata", routePrefix:"odata", model:GetEdmModel());
            });

            app.UseSwagger	( );

			string swaggerEndpoint = string.Format( "/swagger/{0}/swagger.json", swaggerVersion);

			// --- Items in SwaggerDropdown (20210604 SDE)

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
				c.SwaggerEndpoint("/swagger/v2/swagger.json", "My API V2");
				c.SwaggerEndpoint( url:"/swagger/matchingVersion/swagger.json", name:"keyNeeds2Match");

				c.SwaggerEndpoint( url:"/swagger/v666/swagger.json", name:"My API V666");
				c.SwaggerEndpoint( url: swaggerEndpoint, name:swaggerKey);

				c.SwaggerEndpoint( url:"/swagger/v10/swagger.json", name:"v10");

				// --- Je kunt een customstylesheet injecten
				c.InjectStylesheet("/StyleSheet1.css");
            });
		}

		//c.SwaggerDoc( "v10", info:new OpenApiInfo( ){ Title = "ZE TITLE", Version = "v10" });
        Microsoft.OData.Edm.IEdmModel GetEdmModel()
        {
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", typeof( Program).Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));
            var builder = new ODataConventionModelBuilder();
            builder.EntitySet<WeatherForecast>("WeatherForecast");
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

namespace Whoua.Core.Api.Model
{
	#region -- Using directives --
	using System;
	#endregion

    public class WeatherForecast
    {
        public Guid			Id					{ get; set; }
        public DateTime		Date				{ get; set; }
        public int			TemperatureC		{ get; set; }
        public int			TemperatureF => 32 + (int)(TemperatureC / 0.5556);
        public string		Summary				{ get; set; }
    }
}