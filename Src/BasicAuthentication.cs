/**
 * @Name BasicAuthentication.cs
 * @Purpose 
 * @Date 03 June 2022, 11:16:49
 * @Author S.Deckers
 * @url https://medium.com/nerd-for-tech/authentication-and-authorization-in-net-core-web-api-using-jwt-token-and-swagger-ui-cc8d05aef03c
 * @Description 
 * JWT  : JSON Web Token (authorisatie)
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

    [ Produces      ( "application/json")]
    [ Route         ( "api/[controller]")]
    [ ApiController ( )]
    [ Authorize     ( AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
    public class BaseController : ControllerBase
    { }

    public class AuthenticateController : BaseController
    {
        private const string swaggerControllerDescription = "AuthenticateController";

        private IConfiguration _config;

        public AuthenticateController(IConfiguration config)
        {
            d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

            _config = config;
        }

        private string GenerateJSONWebToken(LoginModel userInfo)
        {
            d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

            string key      = Global.JwtKey;
            string issuer   = Global.ValidIssuer;

            var securityKey = new SymmetricSecurityKey  ( Encoding.UTF8.GetBytes( key));
            var credentials = new SigningCredentials    ( securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken( issuer:issuer, audience:issuer, claims:null, expires: DateTime.Now.AddMinutes(120), signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Original implementation
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        private LoginModel AuthenticateUserSync( LoginModel login)
        {
            d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

            LoginModel user = null;

            //Validate the User Credentials    
            //Demo Purpose, I have Passed HardCoded User Information    
            if (login.UserName == "Jay")
            {
                user = new LoginModel { UserName = "Jay", Password = "123456" };
            }
            return user;
        }

        [ AllowAnonymous    ( )]
        [ HttpPost          ( nameof(LoginSync))]
        [ SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "LoginSync" )]
        public IActionResult LoginSync( [FromBody] LoginModel data)
        {
            d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

            IActionResult response = Unauthorized();
            var user = AuthenticateUserSync(data);
            if (user != null)
            {
                var tokenString = GenerateJSONWebToken(user);
                response = Ok(new { Token = tokenString , Message = "Success" });
            }
            return response;
        }

        [ AllowAnonymous    ( )]
        [ HttpPost          ( nameof(LoginAsync))]
        [ SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "LoginASync" )]
        public async Task<IActionResult> LoginAsync([FromBody] LoginModel data)
        {
            IActionResult response = Unauthorized();
            var user = await AuthenticateUserASync(data);
            if(user != null)
            {
                var tokenString = GenerateJSONWebToken(user);
                response = Ok(new { Token = tokenString , Message = "Success" });
            }
            return response;
        }

        private async Task<LoginModel> AuthenticateUserASync(LoginModel login)
        {
            LoginModel AuthenticateUserSync( LoginModel login)
            {
                d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

                LoginModel user = null;

                if( login.UserName == "Jay")
                {
                    user = new LoginModel { UserName = "Jay", Password = "123456" };
                }
                return user;
            }
            d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

            var result = await Task.Run(() => AuthenticateUserSync( login));
            return( result);
        }

        [ HttpGet(nameof(Get))]
        public async Task<IEnumerable<string>> Get()
        {
            d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

            var accessToken = await HttpContext.GetTokenAsync("access_token");

            List<string> items = new List<string>( )
            {
                "Success:"
            ,   DateTime.Now.ToString()
            };

            return( items);
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

    public class LoginModel
    {
        [ Required()]
        public string UserName { get; set; }

        [ Required()]
        public string Password { get; set; }
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
	using d = System.Diagnostics.Debug;	
	#endregion

	#endregion

	public static class Global
	{
		public static int	 CallCount		= 0;
		public static string JwtKey			= "Thisismysecretkey-make sure it is long enough";
		public static string ValidIssuer	= "https://localhost:44371";
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
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

		public void ConfigureServices( IServiceCollection services )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

            services.AddControllers	( );
            services.AddMvc			( );

            // Enable Swagger 
            services.AddSwaggerGen(swagger =>
            {
                //This is to generate the Default UI of Swagger Documentation
                swagger.SwaggerDoc("v1", new OpenApiInfo
                { 
                    Version			= "v1", 
                    Title			= "JWT Token Example",
                    Description		= "<h3>Json Web Token (JWT) Authorisation example</h3><ol><li>Authorized method fails</li><li>Login to get a token</li><li>Apply to token using the format 'Bearer token' in the 'Authorize' section</li><li>Authorized method works</li>"
                });

                // To Enable authorization using Swagger (JWT)
                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name			= "Authorization",
                    Type			= SecuritySchemeType.ApiKey,
                    Scheme			= "Bearer",
                    BearerFormat	= "JWT",
                    In				= ParameterLocation.Header,
                    Description		= "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                });

                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type	= ReferenceType.SecurityScheme,
                                    Id		= "Bearer"
                                }
                            },
                            new string[] {}
                    }
                });
            });

			/* --- Ophalen json :
            string validIssuer      = Configuration["Jwt:Issuer"]; 

			json : 
 "Jwt": {
    "Key": "Thisismysecretkey",
    "Issuer": "https://localhost:44371"
  }
*/
			string validIssuer		= Global.ValidIssuer;
            string validAudience    = Global.ValidIssuer;
			string issuerSigningKey	= Global.JwtKey;

            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme	= JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme		= JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer				= true,
                    ValidateAudience			= true,
                    ValidateLifetime			= false,
                    ValidateIssuerSigningKey	= true,
                    ValidIssuer					= validIssuer,
                    ValidAudience				= validAudience,
                    IssuerSigningKey			= new SymmetricSecurityKey(Encoding.UTF8.GetBytes( issuerSigningKey))
                };
            });
		}

		/// <summary date="03-06-2022, 11:19:19" author="S.Deckers">
		/// Configure
		/// </summary>
		/// <param name="app"></param>
		/// <param name="env"></param>
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
            app.UseSwaggerUI				( c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1"); });
		}
	}
}