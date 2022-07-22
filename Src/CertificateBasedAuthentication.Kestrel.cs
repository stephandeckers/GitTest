/**
 * @Name CertificateBasedAuthentication.cs
 * @Purpose 
 * @Date 18 June 2022, 06:37:54
 * @Author S.Deckers
 * @Description
 * PM> install-package Microsoft.AspNetCore.Authentication.Certificate -version 5.0.16
 */

namespace Whoua.Core.Api.Controllers
{
	#region -- Using directives --
	using System;
    using System.Text;
	using System.Collections.Generic;
	using System.Linq;
	using System.Security.Cryptography.X509Certificates;
	using System.Threading.Tasks;
    using System.IdentityModel.Tokens.Jwt;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.OData.Deltas;
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.Extensions.Logging;
	using Microsoft.AspNetCore.Http;
	using Microsoft.Extensions.DependencyInjection;

	using Swashbuckle.AspNetCore.Annotations;

	using Whoua.Core.Api.Model;
	using Whoua.Core.Api.Supporting;
	
	using d=System.Diagnostics.Debug;
	using Microsoft.AspNetCore.OData.Routing.Controllers;

	#endregion

	#region -- Controllers --

	[ ApiController			( )]
	[ ApiExplorerSettings	( IgnoreApi = false)]
	[ Produces				( "application/json", "application/json;odata.metadata=none")]
	[ Route					( "api/[controller]")]
	//[ Authorize				( AuthenticationSchemes = AUTHSCHEME, Policy = VIEWERROLE)]
	public abstract class IfRFBaseController : ODataController
	{
		public const string VIEWERROLE = "Viewer";
		public const string EDITORROLE = "Editor";
		public const string ADMINROLE = "Admin";
		public const string AUTHSCHEME = "Webseal";
	}

	public class WebSealWeatherForecastController : IfRFBaseController
	{
		private static readonly string[] strings = new[]
		{
			"one", "two", "three", "four"
		};

		private const string swaggerControllerDescription = "Neda test - WebSeal";

		[ HttpGet			( )]
		[ Authorize			( AuthenticationSchemes = AUTHSCHEME, Policy = VIEWERROLE )]
		[ SwaggerOperation	( Tags = new[] { swaggerControllerDescription }, Summary = "Get All items" )]
		public IEnumerable<string> Get( )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			// --- Get HttpContext without SI/delivering through ctor (20220623 SDE)

			//IHttpContextAccessor httpContextAccessor = HttpContext.RequestServices.GetRequiredService<IHttpContextAccessor>(); // Microsoft.Extensions.DependencyInjection
			//httpContextAccessor.HttpContext.Dump();

			return (strings).ToArray( );
		}
	}

	[ApiController]
	[Route( "api/[controller]" )]
	public class WeatherForecastController : ControllerBase
	{
		private static readonly string[] strings = new[]
		{
			"one", "two", "three", "four"
		};

		private const string swaggerControllerDescription = "Neda test";

		[ HttpGet			( )]
		[ SwaggerOperation	( Tags = new[] { swaggerControllerDescription }, Summary = "Get All items" )]
		public IEnumerable<string> Get( )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			// --- Get HttpContext without SI/delivering through ctor (20220623 SDE)

			//IHttpContextAccessor httpContextAccessor = HttpContext.RequestServices.GetRequiredService<IHttpContextAccessor>(); // Microsoft.Extensions.DependencyInjection
			//httpContextAccessor.HttpContext.Dump();

			return (strings).ToArray( );
		}
	}

	#endregion
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
	using System.IO;
	using System.Collections;
	using System.Linq;
	using System.Net;
	using System.Threading.Tasks;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Net.Security;
	using System.Security.Cryptography.X509Certificates;
	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Hosting;
	using Microsoft.AspNetCore.Http;
	using Microsoft.Extensions.Configuration;
	using Microsoft.AspNetCore.Mvc.Authorization;
	using Microsoft.AspNetCore.Authorization;
	using Swashbuckle.AspNetCore.SwaggerGen;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.OpenApi.Models;

	using Microsoft.AspNetCore.Hosting.Server;
	using Microsoft.AspNetCore.Hosting.Server.Features;

	using Whoua.Core.Api.Logging;
	using Whoua.Core.Api.AuthHandling;
	using Whoua.Core.Api.Supporting;

	using d = System.Diagnostics.Debug;
	using Microsoft.AspNetCore.Authentication.Certificate;
	using Microsoft.AspNetCore.Authentication.JwtBearer;
	using Microsoft.AspNetCore.Server.Kestrel.Core;
	using Microsoft.Extensions.Logging;
	using System.Security.Claims;
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

			/*
			var configuration = new ConfigurationBuilder()
					.AddEnvironmentVariables()
					.AddCommandLine(args)
					.AddJsonFile("appsettings.json")
					.Build();

			IHostBuilder hostBuilder = CreateHostBuilder( args );
			hostBuilder.ConfigureAppConfiguration( builder =>
			{
				builder.Sources.Clear();
				builder.AddConfiguration( configuration);
			});

			IHost host = hostBuilder.Build();
			host.Run( );
			*/
		}

		public static IHostBuilder CreateHostBuilder1( string[] args ) =>
			Host.CreateDefaultBuilder( args )
				.ConfigureWebHostDefaults( webBuilder =>
				 {
					 webBuilder.UseStartup<Startup>( );
				 } );


        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
              .ConfigureWebHostDefaults(webBuilder =>
                {
                    d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));
                    webBuilder.UseStartup<Startup>();
                    d.WriteLine("Configuring web host defaults, after startup");

                    // Below code is to support passthrough termination in OpenShift. Currently using Edge termination.
                    // Passthrough would be required if we need to go for 2-way certificate authentication

                    // add the certificate from the place as defined in env variables

                    //IDictionary _envVar = Environment.GetEnvironmentVariables();

					string serverCertificatePath		= @"C:\dev\code\certificates\ifrf-dev.pfx";
					string serverCertificatePassword	= @"7eO5cOYu98PtY0lR5tHB";

                    //string serverCertificatePath=envVar["ServerCertificatePath"].ToString(), 
					//string serverCertificatePassword = _envVar["ServerCertificatePassword"].ToString()

					IDictionary envVar = Environment.GetEnvironmentVariables();

					int port = 443;

					string v = envVar["ASPNETCORE_ENVIRONMENT"].ToString();
					if( v == "Development")
					{
						d.WriteLine( "Development running");
						port = 8080;
					}
					
                    ConfigureKestrel( webBuilder, port, serverCertificatePath, serverCertificatePassword);
                });
		/*
        private static void ConfigureKestrel1( IWebHostBuilder webBuilder, int port, string serverCertPath, string serverCertPwd)
        {
            _ = webBuilder.UseKestrel(options =>
            {
				d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

                d.WriteLine("Using Kestrel");
                X509Certificate2 x509Cert = new X509Certificate2( File.ReadAllBytes( serverCertPath), serverCertPwd);

                d.WriteLine( $"Subject: {x509Cert.Subject}");
                d.WriteLine( $"Issuer: {x509Cert.Issuer}");
                d.WriteLine( $"IssuerName: {x509Cert.IssuerName}");
                d.WriteLine( $"Subject: {x509Cert.Subject}");
                d.WriteLine( $"SubjectName: {x509Cert.SubjectName}");

                options.Listen(IPAddress.Any, port, listenOptions =>
                {
                    _ = listenOptions.UseHttps((a, b, c, d) =>
                    {
                        var serverChain = new X509Certificate2Collection();
                        serverChain.Import(serverCertPath, serverCertPwd);
                        return new ValueTask<SslServerAuthenticationOptions>(new SslServerAuthenticationOptions
                        {
                            ServerCertificateContext = SslStreamCertificateContext.Create(x509Cert, serverChain),
                            // require a client certificate, note that with UseHttps the ConfigureWebHostDefaults is ignored
                            ClientCertificateRequired = true,
                            RemoteCertificateValidationCallback = CertificateValidator.StaticValidateCertificate
                        });
                    },
                    null);
                });
            });
        }
		*/

        private static void ConfigureKestrel( IWebHostBuilder webBuilder, int port, string serverCertPath, string serverCertPwd)
        {
            webBuilder = webBuilder.UseKestrel(options =>
            {
				d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

                d.WriteLine("Using Kestrel");
                X509Certificate2 x509Cert = new X509Certificate2( File.ReadAllBytes( serverCertPath), serverCertPwd);

                d.WriteLine( $"Subject: {x509Cert.Subject}");
                d.WriteLine( $"Issuer: {x509Cert.Issuer}");
                d.WriteLine( $"IssuerName: {x509Cert.IssuerName}");
                d.WriteLine( $"Subject: {x509Cert.Subject}");
                d.WriteLine( $"SubjectName: {x509Cert.SubjectName}");

                options.Listen(IPAddress.Any, port, builder =>
                {
					//SslStream
					//SslClientHelloInfo
					//objectstate
					//System.Threading.CancellationToken

                    //ListenOptions listenOptions = builder.UseHttps((sslStream, sslClientHelloInfo, objectState, cancellationToken) =>
					ListenOptions listenOptions = builder.UseHttps((sslStream, sslClientHelloInfo, objectState, cancellationToken) =>
                    {
						var a = sslStream;
						var b = sslClientHelloInfo;
						var c = objectState;
						var d1 = cancellationToken;

						d.WriteLine( $"{b.ServerName}, {b.SslProtocols}"); // localhost, Tls, Tls12

						var serverChain = new X509Certificate2Collection();
                        serverChain.Import(serverCertPath, serverCertPwd);
						CertificateValidator certificateValidator = new CertificateValidator();
			//IHttpContextAccessor context = app.ApplicationServices.GetService<IHttpContextAccessor>();
			//CertificateValidator.Configure( context);

						//IHttpContextAccessor context = app.ApplicationServices.GetService<IHttpContextAccessor>();
						//certificateValidator.IHttpContextAccessor = context;

                        return new ValueTask<SslServerAuthenticationOptions>(new SslServerAuthenticationOptions
                        {
                            ServerCertificateContext = SslStreamCertificateContext.Create(x509Cert, serverChain),
                            // require a client certificate, note that with UseHttps the ConfigureWebHostDefaults is ignored
                            ClientCertificateRequired = true,
                            //RemoteCertificateValidationCallback = CertificateValidator.StaticValidateCertificate							

							RemoteCertificateValidationCallback = certificateValidator.ValidateCertificate,							
                        });
                    },
                    null);
                });

				//https://stackoverflow.com/questions/65453379/custom-server-certificate-with-kestrel-connectionhandler-and-sslstream
				//options.Listen(address:IPAddress.Parse("127.0.0.9"), port:8005, configure:builder =>
				//{
				//	// child thumbprint (obfuscated for posting)
				//	// certificate is found
					//var certificate = X509CertificateUtility.Find("4F6FFF21dFFF0FF743FFF44338F41F8FFFF46491");

					//var b = builder.ApplicationServices
				//	// TODO: Use TLS 1.3 once Microsoft fixes their bugs
				//	builder.UseHttps(new HttpsConnectionAdapterOptions
				//	{
				//		SslProtocols = SslProtocols.Tls12,
				//		CheckCertificateRevocation = false,
				//		ClientCertificateMode = ClientCertificateMode.NoCertificate,
				//		ServerCertificate = certificate,
				//		ClientCertificateValidation = (a, b, c) => true

				//	builder.UseHttps( 
				});

/*
 WebHost.CreateDefaultBuilder(args)
    .ConfigureServices(services => { })
    .UseKestrel(options =>
    {
        options.Listen(IPAddress.Parse("127.0.0.9"), 8005, builder =>
        {
            // child thumbprint (obfuscated for posting)
            // certificate is found
            var certificate = X509CertificateUtility.Find("4F6FFF21dFFF0FF743FFF44338F41F8FFFF46491");

            // TODO: Use TLS 1.3 once Microsoft fixes their bugs
            builder.UseHttps(new HttpsConnectionAdapterOptions
            {
                SslProtocols = SslProtocols.Tls12,
                CheckCertificateRevocation = false,
                ClientCertificateMode = ClientCertificateMode.NoCertificate,
                ServerCertificate = certificate,
                ClientCertificateValidation = (a, b, c) => true
            });

            builder.UseConnectionHandler<KairosConnectionHandler>();
        });
    })
    .UseStartup<Startup>();
*/
		}
	}

	public class Startup
	{
		public	const		string			PRXCORS = "PrxCors";
		private readonly	LoggerManager	_logger;

		public Startup( )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			string logLevel = "Debug";
			_logger = new LoggerManager( logLevel, logLevel);
		}

		public void Configure( IApplicationBuilder app, IWebHostEnvironment env )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			//IServerAddressesFeature addressFeature = null;

			//var server = app.ApplicationServices.GetService<IServer>();
			//addressFeature = server.Features.Get<IServerAddressesFeature>();

			//foreach (var address in addressFeature.Addresses)
			//{
			//	d.WriteLine("Kestrel is listening on address: " + address);
			//}

			//var serverFeatures = app.ServerFeatures;

			//foreach( var item in serverFeatures)
			//{
			//	d.WriteLine( $"[{item.Key}]=[{item.Value}]");
			//}

			app.UseDeveloperExceptionPage	( );
            app.UseHttpsRedirection			( );
            app.UseRouting					( );
            app.UseAuthorization			( );
            app.UseEndpoints				( e => { e.MapControllers(); });
            app.UseAuthentication			( );
			app.UseCors						( Startup.PRXCORS);
            app.UseSwagger					( );
            app.UseSwaggerUI				( c => 
			{ 
				c.SwaggerEndpoint			( "/swagger/v1/swagger.json", "Foo API v1"); 
				c.DisplayOperationId		( );
				//c.EnableTryItOutByDefault	( ); // --- only available in install-package Swashbuckle.AspNetCore -version 6.1.3
			});

			// --- Headers not yet available (20220623 SDE)

			//IHttpContextAccessor context = app.ApplicationServices.GetService<IHttpContextAccessor>();
			//CertificateValidator.Configure( context);
		}

		public void ConfigureServices1( IServiceCollection services )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			services.AddSingleton<ILoggerManager>(_logger);

            services.AddCors(options =>
            {
                options.AddPolicy(name: PRXCORS,
                    builder =>
                    {
                        builder
                        .SetIsOriginAllowed	( origin => true) // allow any origin
                        .AllowAnyHeader		( )
                        .AllowAnyMethod		( )
                        .AllowCredentials	( );
                    });
            });

			services.RegisterDiContainers();
			_logger.Debug("Configuring services");

			services.AddHttpContextAccessor	( );

			string srcTitle		= $"mTLS - {System.DateTime.Now.ToString()}";
			string src			= "no source";
			string theUrl		= @$"<a href={src} target=_blank>{srcTitle}</a>";
			string description	= $"Swagger document filters<br/><br/>{theUrl}";

			services.AddAuthentication( "Webseal" )
			.AddScheme<WebsealAuthenticationOptions, WebsealAuthenticationHandler>( Controllers.IfRFBaseController.AUTHSCHEME, op => { } );

			services.AddAuthorization( options =>
			 {
				 options.AddPolicy( Controllers.IfRFBaseController.VIEWERROLE, policy => policy.Requirements.Add( new WebsealAuthorizationRequirement( Controllers.IfRFBaseController.VIEWERROLE ) ) );
				 options.AddPolicy( Controllers.IfRFBaseController.EDITORROLE, policy => policy.Requirements.Add( new WebsealAuthorizationRequirement( Controllers.IfRFBaseController.EDITORROLE ) ) );
				 options.AddPolicy( Controllers.IfRFBaseController.ADMINROLE, policy => policy.Requirements.Add( new WebsealAuthorizationRequirement( Controllers.IfRFBaseController.ADMINROLE ) ) );
			 } );

			services.AddAuthentication	( CertificateAuthenticationDefaults.AuthenticationScheme)
                    .AddCertificate		( )
                    // Adding an ICertificateValidationCache results in certificate auth caching the results.
                    // The default implementation uses a memory cache.
                    .AddCertificateCache( );

			services.AddControllers			( );

			services.AddSwaggerGen( c =>
			{
				c.SwaggerDoc									( name: "v1", info: new OpenApiInfo { Title = "Certificate Based Authentication", Version = "v1", Description = description } );
				c.EnableAnnotations								( );
				c.ResolveConflictingActions						( apiDescription => apiDescription.First());				
			});
		}

		public void ConfigureServices( IServiceCollection services )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			services.AddSingleton<ILoggerManager>(_logger);

            services.AddCors(options =>
            {
                options.AddPolicy(name: PRXCORS,
                    builder =>
                    {
                        builder
                        .SetIsOriginAllowed	( origin => true) // allow any origin
                        .AllowAnyHeader		( )
                        .AllowAnyMethod		( )
                        .AllowCredentials	( );
                    });
            });

			services.RegisterDiContainers();
			_logger.Debug("Configuring services");

			services.AddHttpContextAccessor	( );

			string srcTitle		= $"mTLS - {System.DateTime.Now.ToString()}";
			string src			= "no source";
			string theUrl		= @$"<a href={src} target=_blank>{srcTitle}</a>";
			string description	= $"Swagger document filters<br/><br/>{theUrl}";

			/*
			services.AddAuthentication( "Webseal" )
			.AddScheme<WebsealAuthenticationOptions, WebsealAuthenticationHandler>( Controllers.IfRFBaseController.AUTHSCHEME, op => { } );

			services.AddAuthorization( options =>
			 {
				 options.AddPolicy( Controllers.IfRFBaseController.VIEWERROLE, policy => policy.Requirements.Add( new WebsealAuthorizationRequirement( Controllers.IfRFBaseController.VIEWERROLE ) ) );
				 options.AddPolicy( Controllers.IfRFBaseController.EDITORROLE, policy => policy.Requirements.Add( new WebsealAuthorizationRequirement( Controllers.IfRFBaseController.EDITORROLE ) ) );
				 options.AddPolicy( Controllers.IfRFBaseController.ADMINROLE, policy => policy.Requirements.Add( new WebsealAuthorizationRequirement( Controllers.IfRFBaseController.ADMINROLE ) ) );
			 } );
			*/

			/*
			services.AddAuthentication	( CertificateAuthenticationDefaults.AuthenticationScheme)
                    .AddCertificate		( )
                    // Adding an ICertificateValidationCache results in certificate auth caching the results.
                    // The default implementation uses a memory cache.
                    .AddCertificateCache( );
			*/

			services.AddAuthentication	( CertificateAuthenticationDefaults.AuthenticationScheme)
                    .AddCertificate		( options =>
					{
						options.AllowedCertificateTypes = CertificateTypes.SelfSigned;
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

							return Task.CompletedTask;
						} 
					};
						//OnCertificateValidated = context => {
						//		var validationService = context.HttpContext.RequestServices.GetService < CertificateValidation > ();
						//		if (validationService.ValidateCertificate(context.ClientCertificate)) {
						//			context.Success();
						//		} else {
						//			context.Fail("Invalid certificate");
						//		}
						//		return Task.CompletedTask;
						//	},
						//	OnAuthenticationFailed = context => {
						//		context.Fail("Invalid certificate");
						//		return Task.CompletedTask;
						//	}
					} )
                    // Adding an ICertificateValidationCache results in certificate auth caching the results.
                    // The default implementation uses a memory cache.
                    .AddCertificateCache( );

			/*
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

							return Task.CompletedTask;
						} 
					};
*/
/*
 

            services.AddAuthentication(CertificateAuthenticationDefaults.AuthenticationScheme).AddCertificate(options => {
                options.AllowedCertificateTypes = CertificateTypes.SelfSigned;
                options.Events = new CertificateAuthenticationEvents {
                    OnCertificateValidated = context => {
                            var validationService = context.HttpContext.RequestServices.GetService < CertificateValidation > ();
                            if (validationService.ValidateCertificate(context.ClientCertificate)) {
                                context.Success();
                            } else {
                                context.Fail("Invalid certificate");
                            }
                            return Task.CompletedTask;
                        },
                        OnAuthenticationFailed = context => {
                            context.Fail("Invalid certificate");
                            return Task.CompletedTask;
                        }
                };
*/
			services.AddAuthentication( "Webseal" ).AddScheme<WebsealAuthenticationOptions, WebsealAuthenticationHandler>( Controllers.IfRFBaseController.AUTHSCHEME, op => { } );

			var authPolicyBuilder = new AuthorizationPolicyBuilder()
				.RequireAuthenticatedUser()
				.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);

			services.AddAuthorization( options =>
			 {
				 options.AddPolicy( Controllers.IfRFBaseController.VIEWERROLE, policy => policy.Requirements.Add( new WebsealAuthorizationRequirement( Controllers.IfRFBaseController.VIEWERROLE ) ) );
				 options.AddPolicy( Controllers.IfRFBaseController.EDITORROLE, policy => policy.Requirements.Add( new WebsealAuthorizationRequirement( Controllers.IfRFBaseController.EDITORROLE ) ) );
				 options.AddPolicy( Controllers.IfRFBaseController.ADMINROLE, policy => policy.Requirements.Add( new WebsealAuthorizationRequirement( Controllers.IfRFBaseController.ADMINROLE ) ) );
				 options.DefaultPolicy = authPolicyBuilder.Build();
			 } );

			services.AddControllers			( );

			
			services.AddSwaggerGen( c =>
			{
				c.SwaggerDoc									( name: "v1", info: new OpenApiInfo { Title = "Certificate Based Authentication", Version = "v1", Description = description } );
				c.EnableAnnotations								( );
				c.ResolveConflictingActions						( apiDescription => apiDescription.First());				
			});
		}
	}
}

//namespace Whoua.Core.Api.Services
//{
//	using System.Reflection;
//	using Whoua.Core.Api.Logging;
//	using System.Security.Cryptography.X509Certificates;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Logging;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//	using d=System.Diagnostics.Debug;

//	public class SampleCertificateValidationService : ICertificateValidationService
//	{
//		public bool ValidateCertificate(X509Certificate2 clientCertificate)
//		{
//			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

//			// Don't hardcode passwords in production code.
//			// Use a certificate thumbprint or Azure Key Vault.
//			//var expectedCertificate = new X509Certificate2(	Path.Combine("/path/to/pfx"), "1234");

//			//return clientCertificate.Thumbprint == expectedCertificate.Thumbprint;
//			return( true);
//		}
//	}
//}
namespace Whoua.Core.Api.AuthHandling
{
	#region -- Using directives --
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using System.Security.Claims;
	using System.Threading.Tasks;
	using System.Net.Security;
	using System.Security.Cryptography.X509Certificates;
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.Extensions.Configuration;
	using System.Text.Encodings.Web;
	using Microsoft.AspNetCore.Authentication;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Logging;
	using Microsoft.Extensions.Options;
	using Whoua.Core.Api.Logging;
	using Whoua.Core.Api.Supporting;
	using d=System.Diagnostics.Debug;
	using Microsoft.AspNetCore.Http;
	#endregion

	public class WebsealAuthenticationOptions
        : AuthenticationSchemeOptions
    {}

    public class WebsealAuthenticationHandler
        : AuthenticationHandler<WebsealAuthenticationOptions>
    {
        ILoggerManager _logger;
        public WebsealAuthenticationHandler
		(
			IOptionsMonitor<WebsealAuthenticationOptions>	options
		,	ILoggerFactory									loggerFactory
		,	UrlEncoder										encoder
		,	ISystemClock									clock
		,	ILoggerManager									logger
		)
            : base( options, loggerFactory, encoder, clock)
        {
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

            _logger = logger;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			//Request.Headers.Dump( );

            // validation comes in here
            if (!Request.Headers.ContainsKey("iv-user"))
            {
                string failureMessage = "Header iv-user Not Found.";
                _logger.Warn(failureMessage);
                return Task.FromResult(AuthenticateResult.Fail(failureMessage));
            }
            // create claims list from the model
            List<Claim> claims = new List<Claim>();
            var UserId = Request.Headers["iv-user"].ToString();
            Context.Items.Add("iv-user", UserId);

            claims.Add(new Claim(ClaimTypes.NameIdentifier, UserId));

            List<string> ivgroups = new List<string>();

            if (Request.Headers.ContainsKey("iv-groups"))
            {
                string ivGroupsString = Request.Headers["iv-groups"].ToString();
                string[] ivGroups = ivGroupsString.Split(",");
                foreach (string aGroup in ivGroups)
                {
                    string aGroupUnquoted = aGroup.Replace("\"", "");
                    claims.Add(new Claim(ClaimTypes.Role, aGroupUnquoted));
                    ivgroups.Add(aGroupUnquoted);
                }
            }

            Context.Items.Add("iv-groups", ivgroups);

            // generate claimsIdentity on the name of the class
            var claimsIdentity = new ClaimsIdentity(claims, nameof(WebsealAuthenticationHandler));

            // generate AuthenticationTicket from the Identity
            // and current authentication scheme
            var ticket = new AuthenticationTicket( new ClaimsPrincipal(claimsIdentity), this.Scheme.Name);

            // pass on the ticket to the middleware
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }

    internal class WebsealAuthorizationRequirement : IAuthorizationRequirement
    { 
        public string RequiredRole { get; }

        public WebsealAuthorizationRequirement(string requiredRole)
        {
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

            RequiredRole = requiredRole;
        }
    }

	internal class WebsealAuthorizationHandler : AuthorizationHandler<WebsealAuthorizationRequirement>
    {
        private readonly IDictionary<string, IList<string>> roleGroupMappings = new Dictionary<string, IList<string>>();

        public WebsealAuthorizationHandler(IConfiguration configuration)
        {
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

            var rgMaps = configuration.GetSection("RoleGroupMappings").GetChildren();
            foreach(IConfigurationSection anRgMap in rgMaps)
            {
                var		children	= anRgMap.GetChildren();
                string	role		= children.Single(c => c.Key.Equals("Role")).Value;
                var		groups		= children.Single(c => c.Key.Equals("Groups")).GetChildren().Select(gr => gr.Value).ToList();
                roleGroupMappings.Add(role, groups);
            }
        }

        protected override Task HandleRequirementAsync
		(
			AuthorizationHandlerContext		context
		,	WebsealAuthorizationRequirement requirement
		)
        {
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

            if (!context.User.HasClaim(c => c.Type == ClaimTypes.Role))
            {
                return Task.CompletedTask;
            }

            if (!roleGroupMappings.ContainsKey(requirement.RequiredRole))
            {
                throw new KeyNotFoundException($"Role {requirement.RequiredRole} not present in application configuration");
            }

            IList<string> allowedGroups = roleGroupMappings[requirement.RequiredRole];
            foreach (string anAllowedGroup in allowedGroups)
            {
                if( context.User.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value.Equals(anAllowedGroup)))
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
            }

            return Task.CompletedTask;
        }
    }

	public class CertificateValidator
    {
		private static IHttpContextAccessor _context = null;

       internal void Configure(IHttpContextAccessor context)
       {
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

           _context = context;
       }

		internal IHttpContextAccessor IHttpContextAccessor { get; set; }

		public CertificateValidator( HttpContext httpContext)
		{
			string s = $"context.hash={this.GetHashCode( )}]";
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, s));
		}

		public CertificateValidator( )
		{
			string s = $"hash={this.GetHashCode( )}]";
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, s));
		}
        public bool ValidateCertificate
		(
			object			o
		,	X509Certificate	cert
		,	X509Chain		clientChain
		,	SslPolicyErrors err
		)
        {
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			string s = string.Empty;			

			// Validate the certificate chain

			//d.WriteLine( $"o={o.ToString()}");
			//d.WriteLine( $"cert={cert}");
			//d.WriteLine( $"clientChain={clientChain}");

            if( err != SslPolicyErrors.None)
            {
				s = $"SSL Policy err:{err.ToString()}";
				d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, s));
                return( false);
            }

            // Validate that all attributes are OK
            var issuer = new Dictionary<string, string>();
            cert.Issuer.Split(",").Select(o => o.Trim()).ToList().ForEach(x =>
            {
                var		issuerKvp	= x.Split("=");
                string	key			= issuerKvp[0];
                string	value		= issuerKvp[1];
                if (!issuer.ContainsKey(key))
                {
					issuer.Add(key, value);
                }
            });

            var subject = new Dictionary<string, string>();
            cert.Subject.Split(",").Select(o => o.Trim()).ToList().ForEach(x =>
            {
                var		subjectKvp	= x.Split("=");
                string	key			= subjectKvp[0];
                string	value		= subjectKvp[1];

                if (subjectKvp[0].Equals("OU") && subjectKvp[1].Contains(":"))
                {
                    key = subjectKvp[0] = "OU_" + subjectKvp[1].Split(":")[0];
                    value = subjectKvp[1].Split(":")[1];
                }

                if (!subject.ContainsKey("key"))
                {
                    subject.Add(key, value);
                }
            });			

			IList<string> allowedCN = new List<string> { "ifh-itt", "intra.web.bc", "ifrf-dev"};

			//issuer.Dump( "issuer");
			//subject.Dump( "subject");

			bool result = ValidateIssuerAndSubject( issuer, subject, allowedCN);

			if( result)
			{				
				// --- Add header (20220622 SDE)

				HttpContext httpContext = new HttpContextAccessor().HttpContext;

				// --- from https://www.thecodehubs.com/use-dependency-injection-in-static-class-with-net-core/

				if( _context == null)
				{
					d.WriteLine( $"cert is accepted, context not available");
					return (result);
				}

				if( _context.HttpContext == null)
				{
					d.WriteLine( $" cert is accepted, context.HttpContext not available");
					return (result);
				}

				if( _context.HttpContext.Request == null)
				{
					d.WriteLine( $"cert is accepted, context.HttpContext.Request not available");
					return (result);
				}
				_context.HttpContext.Request.Headers[ "NEDA_GROUP"] = "NEDA001";
				d.WriteLine( $"[{cert.ToString()}] is accepted, headers added");
				return (result);
			}

			d.WriteLine( $"[{cert.ToString()}] is not accepted");
            return( result);
		}

        public static bool StaticValidateCertificate
		(
			object			o
		,	X509Certificate	cert
		,	X509Chain		clientChain
		,	SslPolicyErrors err
		)
        {
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			string s = string.Empty;			

			// Validate the certificate chain

			//d.WriteLine( $"o={o.ToString()}");
			//d.WriteLine( $"cert={cert}");
			//d.WriteLine( $"clientChain={clientChain}");

            if( err != SslPolicyErrors.None)
            {
				s = $"SSL Policy err:{err.ToString()}";
				d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, s));
                return( false);
            }

            // Validate that all attributes are OK
            var issuer = new Dictionary<string, string>();
            cert.Issuer.Split(",").Select(o => o.Trim()).ToList().ForEach(x =>
            {
                var		issuerKvp	= x.Split("=");
                string	key			= issuerKvp[0];
                string	value		= issuerKvp[1];
                if (!issuer.ContainsKey(key))
                {
					issuer.Add(key, value);
                }
            });

            var subject = new Dictionary<string, string>();
            cert.Subject.Split(",").Select(o => o.Trim()).ToList().ForEach(x =>
            {
                var		subjectKvp	= x.Split("=");
                string	key			= subjectKvp[0];
                string	value		= subjectKvp[1];

                if (subjectKvp[0].Equals("OU") && subjectKvp[1].Contains(":"))
                {
                    key = subjectKvp[0] = "OU_" + subjectKvp[1].Split(":")[0];
                    value = subjectKvp[1].Split(":")[1];
                }

                if (!subject.ContainsKey("key"))
                {
                    subject.Add(key, value);
                }
            });			

			IList<string> allowedCN = new List<string> { "ifh-itt", "intra.web.bc", "ifrf-dev"};

			//issuer.Dump( "issuer");
			//subject.Dump( "subject");

			bool result = ValidateIssuerAndSubject( issuer, subject, allowedCN);

			if( result)
			{				
				// --- Add header (20220622 SDE)

				// --- from https://www.thecodehubs.com/use-dependency-injection-in-static-class-with-net-core/

				if( _context == null)
				{
					d.WriteLine( $"cert is accepted, context not available");
					return (result);
				}

				if( _context.HttpContext == null)
				{
					d.WriteLine( $" cert is accepted, context.HttpContext not available");
					return (result);
				}

				if( _context.HttpContext.Request == null)
				{
					d.WriteLine( $"cert is accepted, context.HttpContext.Request not available");
					return (result);
				}
				_context.HttpContext.Request.Headers[ "NEDA_GROUP"] = "NEDA001";
				d.WriteLine( $"[{cert.ToString()}] is accepted, headers added");
				return (result);
			}

			d.WriteLine( $"[{cert.ToString()}] is not accepted");
            return( result);
		}

		/// <summary>
		/// Validates the issuer and subject.
		/// </summary>
		/// <param name="issuer">The issuer.</param>
		/// <param name="subject">The subject.</param>
		/// <param name="allowedCN">The allowed cn.</param>
		/// <returns></returns>
		private static bool ValidateIssuerAndSubject
		(
			Dictionary<string, string>	issuer
		,	Dictionary<string, string>	subject
		,	IList<string>				allowedCN
		)
        {
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			//return( true);
			//issuer.Dump(  "issuer");
			//subject.Dump( "subject");

			string s=string.Empty;
			string issuerCN				= issuer[ "CN"];
			string issuerOrganisation	= issuer[ "O"].ToUpper();
			
            if (!issuerCN.Equals("ProximusCorporateIssuingCA"))
			{
				s=$"issuer.CN={issuerCN} != 'ProxinusCorporateIssuingCA";
				d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, s));
				return( false);
			}

            if (!(issuerOrganisation.Contains("PROXIMUS") || issuerOrganisation.Contains("BELGACOM")))
			{
				s=$"issuer.issuerOrganisation={issuerOrganisation} != 'PROXINMUS/BELGACOM";
				d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, s));
				return( false);
			}

			string subjectCN = subject["CN"].ToUpper();

            if (!(allowedCN.Any(cn => cn.ToUpper().Contains( subjectCN))))
            {
				s=$"subjectCN {subjectCN} not found in allowedCN";
				d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, s));
                return( false);
            }

			s=$"cert accepted";
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, s));
            return(true);
        }
    }
}

namespace Whoua.Core.Api.Logging
{
	#region -- Using directives --
	using Microsoft.AspNetCore.Http;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	#endregion

    public interface ILoggerManager
    {
        void Info			( string message, string context = "");
        void Warn			( string message, string context = "");
        void Debug			( string message, string context = "");
        void Error			( string message, string context = "");
        void Trace			( string message, string context = "");
        void LogWebRequest	( Type wrapperType, HttpContext httpContext, bool hasResponse, long? responseTimeMs = null);

        public IDictionary<string, string> GetLogLevels();

        public bool IsTraceEnabled	{ get; }
        public bool IsDebugEnabled	{ get; }
        public bool IsInfoEnabled	{ get; }
        public bool IsWarnEnabled	{ get; }
        public bool IsErrorEnabled	{ get; }
    }

	public class LoggerManager : ILoggerManager
    {
        const string LOGGERNAME = "CustomLogger";

        public LoggerManager() : this(fileLogLevel: "Info", consoleLogLevel: "Debug")
        { }

        public LoggerManager(string fileLogLevel, string consoleLogLevel)
        { }

        public bool IsTraceEnabled	{ get => true; }
        public bool IsDebugEnabled	{ get => true; }
        public bool IsInfoEnabled	{ get => true; }
        public bool IsWarnEnabled	{ get => true; }
        public bool IsErrorEnabled	{ get => true; }

        public IDictionary<string,string> GetLogLevels()
        {
            IDictionary<string, string> loglevels = new Dictionary<string, string>();

            return loglevels;
        }

        public void Trace	( string message, string context = "")		{ }
        public void Debug	( string message, string context = "")		{ }
        public void Info	( string message, string context="")		{ }
        public void Warn	( string message, string context = "")		{ }
        public void Error	( string message, string context = "")		{ }
        public void LogWebRequest( Type wrapperType, HttpContext httpContext,  bool hasResponse, long? responseTimeMs = null) { }
    }
}

namespace Whoua.Core.Api.Supporting
{
	#region -- Using directives --
	using System.Collections;
	using System.Collections.Generic;
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Http;
	using Microsoft.EntityFrameworkCore.Diagnostics;
	using Microsoft.Extensions.DependencyInjection;

	using Whoua.Core.Api.AuthHandling;
	using d=System.Diagnostics.Debug;
	#endregion

	public static class ApiExtensions
	{
		public static void RegisterDiContainers(this IServiceCollection services)
		{
			services.AddSingleton<IHttpContextAccessor,		HttpContextAccessor>();
			services.AddSingleton<IAuthorizationHandler,	WebsealAuthorizationHandler>();			
		}

		public static void Dump(this Dictionary<string, string> dictionary, string name)
		{
			d.WriteLine( $"Dumping '{name}'");
			foreach( KeyValuePair<string, string> item in dictionary)
			{
				d.WriteLine( $"key={item.Key},value={item.Value}");
			}
		}

		public static void Dump(this HttpContext context)
		{
			d.WriteLine( $"Dumping '{context.ToString()}'");

			if( context.Request == null)	 d.WriteLine( $" null Request");

			if( context?.Request?.Headers == null)	 d.WriteLine( $" null Request.Headers");

			if( context?.Request?.Headers?.Count == null)	 d.WriteLine( $" {context?.Request?.Headers?.Count} Request.Headers");
		}

		public static void Dump( this IHeaderDictionary headers)
		{
			d.WriteLine( $"{headers.Count} headers");

			foreach(var item in headers)
			{
				d.WriteLine( $"{item.Key}={item.Value}");
			}
		}
	}
}