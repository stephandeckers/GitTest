/**
 * @Name FreeSplitterPositionSearchService.cs
 * @Purpose 
 * @Date 23 February 2022, 12:34:46
 * @Author S.Deckers
 * @Description 
 */

namespace PXS.IfRF.Services
{
	#region -- Using directives --
	using System;
	using System.Linq;
	using System.Data;
	using System.Data.Common;
	using System.Collections.Generic;
	using Oracle.ManagedDataAccess.Client;
	using PXS.IfRF.Data.Model;
	using PXS.IfRF.Logging;	
	using PXS.IfRF.Data;
	using PXS.IfRF.BusinessLogic;
	using PXS.IfRF.Supporting;
	using d = System.Diagnostics.Debug;
	using Microsoft.EntityFrameworkCore;
	#endregion

	public partial class FreeSplitterPositionSearchService: IFreeSplitterPositionSearchService
	{
		private readonly ModelContext	_ifrfContext	= null;
		private readonly ILoggerManager _logger			= null;
		private readonly ISharedMethods _sharedMethods;

		public FreeSplitterPositionSearchService(ModelContext ifrfContext, ISharedMethods sharedMethods, ILoggerManager logger)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			_ifrfContext	= ifrfContext;
			_sharedMethods	= sharedMethods;
			_logger			= logger;
		}

		public List<FreeSplitterPositionSearchResponse> GetFreeSplitterPositions( FreeSplitterPositionSearchRequest request)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			string	relatedPositionType	= request.RelatedOltPositionType	?? "GPON";
			string	operationalStatus	= request.PositionStatus			?? "O";
			int		posToRetrieve		= request.PosToRetrieve				?? 1;
			string	popStatus			= request.PopStatus					?? "Y";
			string	sparePosition		= request.SparePositions			?? "1";	// --- # of Sparepositions inside splitter to keep

			int theSparePosition = System.Convert.ToInt32( sparePosition);

			List<FreeSplitterPositionSearchResponse> responseCollection = getResponseCollection
			( 
				popName:				request.PopName
			,	owner:					request.Owner
			,	relatedPositionType:	relatedPositionType
			,	operationalStatus:		operationalStatus
			,	odfTrayNr:				request.OdfTrayNr
			,	posToRetrieve:			posToRetrieve
			,	sparePositions:			theSparePosition
			,	popStatus:				popStatus
			,	internalSubrackId:		request.InternalSubrackId
			);

			return( responseCollection);
		}

		/// <summary date="23-02-2022, 12:36:34" author="S.Deckers">
		/// Gets the response collection.
		/// </summary>
		/// <returns></returns>
		private List<FreeSplitterPositionSearchResponse> getResponseCollection
		(
			string	popName
		,	string	owner
		,	string	relatedPositionType
		,	string	operationalStatus
		,	int?	odfTrayNr
		,	int		posToRetrieve
		,	int		sparePositions
		,	string	popStatus
		,	int?	internalSubrackId
		)
        {
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			List<FreeSplitterPositionSearchResponse>	items			= new List<FreeSplitterPositionSearchResponse>();
			Dictionary<long, IncomingInfo>				incomingInfo	= getSplitterOltPositions( popName, popStatus, operationalStatus, owner, relatedPositionType, odfTrayNr);

			foreach(KeyValuePair<long, IncomingInfo> kvp in incomingInfo)
			{							
				if( items.Count >= posToRetrieve)
				{
					//d.WriteLine( string.Format( "{0} items fetched, needed {1}, breaking", items.Count, posToRetrieve) );
					break;
				}

				long theSplitter = kvp.Key;	
				int	 needed		 = posToRetrieve - items.Count;

				//d.WriteLine( string.Format( "needed {0} fetched {1} total={2}", needed, items.Count, posToRetrieve) );

				List<SrPosition> ports = getFreeOutPortCollection( theSplitter, popStatus, operationalStatus, owner, needed, sparePositions, internalSubrackId);

				items.AddRange( constructResponse( kvp.Value, ports));
			}

			d.WriteLine( string.Format( "{0} items", items.Count()) );

			return( items);
		}

		/// <summary date="23-02-2022, 12:36:55" author="S.Deckers">
		/// Constructs the response.
		/// </summary>
		/// <param name="oltPosition">The olt position.</param>
		/// <param name="freePorts">The free ports.</param>
		/// <returns></returns>
		private List<FreeSplitterPositionSearchResponse> constructResponse( IncomingInfo incomingInfo, List<SrPosition> freePorts)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			List<FreeSplitterPositionSearchResponse> items = new List<FreeSplitterPositionSearchResponse>( );

			foreach( var item in freePorts)
			{
				items.Add
				( 
					new FreeSplitterPositionSearchResponse( ) 
					{ 										
						OLTPosition			= incomingInfo.OltPosition
					,	PPPosition			= incomingInfo.PPPosition
					,	SplitterPosition	= item 
					}	
				);
			}

			return( items);
		}

		/// <summary date="23-02-2022, 12:32:04" author="S.Deckers">
		/// Gets the free out port collection.
		/// </summary>
		///
		/// <remarks date="23-02-2022, 12:31:57" author="S.Deckers">
		/// We create items directly from the sql to avoid db-roundtrips (performance issue)
		/// </remarks>
		/// 
		/// <returns></returns>
		private List<SrPosition> getFreeOutPortCollection
		( 
			long	theSplitter
		,	string	popStatus
		,	string	operationalStatus
		,	string	owner
		,	int		asked
		,	int		spare
		,	int?	internalSubrackId
		)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			int getAvailablePorts( DbConnection connection, long theSplitter, string popStatus, string operationalStatus)
			{
				string sql =
				@"select count(*)
					from subrack
						inner join pop 					on pop.id 					= subrack.pop_id
						inner join ref_pl_pop_type 		on ref_pl_pop_type.key 		= pop.type
						left outer join rackspace 		on rackspace.id				= subrack.rackspace_id
						inner join sr_position			on sr_position.subrack_id 	= subrack.id
						inner join rs_resource			on rs_resource.id			= sr_position.id
						inner join connection		c1	on c1.to_id					= sr_position.id
						left outer join connection	c2	on c2.from_id				= c1.to_id	
				where 1=1
					and sr_position.type 	= 'SPLO'
					and c1.from_id = :theSplitter
					and rs_resource.operational_status = :operationalStatus
					and c2.to_id is null";

				if( popStatus == "Y")
				{
					sql += " and pop.status = 'RFP'";
				}

				if( internalSubrackId.HasValue)
				{					
					sql += " and subrack.id = :theSubrack";
				}

				DbCommand cmd = connection.CreateCommand();
				cmd.CommandText = sql;
				cmd.Parameters.Add( new OracleParameter() { ParameterName = "theSplitter",			DbType = DbType.Int64,	Direction = ParameterDirection.Input, Value = theSplitter });
				cmd.Parameters.Add( new OracleParameter() { ParameterName = "operationalStatus",	DbType = DbType.String, Direction = ParameterDirection.Input, Value = operationalStatus });

				if( internalSubrackId.HasValue)
				{
					long theSubrack = System.Convert.ToInt64( internalSubrackId);
					cmd.Parameters.Add( new OracleParameter() { ParameterName = "theSubrack",	DbType = DbType.String, Direction = ParameterDirection.Input, Value = theSubrack });
				}

				object result	= cmd.ExecuteScalar( );

				return( System.Convert.ToInt32( result));
			}

			string sql =
@"select sr_position.id sr_position_id
	,	sr_position.position_id
	,	sr_position.type
	,	sr_position.pos_group
	,	sr_position.utac
	,	sr_position.line_id
	,	sr_position.comments
	,	sr_position.optics
	,	sr_position.pxs_id
	,	sr_position.seq_nr
	,	sr_position.ga_id
	,	sr_position.spec_id
	,	sr_position.subrack_id
	,	subrack.seq_nr
	,	subrack.subrack_id subrack_subrack_id
	,	subrack.type subrack_type
	,	subrack.comments
	,	subrack.pxs_id
	,	subrack.asb_material_code
	,	subrack.name
	,	subrack.pop_id
	,	subrack.spec_id
	,	subrack.rackspace_id
	,	subrack.from_hu
	,	subrack.to_hu
	,	rackspace.frame_id
	,	rackspace.nr_hu
	,	rackspace.frame_hu_t
	,	rackspace.frame_hu_b
	,	rackspace.frame_type
	,	rackspace.power_nominal
	,	rackspace.power_peak
	,	rackspace.loudness_nominal
	,	rackspace.loudness_peak	
	,	rackspace.pop_id
	,	rs_resource.order_id
	,	rs_resource.type rs_resource_type
	,	rs_resource.lifecycle_status
	,	rs_resource.operational_status
	,	rs_resource.operational_status
	,	rs_resource.created_date
	,	rs_resource.modified_date
	,	rs_resource.created_by
	,	rs_resource.modified_by
	,	pop.id			pop_id
	,	pop.ga_id		pop_ga_id
	,	pop.name		pop_name
	,	pop.type		pop_type
	,	pop.model		pop_model
	,	pop.comments	pop_comments
	,	pop.status		pop_status
	,	pop.central_pop_id 
	,	pop.ap_cp_length
	,	pop.nr_of_lines
	,	pop.node
	,	pop.rf_area_id
	,	pop.pop_assembly_id
	,	pop.location_name
	,	pop.street
	,	pop.nr
	,	pop.suffix
	,	pop.subaddress
	,	pop.postcode
	,	pop.municipality
	,	pop.coor_x
	,	pop.coor_y
	,	ref_pl_pop_type.display_name
	,	ref_pl_pop_type.rank
	,	ref_pl_pop_type.description
	,	ref_pl_pop_type.status ref_pl_pop_type_status
	,	ref_pl_pop_type.external_ref
	,	c1.id
	,	c1.from_id
	,	c1.to_id
	,	c2.id
	,	c2.from_id
	,	c2.to_id
	from subrack
		inner join pop 					on pop.id 					= subrack.pop_id
		inner join ref_pl_pop_type 		on ref_pl_pop_type.key 		= pop.type
		left outer join rackspace 		on rackspace.id				= subrack.rackspace_id
		inner join sr_position			on sr_position.subrack_id 	= subrack.id
		inner join rs_resource			on rs_resource.id			= sr_position.id
		inner join connection		c1	on c1.to_id					= sr_position.id
		left outer join connection	c2	on c2.from_id				= c1.to_id	
where 1=1
	and sr_position.type 	= 'SPLO'
	and c1.from_id = :theSplitter
	and rs_resource.operational_status = :operationalStatus
	and c2.to_id is null";

			if( popStatus == "Y")
			{
				sql += " and pop.status = 'RFP'";
			}

			if( internalSubrackId.HasValue)
			{					
				sql += " and subrack.id = :theSubrack";
			}
sql += 
@" order by 
	sr_position.id
,	rackspace.frame_id
,	subrack.subrack_id
,	sr_position.pos_group
,	sr_position.position_id";

			List<SrPosition> items = new List<SrPosition>( );

			DbConnection connection = _ifrfContext.GetOpenConnection();			

			int available = getAvailablePorts( connection, theSplitter, popStatus, operationalStatus);

			if( available == 0)
			{
				return( items);
			}

			int required			= asked + spare;
			int availableMinusSpare	= available - spare;
			//d.WriteLine( string.Format( "asked={0}, spare={1}, required={2}, available={3}, availableMinusSpare={4}", asked, spare, required, available, availableMinusSpare));

			if( availableMinusSpare <= 0)
			{
				return( items);
			}

			try
			{
				DbCommand cmd = connection.CreateCommand();
				cmd.CommandText = sql;
				cmd.Parameters.Add( new OracleParameter() { ParameterName = "theSplitter",			DbType = DbType.Int64,	Direction = ParameterDirection.Input, Value = theSplitter });
				cmd.Parameters.Add( new OracleParameter() { ParameterName = "operationalStatus",	DbType = DbType.String, Direction = ParameterDirection.Input, Value = operationalStatus });

				if( internalSubrackId.HasValue)
				{
					long theSubrack = System.Convert.ToInt64( internalSubrackId);
					cmd.Parameters.Add( new OracleParameter() { ParameterName = "theSubrack",	DbType = DbType.String, Direction = ParameterDirection.Input, Value = theSubrack });
				}

				DbDataReader reader	= cmd.ExecuteAndLogReader( _logger);
				
				while( reader.Read())
				{
					SrPosition srPosition	= srPositionFromReader( reader);
					srPosition.Resource		= resourceFromReader( srPosition.Id, reader);
					srPosition.Subrack		= subrackFromReader	( srPosition.SubrackId.Value, reader);

					// --- Subrack.Pop (20220223 SDE)

					if( srPosition.Subrack.PopId != null)
					{
						srPosition.Subrack.Pop				= popFromReader( srPosition.Subrack.PopId.Value, reader);
						srPosition.Subrack.Pop.RefPopType	= refPopTypeFromReader( srPosition.Subrack.Pop.Type, reader);
					}

                    if( srPosition.Subrack.RackSpaceId != null)
					{
						srPosition.Subrack.RackSpace = rackspaceFromReader( srPosition.Subrack.RackSpaceId.Value, reader);
					}

                    srPosition.Subrack.Name = _sharedMethods.GetSubrackName	( subrack: srPosition.Subrack, owner: owner);

                    srPosition.PositionName = _sharedMethods.GetPositionName( position: srPosition, owner: owner);
					
					srPosition = removeRedundantHeaders( srPosition);

					items.Add( srPosition);

					// --- Check amount (20220223 SDE)

					int itemCount = items.Count();

					//d.WriteLine( string.Format( "itemCount={0}, availableMinusSpare={1}, asked={2}", itemCount, availableMinusSpare, asked));

					if( itemCount >= availableMinusSpare)
					{
						//d.WriteLine( string.Format( "All available items fetched:itemCount={0} >= availableMinusSpare={1}", itemCount, availableMinusSpare));
						return( items);
					}

					if( itemCount >= asked)
					{ 
						//d.WriteLine( string.Format( "items needed fetched:itemCount={0} >= asked={1}", itemCount, asked));
						return( items);
					}
				}				
				return( items);
			}
			finally
			{
				if( connection.State == ConnectionState.Open)
				{
					connection.Close();
				}
			}
		}

		/// <summary date="23-02-2022, 13:01:38" author="S.Deckers">
		/// Removes the redundant headers.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns></returns>
		private SrPosition removeRedundantHeaders( SrPosition item)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			item.Utac		= null;
			item.LineId		= null;
			item.Comments	= null;
			item.PxsId		= null;
			item.SeqNr		= null;
			item.SubrackId	= null;

			item.Subrack.SeqNr				= null;
			item.Subrack.Type				= null;
			item.Subrack.Comments			= null;
			item.Subrack.PxsId				= null;
			item.Subrack.AsbMaterialCode	= null;
			item.Subrack.RackSpaceId		= null;
			item.Subrack.FromHu				= null;
			item.Subrack.ToHu				= null;
			item.Subrack.PopId				= null;

			if( item.Subrack.Pop != null)
			{
				item.Subrack.Pop.Model			= null;
				item.Subrack.Pop.Comments		= null;
				item.Subrack.Pop.Municipality	= null;
				item.Subrack.Pop.ApCpLength		= null;
				item.Subrack.Pop.Node			= null;
				item.Subrack.Pop.RfAreaId		= null;
				item.Subrack.Pop.LocationName	= null;
				item.Subrack.Pop.Street			= null;
				item.Subrack.Pop.Nr				= null;
				item.Subrack.Pop.Suffix			= null;
				item.Subrack.Pop.Subaddress		= null;
				item.Subrack.Pop.Postcode		= null;

				item.Subrack.Pop.RefPopType.DisplayName = null;
				item.Subrack.Pop.RefPopType.Description = null;
				item.Subrack.Pop.RefPopType.Status		= null;
				item.Subrack.Pop.RefPopType.Key			= null;
				item.Subrack.Pop.RefPopType.Owner		= null;
				item.Subrack.Pop.RefPopType.Rank		= null;
			}

			if( item.Subrack.RackSpace != null)
			{
				item.Subrack.RackSpace.NrHu				= null;
				item.Subrack.RackSpace.FrameHuT			= null;
				item.Subrack.RackSpace.FrameHuB			= null;			
				item.Subrack.RackSpace.FrameType		= null;
				item.Subrack.RackSpace.PowerNominal		= null;
				item.Subrack.RackSpace.PowerPeak		= null;
				item.Subrack.RackSpace.LoudnessNominal	= null;
				item.Subrack.RackSpace.LoudnessPeak		= null;
			}

			return( item);
		}

		/// <summary date="19-02-2022, 23:11:55" author="S.Deckers">
		/// Returns list with OltPositions
		/// </summary>
		/// 
		/// <description date="19-02-2022, 23:13:27" author="S.Deckers">
		/// Fetch List with GPON positions connected to the incoming sides of splitters for given POP
		/// </description>
		/// 
		/// <returns></returns>
		private Dictionary<long, IncomingInfo> getSplitterOltPositions
		(
			string	popName
		,	string	popStatus
		,	string	operationalStatus
		,	string	owner
		,	string	relatedPositionType
		,	int?	odfTrayNr
		)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			Dictionary<long, IncomingInfo> items = new Dictionary<long, IncomingInfo>( );

			string sql = 
@"select sr_position.id
	from subrack
		inner join sr_position		on sr_position.subrack_id	= subrack.id
		inner join rs_resource		on rs_resource.id			= sr_position.id
		inner join pop 				on pop.id 					= subrack.pop_id
		left outer join rackspace	on rackspace.id = subrack.rackspace_id
where sr_position.type = 'SPLI'
	and pop.name = :popName
	and rs_resource.operational_status = :operationalStatus";

			if( popStatus == "Y")
			{
				sql += " and pop.status = 'RFP'";
			}

			if( odfTrayNr.HasValue)
			{
				if( owner == "JV1")
				{
					if ((odfTrayNr % 2) == 0)
					{
						sql += " and rackspace.frame_id = '202'";
					}
					else if ((odfTrayNr % 2) != 0)
					{
						sql += " and rackspace.frame_id = '201'";
					}
				}
			}

			//sql += " order by subrack.id";
			sql += " order by sr_position.id";

			DbConnection connection = _ifrfContext.GetOpenConnection();

			try
			{
				DbCommand cmd = connection.CreateCommand();
				cmd.CommandText = sql;

				cmd.Parameters.Add( new OracleParameter() { ParameterName = "popName",				DbType = DbType.String, Direction = ParameterDirection.Input, Value = popName });
				cmd.Parameters.Add( new OracleParameter() { ParameterName = "operationalStatus",	DbType = DbType.String, Direction = ParameterDirection.Input, Value = operationalStatus });

				DbDataReader reader	= cmd.ExecuteAndLogReader( _logger);

				while( reader.Read())
				{
					long theSplitter = System.Convert.ToInt64( reader[ "id"]);

					IncomingInfo incomingInfo = getSplitterOltPosition( connection, theSplitter, owner, relatedPositionType);

					if( incomingInfo == null)
					{
						continue;
					}

					items.Add( key:theSplitter, value:incomingInfo);
				}
			}
			finally
			{
				if( connection.State == ConnectionState.Open)
				{
					connection.Close();
				}
			}

			return( items);
		}

		/// <summary date="22-02-2022, 13:44:48" author="S.Deckers">
		/// Gets GPON-position connected to splitter
		/// </summary>
		/// <param name="connection">The connection.</param>
		/// <param name="theSplitter">The splitter.</param>
		/// <param name="owner">The owner.</param>
		/// <returns></returns>
		private IncomingInfo getSplitterOltPosition( DbConnection connection, long theSplitter, string owner, string positionType)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			string sql = 
@"with t as
(
select 	connection.id
	,	level
	,	SYS_CONNECT_BY_PATH( connection.from_id, '/') path
	,  	connection.from_id
	,  	connection.to_id 
	,	sr_position.type
	from connection 
		inner join sr_position on sr_position.id = connection.from_id
where 1=1
	start with to_id = :theSplitter connect by nocycle prior from_id = to_id
and 2=2
) select	t.from_id
	,		t.type
	from t
	where 1=1
		--and t.type = :thePositionType
		and :thePositionType in (select t.type from t)
	and 2=2";

			DbCommand cmd = connection.CreateCommand();
			cmd.CommandText = sql;
			cmd.Parameters.Add( new OracleParameter() { ParameterName = "theSplitter",		DbType = DbType.Int64,	Direction = ParameterDirection.Input, Value = theSplitter });
			cmd.Parameters.Add( new OracleParameter() { ParameterName = "thePositionType",	DbType = DbType.String, Direction = ParameterDirection.Input, Value = positionType });

			DbDataReader reader	= cmd.ExecuteAndLogReader( _logger);

			IncomingInfo incomingInfo = null;

			// --- Read GPON-port and PP-items (20220222 SDE)

			while( reader.Read())
			{
				if( incomingInfo == null)
				{
					incomingInfo = new IncomingInfo( );
				}

				string theType = System.Convert.ToString( reader[ "TYPE"]);

				if( theType == "PP")
				{
					long id = Convert.ToInt64( reader["FROM_ID"]);
					incomingInfo.PPPosition = constructPPPosition( id, owner);
				}

				if( theType == "GPON")
				{
					long id = Convert.ToInt64( reader["FROM_ID"]);
					incomingInfo.OltPosition = constructGPONPosition( id, owner);
				}
			}
			
			return( incomingInfo);
		}

		/// <summary date="08-03-2022, 08:09:10" author="S.Deckers">
		/// Constructs the pp position.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="owner">The owner.</param>
		/// <returns></returns>
		private SrPosition constructPPPosition( long id, string owner)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			// --- Construct the PPPosition (20220222 SDE)

			SrPosition p = this._ifrfContext.SrPositions.Where	( x => x.Id == id).FirstOrDefault( );

			var connections = this._ifrfContext.Connections.Where( x => x.FromId == id).Include( rs => rs.Resource).ToList();

			p.ConnectionFroms = connections;

			return( p);
		}

		/// <summary date="08-03-2022, 08:03:12" author="S.Deckers">
		/// Constructs the gpon position.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="owner">The owner.</param>
		/// <returns></returns>
		private SrPosition constructGPONPosition( long id, string owner)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			// --- Construct the OltPosition (20220222 SDE)

			SrPosition	oltItem = this._ifrfContext.SrPositions.Where	( x => x.Id == id).FirstOrDefault( );
			Subrack		subrack = this._ifrfContext.Subracks.Where		( x => x.Id == oltItem.SubrackId).FirstOrDefault( );

			oltItem.Subrack					= subrack;
			oltItem.Subrack.Pop				= this._ifrfContext.Pops.Where			( x => x.Id == oltItem.Subrack.PopId).FirstOrDefault( );
			oltItem.Subrack.Pop.RefPopType	= this._ifrfContext.RefPlPopTypes.Where ( x => x.Key == oltItem.Subrack.Pop.Type).FirstOrDefault( );

			oltItem.Subrack.Name = _sharedMethods.GetSubrackName	( subrack: oltItem.Subrack, owner: owner);
			oltItem.PositionName = _sharedMethods.GetPositionName	( position: oltItem, owner: owner);

			if( oltItem.Subrack.RackSpaceId != null)
			{
				oltItem.Subrack.RackSpace = this._ifrfContext.RackSpaces.Where(x => x.Id == oltItem.Subrack.RackSpaceId).FirstOrDefault();
			}

			SrPosition oltPosition = new SrPosition( );
			oltPosition.Id									= oltItem.Id;
			oltPosition.PositionId							= oltItem.PositionId;
			oltPosition.Type								= oltItem.Type;
			oltPosition.PosGroup							= oltItem.PosGroup;
			oltPosition.PositionName						= oltItem.PositionName;
			oltPosition.Subrack								= new Subrack();
			oltPosition.Subrack.Id							= oltItem.Subrack.Id;
			oltPosition.Subrack.SubrackId					= oltItem.Subrack.SubrackId;
			oltPosition.Subrack.Name						= oltItem.Subrack.Name;
			oltPosition.Subrack.Pop							= new Pop();
			oltPosition.Subrack.Pop.Id						= oltItem.Subrack.Pop.Id;
			oltPosition.Subrack.Pop.Name					= oltItem.Subrack.Pop.Name;
			oltPosition.Subrack.Pop.Status					= oltItem.Subrack.Pop.Status;
			oltPosition.Subrack.Pop.Type					= oltItem.Subrack.Pop.Type;
			oltPosition.Subrack.Pop.RefPopType				= new RefPlPopType();
			oltPosition.Subrack.Pop.RefPopType.ExternalRef	= oltItem.Subrack.Pop.RefPopType.ExternalRef;

			if( oltItem.Subrack.RackSpaceId != null)
			{
				oltPosition.Subrack.RackSpace			= new RackSpace( );
				oltPosition.Subrack.RackSpace.Id		= oltItem.Subrack.RackSpace.Id;
				oltPosition.Subrack.RackSpace.FrameId	= oltItem.Subrack.RackSpace.FrameId;
				oltPosition.Subrack.RackSpace.PopId		= oltItem.Subrack.RackSpace.PopId;
			}

			return( oltPosition);
		}

        private SrPosition srPositionFromReader( System.Data.Common.DbDataReader reader)
        {
			var item = new SrPosition( )
			{
				Id				= System.Convert.ToInt64( reader[ "sr_position_id"])
			,	Comments		= reader.ReadStringValue		( "comments")
			,	PositionId		= reader.ReadStringValue		( "position_id")
			,	Type			= reader.ReadStringValue		( "type")
			,	PosGroup		= reader.ReadStringValue		( "pos_group")
			,	Utac			= reader.ReadStringValue		( "utac")
			,	LineId			= reader.ReadStringValue		( "line_id")
			,	ConnectorType	= reader.ReadStringValue		( "optics")
			,	PxsId			= reader.ReadStringValue		( "pxs_id")
			,	SubrackId		= reader.ReadNullableLongValue	( "subrack_id")
			,	SeqNr			= reader.ReadStringValue		( "seq_nr")
			,	SpecId			= reader.ReadNullableLongValue	( "spec_id")
			,	GaId			= reader.ReadNullableLongValue	( "ga_id")
			};

			return( item);
        }

        private Resource resourceFromReader( long pk, System.Data.Common.DbDataReader reader)
        {
			var item = new Resource( )
			{
				Id					= pk
			,	CreatedBy			= reader.ReadStringValue		( "created_by")
			,	CreatedDate			= System.Convert.ToDateTime		( reader["created_date"] )
			,	LifecycleStatus		= reader.ReadStringValue		( "lifecycle_status")
			,	ModifiedBy			= reader.ReadStringValue		( "modified_by")
			,	ModifiedDate		= System.Convert.ToDateTime		( reader["modified_date"] )
			,	OperationalStatus	= reader.ReadStringValue		( "operational_status")
			,	OrderId				= reader.ReadNullableLongValue	( "order_id" )
			,	Type				= reader.ReadStringValue		( "rs_resource_type")
			};

			return( item);
        }

        private Subrack subrackFromReader( long pk, System.Data.Common.DbDataReader reader)
        {
			var item = new Subrack( )
			{
				Id				= pk
			,	AsbMaterialCode	= reader.ReadStringValue		( "asb_material_code")
			,	Comments		= reader.ReadStringValue		( "comments")
			,	FromHu			= reader.ReadNullableLongValue	( "from_hu" )
			,	Name			= reader.ReadStringValue		( "name")
			,	PopId			= reader.ReadNullableLongValue	( "pop_id" )
			,	PxsId			= reader.ReadStringValue		( "pxs_id")
			,	RackSpaceId		= reader.ReadNullableLongValue	( "rackspace_id" )
			,	SeqNr			= reader.ReadStringValue		( "seq_nr")
			,	SpecId			= reader.ReadNullableLongValue	( "spec_id" )
			,	SubrackId		= reader.ReadStringValue		( "subrack_subrack_id")
			,	ToHu			= reader.ReadNullableLongValue	( "to_hu" )
			,	Type			= reader.ReadStringValue		( "subrack_type")
			};

			return( item);
        }

		private Pop popFromReader( long pk, System.Data.Common.DbDataReader reader)
		{
			var item = new Pop( )
			{
				Id				= pk
			,	ApCpLength		= reader.ReadNullableLongValue		( "ap_cp_length")
			,	CentralPopId	= reader.ReadNullableLongValue		( "central_pop_id")
			,	Comments		= reader.ReadStringValue			( "pop_comments")
			,	CoorX			= reader.ReadNullableDecimalValue	( "coor_x")
			,	CoorY			= reader.ReadNullableDecimalValue	( "coor_y")
			,	GaId			= reader.ReadNullableLongValue		( "pop_ga_id")
			,	LocationName	= reader.ReadStringValue			( "location_name")
			,	Model			= reader.ReadStringValue			( "pop_model")
			,	Municipality	= reader.ReadStringValue			( "municipality")
			,	Name			= reader.ReadStringValue			( "pop_name")
			,	Node			= reader.ReadStringValue			( "node")
			,	Nr				= reader.ReadStringValue			( "nr")
			,	NrOfLines		= reader.ReadNullableLongValue		( "nr_of_lines")
			,	PopAssemblyId	= reader.ReadNullableLongValue		( "pop_assembly_id")
			,	Postcode		= reader.ReadStringValue			( "postcode")
			,	RfAreaId		= reader.ReadNullableLongValue		( "rf_area_id")
			,	Status			= reader.ReadStringValue			( "pop_status")
			,	Street			= reader.ReadStringValue			( "street")
			,	Subaddress		= reader.ReadStringValue			( "subaddress")
			,	Suffix			= reader.ReadStringValue			( "suffix")
			,	Type			= reader.ReadStringValue			( "pop_type")
			};
			return( item);
		}

		private RefPlPopType refPopTypeFromReader( string key, DbDataReader reader)
		{
			var item = new RefPlPopType( )
			{
				Key			= key
			,	Rank		= reader.ReadNullableLongValue	( "rank")
			,	DisplayName = reader.ReadStringValue		( "display_name")
			,	Description = reader.ReadStringValue		( "description")
			,	Status		= reader.ReadNullableLongValue	( "ref_pl_pop_type_status")
			,	ExternalRef = reader.ReadStringValue		( "external_ref")
			};
			return( item);
		}

		private RackSpace rackspaceFromReader( long pk, DbDataReader reader)
		{
			var item = new RackSpace( )
			{
				Id				= pk
			,	FrameId			= reader.ReadStringValue		( "frame_id")
			,	NrHu			= reader.ReadNullableLongValue	( "nr_hu" )
			,	FrameHuB		= reader.ReadNullableLongValue	( "frame_hu_b" )
			,	FrameHuT		= reader.ReadNullableLongValue	( "frame_hu_t" )
			,	FrameType		= reader.ReadStringValue		( "frame_type")
			,	PowerNominal	= reader.ReadNullableLongValue	( "power_nominal" )
			,	PowerPeak		= reader.ReadNullableLongValue	( "power_peak" )
			,	LoudnessNominal	= reader.ReadNullableLongValue	( "loudness_nominal" )
			,	LoudnessPeak	= reader.ReadNullableLongValue	( "loudness_peak" )
			,	PopId			= reader.ReadNullableLongValue	( "pop_id" ).Value
			};

			return( item);
		}
	}
}