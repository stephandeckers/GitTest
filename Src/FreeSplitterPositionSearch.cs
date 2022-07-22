/**
 * @Name FreeSplitterPositionSearch.cs
 * @Purpose 
 * @Date 24 January 2022, 10:07:03
 * @Author S.Deckers
 * @Description 
 */

namespace Whoua.Core.Api.Controllers
{
	#region -- Using directives --
	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.Linq;
	using System.ComponentModel.DataAnnotations;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Hosting;
	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.OpenApi.Models;	
	using Microsoft.AspNetCore.Http;
	using Swashbuckle.AspNetCore.Annotations;
	using d=System.Diagnostics.Debug;
	using System.Text.Json.Serialization;
	#endregion

	internal static class SwaggerDescription
	{
		internal const string Title				= "Foo";
		internal const string Summary			= "FreeSplitterPositionSearch demo";
		internal const string OpenApiInfoTitle	= "Ze demo";
	}

	[ Microsoft.AspNetCore.Mvc.ApiController	( )]	
	[ Microsoft.AspNetCore.Mvc.Route			("api/[controller]/[action]")]
	public class FooController : Microsoft.AspNetCore.Mvc.ControllerBase
	{
		[ HttpPost				( Name = "EnumRequest" )]
		[ ProducesResponseType	( StatusCodes.Status200OK )]
		[ ProducesResponseType	( typeof( string ), StatusCodes.Status400BadRequest )]
		[ SwaggerOperation		( Tags = new[] { SwaggerDescription.Title }, Summary = SwaggerDescription.Summary )]
		public IActionResult DoIt2
		( 
			YesNo yesNo
		)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			d.WriteLine( yesNo.ToString());
			return( Ok( "Yep"));
		}

		[ HttpPost				( Name = "FreeSplitterPositionSearch" )]
		[ ProducesResponseType	( StatusCodes.Status200OK )]
		[ ProducesResponseType	( typeof( string ), StatusCodes.Status400BadRequest )]
		[ SwaggerOperation		( Tags = new[] { SwaggerDescription.Title }, Summary = SwaggerDescription.Summary )]
		public IQueryable<FreeSplitterPositionSearchResponse> DoIt
		( 
			[ FromBody()] FreeSplitterPositionSearchRequest r
		)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			List<FreeSplitterPositionSearchResponse> items = new List<FreeSplitterPositionSearchResponse>()
			{
				new FreeSplitterPositionSearchResponse( ) { SplitterPosition = new SrPosition( ) { P1 = "sp1", P2 = "sp2" }, OLTPosition = new SrPosition( ) { P1 = "op1", P2 = "op2" } }
			,	new FreeSplitterPositionSearchResponse( ) { SplitterPosition = new SrPosition( ) { P1 = "sp3", P2 = "sp4" }, OLTPosition = new SrPosition( ) { P1 = "op3", P2 = "op4" } }
			};

			return( items.AsQueryable( ));
		}
	}

	public enum YesNo
	{
		Yes
	,	No
	};

	public class FreeSplitterPositionSearchRequest
	{
		[ Required( )]
		public string	Owner					{ get; set; }
		[ Required( )]
		public string	PopName					{ get; set; }
		public int?		OdfTrayNr				{ get; set; }
		public string	PositionStatus			{ get; set; }
		public string	RelatedOltPositionType	{ get; set; }
		public int?		PosToRetrieve			{ get; set; }
		public string	PopStatus				{ get; set; }
		public YesNo	PopStatus1				{ get; set; }

		public override string ToString( )
		{
			StringBuilder sb = new StringBuilder( );
			sb.AppendFormat( string.Format( "Owner={0}",					Owner));
			sb.AppendFormat( string.Format( ", PopName={0}",				PopName));
			sb.AppendFormat( string.Format( ", OdfTrayNr={0}",				OdfTrayNr));
			sb.AppendFormat( string.Format( ", PositionStatus={0}",			PositionStatus));
			sb.AppendFormat( string.Format( ", RelatedOltPositionType={0}", RelatedOltPositionType));
			sb.AppendFormat( string.Format( ", PosToRetrieve={0}",			PosToRetrieve));
			return( sb.ToString());
		}

		internal void Dump()
		{
			d.WriteLine( this.ToString());
		}
	}

	public class FreeSplitterPositionSearchResponse
	{
		public SrPosition SplitterPosition	{ get; set; }
		public SrPosition OLTPosition		{ get; set; }
	}

	public partial class SrPosition
	{
		public string	P1					{ get; set; }
		public string	P2					{ get; set; }
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

			//services.AddControllers	( );
			services.AddControllers().AddJsonOptions(x =>
			{
				// serialize enums as strings in api responses (e.g. Role)
				x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
			});

			services.AddSwaggerGen( c =>
			{
				c.SwaggerDoc( name: "v1", info: new OpenApiInfo { Title = SwaggerDescription.OpenApiInfoTitle, Version = "v1" } );
				c.EnableAnnotations();
			});
		}

		public void Configure( Microsoft.AspNetCore.Builder.IApplicationBuilder app)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

			app.UseSwagger	( );
			app.UseSwaggerUI( c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Foo rUlez"); });
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