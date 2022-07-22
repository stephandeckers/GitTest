/**
 * @Name Swagger6.cs
 * @Purpose 
 * @Date 20 October 2021, 19:33:41
 * @Author S.Deckers
 * @Description 
 */

namespace Whoua.Core
{
	public static class Global
	{
		public static int CallCount = 0;
	}
}

namespace Whoua.Core.Api
{
	internal sealed partial class AssemblyProperties
	{
		public const string Author		= "SDE Computing";
		public const string Description = "Foo Rulez";
		public const string Name		= "OData zTuff";
		public const string Version		= "1.2";
	}
}

namespace Whoua.Core.Api.Controllers
{
	#region -- Using directives --
	using System;
	using System.Collections.Generic;

	using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

	using Whoua.Core.Data.Model;

	using d = System.Diagnostics.Debug;

	#endregion

	[ Route("api/[controller]")]
	[ ApiController( )]
	public class EmployeeController : ControllerBase
	{
		[ HttpGet]
		[ SwaggerOperation( Tags = new[] { "Employee" }, Summary = "Get Employees" )]
		public IEnumerable<Employee> GetAll()
		{
			return GetEmployees();
		}

		//[ HttpGet("{id}", Name = "Get")]
		[ HttpGet("{id}")]
		[ SwaggerOperation( Tags = new[] { "Employee" }, Summary = "Get single Employee" )]
		public Employee Get(int id)
		{
			return GetEmployees().Find(e => e.Id == id);
		}

		[ HttpPost]
		[ Produces("application/json")]
		[ ProducesResponseType( 201)]
		[ ProducesResponseType( 400)]
		[ ProducesResponseType( 666)]
		[ Swashbuckle.AspNetCore.Annotations.SwaggerOperation( Tags = new[] { "Employee" }, Summary = "Create single Item" )]
		public Employee Post([FromBody] Employee employee)
		{
			// Logic to create new Employee
			return new Employee();
		}

		[ HttpPut("{id}")]
		[ SwaggerOperation( Tags = new[] { "Employee" }, Summary = "Put Employee" )]
		public void Put(int id, [FromBody] Employee employee)
		{
			// Logic to update an Employee
		}

		[ HttpDelete("{id}")]
		[ SwaggerOperation( Tags = new[] { "Employee" }, Summary = "Delete Employee" )]
		public void Delete(int id)
		{
		}

		private List<Employee> GetEmployees()
		{
			return new List<Employee>()
			{
				new Employee()
				{
					Id = 1,
					FirstName= "John",
					LastName = "Smith",
					EmailId ="John.Smith@gmail.com"
				},
				new Employee()
				{
					Id = 2,
					FirstName= "Jane",
					LastName = "Doe",
					EmailId ="Jane.Doe@gmail.com"
				}
			};
		}
	}
}

namespace Whoua.Core.Data.Model
{
	#region -- Using directives --
	using System.ComponentModel.DataAnnotations;
	#endregion

	public class Employee
	{
		public int		Id			{ get; set; }
		public string	FirstName	{ get; set; }
		public string	LastName	{ get; set; }

		[Required]
		public string	EmailId		{ get; set; }
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
	using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

	using Whoua.Core;
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

		/// <summary date="04-06-2021, 12:11:46" author="S.Deckers">
		/// Configures the services.
		/// </summary>
		/// <param name="services">The services.</param>
		public void ConfigureServices( IServiceCollection services)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

			// Register the Swagger generator, defining 1 or more Swagger documents
			services.AddSwaggerGen(c =>
			{
				c.EnableAnnotations( );
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
			});
			services.AddControllers();
		}

		public void Configure( IApplicationBuilder app, IWebHostEnvironment env)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

			app.UseDeveloperExceptionPage();

			// Enable middleware to serve generated Swagger as a JSON endpoint.
			app.UseSwagger();
			// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
			// specifying the Swagger JSON endpoint.
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
				c.DisplayOperationId();
			});

			//app.useswaggerui(c =>
			//{
			//	c.swaggerendpoint(url: swaggerendpointifrf, name: swaggerversionifrf);
			//	c.defaultmodelsexpanddepth(1);
			//	c.defaultmodelexpanddepth(1);
			//	c.defaultmodelrendering(modelrendering.model);
			//	c.displayoperationid();
			//	c.injectstylesheet("/css/swaggercompactstyle.css");
			//});
		}
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