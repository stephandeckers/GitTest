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
	[ Authorize				( AuthenticationSchemes = AUTHSCHEME, Policy = VIEWERROLE)]
	public abstract class IfRFBaseController : ODataController
	{
		public const string VIEWERROLE			= "Viewer";
		public const string EDITORROLE_lvl_1	= "Editor1";
		public const string EDITORROLE_lvl_2	= "Editor2";
		public const string ADMINROLE			= "Admin";
		public const string AUTHSCHEME			= "CertAndWebseal";

		// --- needs to be present in appsettings.json
		public const string NEDA_GRP_001		= "NEDA001";
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

			return (strings).ToArray( );
		}

		[ HttpGet			( "lvl1")]
		[ Authorize			( AuthenticationSchemes = AUTHSCHEME, Policy = EDITORROLE_lvl_1 )]
		//[ Authorize			( AuthenticationSchemes = AUTHSCHEME, Policy = EDITORROLE )]
		[ SwaggerOperation	( Tags = new[] { swaggerControllerDescription }, Summary = "Editor-lvl1" )]
		public IEnumerable<string> Get_lvl1( )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			return (strings).ToArray( );
		}

		[ HttpGet			( "lvl2")]
		[ Authorize			( AuthenticationSchemes = AUTHSCHEME, Policy = EDITORROLE_lvl_2 )]
		//[ Authorize			( AuthenticationSchemes = AUTHSCHEME, Policy = EDITORROLE )]
		[ SwaggerOperation	( Tags = new[] { swaggerControllerDescription }, Summary = "Editor-lvl2" )]
		public IEnumerable<string> Get_lvl2( )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			return (strings).ToArray( );
		}
	}

	[ApiController]
	[Route( "api/[controller]" )]
	[Authorize()]
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
	using System.Linq;
	using System.Threading.Tasks;
	using System.Security.Cryptography.X509Certificates;
	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Hosting;
	using Microsoft.OpenApi.Models;

	using Whoua.Core.Api.Logging;
	using Whoua.Core.Api.AuthHandling;
	using Whoua.Core.Api.Supporting;
	
	using Microsoft.AspNetCore.Authentication.Certificate;
	using Microsoft.AspNetCore.Server.Kestrel.Https;
	using Microsoft.Extensions.Logging;

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
					webBuilder.ConfigureKestrel (o => 
					{ 
						o.ConfigureHttpsDefaults(o => o.ClientCertificateMode = ClientCertificateMode.RequireCertificate);	 // --- Require certificate
					});
				 } );
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

			app.UseDeveloperExceptionPage	( );
            app.UseHttpsRedirection			( );
            
            app.UseRouting					( ); // --- 1. The order matters
			app.UseAuthentication			( ); // --- 2.
            app.UseAuthorization			( ); // --- 3.

            app.UseEndpoints				( e => { e.MapControllers(); });
              
            app.UseCors						( Startup.PRXCORS);
            
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

			services.AddSingleton<ILoggerManager>(_logger);

			services.AddCors(options =>
			{
				options.AddPolicy(name: PRXCORS,
					builder =>
					{
						builder
						.SetIsOriginAllowed(origin => true) // allow any origin
						.AllowAnyHeader()
						.AllowAnyMethod()
						.AllowCredentials();
					});
			});

			services.RegisterDiContainers();

			_logger.Debug("Configuring services");

			services.AddControllers			( );
			services.AddHttpContextAccessor	( );

			string srcTitle		= $"MTLS authentication - {System.DateTime.Now.ToString()}";
			string src			= "no src / STU";
			string theUrl		= @$"<a href={src} target=_blank>{srcTitle}</a>";
			string description	= $"Mutual Transport Layer Security<br/><br/>{theUrl}";

			services.AddSwaggerGen( c =>
			{
				c.SwaggerDoc									( name: "v1", info: new OpenApiInfo { Title = "Swagger Document filters", Version = "v1", Description = description } );
				c.EnableAnnotations								( );
				c.ResolveConflictingActions						( apiDescription => apiDescription.First());				
			});

			services.AddAuthentication( Controllers.IfRFBaseController.AUTHSCHEME )
			.AddScheme<WebsealAuthenticationOptions, CertAndWebsealAuthenticationHandler>( Controllers.IfRFBaseController.AUTHSCHEME, op => { } );

			services.AddAuthorization( options =>
			 {
				 options.AddPolicy( Controllers.IfRFBaseController.VIEWERROLE,			policy => policy.Requirements.Add( new CommonAuthorizationRequirement( Controllers.IfRFBaseController.VIEWERROLE ) ) );
				 options.AddPolicy( Controllers.IfRFBaseController.EDITORROLE_lvl_1,	policy => policy.Requirements.Add( new CommonAuthorizationRequirement( Controllers.IfRFBaseController.EDITORROLE_lvl_1 ) ) );
				 options.AddPolicy( Controllers.IfRFBaseController.EDITORROLE_lvl_2,	policy => policy.Requirements.Add( new CommonAuthorizationRequirement( Controllers.IfRFBaseController.EDITORROLE_lvl_2 ) ) );
				 options.AddPolicy( Controllers.IfRFBaseController.ADMINROLE,			policy => policy.Requirements.Add( new CommonAuthorizationRequirement( Controllers.IfRFBaseController.ADMINROLE ) ) );
			 } );
		}
	}	
}

namespace Whoua.Core.Api.AuthHandling
{
	#region -- Using directives --
	using System;
	using System.Linq;
	using System.Reflection;
	using System.Collections;
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
	using Whoua.Core.Api.Controllers;
	using Whoua.Core.Api.Supporting;
	using d=System.Diagnostics.Debug;
	using Microsoft.AspNetCore.Http;
	#endregion

	public class WebsealAuthenticationOptions
        : AuthenticationSchemeOptions
    {}

	public class CertAndWebsealAuthenticationHandler
        : AuthenticationHandler<WebsealAuthenticationOptions>
    {
        readonly ILoggerManager _logger;
        public CertAndWebsealAuthenticationHandler
		(
			IOptionsMonitor<WebsealAuthenticationOptions>	options
		,	ILoggerFactory									loggerFactory
		,	UrlEncoder										encoder
		,	ISystemClock									clock
		,	ILoggerManager									logger
            )
            : base(options, loggerFactory, encoder, clock)
        {
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

            _logger = logger;
        }

        private enum CertificateAuthorizationType
        {
            Invalid,
            Webseal,
            CN
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

            // First check the certificate

            string groupFromCN;
            CertificateAuthorizationType certType = GetCertificateAuthorization(out groupFromCN);

            List<Claim> claims = new();

            if (certType == CertificateAuthorizationType.Invalid)
            {
                string failureMessage = "Invalid CN in certificate";
                _logger.Warn(failureMessage);
                return Task.FromResult(AuthenticateResult.Fail(failureMessage));
            }

            string claimsType = string.Empty;
            if (certType == CertificateAuthorizationType.Webseal)
            {
                claimsType = "Webseal";
                AddWebsealClaims(ref claims);
            } 
            else if (certType == CertificateAuthorizationType.CN)
            {
                claimsType = "CN";
                AddOtherClaims(ref claims, groupFromCN);
            }

            // generate claimsIdentity on the name of the class
            var claimsIdentity = new ClaimsIdentity(claims, claimsType);

            // generate AuthenticationTicket from the Identity
            // and current authentication scheme
            var ticket = new AuthenticationTicket( new ClaimsPrincipal(claimsIdentity), this.Scheme.Name);

            // pass on the ticket to the middleware
            var res = Task.FromResult(AuthenticateResult.Success(ticket));

            return( res);
        }

        private CertificateAuthorizationType GetCertificateAuthorization(out string groupFromCN)
        {
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

            IDictionary _envVar = Environment.GetEnvironmentVariables();
            IList<string> allowedCN = _envVar["AllowedCN"].ToString().ToUpper().Split('|').ToList();

            var subject = new Dictionary<string, string>();
            var clientCert = Request.HttpContext.Connection.ClientCertificate;
            clientCert.Subject.Split(",").Select(o => o.Trim()).ToList().ForEach(x =>
            {
                var subjectKvp = x.Split("=");
                string key = subjectKvp[0];
                string value = subjectKvp[1];
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

            string subjectCN = subject["CN"].ToUpper();
            groupFromCN = string.Empty;

            if (!(allowedCN.Any(cn => cn.ToUpper().Contains(subjectCN))))
            {
                return CertificateAuthorizationType.Invalid;
            }
			
            if (subjectCN.StartsWith("INTRA.") && subjectCN.EndsWith(".WEB.BC"))
            {
                // webseal, will use http headers to get roles
                return CertificateAuthorizationType.Webseal;
            }

            if (subjectCN.StartsWith("IFRF-DEV"))
            {
                // webseal, will use http headers to get roles
                return CertificateAuthorizationType.Webseal;
            }

            // no webseal, will use first part of CN to get roles
            groupFromCN = subjectCN.Split("-")[0];
            return CertificateAuthorizationType.CN;

        }

        private void AddWebsealClaims(ref List<Claim> claims)
        {
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

            // create claims list from the model
            string UserId = Request.Headers["iv-user"].ToString();

            claims.Add(new Claim(ClaimTypes.NameIdentifier, UserId));

            if (Request.Headers.ContainsKey("iv-groups"))
            {
                string ivGroupsString = Request.Headers["iv-groups"].ToString();
                string[] ivGroups = ivGroupsString.Split(",");
                foreach (string aGroup in ivGroups)
                {
                    string aGroupUnquoted = aGroup.Replace("\"", "");
                    claims.Add(new Claim(ClaimTypes.Role, aGroupUnquoted));
                }
            }
        }

        private static void AddOtherClaims(ref List<Claim> claims, string groups)
        {
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			//2do:Add some flexible mapping between groups and roles

			//string theRole = Controllers.IfRFBaseController.EDITORROLE_lvl_2;
			if( groups == "NEDA DIVISION")
			{
				claims.Add( new Claim( ClaimTypes.NameIdentifier,	IfRFBaseController.NEDA_GRP_001));
				claims.Add( new Claim( ClaimTypes.Role,				IfRFBaseController.NEDA_GRP_001));
			}
        }
    }

    internal class CommonAuthorizationRequirement : IAuthorizationRequirement
    { 
        public string RequiredRole { get; }

        public CommonAuthorizationRequirement(string requiredRole)
        {
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

            RequiredRole = requiredRole;
        }
    }

	internal class CommonAuthorizationHandler : AuthorizationHandler<CommonAuthorizationRequirement>
    {
        private readonly IDictionary<string, IList<string>> roleGroupMappings = new Dictionary<string, IList<string>>();

        public CommonAuthorizationHandler( IConfiguration configuration)
        {
            d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

            var rgMaps = configuration.GetSection("RoleGroupMappings").GetChildren();
            foreach(IConfigurationSection anRgMap in rgMaps)
            {
                var children = anRgMap.GetChildren();
                string role = children.Single(c => c.Key.Equals("Role")).Value;
                var groups = children.Single(c => c.Key.Equals("Groups")).GetChildren().Select(gr => gr.Value).ToList();
                roleGroupMappings.Add(role, groups);
            }
        }

        protected override Task HandleRequirementAsync( AuthorizationHandlerContext context, CommonAuthorizationRequirement requirement)
        {
            d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodBase.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

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
                if (context.User.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value.Equals(anAllowedGroup)))
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
            }

            return Task.CompletedTask;
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
			services.AddSingleton<IAuthorizationHandler,	CommonAuthorizationHandler>();			
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