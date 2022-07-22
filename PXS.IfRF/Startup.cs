/**
 * @Name Startup.cs
 * @Purpose 
 * @Date 26 May 2021, 13:07:41
 * @Author S.Deckers
 * @Description 
 */

namespace PXS.IfRF
{
	#region -- Using directives --
	using System.Linq;
	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Hosting;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.AspNet.OData.Extensions;
	using OData.Swagger.Services;
	using Microsoft.OData.Edm;
	using Microsoft.AspNet.OData.Builder;
	using PXS.IfRF.ErrorHandling;
	using System;
	using PXS.IfRF.Controllers;
	using PXS.IfRF.Logging;
	using PXS.IfRF.Data.Model;
	using Swashbuckle.AspNetCore.SwaggerUI;
	using Newtonsoft.Json;
	using System.Collections;
	using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Rewrite;
	using Microsoft.AspNetCore.Authentication.Certificate;
	using PXS.IfRF.AuthHandling;
	using d=System.Diagnostics.Debug;
	#endregion

	public class Startup
	{
		private readonly string swaggerVersionIfRF;

		public const string PRXCORS				= "PrxCors";
		public const string CERTIFICATESCHEME	= "Certificate";

		private readonly LoggerManager	_logger;
		private readonly IDictionary	_envVar;

		/// <summary>
		/// Initializes a new instance of the <see cref="Startup"/> class.
		/// </summary>
		public Startup()
		{
			_envVar = Environment.GetEnvironmentVariables();

			Version assemblyVersion = GetType().Assembly.GetName().Version;

			swaggerVersionIfRF = "v" + assemblyVersion.Major + "." + assemblyVersion.Minor;

			CheckEnvironmentVariables( );

			string fileLogLevel		= _envVar["FileLogLevel"].ToString( );
			string consoleLogLevel	= _envVar["ConsoleLogLevel"].ToString( );

			_logger = new LoggerManager(fileLogLevel, consoleLogLevel);

			_logger.Debug("Logger initialized");
		}

		private void CheckEnvironmentVariables( )
		{
			if( _envVar["FileLogLevel"] == null)		throw new NotSupportedException("Environment variable 'FileLogLevel' not set");
			if( _envVar["ConsoleLogLevel"] == null)		throw new NotSupportedException("Environment variable 'ConsoleLogLevel' not set");
			if( _envVar["ConnectionString"] == null)	throw new NotSupportedException("Environment variable 'ConnectionString' not set");
		}

		public void ConfigureServices(IServiceCollection services)
		{
			_logger.Debug("Configuring services");

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

			services.AddControllers()
				.AddNewtonsoftJson(
				options =>
				{
					options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
					options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
					options.SerializerSettings.ContractResolver = ShouldSerializeContractResolver.Instance;
				})
				.AddJsonOptions(
				options =>
				{
					options.JsonSerializerOptions.PropertyNamingPolicy = null;
					options.JsonSerializerOptions.IgnoreNullValues = true;
				}
				);

			services.AddOData();

			// --- Swagger pages after selecting dropdown(20210604 SDE)

			services.AddSwaggerGen(c =>
			{
				c.OperationFilter<SwaggerQueryParameter>();
				c.OperationFilter<SwaggerOperationFilter>();
				c.SchemaFilter<DeltaSchemaFilter>();
				c.SwaggerDoc(name: swaggerVersionIfRF, info: new Microsoft.OpenApi.Models.OpenApiInfo { Title = "rented-fiber-areas", Version = swaggerVersionIfRF });
				c.OrderActionsBy((apiDesc) => $"{apiDesc.ActionDescriptor.RouteValues["controller"]}_{apiDesc.HttpMethod}");
				c.EnableAnnotations();
			});

			services.AddOdataSwaggerSupport();

			string connString = _envVar["ConnectionString"].ToString( );

			services.AddDbContext<ModelContext>(options => options.UseOracle(connString));

			services.AddAuthentication( IfRFBaseController.AUTHSCHEME )
			.AddScheme<WebsealAuthenticationOptions, CertAndWebsealAuthenticationHandler>( IfRFBaseController.AUTHSCHEME, op => { } );

			services.AddAuthorization( options =>
			 {
				 options.AddPolicy( IfRFBaseController.VIEWERROLE,			policy => policy.Requirements.Add( new CommonAuthorizationRequirement( IfRFBaseController.VIEWERROLE ) ) );
				 options.AddPolicy( IfRFBaseController.EDITORROLE_lvl_1,	policy => policy.Requirements.Add( new CommonAuthorizationRequirement( IfRFBaseController.EDITORROLE_lvl_1 ) ) );
				 options.AddPolicy( IfRFBaseController.EDITORROLE_lvl_2,	policy => policy.Requirements.Add( new CommonAuthorizationRequirement( IfRFBaseController.EDITORROLE_lvl_2 ) ) );
				 options.AddPolicy( IfRFBaseController.ADMINROLE,			policy => policy.Requirements.Add( new CommonAuthorizationRequirement( IfRFBaseController.ADMINROLE ) ) );
			 } );

			services.Configure<ApiBehaviorOptions>(options =>
			{
				options.InvalidModelStateResponseFactory = 
				context => InvalidModelStateResponseGenerator.GenerateInvalidModelStateResponse(context, _logger);
			});

            services.AddAuthentication	( CertificateAuthenticationDefaults.AuthenticationScheme)
                    .AddCertificate		( )
                    // Adding an ICertificateValidationCache results in certificate auth caching the results.
                    // The default implementation uses a memory cache.
                    .AddCertificateCache( );
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			_logger.Debug("Configure");

			//if (env.IsDevelopment())
			//{
				app.UseDeveloperExceptionPage();
			//}

			// global error handler
			app.UseMiddleware<ErrorHandlerMiddleware>();

			app.UseRouting();

			// Custom middleware to log all requests and responses
			//app.UseRequestResponseLogging();

			var option = new RewriteOptions();
			option.AddRedirect("^$", "swagger");
			app.UseRewriter(option);

			app.UseCors(PRXCORS);

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
				endpoints.EnableDependencyInjection();
				endpoints.Select().Filter().OrderBy().Count().Expand().MaxTop(100);
				endpoints.MapODataRoute("api", "api", GetEdmModel());
			}); 
			
			app.UseSwagger();

			app.UseStaticFiles();
			app.UseSwagger(c =>
			{
				c.RouteTemplate = "swagger/{documentName}/swagger.json";
			});

			string swaggerEndpointIfRF = ($"/swagger/{swaggerVersionIfRF}/swagger.json");


			// --- Swagger dropdown (20210604 SDE)

			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint(url: swaggerEndpointIfRF, name: swaggerVersionIfRF);
				c.DefaultModelsExpandDepth(1);
				c.DefaultModelExpandDepth(1);
				c.DefaultModelRendering(ModelRendering.Model);
				c.DisplayOperationId();
				c.InjectStylesheet("/css/SwaggerCompactStyle.css");
				//c.EnableTryItOutByDefault	( ); // Swashbuckle.AspNetCore -version 6.1.3
			});
		}

		private IEdmModel GetEdmModel()
		{
			ODataConventionModelBuilder builder = new ODataConventionModelBuilder();

            builder.EntitySet<MetaRackspaceDefault>		( "RackspaceDefault" );
			builder.EntitySet<MetaSpecPopAssembly>		( "SpecPopAssembly" );
			builder.EntitySet<MetaSpecPopSubrackMap>	( "SpecPopSubrackMap" );
			builder.EntitySet<MetaSpecSrPosGroup>		( "SpecSrPosGroup" );
			builder.EntitySet<MetaSpecSrPosition>		( "SpecSrPosition" );
			builder.EntitySet<MetaSpecSubrack>			( "SpecSubrack" );

			// --- Picklists

			builder.EntitySet<RefPlOwner>				( "Ref_Owner" );
			builder.EntitySet<RefPlPopType>				( "Ref_PopType" );
			builder.EntitySet<RefPlPopModel>			( "Ref_PopModel" );
			builder.EntitySet<RefPlPopStatus>			( "Ref_PopStatus" );
			builder.EntitySet<RefPlRoute>				( "Ref_Route" );
			builder.EntitySet<RefPlSubrackType>			( "Ref_SubrackType" );
			builder.EntitySet<RefPlFrameType>			( "Ref_FrameType" );
			builder.EntitySet<RefPlPositionType>		( "Ref_PositionType" );
			builder.EntitySet<RefPlConnectionType>		( "Ref_ConnectionType" );
			builder.EntitySet<RefPlOrderType>			( "Ref_OrderType" );
			builder.EntitySet<RefPlOrderStatus>			( "Ref_OrderStatus" );
			builder.EntitySet<RefEqPortType>			( "Ref_EqPortType" );

			// --- Resource helper classes

			builder.EntitySet<Resource>					( "Resource" );
			builder.EntitySet<ResourceOrder>			( "ResourceOrder" );
			builder.EntitySet<ResourceCharacteristic>	( "ResourceCharacteristic" );

			// --- Resources

			builder.EntitySet<Connection>				( "Connection" );
			var cu = builder.StructuralTypes.First(t => t.ClrType == typeof(Connection));
			cu.AddProperty(typeof(Connection).GetProperty("CustomerReference"));

			builder.EntitySet<Pop>						( "Pop" );
			builder.EntitySet<RackSpace>				( "RackSpace" );
			builder.EntitySet<RfArea>					( "RfArea" );
			builder.EntitySet<SrPosition>				( "SrPosition" );
			builder.EntitySet<Subrack>					( "Subrack" );

			// --- Other

			builder.EntitySet<RefZone>					( "Ref_Zone" );
			builder.EntitySet<RefPl>					( "RefData" );
			builder.StructuralTypes.Single(t => t.ClrType == typeof(Resource)).AddAllProperties();
			builder.StructuralTypes.Single(t => t.ClrType == typeof(Subrack)).AddAllProperties();

			IEdmModel model = builder.GetEdmModel();
			return (model);
		}
	}
}