/**
 * @Name CertificateBasedAuthentication.v1.cs
 * @Purpose 
 * @Date 21 June 2022, 07:48:57
 * @Author S.Deckers
 * @url https://blog.kritner.com/2020/07/15/setting-up-mtls-and-kestrel/ 
 */

namespace Whoua.Core.Api.Controllers
{
	#region -- Using directives --
	using System;
	using Swashbuckle.AspNetCore.Annotations;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
	using d=System.Diagnostics.Debug;
	#endregion	

    [ Route("api/[controller]")]
    [ ApiController]
    public class TestController: ControllerBase 
	{
		private const string swaggerControllerDescription = "Certificatebased Authentication Sample";

        [ HttpGet	( )]
		[ SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "No certificate" )]
        public string Get()
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			string s = $"The action works fine without a certificate:{System.DateTime.Now}";
			return( s);
		}

        [ Authorize	( )]
        [ HttpPost	( )]
		[ SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "Certificate required" )]
        public string Post()
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			string s = $"The action works fine only with a certificate:{System.DateTime.Now}";
			return( s);
		}
    }
}

namespace Whoua.Core.Api
{
	#region -- Using directives --
	using System;
	using System.Linq;
	using System.Threading.Tasks;
	using System.Collections.Generic;
	using System.Reflection;
	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Hosting;
	using Microsoft.AspNetCore.Authentication.Certificate;
	using Swashbuckle.AspNetCore.SwaggerGen;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.OpenApi.Models;
	using Whoua.Core.Api.Supporting;
	using Microsoft.AspNetCore.Server.Kestrel.Https;
	using d = System.Diagnostics.Debug;
	using System.Security.Cryptography.X509Certificates;
	using Microsoft.Extensions.Logging;
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
					webBuilder.ConfigureKestrel (o => 
					{ 
						o.ConfigureHttpsDefaults(o => o.ClientCertificateMode = ClientCertificateMode.RequireCertificate);	 // --- Require certificate
					});
				 } );
	}

	public class Startup
	{
		public void Configure( IApplicationBuilder app, IWebHostEnvironment env )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			app.UseDeveloperExceptionPage	( );
            app.UseHttpsRedirection			( );
            app.UseRouting					( ); // --- 1. The order matters
			app.UseAuthentication			( ); // --- 2.
            app.UseAuthorization			( ); // --- 3.

            app.UseEndpoints				( e => { e.MapControllers(); });
                       
            app.UseSwagger					( );
            app.UseSwaggerUI				( c => 
			{ 
				c.SwaggerEndpoint	( "/swagger/v1/swagger.json", "Foo API v1"); 
				c.DisplayOperationId( );
			});
		}

		public void ConfigureServices( IServiceCollection services )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			services.AddControllers			( );
			services.AddHttpContextAccessor	( );

			string srcTitle		= $"MTLS authentication - {System.DateTime.Now.ToString()}";
			string src			= "https://blog.kritner.com/2020/07/15/setting-up-mtls-and-kestrel/";
			string theUrl		= @$"<a href={src} target=_blank>{srcTitle}</a>";
			string description	= $"Mutual Transport Layer Security<br/><br/>{theUrl}";

			services.AddSwaggerGen( c =>
			{
				c.SwaggerDoc									( name: "v1", info: new OpenApiInfo { Title = "Swagger Document filters", Version = "v1", Description = description } );
				c.EnableAnnotations								( );
				c.ResolveConflictingActions						( apiDescription => apiDescription.First());				
			});

			services
				.AddAuthentication( CertificateAuthenticationDefaults.AuthenticationScheme)
				.AddCertificate(options =>
				{
					// Only allow chained certs, no self signed
					options.AllowedCertificateTypes = CertificateTypes.Chained;
					// Don't perform the check if a certificate has been revoked - requires an "online CA", which was not set up in our case.
					options.RevocationMode = X509RevocationMode.NoCheck;
					options.Events = new CertificateAuthenticationEvents()
					{
						OnAuthenticationFailed = context =>
						{
							var logger = context.HttpContext.RequestServices.GetService<ILogger<Startup>>();

							logger.LogError(context.Exception, "Failed auth.");

							return Task.CompletedTask;
						},
						OnCertificateValidated = context =>
						{
							var logger = context.HttpContext.RequestServices.GetService<ILogger<Startup>>();

							// You should implement a service that confirms the certificate passed in
							// was signed by the root CA.
                    
							// Otherwise, a certificate that is valid to one of the other trusted CAs on the webserver,
							// would be valid in this case as well.
							// https://blog.kritner.com/2020/07/22/setting-up-mtls-and-kestrel-cont/
                    
							logger.LogInformation( $"You did it my dudes! {System.DateTime.Now.ToString()}");
							d.WriteLine( context.HttpContext.ToString());

							return Task.CompletedTask;
						} 
					};
				});
		}
	}
}

namespace Whoua.Core.Api.Supporting
{
	#region -- Using directives --
	using System;
	using System.Linq;
	using System.Security.Cryptography.X509Certificates;
	using d=System.Diagnostics.Debug;
	#endregion

	//public class CertificateValidation 
	//{
	//	public bool ValidateCertificate( X509Certificate2 clientCertificate) 
	//	{
	//		d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

	//		//string[] allowedThumbprints = {	"7ED035ABAB1B8CCDFF561935D3C55BE91EAB3DFB"};

	//		string[] allowedThumbprints = {	"1B181F82155DE12CF14735EEFF9F3B3A9B01E6C5"};
			
	//		if (allowedThumbprints.Contains(clientCertificate.Thumbprint)) 
	//		{
	//			return( true);
	//		}
	//		return( false);
	//	}
 //   }
}