/**
 * @Name Patch.cs
 * @Purpose 
 * @Date 14 October 2021, 06:50:54
 * @Author S.Deckers
 * @url 
 * https://blog.magnusmontin.net/2013/05/30/generic-dal-using-entity-framework/
 * https://codingblast.com/entity-framework-core-generic-repository/
 * @Packages
 * PM> Install-Package Microsoft.AspNetCore.Odata -version 7.5.8
 * PM> Install-Package OData.Swagger -version 1.0.0
 * PM> Install-Package Microsoft.AspNetCore.Mvc.NewtonsoftJson -version 3.1.16
 * PM> Install-Package Oracle.EntityFrameworkCore -version 5.21.1
 * 
 * @OData/curl
 * curl -X GET "https://localhost:5001/api/Pop" -H  "accept: application/json"
 * curl -X GET "https://localhost:5001/api/RfArea" -H  "accept: application/json"
 * curl -X GET "https://localhost:5001/api/Pop?$top=5" -H  "accept: application/json"
 * curl -X GET "https://localhost:5001/api/Pop?$select=Id,Type&$orderby=Name&$filter=contains(Type,%20'CP')" -H  "accept: application/json"
 * curl -X GET "https://localhost:5001/api/Pop?$select=Id,Type&$filter=id%20gt%2050" -H  "accept: application/json"
 * curl -X GET "https://localhost:5001/api/Pop?$select=Id,Type&$filter=Type%20eq%20'CP'" -H  "accept: application/json"
 * curl -X GET "https://localhost:5001/api/Pop?$select=Id,Type&$filter=Type%20eq%20'CP'%20and%20Id%20gt%2050" -H  "accept: application/json"
 * 
 * 2do:
 * null values
 * business logic
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
	using System.Text;	
	using System.Linq;
	using System.Collections.Generic;

	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNet.OData;
	
	using Swashbuckle.AspNetCore.Annotations;

	using Whoua.Core;
	using Whoua.Core.Services;
	using Whoua.Core.Api;
	using Whoua.Core.Data.Model;

	using d = System.Diagnostics.Debug;
	using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

#endregion

	[ ApiController			( )]
	[ ApiExplorerSettings	( IgnoreApi = false)]
	[ Produces				( "application/json", "application/json;odata.metadata=none")]
	[ Route					( "api/[controller]") ]
	public class GenericCrudController<TEntity> : ODataController where TEntity : class, IEntity
	{
		protected IGenericRepository<TEntity>	_genericRepository;

		protected string FriendlyName { get; set; }

		public GenericCrudController
		( 
			IGenericRepository<TEntity> genericRepository
		,	string						friendlyName
		)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			this._genericRepository	= genericRepository;
			this.FriendlyName		= friendlyName;
		}

		public virtual IQueryable<TEntity> GetAll()
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			var items = this._genericRepository.GetAll();
			return( items);
		}

		public virtual ActionResult<TEntity> Get( long id)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			SingleResult<TEntity> item = this._genericRepository.Get( id);
			if( item.Queryable.Count( ) == 0)
			{
				return Ok( new OperationFailureResult( $"{ this.FriendlyName } with id {id} not found" ) );
			}

			return Ok( item );
		}

		public virtual ActionResult<TEntity> Create( [FromBody] TEntity item )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			if (!ModelState.IsValid)
			{
				return BadRequest( ModelState);
			}

			(BusinessLogicResult res, SingleResult<TEntity> singleResult) = _genericRepository.Create( item);
			if( res.Succeeded)
			{
				return Ok( singleResult);
			}

			return Ok( new OperationFailureResult( res.ErrorMessages ) );
		}

		public virtual ActionResult<TEntity> Patch( long id, [FromBody] Delta<TEntity> item )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );
			if (!ModelState.IsValid)
			{
				return BadRequest( ModelState );
			}

			(BusinessLogicResult res, TEntity patched) = _genericRepository.Patch( id, item );
			if (res.Succeeded)
			{
				return Ok( patched );
			}

			return Ok( new OperationFailureResult( res.ErrorMessages ) );
		}

		public virtual IActionResult Delete( long id)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			if( _genericRepository.Delete( id))
			{
				return Ok();
			}

			return Ok($"Error deleting { this.FriendlyName } with id {id}");
		}
	}

	public class PopController : GenericCrudController<Pop>
	{
		private const string swaggerControllerDescription = "Pop";

		public PopController( IPopRepository genericRepository)
			: base( genericRepository, swaggerControllerDescription)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );
		}

		[ HttpGet( )]
		[ EnableQuery( MaxExpansionDepth = 4 )]
		[ SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "Retrieve all Items" )]
		public override IQueryable<Pop> GetAll( )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );
			return( base.GetAll( ));
		}

		[ HttpGet( "{id:long}" )]
		[ EnableQuery( MaxExpansionDepth = 4 )]
		[ SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "Retrieve single Item" )]
		public override ActionResult<Pop> Get( long id )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );
			return( base.Get( id));
		}

		[ HttpPost( )]
		[ EnableQuery( MaxExpansionDepth = 4 )]
		[ SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "Create single item" )]
		public override ActionResult<Pop> Create( [FromBody] Pop item )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );
			return (base.Create( item ));
		}

		[ HttpPatch( )]
		[ SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "Update specific item" )]
		public override ActionResult<Pop> Patch( long id, [FromBody] Delta<Pop> item )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );
			return( base.Patch( id, item));
		}

		[ HttpDelete( "{id:int}" )]
		[ SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "Delete single item" )]
		public override IActionResult Delete( long id )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );
			return (base.Delete( id ));
		}
	}

	public class RfAreaController : GenericCrudController<RfArea>
	{
		private const string swaggerControllerDescription = "RfArea";

		public RfAreaController( IRfAreaRepository genericRepository)
			: base( genericRepository, swaggerControllerDescription)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );
		}

		[ HttpGet( )]
		[ EnableQuery( MaxExpansionDepth = 4 )]
		[ SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "Retrieve all Items" )]
		public override IQueryable<RfArea> GetAll( )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );
			return( base.GetAll( ));
		}

		[ HttpGet( "{id:long}" )]
		[ EnableQuery( MaxExpansionDepth = 4 )]
		[ SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "Retrieve single Item" )]
		public override ActionResult<RfArea> Get( long id )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );
			return( base.Get( id));
		}

		[ HttpPost( )]
		[ EnableQuery( MaxExpansionDepth = 4 )]
		[ SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "Create single item" )]
		public override ActionResult<RfArea> Create( [FromBody] RfArea item )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );
			return (base.Create( item ));
		}

		[ HttpPatch( )]
		[ SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "Update specific item" )]
		public override ActionResult<RfArea> Patch( long id, [FromBody] Delta<RfArea> item )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );
			return( base.Patch( id, item));
		}

		[ HttpDelete( "{id:int}" )]
		[ SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "Delete single item" )]
		public override IActionResult Delete( long id )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );
			return (base.Delete( id ));
		}
	}
}

namespace Whoua.Core.Services
{
	#region -- Using directives --
	using System;
	using System.Data;
	using System.Data.Common;
	using System.Linq;
	using Oracle.ManagedDataAccess.Client;

	using Microsoft.AspNet.OData;

	using Whoua.Core;
	using Whoua.Core.Data.Model;

	using d = System.Diagnostics.Debug;
	using System.ComponentModel.DataAnnotations;
	using Microsoft.EntityFrameworkCore;
	#endregion

	public interface IEntity
	{
		long Id { get; set; }
	}

	public interface IGenericRepository<TEntity> where TEntity : class, IEntity
	{
		IQueryable<TEntity>								GetAll	( ); 
		SingleResult<TEntity>							Get		( long id);
		(BusinessLogicResult, SingleResult<TEntity>)	Create	( TEntity entity);
		bool											Delete	( long id);
		(BusinessLogicResult, TEntity)					Patch	( long id, Delta<TEntity> entity);

		/* --- async zTuff 
		Task<TEntity>			GetById	( int id); 
		Task				Create	( TEntity entity); 
		Task				Update	( int id, TEntity entity); 
		Task				Delete	( int id);
		*/
	}

	public class GenericRepository<TEntity> : IGenericRepository<TEntity>
		where TEntity : class, IEntity
	{
		private readonly ModelContext _dbContext;

		public GenericRepository(ModelContext dbContext)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));
			_dbContext = dbContext;
		}

		public IQueryable<TEntity> GetAll()
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));
			return _dbContext.Set<TEntity>().AsNoTracking();
		}

		public SingleResult<TEntity> Get( long id)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

			var item1 = _dbContext.Set<TEntity>().AsNoTracking( ).Where( x => x.Id == id);
			SingleResult<TEntity> item = SingleResult.Create( item1);
			return( item);
		}

		public (BusinessLogicResult, SingleResult<TEntity>)	Create( TEntity entity)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

			BusinessLogicResult businessLogicResult = new BusinessLogicResult( );

			// --- simulate error
			//businessLogicResult.ErrorMessages.Add( "It failed" );
			//businessLogicResult.ErrorMessages.Add( "It failed bad" );

			//--- Invoke _logic.BeforeCreate( entity);

			string theType = entity.GetType().ToString();
			theType = theType.Substring( theType.LastIndexOf( ".")+1);

			entity.Id = getNextId( theType);

			var theSet = _dbContext.Set< TEntity>( );
			theSet.Add( entity);
			_dbContext.SaveChanges( );

			var item1 = _dbContext.Set<TEntity>().AsNoTracking( ).Where( x => x.Id == entity.Id);
			SingleResult<TEntity> item = SingleResult.Create( item1);
			return( businessLogicResult, item);
		}

		public (BusinessLogicResult, TEntity) Patch( long id, Delta<TEntity> entity )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));
			BusinessLogicResult businessLogicResult = new BusinessLogicResult( );

			var existing = _dbContext.Set<TEntity>().SingleOrDefault( x => x.Id == id);

			if( existing == null)
			{
				businessLogicResult.ErrorMessages.Add( $"item { id } not found");
				return( businessLogicResult, null);
			}

			entity.Patch( existing);
			_dbContext.SaveChanges( );
			return( businessLogicResult, existing);
		}

		public bool	Delete( long id)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

			var item = _dbContext.Set<TEntity>().Where( x => x.Id == id).SingleOrDefault();
			if( item != null)
			{
				_dbContext.Remove		( item);
				_dbContext.SaveChanges	( );
				return( true);
			}
			
			return( false);
		}

		private long getNextId( string theType)
		{
			DbConnection	connection	= this._dbContext.Database.GetDbConnection( );
			DbCommand		cmd			= connection.CreateCommand( );

			string sql = 
@"
declare
	tBuf	varchar( 1000);
	
	procedure getNextSequence
	as
		vId int;
	begin
		insert into rs_resource( type, 		created_date, 	modified_date, 	created_by, modified_by)
						values ( :vType,	sysdate,		sysdate,		'SDE',		'SDE')
						returning id into vId; 
		tBuf := utl_lms.format_message( 'i=[%s]', to_char( vId));
		dbms_output.put_line( tBuf);
		select vId into :vPk from dual;
	end;
begin
	getNextSequence( );
end;";
			cmd.CommandText = sql;
			cmd.Parameters.Add(new OracleParameter() { ParameterName = ":vType",	DbType = DbType.String, Direction = ParameterDirection.Input, Value = theType });
			OracleParameter pOut = new OracleParameter() { ParameterName = "vPk",	DbType = DbType.Int64, Direction = ParameterDirection.Output};
			cmd.Parameters.Add( pOut);

			try
			{
				connection.Open();
				cmd.ExecuteNonQuery();

				long thePk = Convert.ToInt64( pOut.Value);
				return( thePk);
			}
			finally
			{
				if( connection.State == ConnectionState.Open)
				{
					connection.Close();
				}
			}
		}
	}

	public interface IPopRepository		: IGenericRepository<Pop>		{ }
	public interface IRfAreaRepository	: IGenericRepository<RfArea>	{ }

	public class PopRepository : GenericRepository<Pop>, IPopRepository
	{
		public PopRepository	( ModelContext dbContext )	: base( dbContext )		{	}
	}	

	public class RfAreaRepository : GenericRepository<RfArea>, IRfAreaRepository
	{
		public RfAreaRepository( ModelContext dbContext )	: base( dbContext )		{	}
	}
}

namespace Whoua.Core.Data.Model
{
	#region -- Using directives --
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using Microsoft.EntityFrameworkCore;
	using Whoua.Core.Services;
	using d=System.Diagnostics.Debug;
	#endregion

	public class OperationFailureResult
	{
		public OperationFailureResult()
		{
			ErrorMessages = new List<string>();
		}

		public OperationFailureResult(List<string> errorMessages)
		{
			ErrorMessages = errorMessages;
		}

		public OperationFailureResult(string errorMessage)
		{
			ErrorMessages = new List<string> { errorMessage };
		}

		[ NotMapped( )]
		public List<string> ErrorMessages { get; set; }
	}

	public class BusinessLogicResult
	{
		public BusinessLogicResult()
		{
			ErrorMessages = new List<string>();
		}

		public BusinessLogicResult(string errorMessage)
		{
			ErrorMessages = new List<string> { errorMessage };
		}

		public bool Succeeded
		{
			get
			{
				return ErrorMessages.Count == 0;
			}
		}

		public List<string> ErrorMessages { get; set; }
	}


	[ Table( "RF_AREA")]
	public partial class RfArea : IEntity
	{
		public RfArea()
		{
			Pops = new HashSet<Pop>();
		}

		[ Key	( )]
		[ Column( "ID", TypeName = "NUMBER(18)")]
		public long Id { get; set; }

		[ Column		( "CODE")]
		[ StringLength	( 255)]
		public string Code { get; set; }

		[ Column		( "NAME")]
		[ StringLength	( 255)]
		public string Name { get; set; }

		[ Column("OWNER")]
		[ StringLength(4)]
		public string Owner { get; set; }

		[ Column("ZONE_ID", TypeName = "NUMBER(18)")]
		public long? ZoneId { get; set; }

		[ InverseProperty(nameof(Pop.RfArea))]
		public virtual ICollection<Pop> Pops { get; set; }
	}

	[ Table("POP")]
	public partial class Pop : IEntity
	{
		public Pop()
		{
			InverseCentralPop = new HashSet<Pop>();
		}

		[ Key( )]
		[ Column("ID", TypeName = "NUMBER(18)")]
		public long Id { get; set; }

		[ Column("NAME")]
		[ StringLength(255)]
		public string Name { get; set; }

		[ Column("TYPE")]
		[ StringLength(4)]
		public string Type { get; set; }

		[ Column("MODEL")]
		[ StringLength(255)]
		public string Model { get; set; }

		[ Column("COMMENTS")]
		[ StringLength(255)]
		public string Comments { get; set; }

		[ Column("STATUS")]
		[ StringLength(4)]
		public string Status { get; set; }

		[ Column("CENTRAL_POP_ID", TypeName = "NUMBER(18)")]
		public long? CentralPopId { get; set; }

		[ Column("AP_CP_LENGTH", TypeName = "NUMBER(18)")]
		public long? ApCpLength { get; set; }

		[ Column("NR_OF_LINES", TypeName = "NUMBER(18)")]
		public long? NrOfLines { get; set; }

		[ Column("NODE")]
		[ StringLength(255)]
		public string Node { get; set; }

		[ Column("RF_AREA_ID", TypeName = "NUMBER(18)")]
		public long? RfAreaId { get; set; }

		[ Column("POP_ASSEMBLY_ID", TypeName = "NUMBER(18)")]
		public long? PopAssemblyId { get; set; }

		[ Column("LOCATION_NAME")]
		[ StringLength(255)]
		public string LocationName { get; set; }

		[ Column("STREET")]
		[ StringLength(255)]
		public string Street { get; set; }

		[ Column("NR")]
		[ StringLength(255)]
		public string Nr { get; set; }

		[ Column("SUFFIX")]
		[ StringLength(255)]
		public string Suffix { get; set; }

		[ Column("SUBADDRESS")]
		[ StringLength(255)]
		public string Subaddress { get; set; }

		[ Column("POSTCODE")]
		[ StringLength(255)]
		public string Postcode { get; set; }

		[ Column("MUNICIPALITY")]
		[ StringLength(255)]
		public string Municipality { get; set; }

		[ Column("COOR_X", TypeName = "NUMBER(38)")]
		public decimal? CoorX { get; set; }

		[ Column("COOR_Y", TypeName = "NUMBER(38)")]
		public decimal? CoorY { get; set; }

		[ Column("GA_ID", TypeName = "NUMBER(18)")]
		public long? GaId { get; set; }
		
		[ ForeignKey(nameof(CentralPopId))]
		[ InverseProperty(nameof(Pop.InverseCentralPop))]
		public virtual Pop CentralPop { get; set; }

		[ InverseProperty(nameof(Pop.CentralPop))]
		public virtual ICollection<Pop> InverseCentralPop { get; set; }

		[ ForeignKey(nameof(RfAreaId))]
		[ InverseProperty("Pops")]
		public virtual RfArea RfArea { get; set; }
	}

	public partial class ModelContext : DbContext
	{
		public ModelContext( DbContextOptions<ModelContext> options )
			: base( options )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));
		}
		
		public virtual DbSet<Pop>		PopCollection		{ get; set; }
		public virtual DbSet<RfArea>	RfAreaCollection	{ get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));
			modelBuilder.HasAnnotation("Relational:Collation", "USING_NLS_COMP");
		}
	}
}

namespace Whoua.Core.Api
{
	#region -- Using directives --
	using System;
	using System.IO;
	using System.Reflection;
	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.AspNetCore.StaticFiles;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Hosting;
	using Microsoft.OpenApi.Models;

	using Microsoft.AspNet.OData.Extensions;
	using OData.Swagger.Services;
	using Microsoft.OData.Edm;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.AspNetCore.Rewrite;
	using Microsoft.AspNet.OData.Builder;

	using Swashbuckle.AspNetCore.SwaggerUI;

	using System.Collections.Generic;
	using Microsoft.AspNet.OData.Formatter;
	using System.Linq;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Serialization;
	using Swashbuckle.AspNetCore.SwaggerGen;
	using Microsoft.Net.Http.Headers;
	using Microsoft.AspNet.OData;

	using Whoua.Core;
	using Whoua.Core.Services;
	using Whoua.Core.Data.Model;

	using d=System.Diagnostics.Debug;
	#endregion

	public class Startup
	{
		private readonly string swaggerVersionIfRF;

		public Startup( IConfiguration configuration)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));
			Configuration = configuration;

			Version assemblyVersion = GetType().Assembly.GetName().Version;
			swaggerVersionIfRF = "v" + assemblyVersion.Major + "." + assemblyVersion.Minor;
		}

		public IConfiguration Configuration { get; }

		//public const string PRXCORS = "PrxCors";

		/// <summary date="04-06-2021, 12:11:46" author="S.Deckers">
		/// Configures the services.
		/// </summary>
		/// <param name="services">The services.</param>
		public void ConfigureServices(IServiceCollection services)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

			services.AddControllers()
				.AddNewtonsoftJson(
				options =>
				{
					options.SerializerSettings.ReferenceLoopHandling	= ReferenceLoopHandling.Ignore;
					options.SerializerSettings.NullValueHandling		= NullValueHandling.Ignore;
					options.SerializerSettings.ContractResolver			= ShouldSerializeContractResolver.Instance;
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
				c.OperationFilter	<SwaggerQueryParameter>	( );
				c.OperationFilter	<SwaggerOperationFilter>( );
				c.SchemaFilter		<DeltaSchemaFilter>		( );
				c.SwaggerDoc		( name: swaggerVersionIfRF, info: new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Foo rulez", Version = swaggerVersionIfRF });
				c.OrderActionsBy	( (apiDesc) => $"{apiDesc.ActionDescriptor.RouteValues["controller"]}_{apiDesc.HttpMethod}");
				c.EnableAnnotations	( );
			});

			services.AddOdataSwaggerSupport();

			//string connString =  "User Id=IFRF_SCHEMA;Password=Sch4IFRF_DeV;Data Source=IFRF70D:1540/IFRF70D.BC;";
			string connString =  "User Id=c##ifh_owner;Password=ifh_owner;Data Source=LP07:1521/ORCLCDB.localdomain;";
			services.AddDbContext<ModelContext>(options => options.UseOracle(connString));

			services.AddScoped<IPopRepository,			PopRepository>();
			services.AddScoped<IRfAreaRepository,		RfAreaRepository>();
		}

		/// <summary date="04-06-2021, 12:11:59" author="S.Deckers">
		/// Configures the specified application.
		/// </summary>
		/// <param name="app">The application.</param>
		/// <param name="env">The env.</param>
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			// global error handler
			//app.UseMiddleware<ErrorHandlerMiddleware>();

			app.UseRouting();

			// Custom middleware to log all requests and responses
			//app.UseRequestResponseLogging();

			var option = new RewriteOptions();
			option.AddRedirect("^$", "swagger");
			app.UseRewriter(option);

			//app.UseCors(PRXCORS);

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers			( );
				endpoints.EnableDependencyInjection	( );
				endpoints.Select().Filter().OrderBy().Count().Expand().MaxTop(100);
				//endpoints.MapODataRoute("api", "api", GetEdmModel()); // --- Removed ( https://devblogs.microsoft.com/odata/enabling-endpoint-routing-in-odata/)
			}); 
			
			app.UseSwagger		( );
			app.UseStaticFiles	( );

			app.UseSwagger(c =>
			{
				c.RouteTemplate = "swagger/{documentName}/swagger.json";
			});

			string swaggerEndpointIfRF = ($"/swagger/{swaggerVersionIfRF}/swagger.json");

			// --- Swagger dropdown (20210604 SDE)

			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint(url: swaggerEndpointIfRF, name: swaggerVersionIfRF);
				c.DefaultModelsExpandDepth	( 1);
				c.DefaultModelExpandDepth	( 1);
				c.DefaultModelRendering		( ModelRendering.Model);
				c.DisplayOperationId		( );
				//c.InjectStylesheet("/css/SwaggerCompactStyle.css");
			});
		}

		private IEdmModel GetEdmModel()
		{
			ODataConventionModelBuilder builder = new ODataConventionModelBuilder();

			IEdmModel model = builder.GetEdmModel();
			return (model);
		}

		private static void SetOutputFormatters(IServiceCollection services)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			services.AddMvcCore(options =>
			{
				IEnumerable<ODataOutputFormatter> outputFormatters =
					options.OutputFormatters.OfType<ODataOutputFormatter>()
						.Where(foramtter => foramtter.SupportedMediaTypes.Count == 0);

				foreach (var outputFormatter in outputFormatters)
				{
					outputFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/odata"));
				}
			});
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

	public class SwaggerOperationFilter:IOperationFilter
	{
		private const string	_deltaParam = "Delta";
		private const string	APPJSON		= "application/json";

		private readonly IList<string> dataNamespaces;
		public SwaggerOperationFilter()
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			const string theNamespace = "Whoua.Core.Data.Model";

			dataNamespaces = AppDomain.CurrentDomain
				.GetAssemblies()
				.SelectMany(t => t.GetTypes())
				.Where(t => (t.IsClass || t.IsAbstract) && ( t.Namespace == theNamespace ))
				.Select(n => n.Name).ToList();

			foreach( var item in dataNamespaces)
			{
				d.WriteLine( item.ToString( ) );
			}
		}

		public void Apply( OpenApiOperation operation, OperationFilterContext context)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			if (operation.RequestBody == null) return;

			RemoveRedundantResponseHeaders	(operation);
			RemoveRedundantRequestHeaders	(operation);

			var deltaTypes =
				operation.RequestBody
					.Content
					.Where(x => x.Value.Schema.Reference != null && x.Value.Schema.Reference.Id.EndsWith(_deltaParam));

			foreach (var (_, value) in deltaTypes)
			{
				var schema = value.Schema;
				string model = schema.Reference.Id.Substring(0, schema.Reference.Id.Length - _deltaParam.Length);
				schema.Reference.Id = model;
			}

			var schemas = context.SchemaRepository.Schemas;

			var redundantSchemas = schemas.Where(s => !dataNamespaces.Contains(s.Key) && !dataNamespaces.Contains(s.Key.Replace("Delta","")));

			foreach (var (key, value) in redundantSchemas)
			{
				schemas.Remove(key);
			}
		}

		private void RemoveRedundantRequestHeaders(OpenApiOperation operation)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			var backupJson = operation.RequestBody.Content.SingleOrDefault(x => x.Key.Equals(APPJSON));

			operation.RequestBody.Content.Clear();
			operation.RequestBody.Content.Add(backupJson);
		}

		private void RemoveRedundantResponseHeaders(OpenApiOperation operation)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			OpenApiResponses responses = operation.Responses;
			foreach (var response in responses.Values)
			{
				OpenApiMediaType backupJsonType = response.Content.FirstOrDefault(c => c.Key.Equals(APPJSON)).Value;

				response.Content.Clear();

				if (backupJsonType != null)
				{
					response.Content.Add(APPJSON, backupJsonType);
				}
			}
		}
	}

	public class ShouldSerializeContractResolver : DefaultContractResolver
	{
		public static readonly ShouldSerializeContractResolver Instance = new ShouldSerializeContractResolver();

		protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
		{
			//d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			JsonProperty property = base.CreateProperty(member, memberSerialization);

			if (property.PropertyType != typeof(string))
			{
				if (property.PropertyType.GetInterface(nameof(System.Collections.IEnumerable)) != null)
				{
					property.ShouldSerialize =
						instance =>
						{
							PropertyInfo propInfo = instance?.GetType().GetProperty(property.PropertyName);
							if (propInfo is null)
							{ 
								return false; 
							}
							else
							{
								return (propInfo.GetValue(instance) as IEnumerable<object>)?.Count() > 0;
							}
						};
				}
			}

			if (property.PropertyType.IsValueType)
			{
			}
			return property;
		}
	}

	public class SwaggerQueryParameter : IOperationFilter
	{
		public void Apply( OpenApiOperation operation, OperationFilterContext context)
		{
			if (operation.Parameters == null) operation.Parameters = new List<OpenApiParameter>();

			var attr = context.MethodInfo.CustomAttributes.ToList();

			if (attr.Any(a => a.AttributeType == typeof(EnableQueryAttribute)))
			{
				OpenApiSchema stringSchema = new OpenApiSchema { Type = "string" };

				operation.Parameters.Add(new OpenApiParameter()
				{
					Name = "$select",
					In = ParameterLocation.Query,
					Description = "Select specific fields, comma separated, e.g. $select=name,type",
					Required = false,
					Schema = stringSchema,
					AllowReserved = true
				});

				operation.Parameters.Add(new OpenApiParameter()
				{
					Name = "$orderby",
					In = ParameterLocation.Query,
					Description = "Order records by a field asc or descending e.g. $orderby=name,type desc",
					Required = false,
					Schema = stringSchema,
					AllowReserved = true
				});

				operation.Parameters.Add(new OpenApiParameter()
				{
					Name = "$count",
					In = ParameterLocation.Query,
					Description = "Count the number of records. e.g. $count=true. To have only the count number, include $top=0",
					Required = false,
					Schema = stringSchema,
					AllowReserved = true
				});

				operation.Parameters.Add(new OpenApiParameter()
				{
					Name = "$top",
					In = ParameterLocation.Query,
					Description = "only return the n top records. e.g. $top=5",
					Required = false,
					Schema = stringSchema,
					AllowReserved = true
				});

				operation.Parameters.Add(new OpenApiParameter()
				{
					Name = "$skip",
					In = ParameterLocation.Query,
					Description = "Skip the first n records. e.g $skip=10",
					Required = false,
					Schema = stringSchema,
					AllowReserved = true
				});

				operation.Parameters.Add(new OpenApiParameter()
				{
					Name = "$filter",
					In = ParameterLocation.Query,
					Description = "Advanced filtering options. e.g. $filter=indexof(toupper(RfAreaCode),'AR') eq 0",
					Required = false,
					Schema = stringSchema,
					AllowReserved = true
				});

				operation.Parameters.Add(new OpenApiParameter()
				{
					Name = "$search",
					In = ParameterLocation.Query,
					Description = "Only return records matching a specific search expression. e.g. $search=sometext",
					Required = false,
					Schema = stringSchema,
					AllowReserved = true
				});

				operation.Parameters.Add(new OpenApiParameter()
				{
					Name = "$expand",
					In = ParameterLocation.Query,
					Description = "Expands related entities. Generally those names with end with 'Ref'. e.g. $expand=OwnerRef,ZoneRef",
					Required = false,
					Schema = stringSchema,
					AllowReserved = true
				});
			}
		}
	}

	public class DeltaSchemaFilter : ISchemaFilter
	{
		public void Apply(OpenApiSchema schema, SchemaFilterContext context)
		{
			if (context.Type.Name.Contains("Delta"))
			{
				schema.Properties.Clear();
				//context.SchemaRepository.Schemas.Clear();
			}
		}
	}
}