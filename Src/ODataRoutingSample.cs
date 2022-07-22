/**
 * @Name ODataRoutingSample.cs
 * @Purpose 
 * @Date 16 July 2022, 21:26:34
 * @Author S.Deckers
 * @Description https://github.com/OData/AspNetCoreOData/tree/main/sample/ODataRoutingSample lokaal nagebouwd met swagger
 * ondersteuning
 */

namespace Whoua.Core.Api.Controllers
{
	#region -- Using directives --
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using Microsoft.AspNetCore.Mvc;

	using Microsoft.AspNetCore.OData.Query;
	using Microsoft.AspNetCore.OData.Routing.Controllers;
	using Microsoft.AspNetCore.OData.Routing.Attributes;

	using Whoua.Core.Data.Entities;
	using Whoua.Core.Api.Supporting;

	using d = System.Diagnostics.Debug;
	using Swashbuckle.AspNetCore.Annotations;
	using Microsoft.AspNetCore.OData.Results;
	using Microsoft.AspNetCore.OData.Formatter;
	using Microsoft.AspNetCore.OData.Deltas;
	#endregion

	public class ProductsController : ODataController/*ControllerBase*/
    {
        private static List<Product> _products = null;
        public ProductsController()
        {
            d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty ) );

            if( _products != null)
            {
                return;
            }

            int i = 1;
            _products = new List<Product>
            {
                new Product
                {
                    Id = i++,
                    Category = "Goods",
                    Color = Color.Red,
                    CreatedDate = new DateTimeOffset(2001, 4, 15, 16, 24, 8, TimeSpan.FromHours(-8)),
                    UpdatedDate = new DateTimeOffset(2011, 2, 15, 16, 24, 8, TimeSpan.FromHours(-8)),
                    Detail = new ProductDetail { Id = "3", Info = "Zhang" },
                },
                new Product
                {
                    Id = i++,
                    Category = "Magazine",
                    Color = Color.Blue,
                    CreatedDate = new DateTimeOffset(2021, 12, 27, 9, 12, 8, TimeSpan.FromHours(-8)),
                    UpdatedDate = null,
                    Detail = new ProductDetail { Id = "4", Info = "Jinchan" },
                },
                new Product
                {
                    Id = i++,
                    Category = "Fiction",
                    Color = Color.Green,
                    CreatedDate = new DateTimeOffset(1978, 11, 15, 9, 24, 8, TimeSpan.FromHours(-8)),
                    UpdatedDate = new DateTimeOffset(1987, 2, 25, 5, 1, 8, TimeSpan.FromHours(-8)),
                    Detail = new ProductDetail { Id = "5", Info = "Hollewye" },
                },
            };
        }

        [HttpGet]
        //[EnableQuery]
        [ EnableQuery( PageSize=2)] // get odata.nextLink
        public IActionResult Get(CancellationToken token)
        {
            d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty ) );
            return Ok( _products);
        }

        [EnableQuery]
        [HttpGet]
        public IActionResult Get(int key)
        {
            d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty ) );

            var product = _products.FirstOrDefault(p => p.Id == key);
            if (product == null)
            {
                return NotFound($"Not found product with id = {key}");
            }

            return Ok(product);
        }

        [HttpPost]
       // [EnableQuery]
        public IActionResult Post([FromBody]Product product, CancellationToken token)
        {
            d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty ) );

            _products.Add(product);
            return Created(product);
        }

        [HttpPut]
        public IActionResult Put(int key, [FromBody]Delta<Product> product)
        {
            d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty ) );

            var original = _products.FirstOrDefault(p => p.Id == key);
            if (original == null)
            {
                return NotFound($"Not found product with id = {key}");
            }

            product.Put(original);
            return Updated(original);
        }

        [HttpPatch]
        public IActionResult Patch(int key, Delta<Product> product)
        {
            d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty ) );

            var original = _products.FirstOrDefault(p => p.Id == key);
            if (original == null)
            {
                return NotFound($"Not found product with id = {key}");
            }

            product.Patch(original);
            return Updated(original);
        }

        [HttpDelete]
        public IActionResult Delete(int key)
        {
            d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty ) );

            var original = _products.FirstOrDefault(p => p.Id == key);
            if (original == null)
            {
                return NotFound($"Not found product with id = {key}");
            }

            _products.Remove(original);
            return Ok();
        }

        [HttpGet]
        // ~/....(minSalary=4, maxSalary=5, aveSalary=9)
        public string GetWholeSalary(int minSalary, int maxSalary, string aveSalary/*, CancellationToken token, ODataQueryOptions queryOptions, ODataPath path*/)
        {
            d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty ) );

            return $"Products/GetWholeSalary: {minSalary}, {maxSalary}, {aveSalary}";
        }

        [HttpGet]
        // ~/....(minSalary=4, maxSalary=5)
        public string GetWholeSalary(int minSalary, int maxSalary)
        {
            d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty ) );

            // return $"Products/GetWholeSalary: {minSalary}, {maxSalary}";
            return GetWholeSalary(minSalary, maxSalary, aveSalary: "9");
        }

        [HttpGet]
        [ActionName("GetWholeSalary")] //
        // ~/....(minSalary=4, maxSalary=5)
        public string GetWholeSalary(int minSalary, string aveSalary)
        {
            d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty ) );

            int maxSalary = 10;
            // return $"Products/GetWholeSalary: {minSalary}, {maxSalary}";
            return GetWholeSalary(minSalary, maxSalary, aveSalary);
        }

        [HttpGet]
        // ~/....(minSalary=4)
        // ~/....(minSalary=4, name='abc')
        public string GetWholeSalary(int minSalary, /*[FromBody]*/double name)
        {
            d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty ) );

            return $"Products/GetWholeSalary: {minSalary}, {name}";
        }

        [HttpGet]
        // GetWholeSalary(order={order}, name={name})
        // http://localhost:5000/Products/Default.GetWholeSalary(order='2',name='abc')
        public string GetWholeSalary(string order, string name)
        {
            d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty ) );
            return $"Products/GetWholeSalary: {order}, {name}";
        }

        [HttpGet]
        public string GetOptional()
        {
            d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty ) );

            return "GetOptional without parameter";
        }

        [HttpGet]
        public string GetOptional(string param)
        {
            return $"GetOptional without parameter value: param = {param}";
        }

        [HttpGet("CalculateSalary(minSalary={min},maxSalary={max})")]
        public string CalculateSalary(int min, int max)
        {
            return $"Unbound function call on CalculateSalary: min={min}, max={max}";
        }

        [HttpGet("CalculateSalary(minSalary={min},maxSalary={max},wholeName={name})")]
        public string CalculateSalary(int min, int max, string name)
        {
            return $"Unbound function call on CalculateSalary: min={min}, max={max}, name={name}";
        }
    }
}

namespace Whoua.Core.Data.Entities
{
	#region -- Using directives --
	using System.Collections.Generic;
	using System.Linq;
	using System.Web;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.ComponentModel.DataAnnotations;
	#endregion

    public class ProductDetail
    {
        public string Id    { get; set; }
        public string Info  { get; set; }
    }

    public enum Color
    {
        Red,
        Green,
        Blue
    }

    public class Product
    {
        public int                      Id          { get; set; }
        public string                   Category    { get; set; }
        public Color                    Color       { get; set; }
        public DateTimeOffset           CreatedDate { get; set; }
        public DateTimeOffset?          UpdatedDate { get; set; }
        public virtual ProductDetail    Detail      { get; set; }
    }

    public class Person
    {
        // [Column(Order = 1)] // This attribute can be used with [Key] convention model building
                               // It is ignored if the property is added explicitly.
        public string FirstName { get; set; }

        // [Column(Order = 2)]
        public string LastName { get; set; }
    }

    public class Category
    {
        public int Id { get; set; }
    }
    public class Order
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public virtual Category Category { get; set; }
    }

    public class VipOrder : Order
    {
        public virtual Category VipCategory { get; set; }
    }
}

namespace Whoua.Core.Api
{
	#region -- Using directives --
	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Hosting;
	using Microsoft.OData.Edm;
	using Microsoft.AspNetCore.Http;
	using Microsoft.AspNetCore.OData;
	using Whoua.Core;
	using Whoua.Core.Api.Supporting;
	using Whoua.Core.Data.Entities;	
	using Microsoft.OData.ModelBuilder;
    using d=System.Diagnostics.Debug;
	#endregion

	public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

            IEdmModel model0 = EdmModelBuilder.GetEdmModel();
			services.AddControllers( )
				.AddOData( opt => opt.Count( ).Filter( ).Expand( ).Select( ).OrderBy( ).SetMaxTop( 5 )
					 .AddRouteComponents( model0 )
				);

			//services
			//	.AddControllers( )
			//	.AddOData( options =>
			//	 {
			//		 options
			//			 .Select( )
			//			 .Filter( )
			//			 .OrderBy( )
			//			 .Count( )
			//			 .SetMaxTop( 250 )
			//			 .Expand( );

			//		 options.AddRouteComponents( model: model0 );
			//	 } );

            services.AddSwaggerGen( c => c.OperationFilter<SwaggerQueryParameter>( ) );

            /*
			services.AddSwaggerGen(c =>
			{
				c.OperationFilter<SwaggerQueryParameter>( );
				//c.SwaggerDoc		( name: "v1", info: new Microsoft.OpenApi.Models.OpenApiInfo { Title = "OData context", Version = "v1" });
				c.OrderActionsBy	( (apiDesc) => $"{apiDesc.ActionDescriptor.RouteValues["controller"]}_{apiDesc.HttpMethod}");
				c.EnableAnnotations	( );
				//c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
			});
            */
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

			app.UseDeveloperExceptionPage();
			app.UseODataRouteDebug();			
			//app.UseODataOpenApi();    // If you want to use /$openapi, enable the middleware.
			// Add OData /$query middleware
			app.UseODataQueryRequest();

			// Add the OData Batch middleware to support OData $Batch
			app.UseODataBatching();

			app.UseSwagger();

            app.UseSwaggerUI				( c => 
			{ 
				c.SwaggerEndpoint			( "/swagger/v1/swagger.json", "OData 8.x OpenAPI"); 
				c.DisplayOperationId		( );
				c.EnableTryItOutByDefault	( );
			});

			app.UseRouting();

			// Test middleware
			app.Use(next => context =>
			{
				var endpoint = context.GetEndpoint();
				if (endpoint == null)
				{
					return next(context);
				}

				return next(context);
			});

			//app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
        }
    }

	public static class EdmModelBuilder
    {
        public static IEdmModel GetEdmModel()
        {
            var builder = new ODataConventionModelBuilder();
            builder.EntitySet<Product>("Products");
            builder.EntitySet<Person>("People").EntityType.HasKey(c => new { c.FirstName, c.LastName });

            // use the following codes to set the order and change the route template.
            // builder.EntityType<Person>().Property(c => c.FirstName).Order = 2;
            // builder.EntityType<Person>().Property(c => c.LastName).Order = 1;

            // function with optional parameters
            var functionWithOptional = builder.EntityType<Product>().Collection.Function("GetWholeSalary").ReturnsCollectionFromEntitySet<Order>("Orders");
            functionWithOptional.Parameter<int>("minSalary");
            functionWithOptional.Parameter<int>("maxSalary").Optional();
            functionWithOptional.Parameter<string>("aveSalary").HasDefaultValue("129");

            // overload
            functionWithOptional = builder.EntityType<Product>().Collection.Function("GetWholeSalary").ReturnsCollectionFromEntitySet<Order>("Orders");
            functionWithOptional.Parameter<int>("minSalary");
            functionWithOptional.Parameter<double>("name");

            // overload
            functionWithOptional = builder.EntityType<Product>().Collection.Function("GetWholeSalary").ReturnsCollectionFromEntitySet<Order>("Orders");
            functionWithOptional.Parameter<string>("order");
            functionWithOptional.Parameter<string>("name");

            // function with only one parameter (optional)
            functionWithOptional = builder.EntityType<Product>().Collection.Function("GetOptional").ReturnsCollectionFromEntitySet<Order>("Orders");
            functionWithOptional.Parameter<string>("param").Optional();

            // unbound
            builder.Action("ResetData");

            // using attribute routing
            var unboundFunction = builder.Function("CalculateSalary").Returns<string>();
            unboundFunction.Parameter<int>("minSalary");
            unboundFunction.Parameter<int>("maxSalary").Optional();
            unboundFunction.Parameter<string>("wholeName").HasDefaultValue("abc");
            return builder.GetEdmModel();
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

namespace Whoua.Core
{
	public static class Global
	{
		public static int CallCount = 0;
	}
}

namespace Whoua.Core.Api.Supporting
{
	#region -- Using directives --
	using System;
	using System.IO;
	using System.Linq;	
	using System.Reflection;
	using System.Collections.Generic;
	using Microsoft.AspNetCore.Mvc.Controllers;
	using Swashbuckle.AspNetCore.SwaggerGen;
	using Microsoft.OpenApi.Models;
	using Microsoft.AspNetCore.OData.Query;
	using Whoua.Core.Data;
	using Whoua.Core.Data.Entities;
	using d = System.Diagnostics.Debug;	
	#endregion

	public class SwaggerQueryParameter : IOperationFilter
	{
		public void Apply(OpenApiOperation operation, OperationFilterContext context)
		{
			if (operation.Parameters == null) operation.Parameters = new List<OpenApiParameter>();

			var descriptor = context.ApiDescription.ActionDescriptor as ControllerActionDescriptor;
			var queryAttr = descriptor?.FilterDescriptors
				.Where(fd => fd.Filter is EnableQueryAttribute)
				.Select(fd => fd.Filter as EnableQueryAttribute)
				.FirstOrDefault();

			if (queryAttr == null)
				return;

			if (queryAttr.AllowedQueryOptions.HasFlag(AllowedQueryOptions.Select))
				operation.Parameters.Add(new OpenApiParameter()
				{
					Name		= "$select"
				,	In			= ParameterLocation.Query
				,	Schema		= new OpenApiSchema	{ Type = "string" }
				,	Required	= false
				});

			if (queryAttr.AllowedQueryOptions.HasFlag(AllowedQueryOptions.Expand))
				operation.Parameters.Add(new OpenApiParameter()
				{
					Name		= "$expand"
				,	In			= ParameterLocation.Query
				,	Schema		= new OpenApiSchema	{ Type = "string" }
				,	Required	= false
				});

			if (queryAttr.AllowedQueryOptions.HasFlag(AllowedQueryOptions.Filter))
				operation.Parameters.Add(new OpenApiParameter()
				{
					Name		= "$filter"
				,	In			= ParameterLocation.Query
				,	Schema		= new OpenApiSchema	{ Type = "string" }
				,	Required	= false
				});

			if (queryAttr.AllowedQueryOptions.HasFlag(AllowedQueryOptions.Top))
				operation.Parameters.Add(new OpenApiParameter()
				{
					Name		= "$top"
				,	In			= ParameterLocation.Query
				,	Schema		= new OpenApiSchema { Type = "string" }
				,	Required	= false
				});

			if (queryAttr.AllowedQueryOptions.HasFlag(AllowedQueryOptions.Skip))
				operation.Parameters.Add(new OpenApiParameter()
				{
					Name		= "$skip"
				,	In			= ParameterLocation.Query
				,	Schema		= new OpenApiSchema { Type = "string" }
				,	Required	= false
				});


			if (queryAttr.AllowedQueryOptions.HasFlag(AllowedQueryOptions.Skip))
				operation.Parameters.Add(new OpenApiParameter()
				{
					Name		= "$orderby"
				,	In			= ParameterLocation.Query
				,	Schema		= new OpenApiSchema { Type = "string" }
				,	Required	= false
				});

			if (queryAttr.AllowedQueryOptions.HasFlag(AllowedQueryOptions.Count))
				operation.Parameters.Add(new OpenApiParameter()
				{
					Name		= "$count"
				,	In			= ParameterLocation.Query
				,	Schema		= new OpenApiSchema	{ Type = "boolean" }
				,	Required	= false,
				});

			if (queryAttr.AllowedQueryOptions.HasFlag(AllowedQueryOptions.Format))
				operation.Parameters.Add(new OpenApiParameter()
				{
					Name		= "$format"
				,	In			= ParameterLocation.Query
				,	Schema		= new OpenApiSchema	{ Type = "string" }
				,	Required	= false
				});

			if (queryAttr.AllowedQueryOptions.HasFlag(AllowedQueryOptions.SkipToken))
				operation.Parameters.Add(new OpenApiParameter()
				{
					Name		= "$skiptoken"
				,	In			= ParameterLocation.Query
				,	Schema		= new OpenApiSchema	{ Type = "string" }
				,	Required	= false
				});

			if (queryAttr.AllowedQueryOptions.HasFlag(AllowedQueryOptions.Apply))
				operation.Parameters.Add(new OpenApiParameter()
				{
					Name		= "$apply"
				,	In			= ParameterLocation.Query
				,	Schema		= new OpenApiSchema	{ Type = "string" }
				,	Required	= false
				});

			if (queryAttr.AllowedQueryOptions.HasFlag(AllowedQueryOptions.Compute))
				operation.Parameters.Add(new OpenApiParameter()
				{
					Name		= "$compute"
				,	In			= ParameterLocation.Query
				,	Schema		= new OpenApiSchema	{ Type = "string" }
				,	Required	= false
				});

			if (queryAttr.AllowedQueryOptions.HasFlag(AllowedQueryOptions.Search))
				operation.Parameters.Add(new OpenApiParameter()
				{
					Name		= "$search"
				,	In			= ParameterLocation.Query
				,	Schema		= new OpenApiSchema	{ Type = "string" }
				,	Required	= false
				});

			if (queryAttr.AllowedQueryOptions.HasFlag(AllowedQueryOptions.Supported))
				operation.Parameters.Add(new OpenApiParameter()
				{
					Name		= "$supported"
				,	In			= ParameterLocation.Query
				,	Schema		= new OpenApiSchema	{ Type = "string" }
				,	Required	= false
				});
		}
	}
}