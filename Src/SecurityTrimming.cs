/**
 * @Name SecurityTrimming.cs
 * @Purpose 
 * @Date 03 June 2022, 19:25:23
 * @Author S.Deckers
 * @url https://github.com/jenyayel/SwaggerSecurityTrimming
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

    [Route("values")]
    public class ValuesController : Controller
    {
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        [Authorize]
        public void Put(int id, [FromBody]string value)
        {
        }

        // POST api/values
        [HttpPost]
        [Authorize(policy: "can-update")]
        public void Post([FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        [Authorize(policy: "can-delete")]
        public void Delete(int id)
        {
        }
    }

    [Route("protected")]
    [Authorize(policy: "can-update")]
    public class ValuesProtectedController : Controller
    {
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "secret1", "secret2" };
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
    using System.Threading.Tasks;
	using System.Linq;	
	using System.Reflection;
	using System.Collections.Generic;
	#endregion

	#region -- Microsoft --
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Authorization;
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
		//public static string JwtKey			= "Thisismysecretkey-make sure it is long enough";
		//public static string ValidIssuer	= "https://localhost:44371";
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

        public void ConfigureServices(IServiceCollection services)
        {
            d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

            services.AddControllers();

            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        RequireExpirationTime = false,
                        ValidateAudience = false,
                        ValidateIssuer = false,
                        // ##############################################################
                        // WARNING: this is just for demonstration purpose
                        // the signing key should be stored securely elsewhere
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("secretprivatekey42"))
                        // ##############################################################
                    };
                });
            services
                .AddAuthorization(config =>
                {
                    config.AddPolicy("can-update", p => p.RequireClaim("scopes", "api:update"));
                    config.AddPolicy("can-delete", p => p.RequireClaim("scopes", "api:delete"));
                })
                .AddHttpContextAccessor();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title           = "SecurityTrimming",
                    Version         = "v1"
                ,   Description		= "<h3>Swagger security trimming</h3><ol><li>On startup, one (1) method is visible</li><li>Token authenticated user, no scope:eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.e30.lMQZ-L2fcyy9tgM_RyOt7BCyHnnryQweBmhgVUC9Qc4</li><li>Authenticated user, scope 'can-update':eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzY29wZXMiOlsiYXBpOnVwZGF0ZSJdfQ.ok6saWx1101ygDqz-GrhHBJMyINUB2NqpE4k6BYc47s</li><li>Authenticated user, scope 'can-delete':eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzY29wZXMiOlsiYXBpOmRlbGV0ZSJdfQ.6YAU8_DLiyixE2xxoGuZnPTOo6Dzoz4cQ3QzM69p5o4</li><li>Authenticated user, scope 'can-update' & 'can-delete':eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzY29wZXMiOlsiYXBpOnVwZGF0ZSIsImFwaTpkZWxldGUiXX0.DXynNpRlNLUWevAazv4vEOLYDGzkEfI8OAnP2qihJr8</li>"
                }); 
                options.DocumentFilter<SecurityTrimming>();
                options.AddSecurityDefinition("BearerDefinition", new OpenApiSecurityScheme()
                {
                    Name            = "Authorization",
                    Type            = SecuritySchemeType.ApiKey,
                    In              = ParameterLocation.Header,
                    Scheme          = "Bearer"
                ,   Description		= "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement{
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "BearerDefinition"
                            }
                        },
                        new List<string>()
                    }});
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

            app.UseDeveloperExceptionPage();

            app.UseHttpsRedirection ( )
                .UseRouting         ( )
                .UseAuthentication  ( )
                .UseAuthorization   ( )
                .UseDefaultFiles    ( )
                .UseStaticFiles     ( )
                .UseSwagger         ( c => c.RouteTemplate = "swagger/{documentName}/schema.json")
                .UseSwaggerUI(c =>
                {
                    c.RoutePrefix = "swagger";
                    c.SwaggerEndpoint($"/swagger/v1/schema.json", $"API v1");
                    c.InjectJavascript("/swagger-custom.js"); // --- Needed for reloading document
                })
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
        }
	}

    public class SecurityTrimming : IDocumentFilter
    {
        private readonly IServiceProvider _provider;

        public SecurityTrimming(IServiceProvider provider)
        {
            d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

            _provider = provider;
        }

        public void Apply( OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

            var http = _provider.GetRequiredService<IHttpContextAccessor>();
            var auth = _provider.GetRequiredService<IAuthorizationService>();

            foreach (var description in context.ApiDescriptions)
            {
                var authAttributes = description.CustomAttributes().OfType<AuthorizeAttribute>();
                bool showAll = isForbiddenDueAnonymous(http, authAttributes) ||
                                isForbiddenDuePolicy(http, auth, authAttributes);

                if( !showAll)
                {
                    continue;
                }
                //if (!notShowen)
                //    continue; // user passed all permissions checks

                // --- Hide some parts of the document (20220603 SDE)
                var route = "/" + description.RelativePath.TrimEnd('/');
                var path = swaggerDoc.Paths[route];

                // remove method or entire path (if there are no more methods in this path)
                OperationType operation = Enum.Parse<OperationType>(description.HttpMethod, true);
                path.Operations.Remove(operation);
                if (path.Operations.Count == 0)
                {
                    swaggerDoc.Paths.Remove(route);
                }
            }
        }

        private static bool isForbiddenDuePolicy(
            IHttpContextAccessor http,
            IAuthorizationService auth,
            IEnumerable<AuthorizeAttribute> attributes)
        {
            var policies = attributes
                .Where(p => !String.IsNullOrEmpty(p.Policy))
                .Select(a => a.Policy)
                .Distinct();

            var result = Task.WhenAll(policies.Select(p => auth.AuthorizeAsync(http.HttpContext.User, p))).Result;
            return result.Any(r => !r.Succeeded);
        }

        private static bool isForbiddenDueAnonymous(
            IHttpContextAccessor http,
            IEnumerable<AuthorizeAttribute> attributes)
        {
            return attributes.Any() && !http.HttpContext.User.Identity.IsAuthenticated;
        }
    }
}
