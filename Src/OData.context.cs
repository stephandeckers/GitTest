/**
 * @Name OData.context.cs
 * @Purpose 
 * @Date 17 July 2022, 11:25:59
 * @Author S.Deckers
 * @Description 
 * PM> install-package Microsoft.AspNetCore.OData -version 8.0.10
 * PM> install-package OData.Swagger -version 1.0.0
 * 
 *  OData routing betekent dat je de routes van OData neemt, dit tegenover 'Conventional Routing". Het is van belang dat 
 *  OData routing gebruikt, dan krijg je namelijk elementen als 'odata.count' tot je beschikking
 * 
 * Paging
 * ------
 *  Server driven paging: De server bepaalt de grootte van een pagina en geeft een 'odata.nextLink' terug
 *  [EnableQuery( PageSize = 2 )]
 *  
 *  Client Driven Paging: Client bepaalt groote pagina
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
	using Swashbuckle.AspNetCore.Annotations;
	using Microsoft.AspNetCore.OData.Results;
	using Microsoft.AspNetCore.OData.Formatter;
	using Microsoft.AspNetCore.OData.Deltas;

	using d = System.Diagnostics.Debug;
	#endregion

	/// <summary>
	/// Geen metadata nodig hier
	/// </summary>
	public class ProductsController : ODataController
    {
        private static List<Product> _products = null;

		private const string swaggerControllerDescription = "Products";

		static ProductsController( )
		{
			static void init( )
			{
				d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

				_products = new List<Product>( );

				int top = 10;
				for (int i = 0; i < top; i++)
				{
					_products.Add( new Product( ) { Id = i + 1, Name = $"Product {i + 1}", Description = $"Description {i + 1}" } );
				}
			}

			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			init( );
		}

        [ HttpGet		( )]
        [ EnableQuery	( )]
		[ SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "Get Items" )]
        public IActionResult Get( )
        {
            d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty ) );
            return Ok( _products);
        }

		/// <summary date="20-07-2022, 07:07:14" author="S.Deckers">
		/// Als je 2e item nodig heb wat een lijst retouneert met OData context dien je een aparte controller
		/// aan te maken. Deze method geeft geen context terug
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
        [ HttpGet		( "api/[controller]/detail/{id:long}")]
        [ EnableQuery	( )]
		[ SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "3 Get ProductDetails" )]
        //public IEnumerable<ProductDetail> GetProductDetails( long id)
		public IActionResult GetProductDetails( long id)
        {
            d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty ) );

			int i = 1;
			List<ProductDetail> items = new List<ProductDetail>( )
			{ 
				new ProductDetail( ) { Id = i, ProductId = i, DetailName = $"Product {i} - Detail {i++}" }
			,	new ProductDetail( ) { Id = i, ProductId = i, DetailName = $"Product {i} - Detail {i++}" }
			,	new ProductDetail( ) { Id = i, ProductId = i, DetailName = $"Product {i} - Detail {i++}" }
			};

			//return( items.AsEnumerable());
			return Ok( items);
        }

		/// <summary>
		/// Dit request zit in de OData routing table, maar ondanks dat heb ik hier geen count
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
        [ HttpGet		( )]
        [ EnableQuery	( )]
		[ SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "2 Get ProductDetails for category" )]
        public IEnumerable<ProductDetail> GetByCategoryId( long id)
        {
            d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty ) );

			int i = 1;
			List<ProductDetail> items = new List<ProductDetail>( )
			{ 
				new ProductDetail( ) { Id = i, ProductId = i, DetailName = $"Product {i} - Detail {i++}" }
			,	new ProductDetail( ) { Id = i, ProductId = i, DetailName = $"Product {i} - Detail {i++}" }
			,	new ProductDetail( ) { Id = i, ProductId = i, DetailName = $"Product {i} - Detail {i++}" }
			};

			return( items.AsEnumerable());
        }

		[ HttpGet		( )]
        [ EnableQuery	( )]
		[ SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "Get Item" )]
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

        [ HttpPost( )]
		[ SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "Create Item" )]
        public IActionResult Post([FromBody]Product product, CancellationToken token)
        {
            d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty ) );

            _products.Add(product);
            return Created(product);
        }

        [ HttpPatch( )]
		[ SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "Update Item" )]
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

        [ HttpDelete( )]
		[ SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "Delete Item" )]
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
    }

	public class ProductDetailsController : ODataController
	{
		private const string swaggerControllerDescription = "ProductDetails";

        [ HttpGet		( )]
        [ EnableQuery	( )]
		[ SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "Get Items" )]
        public IActionResult Get( )
        {
            d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty ) );

			int i = 1;
			List<ProductDetail> items = new List<ProductDetail>( )
			{ 
				new ProductDetail( ) { Id = i, ProductId = i, DetailName = $"Product {i} - Detail {i++}" }
			,	new ProductDetail( ) { Id = i, ProductId = i, DetailName = $"Product {i} - Detail {i++}" }
			,	new ProductDetail( ) { Id = i, ProductId = i, DetailName = $"Product {i} - Detail {i++}" }
			};

            return Ok( items);
        }
	}

	public class CategoryProductsController : ODataController
    {
        private static List<Product> _products = null;

		private const string swaggerControllerDescription = "CategoryProducts";

		static CategoryProductsController( )
		{
			static void init( )
			{
				d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

				_products = new List<Product>( );

				int top = 20;
				for (int i = 0; i < top; i++)
				{
					_products.Add( new Product( ) { Id = i + 1, Name = $"cProduct {i + 1}", Description = $"cDescription {i + 1}" } );
				}
			}

			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			init( );
		}

        [ HttpGet		( )]
        [ EnableQuery	( )]
		[ SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "Get Items" )]
        public IActionResult Get( )
        {
            d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty ) );
            return Ok( _products);
        }
    }
}

namespace Whoua.Core.Data.Entities
{
	public class Product
    {
        public int		Id			{ get; set; }
        public string	Name		{ get; set; }
        public string	Description { get; set; }
    }

	public class ProductDetail
    {
        public int		Id			{ get; set; }
		public int		ProductId	{ get; set; }
        public string	DetailName	{ get; set; }
    }
}

namespace Whoua.Core.Api
{
	#region -- Using directives --
	using System;
	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.AspNetCore.OData;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Hosting;
	using Microsoft.OData.Edm;
	using Microsoft.OData.ModelBuilder;
	using Microsoft.OpenApi.Models;
	using Swashbuckle.AspNetCore.SwaggerUI;

	using Whoua.Core;
	using Whoua.Core.Api.Supporting;
	using Whoua.Core.Data.Entities;

	using d=System.Diagnostics.Debug;
	#endregion

	public class Startup
	{
		/// <summary date="04-06-2021, 12:11:46" author="S.Deckers">
		/// Configures the services.
		/// </summary>
		/// <param name="services">The services.</param>
		public void ConfigureServices( IServiceCollection services)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));			

			services.AddSwaggerGen(c =>
			{
				c.OperationFilter<SwaggerQueryParameter>( );
				c.SwaggerDoc		( name: "v1", info: new Microsoft.OpenApi.Models.OpenApiInfo { Title = "OData context", Version = "v1" });
				c.EnableAnnotations	( );
				//c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
			});

			string routename = "api"; // ---  needs to map routename in controller (20220716 SDE)

			services
				.AddControllers( )
				.AddOData( options =>
				 {
					 options
						 .Select( )
						 .Filter( )
						 .OrderBy( )
						 .Count( )
						 .SetMaxTop( 250 )
						 .Expand( );

					 options.AddRouteComponents( routePrefix:routename, model: GetEdmModel( ) );
				 } );
		}

		public void Configure( IApplicationBuilder app, IWebHostEnvironment env )
		{
            d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

			app.UseDeveloperExceptionPage();
			app.UseODataRouteDebug();			
			//app.UseODataOpenApi();    // If you want to use /$openapi, enable the middleware.
			// Add OData /$query middleware
			//app.UseODataQueryRequest();

			// Add the OData Batch middleware to support OData $Batch
			//app.UseODataBatching();

			app.UseSwagger();

            app.UseSwaggerUI				( c => 
			{ 
				c.SwaggerEndpoint			( "/swagger/v1/swagger.json", "OData 8.x OpenAPI"); 
				c.DisplayOperationId		( );
				c.EnableTryItOutByDefault	( );
			});

			app.UseRouting();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});		
		}

		private static IEdmModel GetEdmModel( )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty ) );

			var builder = new ODataConventionModelBuilder( );

			string controllerName = "Products"; // --- Needs to Match (20220717 SDE)
			builder.EntitySet<Whoua.Core.Data.Entities.Product>( name: controllerName );
			builder.EntitySet<Whoua.Core.Data.Entities.Product>( name: "CategoryProducts" );

			builder.EntityType<Product>().Collection.Function("GetByCategoryId").ReturnsCollectionFromEntitySet<Product>("Products");

			//ProductDetailsController

			builder.EntitySet<Whoua.Core.Data.Entities.ProductDetail>( name: "ProductDetails" );

			return builder.GetEdmModel( );
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