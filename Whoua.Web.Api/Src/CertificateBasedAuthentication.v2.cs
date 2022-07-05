/**
 * @Name CertificateBasedAuthentication.v2.cs
 * @Purpose 
 * @Date 21 June 2022, 07:48:57
 * @Author S.Deckers
 * @Description 
 * 
 * @notes Waarom hebben we mTLS nodig ? Is het niet voldoende dat de client zich authenticeerd ?
 * 
 * PM>install-package Microsoft.AspNetCore.Authentication.Certificate -version 5.0.17
 */

/* --- notes
url:https://www.c-sharpcorner.com/article/using-certificates-for-api-authentication-in-net-5/

PS>New-SelfSignedCertificate -DnsName "localhost", "localhost" -CertStoreLocation "cert:\LocalMachine\My" -NotAfter (Get-Date).AddYears(10) -FriendlyName "CAlocalhost" -KeyUsageProperty All -KeyUsage CertSign, CRLSign, DigitalSignature

   PSParentPath: Microsoft.PowerShell.Security\Certificate::LocalMachine\My

  Thumbprint                                Subject
  ----------                                -------
  08440A34C3230875FAAF7FC7B540A6EB82A4C211  CN=localhost

PS>$mypwd = ConvertTo-SecureString -String "Server123" -Force -AsPlainText
PS>Get-ChildItem -Path cert:\localMachine\my\08440A34C3230875FAAF7FC7B540A6EB82A4C211 | Export-PfxCertificate -FilePath "C:\usr\stephan\junk\cacert.pfx" -Password $mypwd
PS>$rootcert = ( Get-ChildItem -Path cert:\LocalMachine\My\08440A34C3230875FAAF7FC7B540A6EB82A4C211 )
PS>New-SelfSignedCertificate -certstorelocation cert:\localmachine\my -dnsname "localhost" -Signer $rootcert -NotAfter (Get-Date).AddYears(10) -FriendlyName "Clientlocalhost"

   PSParentPath: Microsoft.PowerShell.Security\Certificate::LocalMachine\my

   Thumbprint                                Subject
   ----------                                -------
   1B181F82155DE12CF14735EEFF9F3B3A9B01E6C5  CN=localhost

PS>$mypwd = ConvertTo-SecureString -String "Client123" -Force -AsPlainText
PS>Get-ChildItem -Path cert:\localMachine\my\08440A34C3230875FAAF7FC7B540A6EB82A4C211 | Export-PfxCertificate -FilePath "C:\usr\stephan\junk\clientcert.pfx" -Password $mypwd
*/
namespace Whoua.Core.Api.Controllers
{
	#region -- Using directives --

	using System;
    using System.Text;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Whoua.Core.Api.Model;
	using Swashbuckle.AspNetCore.Annotations;

    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

	using d=System.Diagnostics.Debug;
	#endregion	

    [ Route("api/[controller]")]
    [ ApiController]
    public class TestController: ControllerBase 
	{
		private const string swaggerControllerDescription = "Certificatebased Authentication Sample";

        [ HttpGet	( )]
		[ SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "No certificate" )]
        public string Get() => "The action works fine without a certificate";

        [ Authorize	( )]
        [ HttpPost	( )]
		[ SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "Certificate required" )]
        public string Post() => "The action works fine only with a certificate";
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
	using System;
	using System.Linq;
	using System.Threading.Tasks;
	using System.Collections.Generic;
	using System.Reflection;
	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Hosting;
	using Microsoft.AspNetCore.Http;
	using Microsoft.Extensions.Configuration;
	using Microsoft.AspNetCore.Mvc.Authorization;
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Authentication.Certificate;
	using Swashbuckle.AspNetCore.SwaggerGen;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.OpenApi.Models;
	using Whoua.Core.Api.Supporting;
	using Microsoft.AspNetCore.Server.Kestrel.Https;
	using d = System.Diagnostics.Debug;	
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
					webBuilder.ConfigureKestrel(o => { o.ConfigureHttpsDefaults(o => o.ClientCertificateMode = ClientCertificateMode.AllowCertificate);	});
				 } );
	}

	public class Startup
	{
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
            app.UseSwaggerUI				( c => 
			{ 
				c.SwaggerEndpoint			( "/swagger/v1/swagger.json", "Foo API v1"); 
				c.DisplayOperationId		( );
				c.EnableTryItOutByDefault	( );
			});
		}

		public void ConfigureServices( IServiceCollection services )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			services.AddControllers							( );
			services.AddTransient < CertificateValidation > ( );
            services.AddAuthentication(CertificateAuthenticationDefaults.AuthenticationScheme).AddCertificate(options => {
                options.AllowedCertificateTypes = CertificateTypes.SelfSigned;
                options.Events = new CertificateAuthenticationEvents 
				{
                    OnCertificateValidated = context => 
					{
						d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

                        var validationService = context.HttpContext.RequestServices.GetService < CertificateValidation > ();
                        if (validationService.ValidateCertificate(context.ClientCertificate)) 
						{
                            context.Success();
                        } else 
						{
                            context.Fail("Invalid certificate");
                        }
                        return( Task.CompletedTask);
					},
                    OnAuthenticationFailed = context => 
					{
						context.Fail("Invalid certificate");
						return( Task.CompletedTask);
					}
                };
            });
			services.AddHttpContextAccessor	( );

			string srcTitle		= $"Certificate based authentication - {System.DateTime.Now.ToString()}";
			string src			= "https://www.c-sharpcorner.com/article/using-certificates-for-api-authentication-in-net-5/";
			string theUrl		= @$"<a href={src} target=_blank>{srcTitle}</a>";
			string description	= $"Certificate based authentication<br/><br/>{theUrl}";

			services.AddSwaggerGen( c =>
			{
				c.SwaggerDoc									( name: "v1", info: new OpenApiInfo { Title = "Swagger Document filters", Version = "v1", Description = description } );
				c.EnableAnnotations								( );
				c.ResolveConflictingActions						( apiDescription => apiDescription.First());				
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

	public class CertificateValidation 
	{
		public bool ValidateCertificate( X509Certificate2 clientCertificate) 
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			//string[] allowedThumbprints = {	"7ED035ABAB1B8CCDFF561935D3C55BE91EAB3DFB"};

			string[] allowedThumbprints = {	"7E881F7009F3BE5EF86C0726E4C92E1793F72BCA"};
			
			if (allowedThumbprints.Contains(clientCertificate.Thumbprint)) 
			{
				return( true);
			}
			return( false);
		}
    }
}