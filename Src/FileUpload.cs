/**
 * @Name FileUpload.cs
 * @Purpose 
 * @Date 18 January 2022, 10:49:30
 * @Author S.Deckers
 * @Description
 * @url https://karthiktechblog.com/aspnetcore/how-to-upload-a-file-with-net-core-web-api-3-1-using-iformfile
 */

namespace Whoua.Core.Api.Controllers
{
	#region -- Using directives --
	using System;
	using System.Text;
	using System.ComponentModel.DataAnnotations;
	using System.Collections.Generic;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Hosting;
	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.OData.Edm;
	using Microsoft.OpenApi.Models;
	using d=System.Diagnostics.Debug;
	using Microsoft.AspNetCore.Http;
	using System.Threading.Tasks;
	using System.Threading;
	using System.IO;
	using System.Linq;
	using Swashbuckle.AspNetCore.Annotations;
	#endregion

	[ Microsoft.AspNetCore.Mvc.ApiController	( )]	
	[ Microsoft.AspNetCore.Mvc.Route			("api/[controller]/[action]")]
	public class FooController : Microsoft.AspNetCore.Mvc.ControllerBase
	{
		private const string swaggerTitle = "Upload demo";

		[ HttpPost				( Name = "Foo2" )]
		[ ProducesResponseType	( StatusCodes.Status200OK )]
		[ ProducesResponseType	( typeof( string ), StatusCodes.Status400BadRequest )]
		[ SwaggerOperation		( Tags = new[] { FooController.swaggerTitle }, Summary = "FileContentResult" )]
		public FileContentResult Foo2
		( 
			[ FromForm()]	ImportRequest	r
		,					IFormFile		file
		)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			List<string> issues = new List<string>( );

			issues.Add( "Issue 1");
			issues.Add( "Issue 2");

			string theFile = writeIssuesToFile( issues);

			FileStream stream = System.IO.File.OpenRead( theFile);

			MemoryStream ms = new MemoryStream( );
			stream.CopyTo( ms);
			var bytes = ms.ToArray( );
			ms.Dispose( );

			string mimeType = "application/octet-stream";
			return new FileContentResult(bytes, mimeType)
			{
				FileDownloadName = "importresult.txt"
			};
		}

		private string writeIssuesToFile( List<string> issues )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));
    
			string theFile = Path.GetTempFileName();

			System.IO.File.WriteAllLines( theFile, issues);

			return( theFile);
		}

		[ HttpPost				( Name = "Foo" )]
		[ ProducesResponseType	( StatusCodes.Status200OK )]
		[ ProducesResponseType	( typeof( string ), StatusCodes.Status400BadRequest )]
		[ SwaggerOperation		( Tags = new[] { FooController.swaggerTitle }, Summary = "FileStreamResult" )]
		public FileStreamResult Foo
		( 
			[ FromForm()]	ImportRequest	r
		,					IFormFile		file
		)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			List<ImportResult> result = new List<ImportResult>( );

			// --- Do some checking on the request over here (20220201 SDE)

			if( string.IsNullOrEmpty( r.Owner))
			{
				result.Add( new ImportResult( ) { Issue = "Owner is a required field" });
			}

			if( string.IsNullOrEmpty( r.SomeField))
			{
				result.Add( new ImportResult( ) { Issue = "SomeField is a required field" });
			}

			// --- The import (20220201 SDE)

			if( file == null)
			{
				result.Add( new ImportResult( ) { Issue = "No file detected" });
				//return( result);
			}

			string theFile = writeFileSync( file);

			var stream = System.IO.File.OpenRead( theFile);
			return new FileStreamResult(stream, "application/octet-stream");
		}

		[ HttpPost				( Name = "Wrapped" )]
		[ ProducesResponseType	( StatusCodes.Status200OK )]
		[ ProducesResponseType	( typeof( string ), StatusCodes.Status400BadRequest )]
		[ SwaggerOperation		( Tags = new[] { FooController.swaggerTitle }, Summary = "IForm type wrapped in a request" )]
		public List<ImportResult> Wrapped
		( 
			[ FromForm()]	ImportRequest	r
		,					IFormFile		file
		)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			List<ImportResult> result = new List<ImportResult>( );

			// --- Do some checking on the request over here (20220201 SDE)

			if( string.IsNullOrEmpty( r.Owner))
			{
				result.Add( new ImportResult( ) { Issue = "Owner is a required field" });
			}

			if( string.IsNullOrEmpty( r.SomeField))
			{
				result.Add( new ImportResult( ) { Issue = "SomeField is a required field" });
			}

			// --- The import (20220201 SDE)

			if( file == null)
			{
				result.Add( new ImportResult( ) { Issue = "No file detected" });
				return( result);
			}

			writeFileSync( file );

			int linenum		= 1;
			int issueCount	= 1;
			result.Add( new ImportResult( ) { LineNum = linenum++, Line = "POPA, 12, 23", Issue = $"There is an issue" });
			result.Add( new ImportResult( ) { LineNum = linenum++, Line = "POPB, 12, 23", Issue = $"issue { issueCount++}" });
			result.Add( new ImportResult( ) { LineNum = linenum++, Line = "POPC, 12, 23", Issue = $"Very bad issue { issueCount++}" });

			return( result);
		}

		[ HttpPost				( Name = "SomeMethod2" )]
		[ ProducesResponseType	( StatusCodes.Status200OK )]
		[ ProducesResponseType	( typeof( string ), StatusCodes.Status400BadRequest )]
		[ SwaggerOperation		( Tags = new[] { FooController.swaggerTitle }, Summary = "Return array of objects" )]
		public List<ImportResult> d2( IFormFile file)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			List<ImportResult> result = new List<ImportResult>( );

			if( file == null)
			{
				result.Add( new ImportResult( ) { Issue = "No file detected" });
				return( result);
			}

			writeFileSync( file );

			int linenum		= 1;
			int issueCount	= 1;
			result.Add( new ImportResult( ) { LineNum = linenum++, Line = "POPA, 12, 23", Issue = $"There is an issue" });
			result.Add( new ImportResult( ) { LineNum = linenum++, Line = "POPB, 12, 23", Issue = $"issue { issueCount++}" });
			result.Add( new ImportResult( ) { LineNum = linenum++, Line = "POPC, 12, 23", Issue = $"Very bad issue { issueCount++}" });

			return( result);
		}

		[ HttpPost				( Name = "SomeMethod1" )]
		[ ProducesResponseType	( StatusCodes.Status200OK )]
		[ ProducesResponseType	( typeof( string ), StatusCodes.Status400BadRequest )]
		[ SwaggerOperation		( Tags = new[] { FooController.swaggerTitle }, Summary = "Return array of strings" )]
		public List<string> d1( IFormFile file)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			List<string> result = new List<string>( );

			if( file == null)
			{
				result.Add( "No file found");
				return( result);
				//return( NoContent());
			}

			writeFileSync( file );

			int linenum = 1;
			result.Add( "Import succeeded");
			result.Add(  $"On second thought");
			result.Add(  $"{ linenum++ }:No a valid connection");
			result.Add(  $"{ linenum++ }:No a valid pop");
			result.Add(  $"{ linenum++ }:Another problem");

			return( result);
		}

		/// <summary>
		/// Waar dient het 'template'-attribuut in de httppost voor ? 
		/// Dat is de naam van een routing template
		/// </summary>
		/// <param name="file">The file.</param>
		/// <returns></returns>
		[ HttpPost				( Name = "uploadsyncNT" )]
		[ ProducesResponseType	( StatusCodes.Status200OK )]
		[ ProducesResponseType	( typeof( string ), StatusCodes.Status400BadRequest )]
		[ SwaggerOperation		( Tags = new[] { FooController.swaggerTitle }, Summary = "Upload file sync-NT" )]
		public IActionResult UploadFileSyncNoTemplate( IFormFile file)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			if( file == null)
			{
				return( NoContent());
			}

			writeFileSync( file );

			return( Ok( ));
		}

		[ HttpPost				( template:"upload", Name = "uploadsync" )]
		[ ProducesResponseType	( StatusCodes.Status200OK )]
		[ ProducesResponseType	( typeof( string ), StatusCodes.Status400BadRequest )]
		[ SwaggerOperation		( Tags = new[] { FooController.swaggerTitle }, Summary = "Upload file sync" )]
		public IActionResult UploadFileSync( IFormFile file)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			if( file == null)
			{
				return( NoContent());
			}

			writeFileSync( file );

			return( Ok( ));
		}

		private string writeFileSync( IFormFile file)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

			string fileName;

			var extension = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];
			fileName = DateTime.Now.Ticks + extension; //Create a new Name for the file due to security reasons.

			var pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\files");

			if (!Directory.Exists(pathBuilt))
			{
				Directory.CreateDirectory(pathBuilt);
			}

			var path = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\files",fileName);

			// --- Show contents (20220131 SDE)

			var result = new StringBuilder();
			using (var reader = new StreamReader(file.OpenReadStream()))
			{
				while (reader.Peek() >= 0)
				{
					string theLine  = reader.ReadLine();
					d.WriteLine( theLine);
					result.AppendLine( theLine); 
				}
			}
    
			using (var stream = new FileStream(path, FileMode.Create))
			{
				file.CopyTo(stream);
			}

			string theFile = string.Format( "{0}", path);
			return( theFile);
		}

		[ HttpPost				( template:"upload", Name = "upload2")]
		[ ProducesResponseType	( StatusCodes.Status200OK)]
		[ ProducesResponseType	( typeof(string), StatusCodes.Status400BadRequest)]
		[ SwaggerOperation		( Tags = new[] { FooController.swaggerTitle }, Summary = "Upload file Async" )]
		public async Task<IActionResult> UploadFileASync( IFormFile file, System.Threading.CancellationToken cancellationToken)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

			if( file == null)
			{
				return( NoContent());
			}

			await writeFileASync( file);

			return Ok();
		}

		private async Task<bool> writeFileASync( IFormFile file)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

			bool isSaveSuccess = false;
			string fileName;
			try
			{
				var extension = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];
				fileName = DateTime.Now.Ticks + extension; //Create a new Name for the file due to security reasons.

				var pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\files");

				if (!Directory.Exists(pathBuilt))
				{
					Directory.CreateDirectory(pathBuilt);
				}

				var path = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\files", fileName);

				using (var stream = new FileStream(path, FileMode.Create))
				{
					await file.CopyToAsync(stream);
				}

				isSaveSuccess = true;
			}
			catch (Exception ex)
			{
				d.WriteLine( ex.ToString());
			}

			return isSaveSuccess;
		}

		public class ImportRequest
		{
			[ Required	( )]
			public string		Owner		{ get; set; }

			[ Required	( )]
			public string		SomeField	{ get; set; }
		}

		public class ImportResult
		{
			public int			LineNum		{ get; set; }
			public string		Line		{ get; set; }
			public string		Issue		{ get; set; }
		}
	}

	/// <summary date="03-06-2021, 16:07:16" author="S.Deckers">
	/// Startup class
	/// </summary>
	public class Startup
	{
		private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;

		public Startup( Microsoft.Extensions.Configuration.IConfiguration configuration)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

			this._configuration = configuration;
		}

		public void ConfigureServices( Microsoft.Extensions.DependencyInjection.IServiceCollection services)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

			services.AddControllers	( );
			//services.AddSwaggerGen	( );

			services.AddSwaggerGen( c =>
			{
				c.SwaggerDoc( name: "v1", info: new OpenApiInfo { Title = "FileUpload", Version = "v1" } );
				c.EnableAnnotations();
			});
		}

		public void Configure( Microsoft.AspNetCore.Builder.IApplicationBuilder app)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

			app.UseSwagger	( );
			app.UseSwaggerUI( c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"); });
			app.UseRouting	( );
			app.UseEndpoints( c => { c.MapControllers();	});
		}
	}

	public static class Global
	{
		public static int CallCount = 0;
	}

	/// <summary date="03-06-2021, 16:09:17" author="S.Deckers">
	/// Entry point
	/// </summary>
	public partial class Program
	{
		public static void Main( string[] args )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", typeof( Program).Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

			 CreateHostBuilder( args).Build().Run( );
		}

		public static Microsoft.Extensions.Hosting.IHostBuilder CreateHostBuilder( string[] args ) =>
			Host.CreateDefaultBuilder( args )
				.ConfigureWebHostDefaults( webBuilder =>
				 {
					 webBuilder.UseStartup<Startup>( );
				 } );
	}
}