/**
 * @Name Patch6.cs
 * @Purpose 
 * @Date 23 May 2022, 07:27:33
 * @Author S.Deckers
 * @Description 
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
	using Microsoft.Extensions.Logging;
	using Microsoft.VisualStudio.Web.CodeGeneration;
	using Microsoft.AspNetCore.Hosting;

	#endregion

	[ ApiController	( )]
	//[ Route			( "[controller]" )]
	[ Route			( "api/[controller]") ]
	//[RoutePrefix("api")]
	public class PictureController : ControllerBase
	{
		private readonly	IPictureService				_pictureService;
		private readonly	ILogger<PictureController>	_logger;
		private readonly	IWebHostEnvironment			_webHostEnvironment;

		public PictureController
		( 
			IPictureService				pictureService
		,	ILogger<PictureController>	logger
		,	IWebHostEnvironment			webHostEnvironment
		)
		{			
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			this._pictureService		= pictureService;
			this._logger				= logger;
			this._webHostEnvironment	= webHostEnvironment;
		}

		[ HttpGet( "v1")]
		[ SwaggerOperation( Tags = new[] { "s1:Get WebRoot" }, Summary = "Get WebRoot" )]
		public ActionResult GetWebRoot( )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

            string contentRootPath	= this._webHostEnvironment.ContentRootPath;
            string webRootPath		= this._webHostEnvironment.WebRootPath;

			return Content(contentRootPath + "\n" + webRootPath);
		}

		[ HttpGet( "v2")]
		[SwaggerOperation( Tags = new[] { "s2:Get Picture" }, Summary = "Get picture without a service. Swagger will render the picture" )]
		public ActionResult GetPictureNoService( )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			string webRootPath = this._webHostEnvironment.WebRootPath;

			string sampleFile = "0000233_verzamelmunten_kopen.jpeg";
			string picture = $@"{webRootPath}\images\{sampleFile}";

			d.WriteLine( $"[{picture}]" );
			PhysicalFileResult physicalFileResult = PhysicalFile( @picture, "image/jpg" );

			return (physicalFileResult);
		}

		[ HttpGet( "v3")]
		[ SwaggerOperation( Tags = new[] { "s3:Get Picture Serviced" }, Summary = "Get picture with a service. Swagger will render the picture" )]
		public ActionResult GetPictureService( )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

   //         string webRootPath		= this._webHostEnvironment.WebRootPath;
			
			//string sampleFile	= "0000233_verzamelmunten_kopen.jpeg";
			//string picture		= $@"{webRootPath}\images\{sampleFile}";

			int thePicture = 345;
			string absoluteFilePath = this._pictureService.GetPicture( thePicture);
			PhysicalFileResult physicalFileResult = PhysicalFile(@absoluteFilePath, "image/jpg");

			return( physicalFileResult);
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

	public enum PictureType
	{
		bmp		// --- ".bmp",
	,	gif		// --- ".gif",
	,	jpeg	// --- ".jpeg",
	,	jpg		// --- ".jpg",
	,	jpe		// --- ".jpe",
	,	jfif	// --- ".jfif",
	,	pjpeg	// ---  ".pjpeg",
	,	pjp		// --- ".pjp",
	,	png		// --- ".png",
	,	tiff	// --- ".tiff",
	,	tif		// --- ".tif"
	}

	public interface IPictureService
	{
		string GetPicture( int pictureId);
	}

	public class PictureService : IPictureService
	{
		public string GetPicture( int pictureId)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

			string thePath = string.Empty;
			return( thePath);
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
	using Microsoft.Extensions.Logging;
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
				c.SwaggerDoc( name: "v1", info: new OpenApiInfo { Title = "Picture demo", Version = "v1" } );
				c.EnableAnnotations();
			});

			services.AddScoped<IPictureService,			PictureService>();
		}

		/// <summary>
		/// Configure
		/// </summary>
		/// <param name="app"></param>
		/// <param name="env"></param>
		//public void Configure( IApplicationBuilder app, IWebHostEnvironment env )
		public void Configure
		( 
			IApplicationBuilder app
		,	IWebHostEnvironment env
		,	ILoggerFactory loggerFactory
		)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

			app.UseDeveloperExceptionPage	( );
			app.UseSwagger					( );
			app.UseSwaggerUI				( c => c.SwaggerEndpoint( url:"/swagger/v1/swagger.json", name:"Picturedemo v1" ) );
			app.UseHttpsRedirection			( );
			app.UseRouting					( );
			app.UseAuthorization			( );
			app.UseEndpoints				( endpoints =>	{ endpoints.MapControllers( );} );
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