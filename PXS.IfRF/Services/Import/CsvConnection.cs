/**
 * @Name CsvConnection.cs
 * @Purpose 
 * @Date 11 March 2022, 12:12:21
 * @Author S.Deckers
 * @Description 
 */

namespace PXS.IfRF.Services.Import
{
	#region -- Using directives --
	using System;
	using System.Text;
	using System.Data;
	using System.Data.Common;
	using System.Linq;
	using System.Collections.Generic;
	using Oracle.ManagedDataAccess.Client;
	using Microsoft.AspNet.OData;
	using PXS.IfRF.Data;
	using PXS.IfRF.Data.Model;
	using PXS.IfRF.BusinessLogic;
	using PXS.IfRF.Logging;
	using PXS.IfRF.Supporting;
	using d=System.Diagnostics.Debug;
	#endregion

	internal enum SqlOperation
	{
		Unknown,
		Insert,
		Update
	}

	/// <summary date="31-01-2022, 14:51:05" author="S.Deckers">
	/// CSV file read
	/// </summary>
	public abstract class GeneralCsvConnection
	{
		public	virtual int		NrColumns	=> throw new System.NotImplementedException( );	

		protected ModelContext			_modelContext;
		protected IConnectionService	_connectionService;
		protected ILoggerManager		_logger;
		protected ISharedMethods		_sharedMethods;

		protected static string[] _allowedRoutes	= null;
		
		public GeneralCsvConnection
		(
			int					LineNr
		,	string				csvLine
		,	string				owner
		,	ModelContext		modelContext
		,	IConnectionService	connectionService
		,	ILoggerManager		logger
		,	ISharedMethods		sharedMethods
		)
		{
			// --- properties (20220314 SDE)

			this.LineNr				= LineNr;
			this.Owner				= owner;
			this._connectionService = connectionService;
			this._modelContext		= modelContext;	
			this._logger			= logger;
			this._sharedMethods		= sharedMethods;

			// --- Strip '"'-chars (20220311 SDE)

			csvLine = csvLine.Replace( @"""", string.Empty);

			string[] cols = csvLine.Split( "," );

			if( cols.Length != this.NrColumns)
			{
				this.InvalidReasons.Add( string.Format( "Row {0}, {1} columns read, expected {2}", this.LineNr, cols.Length, this.NrColumns));
				return;
			}			

			ConstructFromCols( cols);

			if( _allowedRoutes == null)
			{
				_allowedRoutes = this._modelContext.RefPlRoutes.ToList().Select( x => x.Key).ToArray();
			}
		}

		internal virtual void ConstructFromCols( string [] cols)
		{
			throw new System.NotImplementedException( );
		}

		public		int				LineNr			{ get; set; }
		public		string			Owner			{ get; set; } = "-";
		internal	SqlOperation	SqlOperation	{ get; set; } = SqlOperation.Unknown;

internal string toSql =
@"select subrack.id subrack_pk
	,	pop.name
	,	rf_area.owner
	,	rackspace.id
	,	rackspace.frame_id
	,	subrack.id
	,	subrack.subrack_id
	,	sr_position.id sr_position_id
	,	sr_position.position_id	
	from subrack
		inner join pop 				on pop.id 					= subrack.pop_id
		inner join rf_area			on rf_area.id				= pop.rf_area_id
		inner join sr_position 		on sr_position.subrack_id 	= subrack.id
		left outer join rackspace 	on rackspace.id 			= subrack.rackspace_id		
		left outer join connection 	on connection.from_id		= sr_position.id
where 1=1
	and rf_area.owner 	= :theOwner
	and pop.name 		= :thePopName";

		public bool		IsValid				
		{ 
			get
			{
				if( this.InvalidReasons.Count() == 0)
				{
					return( true);
				}

				return( false);
			}
		}

		public List<string> InvalidReasons	{ get; set; } = new List<string>( );		

		// --- Mappings overview (20220311 SDE)
		//		PXS.IfRF.Data.Model.Connection mappings         M  DB						fetch	crud	misc			direct						indirect
		//														-- -----------------------	------	-----	---------------	---------------------------	--------------------------	
		internal	long?	Id				{ get; set; }	//	   connection.id					x
		public		string	ConnectionNr	{ get; set; }	//	N  connection.nr					x						public string Nr
		public		string	Type			{ get; set; }	//	Y  connection.type					x						public string Type
		internal	long?	FromId			{ get; set; }	//	   connection.from_id				x
		internal	long?	ToId			{ get; set; }	//	   connection.to_id					x
		public		string	FromPopName		{ get; set; }	//	Y  connection.from_pop_name	x		x						public string FromPopName
		public		string	FromFrameId		{ get; set; }	//	Y  rackspace.frame_id		x															Fetched via the GET/Connections
		public		string	FromSubrackId	{ get; set; }	//	Y  subrack.subrack_id		x															Fetched via the GET/Connections
		internal	long	FromSubrackPk	{ get; set; }	//	   subrack.id								BL Calculation								Fetched via the GET/Connections
		public		string	FromPositionId	{ get; set; }	//	Y  sr_position.id			x															Fetched via the GET/Connections	
		public		string	ToPopName		{ get; set; }	//  Y  connection.to_pop_name	x		x
		public		string	ToFrameId		{ get; set; }	//	N  rackspace.frame_id		x															Fetched via the GET/Connections
		public		string	ToSubrackId		{ get; set; }	//	N  subrack.subrack_id		x															Fetched via the GET/Connections
		internal	long?	ToSubrackPk		{ get; set; }	//	   subrack.id								BL Calculation								Fetched via the GET/Connections
		public		string	ToPositionId	{ get; set; }	//	N  sr_position.id			x															Fetched via the GET/Connections
		public		string	LineId			{ get; set; }	//	N  connection.line_id				x						public string LineId
		public		string	Length			{ get; set; }	//	N  connection.attenuation			x						public long? Attentuation
		public		string	Route			{ get; set; }	//	N  connection.route			x								public string Route
		
		public virtual void Import()
		{
			if( ! this.IsValid)
			{
				return;
			}

			Validate( );

			if( ! this.IsValid)
			{
				return;
			}

			Connection item = createEntity( );

			if( this.SqlOperation == SqlOperation.Insert)
			{
				Create( item);
				return;
			};

			Patch( item);
		}

		internal Connection createEntity( )
		{
			Connection c1 = new Connection( );

			Subrack sr_from		= this._modelContext.Subracks.Where( x => x.Id == this.FromSubrackPk).Single( );
			string fromSrName	= this._sharedMethods.GetSubrackName( sr_from, this.Owner);
			string toSrName		= string.Empty;

			if( this.ToSubrackPk.HasValue)
			{
				Subrack sr_to		= this._modelContext.Subracks.Where( x => x.Id == FromSubrackPk).Single( );
				toSrName	= this._sharedMethods.GetSubrackName( sr_from, this.Owner);
			}

			if( this.Id.HasValue)
			{
				if( this.SqlOperation == SqlOperation.Insert)
				{
					throw new System.NotSupportedException( "ID not needed for inserts");
				}

				c1.Id			= this.Id.Value; // --- Only needed for updates 
			}
			
			c1.FromSrName	= fromSrName;
			c1.FromId		= this.FromId;
			c1.ToId			= this.ToId;
			c1.FromPopName	= this.FromPopName;
			c1.FromSrName	= fromSrName;
			c1.ToPopName	= this.ToPopName;
			c1.ToSrName		= toSrName;
			c1.LineId		= this.LineId;

			// --- Attentuation (20220524 SDE)

			c1.Attentuation = null;
			if( ! string.IsNullOrEmpty( this.Length))
			{
				long theLength;

				if( Int64.TryParse( this.Length, out theLength))
				{
					c1.Attentuation = theLength;
				}
			}

			// --- Route (20220524 SDE)

			c1.Route = null;
			if( ! string.IsNullOrEmpty( this.Route))
			{
				c1.Route = this.Route;
			}

			c1.Type = this.Type;
			c1.Comments = $"{ this.Owner } import {this.SqlOperation} { System.DateTime.Now.ToString( ) }";

			return( c1);
		}

		internal void Create( Connection item)
		{
			BusinessLogicResult res;

			try
			{
				(res,  item) = _connectionService.Create( item );
				if( ! res.Succeeded)
				{
					this.InvalidReasons.AddRange( res.ErrorMessages);
				}
			}
			catch( System.Exception ex)
			{
				this.InvalidReasons.Add( ex.Message);
			}
		}

		internal void Patch( Connection item)
		{
			Delta<Connection> delta = new Delta<Connection>( );
				
			string propName	= string.Empty;
			object propValue= string.Empty;

			if( item.Id == 0)
			{
				throw new System.NotSupportedException( "ID not needed for updates");
			}

			// --- FromId (20220315 SDE)

			propName	= "FromId";
			propValue	= null;
			if( item.FromId.HasValue)				propValue	= (long)item.FromId;

			if (!delta.TrySetPropertyValue( propName, propValue ))
			{
				this.InvalidReasons.Add( $"Error setting { propName } to { (propValue == null ? "null" : propValue )}" );
			}

			// --- ToId (20220315 SDE)

			propName	= "ToId";
			propValue	= null;
			if( item.ToId.HasValue)				propValue	= (long)item.ToId;

			if (!delta.TrySetPropertyValue( propName, propValue ))
			{
				this.InvalidReasons.Add( $"Error setting { propName } to { (propValue == null ? "null" : propValue )}" );
			}

			// --- ToPopName (20220315 SDE)

			propName	= "ToPopName";
			propValue	= null;
			if ( ! string.IsNullOrEmpty( item.ToPopName))	propValue	= item.ToPopName;

			if (!delta.TrySetPropertyValue( propName, propValue ))
			{
				this.InvalidReasons.Add( $"Error setting { propName } to { (propValue == null ? "null" : propValue )}" );
			}

			// --- ToSrName (20220315 SDE)

			propName	= "ToSrName";
			propValue	= null;
			if ( ! string.IsNullOrEmpty( item.ToSrName))	propValue	= item.ToSrName;

			if (!delta.TrySetPropertyValue( propName, propValue ))
			{
				this.InvalidReasons.Add( $"Error setting { propName } to { (propValue == null ? "null" : propValue )}" );
			}

			// --- FromPopName (20220315 SDE)

			propName	= "FromPopName";
			propValue	= null;
			if ( ! string.IsNullOrEmpty( item.FromPopName))	propValue	= item.FromPopName;

			if (!delta.TrySetPropertyValue( propName, propValue ))
			{
				this.InvalidReasons.Add( $"Error setting { propName } to { (propValue == null ? "null" : propValue )}" );
			}

			// --- FromSrName (20220315 SDE)

			propName	= "FromSrName";
			propValue	= null;
			if ( ! string.IsNullOrEmpty( item.FromSrName))	propValue	= item.FromSrName;

			if (!delta.TrySetPropertyValue( propName, propValue ))
			{
				this.InvalidReasons.Add( $"Error setting { propName } to { (propValue == null ? "null" : propValue )}" );
			}

			// --- Type (20220315 SDE)

			propName	= "Type";
			propValue	= null;
			if ( ! string.IsNullOrEmpty( item.Type))	propValue	= item.Type;

			if (!delta.TrySetPropertyValue( propName, propValue ))
			{
				this.InvalidReasons.Add( $"Error setting { propName } to { (propValue == null ? "null" : propValue )}" );
			}

			// --- LineId (20220315 SDE)

			propName	= "LineId";
			propValue	= null;
			if ( ! string.IsNullOrEmpty( item.LineId))	propValue	= item.LineId;

			if (!delta.TrySetPropertyValue( propName, propValue ))
			{
				this.InvalidReasons.Add( $"Error setting { propName } to { (propValue == null ? "null" : propValue )}" );
			}

			// --- Nr (20220315 SDE)

			propName	= "Nr";
			propValue	= null;
			if ( ! string.IsNullOrEmpty( item.Nr))	propValue	= item.Nr;

			if (!delta.TrySetPropertyValue( propName, propValue ))
			{
				this.InvalidReasons.Add( $"Error setting { propName } to { (propValue == null ? "null" : propValue )}" );
			}

			// --- Attentuation (20220315 SDE)

			propName	= "Attentuation";
			propValue	= null;
			if( item.Attentuation.HasValue)				propValue	= (long)item.Attentuation;

			if (!delta.TrySetPropertyValue( propName, propValue ))
			{
				this.InvalidReasons.Add( $"Error setting { propName } to { (propValue == null ? "null" : propValue )}" );
			}

			// --- Route (20220524 SDE)

			propName	= "Route";
			propValue	= null;
			if ( ! string.IsNullOrEmpty( item.Route))	propValue	= item.Route;

			if (!delta.TrySetPropertyValue( propName, propValue ))
			{
				this.InvalidReasons.Add( $"Error setting { propName } to { (propValue == null ? "null" : propValue )}" );
			}

			// --- Comments (20220315 SDE)

			propName	= "Comments";
			propValue	= null;
			if ( ! string.IsNullOrEmpty( item.Comments))	propValue	= item.Comments;

			if (!delta.TrySetPropertyValue( propName, propValue ))
			{
				this.InvalidReasons.Add( $"Error setting { propName } to { (propValue == null ? "null" : propValue )}" );
			}

			// --- Patch it (20220315 SDE)

			(BusinessLogicResult res, Connection patched) = _connectionService.Patch( item.Id, delta);
			if (!res.Succeeded)
			{
				this.InvalidReasons.AddRange( res.ErrorMessages );
				return;
			}
		}

		internal void Validate( )
		{
			ValidateMandatoryFields	( );	if( this.InvalidReasons.Count() > 0) return;			
			ValidateConnectionTypes ( );	if( this.InvalidReasons.Count() > 0) return;
			ValidateRoutes			( );	if( this.InvalidReasons.Count() > 0) return;
			ValidateFrom			( );
			ValidateTo				( );
		}

		internal virtual void ValidateConnectionTypes( )
		{
			string[] allowedTypes = new string[] { "FD-C", "BH-P", "CT", "BHFD"};			

			if( !allowedTypes.Any( this.Type.Contains))
			{
				this.InvalidReasons.Add( $"Invalid connection type [{this.Type}] for importing");
			}
		}

		internal virtual void ValidateRoutes( )
		{
			if( string.IsNullOrEmpty( this.Route))
			{
				return;
			}

			if (!_allowedRoutes.Any( this.Route.Contains ))
			{
				this.InvalidReasons.Add( $"Invalid Route [{this.Route}] for importing" );
			}
		}

		internal void ValidateMandatoryFields( )
		{
			if( string.IsNullOrEmpty( this.Type))			this.InvalidReasons.Add( "Type is mandatory");
			if( string.IsNullOrEmpty( this.FromPopName))	this.InvalidReasons.Add( "FromPopName is mandatory");
			if( string.IsNullOrEmpty( this.FromFrameId))	this.InvalidReasons.Add( "FromFrameId is mandatory");
			if( string.IsNullOrEmpty( this.FromSubrackId))	this.InvalidReasons.Add( "FromSubrackId is mandatory");
			if( string.IsNullOrEmpty( this.FromPositionId)) this.InvalidReasons.Add( "FromPositionId is mandatory");
			if( string.IsNullOrEmpty( this.ToPopName))		this.InvalidReasons.Add( "ToPopName is mandatory");
		}

		internal void ValidateFrom( )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty ) );

string sql =
@"select subrack.id subrack_pk
	,	pop.name
	,	rf_area.owner
	,	sr_position.id sr_position_id
	,	sr_position.position_id
	,	connection.id connection_id
	from subrack
		inner join pop 			on pop.id 					= subrack.pop_id
		inner join rf_area		on rf_area.id				= pop.rf_area_id
		inner join rackspace 	on rackspace.id 			= subrack.rackspace_id
		inner join sr_position 	on sr_position.subrack_id 	= subrack.id
		left outer join connection on connection.from_id = sr_position.id
where 1=1
	and rf_area.owner = :theJointVenture
	and pop.name = :thePopName
	and rackspace.frame_id = :theFrameId
	and subrack.subrack_id = :theSubrackId
	and sr_position.position_id = :thePositionId
and 2=2";

			DbConnection	connection = this._modelContext.GetOpenConnection();
			DbCommand		cmd			= connection.CreateCommand();
			cmd.CommandText = sql;
			cmd.Parameters.Add( new OracleParameter() { ParameterName = "theJointVenture",	DbType = DbType.String, Direction = ParameterDirection.Input, Value = this.Owner });
			cmd.Parameters.Add( new OracleParameter() { ParameterName = "thePopName",		DbType = DbType.String, Direction = ParameterDirection.Input, Value = this.FromPopName });
			cmd.Parameters.Add( new OracleParameter() { ParameterName = "theFrameId",		DbType = DbType.String, Direction = ParameterDirection.Input, Value = this.FromFrameId });
			cmd.Parameters.Add( new OracleParameter() { ParameterName = "theSubrackId",		DbType = DbType.String, Direction = ParameterDirection.Input, Value = this.FromSubrackId });
			cmd.Parameters.Add( new OracleParameter() { ParameterName = "thePositionId",	DbType = DbType.String, Direction = ParameterDirection.Input, Value = this.FromPositionId });
			
			DbDataReader reader	= cmd.ExecuteAndLogReader( _logger);

			if( ! reader.Read())
			{
				StringBuilder sb = new StringBuilder( );
				sb.AppendFormat( "No FromPosition with PopName=[{0}]",	this.FromPopName);
				sb.AppendFormat( ", JV=[{0}]",						this.Owner);
				sb.AppendFormat( ", FrameId=[{0}]",					this.FromFrameId);
				sb.AppendFormat( ", SubrackId=[{0}]",				this.FromSubrackId);
				sb.AppendFormat( ", PositionId=[{0}] found",		this.FromPositionId);
				this.InvalidReasons.Add( sb.ToString());
				return;
			}

			// --- This needs to be found (20220314 SDE)

			if( ! reader.ReadNullableLongValue ( "sr_position_id").HasValue)
			{
				this.InvalidReasons.Add( $"Error reading FromPosition from db");
				return;
			}

			// --- connection.from_id (20220315 SDE)

			long fromPosition = reader.ReadNullableLongValue ( "sr_position_id").Value;
			this.FromId = fromPosition;
			
			this.FromSubrackPk = reader.ReadNullableLongValue ( "subrack_pk").Value;

			// --- If connection_id we need to update an existing connection (20220314 SDE)

			if( reader.ReadNullableLongValue ( "connection_id").HasValue)
			{
				long theConnectionId = System.Convert.ToInt64( reader.ReadNullableLongValue ( "connection_id").Value);
				this.SqlOperation = SqlOperation.Update;
				this.Id = theConnectionId;
				return;
			}
			
			this.SqlOperation = SqlOperation.Insert;
		}

		internal virtual void ValidateTo( )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty ) );

			string sql = this.toSql;

			DbConnection	connection = this._modelContext.GetOpenConnection();
			DbCommand		cmd			= connection.CreateCommand();
			cmd.Parameters.Add( new OracleParameter() { ParameterName = "theOwner",			DbType = DbType.String, Direction = ParameterDirection.Input,	Value = this.Owner });
			cmd.Parameters.Add( new OracleParameter() { ParameterName = "thePopName",		DbType = DbType.String, Direction = ParameterDirection.Input,	Value = this.ToPopName });

			if( ! string.IsNullOrEmpty( this.ToFrameId))
			{
				sql += " and rackspace.frame_id = :theFrameId";
				cmd.Parameters.Add( new OracleParameter() { ParameterName = "theFrameId",	DbType = DbType.String, Direction = ParameterDirection.Input,	Value = this.ToFrameId });
			}

			if (!string.IsNullOrEmpty( this.ToSubrackId ))
			{
				sql += " and subrack.subrack_id = :theSubrackId";
				cmd.Parameters.Add( new OracleParameter( ) { ParameterName = "theSubrackId", DbType = DbType.String, Direction = ParameterDirection.Input,	Value = this.ToSubrackId } );
			}

			if (!string.IsNullOrEmpty( this.ToPositionId ))
			{
				sql += " and sr_position.position_id = :thePositionId";
				cmd.Parameters.Add( new OracleParameter( ) { ParameterName = "thePositionId", DbType = DbType.String, Direction = ParameterDirection.Input, Value = this.ToPositionId } );
			}

			cmd.CommandText = sql;

			DbDataReader reader	= cmd.ExecuteAndLogReader( _logger);

			if( ! reader.Read())
			{
				StringBuilder sb = new StringBuilder( );
				sb.AppendFormat( "No ToPosition with PopName=[{0}]",	this.ToPopName);
				sb.AppendFormat( ", JV=[{0}]",							this.Owner);

				if( ! string.IsNullOrEmpty( this.ToFrameId))
				{
					sb.AppendFormat( ", FrameId=[{0}]",					this.ToFrameId);
				}

				if( ! string.IsNullOrEmpty( this.ToSubrackId))
				{
					sb.AppendFormat( ", SubrackId=[{0}]",				this.ToSubrackId);
				}

				if( ! string.IsNullOrEmpty( this.ToPositionId))
				{
					sb.AppendFormat( ", PositionId=[{0}] found",		this.ToPositionId);
				}
				this.InvalidReasons.Add( sb.ToString());
				return;
			}

			// --- connection.to_id (20220315 SDE)

			long toPosition = reader.ReadNullableLongValue ( "sr_position_id").Value;
			this.ToId = toPosition;

			this.ToSubrackPk = reader.ReadNullableLongValue ( "subrack_pk").Value;
		}
	}	
}