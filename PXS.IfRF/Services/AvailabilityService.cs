/**
 * @Name AvailabilityService.cs
 * @Purpose 
 * @Date 15 July 2021, 21:58:53
 * @Author S.Deckers
 * @Description 
 */

namespace PXS.IfRF.Services
{
	#region -- Using directives --
	using System;
	using System.Data;
	using System.Data.Common;
	using System.Text;
	using System.Collections.Generic;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.AspNet.OData;
	using Oracle.ManagedDataAccess.Client;
	using PXS.IfRF.Data.Model;
	using System.Linq;
	using PXS.IfRF.Logging;
	using d=System.Diagnostics.Debug;
    using PXS.IfRF.Data;
    using PXS.IfRF.Supporting;
    #endregion

    public class AvailabilityService : IAvailabilityService
    {
        private readonly ModelContext   _ifrfContext    = null;
        private readonly ILoggerManager _logger         = null;

        public AvailabilityService( ModelContext ifrfContext, ILoggerManager logger)
        {
            _ifrfContext	= ifrfContext;
			_logger         = logger;
        }

		public List<AvailabilityResponse> Check( AvailabilityRequest request)
        {
			return( handle_Check( request));
        }

		/// <summary>
		/// Handles the check.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <returns></returns>
		private List<AvailabilityResponse> handle_Check( AvailabilityRequest r)
		{
			string operationalStatus = r.OperationalStatus	?? "O";
			string positionType		 = r.OltPositionType	?? "GPON";

			// --- Get splitters and OLT-equipment (20210731 SDE)

			List<SrPosition>		   splitters			= getSplitterPositions( r.PopName, r.OwnerKey, operationalStatus, positionType, r.OdfTrayNr);			
			List<AvailabilityResponse> responseCollection	= getOltPositions( splitters, positionType, r.OwnerKey);

			return( responseCollection);
		}

		/// <summary>
		/// Gets the splitter positions.
		/// </summary>
		/// <param name="r">The r.</param>
		/// <returns></returns>
		private List<SrPosition> getSplitterPositions
		( 
			string	popName
		,	string	ownerKey
		,	string	operationalStatus
		,	string	positionType
		,	int?	odfTrayNr
		)
		{
string sql =
@"select  rownum n
	,	sr_position.id
	,	position_rs.operational_status
	,	position_rs.lifecycle_status
	from subrack				
		inner join pop 			on pop.id 			= subrack.pop_id
		inner join rf_area 		on rf_area.id 		= pop.rf_area_id
		inner join sr_position 	on sr_position.subrack_id 	= subrack.id
			and sr_position.type = 'SPLO'
		left outer join rackspace on rackspace.id = subrack.rackspace_id
		left outer join connection c_in on c_in.to_id = sr_position.id
		left outer join connection c_out on c_out.from_id = sr_position.id
		
		inner join rs_resource 	sr_rs 	on sr_rs.id = subrack.id
		inner join rs_resource 	pop_rs 	on pop_rs.id = pop.id
		inner join rs_resource	area_rs on area_rs.id = rf_area.id
		inner join rs_resource	position_rs on position_rs.id = sr_position.id
		left join rs_resource	rackspace_rs on rackspace_rs.id = rackspace.id
		left join rs_resource	c_in_rs on c_in_rs.id = rackspace.id
		left join rs_resource	c_out_rs on c_out_rs.id = rackspace.id
where 1=1
	and pop.name 					= :popName
	and rf_area.owner 				= :ownerKey
	and c_out.from_id is null
	
	and sr_rs.operational_status = :operationalStatus
	and pop_rs.operational_status = :operationalStatus
	and area_rs.operational_status = :operationalStatus
	and position_rs.operational_status = :operationalStatus
	and rackspace_rs.operational_status = :operationalStatus
	and c_in_rs.operational_status = :operationalStatus
	and c_out_rs.operational_status = :operationalStatus
and 2=2";
			DbConnection connection = _ifrfContext.GetOpenConnection();

			List<SrPosition> items = new List<SrPosition>( );

			try
			{
				DbCommand		cmd		= connection.CreateCommand();
				cmd.CommandText = sql;
				cmd.Parameters.Add( new OracleParameter( ) { ParameterName = "popName",				DbType = DbType.String, Direction = ParameterDirection.Input, Value = popName});
				cmd.Parameters.Add( new OracleParameter( ) { ParameterName = "ownerKey",			DbType = DbType.String, Direction = ParameterDirection.Input, Value = ownerKey});
				cmd.Parameters.Add( new OracleParameter( ) { ParameterName = "operationalStatus",	DbType = DbType.String, Direction = ParameterDirection.Input, Value = operationalStatus});

				DbDataReader	reader	= cmd.ExecuteAndLogReader( _logger );

				while( reader.Read())
				{
					long id = System.Convert.ToInt64	( reader[ "id"]);

					// --- If traynr has a value we only may accept the given nr (20210805 SDE)

					if( odfTrayNr.HasValue)
					{
						if( id != odfTrayNr)
						{
							continue;
						}
					}

					SrPosition srPosition = new SrPosition( );

					srPosition = this._ifrfContext.SrPositions.Where( x => x.Id == id).FirstOrDefault( );

					//srPosition.OperationalStatus	= System.Convert.ToString	( reader[ "operational_status"]);
					//srPosition.LifecycleStatus		= System.Convert.ToString	( reader[ "lifecycle_status"]);
					srPosition.PositionName			= getPositionName( position:srPosition, positionType:positionType, owner:ownerKey);

					srPosition.Subrack				= this._ifrfContext.Subracks.Where( x => x.Id == srPosition.SubrackId).FirstOrDefault( );
					srPosition.Subrack.Name			= getSubrackName( position:srPosition, positionType:positionType, owner:ownerKey);
					srPosition.Subrack.Pop			= this._ifrfContext.Pops.Where( x => x.Id == srPosition.Subrack.PopId).FirstOrDefault( );
										
					//srPosition.Subrack.Dump( );

					if( srPosition.Subrack.RackSpaceId != null)
					{
						srPosition.Subrack.RackSpace = this._ifrfContext.RackSpaces.Where( x => x.Id ==  srPosition.Subrack.RackSpaceId).FirstOrDefault( );
						//srPosition.Subrack.RackSpace.Dump( );
					}
									
					items.Add( srPosition);
				}

				return( items);
			}
			catch( System.Exception ex)
			{
				d.WriteLine( string.Format( "Exception:{0}", ex.ToString( )));
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

		/// <summary date="31-07-2021, 17:52:19" author="S.Deckers">
		/// Construct list of AvailabilityResponse items
		/// </summary>
		/// 
		/// <notes>
		/// The resulting list contains all of the free splitter ports and their connected OLT-ports
		/// </notes>
		/// 
		/// <param name="splitters">The splitters.</param>
		/// <returns></returns>
		private List<AvailabilityResponse> getOltPositions
		( 
			List<SrPosition>	splitters
		,	string				positionType
		,	string				ownerKey
		)
		{
			List<AvailabilityResponse> responseCollection = new List<AvailabilityResponse>( );
string sql = 
@"
select sr_position.id 
	from TABLE( NetworkPathOrder.getConnectedResource( res_id => :resource_id)) cp
			inner join sr_position on sr_position.id = cp.from_id
	where 1=1
		and sr_position.type = :position_type
	and 2=2
";
			DbConnection connection = _ifrfContext.GetOpenConnection( );
			
/* ----  Requirements mapping (20210805 SDE)
 * 
	Node	Field				Property (*=custom)
	----	-----------------	----------------------------
	R2		ID					srPosition.Id
			POSITION_ID			srPosition.PositionId
			POSITION_TYPE		srPosition.Type
			RESOURCE_ID			srPosition.Id
			LIFECYCLE_STATUS	srPosition.LifecycleStatus
			OPERATIONAL_STATUS	srPosition.OperationalStatus
			POSITION_NAME		srPosition.PositionName*
			POP_NAME			srPosition.Subrack.Pop.Name
			FRAME_ID			srPosition.Subrack.RackSpace.FrameId
			SUBRACK_ID			srPosition.Subrack.Id
			POSITION_GROUP		srPosition.PosGroup
			POSITION_STATUS		srPosition.OperationalStatus
			SUBRACK_NAME		srPosition.Subrack.Name
			CONNECTOR_TYPE		srPosition.Optics
	R3		POSITION_ID			oltItem.Id
			POSITION_NAME		oltItem.PositionName*
			POSITION_TYPE		oltItem.Type
			POP_NAME			oltItem.Subrack.Pop.Name
			FRAME_ID			oltItem.Subrack.RackSpace.FrameId
			SUBRACK_ID			oltItem.Subrack.Id
			SUBRACK_NAME		oltItem.Subrack.Name
			POSITION_GROUP		oltItem.PosGroup
*/

			try
			{
				foreach( SrPosition splitter in splitters)
				{
					DbCommand		cmd		= connection.CreateCommand();					
					cmd.CommandText = sql;
					cmd.Parameters.Add( new OracleParameter( ) { ParameterName = "resource_id",		DbType = DbType.Int64,	Direction = ParameterDirection.Input, Value = splitter.Id});
					cmd.Parameters.Add( new OracleParameter( ) { ParameterName = "position_type",	DbType = DbType.String, Direction = ParameterDirection.Input, Value = positionType});

					DbDataReader reader = cmd.ExecuteReader( );

					while (reader.Read( ))
					{
						long id = Convert.ToInt64	( reader[ "id"]);

						SrPosition oltItem = new SrPosition( );
						oltItem = this._ifrfContext.SrPositions.Where( x => x.Id == id).FirstOrDefault( );

						Subrack subrack			= this._ifrfContext.Subracks.Where( x => x.Id == oltItem.SubrackId).FirstOrDefault( );
						oltItem.Subrack			= subrack;					
						oltItem.PositionName	= getPositionName( position:oltItem, positionType:positionType, owner:ownerKey);
						oltItem.Subrack.Pop		= this._ifrfContext.Pops.Where( x => x.Id == oltItem.Subrack.PopId).FirstOrDefault( );
						oltItem.Subrack.Name	= getSubrackName( position:oltItem, positionType:positionType, owner:ownerKey);

						if( oltItem.Subrack.RackSpaceId != null)
						{
							oltItem.Subrack.RackSpace = this._ifrfContext.RackSpaces.Where( x => x.Id == oltItem.Subrack.RackSpaceId).FirstOrDefault( );
						}

						// --- Construct response (20210731 SDE)

						AvailabilityResponse responseItem = new AvailabilityResponse( )
						{
							SplitterPosition	= splitter
						,	OLTPosition			= oltItem
						,	Connection			= new Connection( ) { Type = "CL-C" }
						};

						responseCollection.Add( responseItem);
					}
				}

				return ( responseCollection);
			}
			catch( System.Exception ex)
			{
				d.WriteLine( string.Format( "Exception:{0}", ex.ToString( )));
				return( responseCollection);
			}
			finally
			{
				if( connection.State == ConnectionState.Open)
				{
					connection.Close();
				}
			}
		}

		/// <summary date="05-08-2021, 20:44:42" author="S.Deckers">
		/// Gets the name of the subrack.
		/// </summary>
		/// <param name="position">The position.</param>
		/// <param name="positionType">Type of the position.</param>
		/// <param name="owner">The owner.</param>
		/// <returns></returns>
		private string getSubrackName
		(
			SrPosition	position
		,	string		positionType
		,	string		owner
		)
		{
/*
If positionType is SPL and owner AMB, then

Display the name of the POP retrieved via subrack-POP relation
Return Subrack> Connected GPON, where we should take the positionType: SPL_IN from the subrack (if more than one IN position, take the first one) fetch for that position the connected positions and apply to the returned position a label
- else if no connection exists, take the SUBRACK_ID
*/
			if( positionType == "SPL")
			{
				// --- JV2 = Ambiorix (20210805 SDE)

				if( owner == "JV2")
				{
					if( position.Subrack != null)
					{
						if( !string.IsNullOrEmpty( position.Subrack?.Pop?.Name))
						{
							return( position.Subrack?.Pop?.Name);
						}

						if( ! string.IsNullOrEmpty( position?.ConnectionTos?.FirstOrDefault()?.FromPopName))
						{
							return( position?.ConnectionTos?.FirstOrDefault()?.FromPopName);
						}

						if( position.SubrackId.HasValue)
						{
							return( position.SubrackId.ToString());
						}
					}

					return( string.Empty);
				}
			}

			// --- OLT (20210805 SDE)

/*
If positionType is OLT regardless of the owner, then

Retrieve Subrack> PXS_ID
*/
			if( positionType == "OLT")
			{
				if( !string.IsNullOrEmpty( position.Subrack?.PxsId))
				{
					return( position.Subrack?.PxsId);
				}
				
				return( string.Empty);
			}

/*
If positionType is any other value, SPL with owner FK or any other type not specifed above

then fetch Name of the POP retrieved via subrack-POP relation
Fetch FRAME_ID via SR_Frame of the Rack_space
and last  if subrack_Id is not null, return subrack_Id and if subrack_Id is null return Subrack_seq_nr 
*/
			if( ! string.IsNullOrEmpty( position?.Subrack?.Pop?.Name))
			{
				return( position?.Subrack?.Pop?.Name);
			}

			if( ! string.IsNullOrEmpty( position?.Subrack?.RackSpace?.FrameId))
			{
				return( position?.Subrack?.RackSpace?.FrameId);
			}

			if( ! string.IsNullOrEmpty( position?.Subrack?.SubrackId))
			{
				return( position?.Subrack?.SubrackId);
			}

			//if( position.Subrack.SeqNr.HasValue)
			//{
			//	return( position.Subrack.SeqNr.ToString());
			//}

			if( string.IsNullOrEmpty( position.Subrack.SeqNr) == false)
			{
				return( position.Subrack.SeqNr.ToString());
			}
			return ( string.Empty);
		}

		/// <summary date="05-08-2021, 20:36:29" author="S.Deckers">
		/// Gets the name of the position.
		/// </summary>
		/// <param name="position">The position.</param>
		/// <param name="positionType">Type of the position.</param>
		/// <param name="owner">The owner.</param>
		/// <returns></returns>
		private string getPositionName
		( 
			SrPosition	position
		,	string		positionType
		,	string		owner		
		)
		{
			// --- SPL/Ambiorix

/*
If positionType is SPL and Owner is AMB (AMBIORIX)
Take the subrack of the position and fetch the subrackName, if no subrack name exists take POP_NAME, else GA_ID else empty
*/
			if( positionType == "SPL")
			{
				// --- JV2 = Ambiorix (20210805 SDE)

				if( owner == "JV2")
				{
					if( position.Subrack != null)
					{
						if( !string.IsNullOrEmpty( position.Subrack.Name))
						{
							return( position.Subrack.Name);
						}

						if( !string.IsNullOrEmpty( position.Subrack?.Pop?.Name))
						{
							return( position.Subrack?.Pop?.Name);
						}

						if( position.GaId.HasValue)
						{
							return( position.GaId.ToString());
						}
					}

					return( string.Empty);
				}

/*
If positionType is anything else so SPL with owner FB or any other type not specified above

 Take the subrack of the position and fetch the subrackName, if no subrack name exists take POP_NAME, else GA_ID else empty (function
If Position_Id is not null return Position_Id, else if Position_id is null, then return Position_seq_nr
*/

					if( position.Subrack != null)
					{
						if( !string.IsNullOrEmpty( position.Subrack.Name))
						{
							return( position.Subrack.Name);
						}

						if( !string.IsNullOrEmpty( position.Subrack?.Pop?.Name))
						{
							return( position.Subrack?.Pop?.Name);
						}

						if( position.GaId.HasValue)
						{
							return( position.GaId.ToString());
						}
					}

					if( !string.IsNullOrEmpty( position.PositionId))
					{
						return( position.PositionId);
					}

					//if( position.SeqNr.HasValue)
					//{
					//	return( position.GaId.ToString());
					//}

					if( string.IsNullOrEmpty( position.SeqNr) == false)
					{
						return( position.GaId.ToString());
					}

					return( string.Empty);
			}

			// --- OLT (20210805 SDE)

/*
if positionType is OLT, regardless of any owner
 Return ncname of the OLT
Take first the  [POSITION_GROUP/PXS_ID] eg 1/LT1/GPON_05
*/
			if( positionType == "OLT")
			{
				if( !string.IsNullOrEmpty( position.PositionName))
				{
					return( position.PositionName);
				}

				return( $"{position.PosGroup}/{position.PxsId}");
			}

			// -- NTP (20210805 SDE)

/*
If positionType is NTP, regardless of the owner
 Then take the POSITION_ID where for NTP this is the LINE_ID
 - Else, Return GA_ID
 - Return SeqNr
*/
			if( positionType == "NTP")
			{
				if( !string.IsNullOrEmpty( position.PositionId))
				{
					return( position.PositionId);
				}

				if( position.GaId.HasValue)
				{
					return( position.GaId.ToString());
				}

				//if( position.SeqNr.HasValue)
				//{
				//	return( position.GaId.ToString());
				//}

				if( string.IsNullOrEmpty( position.SeqNr) == false)
				{
					return( position.GaId.ToString());
				}

				return( string.Empty);
			}

			return( string.Empty);
		}
    }
}