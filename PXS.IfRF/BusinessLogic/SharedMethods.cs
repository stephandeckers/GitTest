using Microsoft.AspNet.OData;
using PXS.IfRF.AuthHandling;
using PXS.IfRF.Data.Model;
using PXS.IfRF.Data.Model.ONTPPositionConnectionSearch;
using PXS.IfRF.Logging;

using System.Linq;

namespace PXS.IfRF.BusinessLogic
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
	using d = System.Diagnostics.Debug;
	using PXS.IfRF.Data;
    using PXS.IfRF.Supporting;
    #endregion

	public class SharedMethods : ISharedMethods
	{
		private readonly ModelContext _ifrfContext = null;
		private readonly ILoggerManager _logger = null;
		private readonly DbConnection _connection;

		public SharedMethods(ModelContext ifrfContext, ILoggerManager logger)
		{
			_ifrfContext = ifrfContext;
			_logger = logger;
			_connection = ifrfContext.GetOpenConnection();
		}

		/// <summary date="05-08-2021, 20:44:42" author="S.Deckers">
		/// Gets the name of the subrack.
		/// </summary>
		/// <param name="position">The position.</param>
		/// <param name="positionType">Type of the position.</param>
		/// <param name="owner">The owner.</param>
		/// <returns></returns>
		public string GetSubrackName(Subrack subrack, string owner)
		{
			/*
			If positionType is SPL and owner AMB, then

			
			*/
			//fetch first SPLI pos
			if (subrack.Type == "SPL" && owner == "JV2")
			{
				SrPosition spli = _ifrfContext.SrPositions.Where(x => x.SubrackId == subrack.Id && x.Type == "SPLI").FirstOrDefault();
				if (spli.Id != 0)
				{
					string OltPosName = null;
					SrPosition relGpon = new SrPosition();

					(relGpon, OltPosName) = TraceSplInToOlt(spli.Id);

					if(relGpon != null)
					{
						relGpon.ConnectionFroms = this._ifrfContext.Connections.Where(x => x.FromId == relGpon.Id).ToHashSet();

						SrPosition relPP = _ifrfContext.SrPositions.Where(x => x.Id == relGpon.ConnectionFroms.FirstOrDefault().ToId).FirstOrDefault();
						if(relPP.Id != 0)
						{
							Subrack patchPanel = _ifrfContext.Subracks.Where(x => x.Id == relPP.SubrackId).FirstOrDefault();
							if (patchPanel.RackSpaceId != null && patchPanel.RackSpace == null)
                            {
								patchPanel.RackSpace = _ifrfContext.RackSpaces.Where(x => x.Id == patchPanel.RackSpaceId).FirstOrDefault();
                            }

							string subrackName = "";
							if (!string.IsNullOrEmpty(patchPanel?.Pop?.Name))
							{
								subrackName = patchPanel?.Pop?.Name + "-";
							}

							if (!string.IsNullOrEmpty(patchPanel?.RackSpace?.FrameId))
							{
								subrackName = subrackName + patchPanel?.RackSpace?.FrameId + "-";
							}

							if (!string.IsNullOrEmpty(patchPanel?.SubrackId))
							{
								subrackName = subrackName + patchPanel?.SubrackId + "-";
							}
							if (!string.IsNullOrEmpty(relPP?.PositionId))
							{
								subrackName = subrackName + relPP?.PositionId;

							}

							return (subrackName);
						}
					}
				}
			}

			// --- OLT (20210805 SDE)

			/*
			If positionType is OLT regardless of the owner, then

			Retrieve Subrack> PXS_ID
			*/
			if (subrack.Type == "OLT")
			{
				if (!string.IsNullOrEmpty(subrack?.PxsId))
				{
					return (subrack?.PxsId);
				}

				return (string.Empty);
			}

			/*
			Default name: If positionType is any other value, or SPL with owner JV1 

			then fetch Name of the POP retrieved via subrack-POP relation
			Fetch FRAME_ID via SR_Frame of the Rack_space
			and last  if subrack_Id is not null, return subrack_Id and if subrack_Id is null return Subrack_seq_nr 
			*/
			string defaultSubrackName = "";
			if (!string.IsNullOrEmpty(subrack?.Pop?.Name))
			{
				defaultSubrackName = subrack?.Pop?.Name + "-";
			}

			if (!string.IsNullOrEmpty(subrack?.RackSpace?.FrameId))
			{
				defaultSubrackName = defaultSubrackName + subrack?.RackSpace?.FrameId + "-";
			}

			if (!string.IsNullOrEmpty(subrack?.SubrackId))
			{
				defaultSubrackName = defaultSubrackName + subrack?.SubrackId;
			}

			return (defaultSubrackName);
		}

		/// <summary date="05-08-2021, 20:36:29" author="S.Deckers">
		/// Gets the name of the position.
		/// </summary>
		/// <param name="position">The position.</param>
		/// <param name="positionType">Type of the position.</param>
		/// <param name="owner">The owner.</param>
		/// <returns></returns>
		public string GetPositionName(SrPosition position, string owner)
		{
			// --- SPL/Ambiorix

			/*
			If positionType is SPL and Owner is AMB (AMBIORIX)
			Check if a related GPON can be found, if so, positionName = subrackName else need to append the positionId to subrackname
			*/
			if (owner == "JV2" && (position.Type == "SPLO" || position.Type == "SPLI"))
			{
				string OltPosName = null;
				SrPosition relGpon = new SrPosition();

				if (position.Type == "SPLI")
				{

					string spliName = "";
					if (!string.IsNullOrEmpty(position.Subrack?.Pop?.Name))
					{
						spliName = position.Subrack?.Pop?.Name + "-";
					}

					if (!string.IsNullOrEmpty(position.Subrack?.RackSpace?.FrameId))
					{
						spliName = spliName + position.Subrack?.RackSpace?.FrameId + "-";
					}

					if (!string.IsNullOrEmpty(position.Subrack?.SubrackId))
					{
						spliName = spliName + position.Subrack?.SubrackId + "-";
					}
					if (!string.IsNullOrEmpty(position.PositionId))
					{
						spliName = spliName + position.PositionId;
					}

					return (spliName);
				}
				else
                {
					position.ConnectionFroms = this._ifrfContext.Connections.Where(x => x.ToId == position.Id).ToHashSet();

					SrPosition spli = _ifrfContext.SrPositions.Where(x => x.Id == position.ConnectionFroms.FirstOrDefault().FromId).FirstOrDefault();

					(relGpon, OltPosName) = TraceSplInToOlt(spli.Id);

				}
				string positionName = null;
				if (relGpon == null)
                {
					if (!string.IsNullOrEmpty(position.Subrack.Name))
					{
						positionName = position.Subrack.Name + "-";
					}

					if (!string.IsNullOrEmpty(position.PositionId))
					{
						positionName = positionName + position.PositionId;
					}
					return positionName;
				}
                else
                {
					return position.Subrack.Name;
                }

				
			}

			// --- OLT (20210805 SDE)

			/*
			 OLT subrack. PXS_ID + /1/ + <position.group>/< position.PXS_ID>
			if positionType is OLT, regardless of any owner
			 eg F80BUL00005/1/LT1/GPON_05
			*/
			if (position.Type == "GPON" || position.Type == "XGSP" || position.Type == "NT" )
			{
				string subrackName = "";
				string positionGroup = "";
				string positionPxsName = "";
				if (!string.IsNullOrEmpty(position.Subrack?.PxsId))
				{
					subrackName = position.Subrack?.PxsId;
				}
				if (!string.IsNullOrEmpty(position.PosGroup))
				{
					positionGroup = position.PosGroup;
				}
				if (!string.IsNullOrEmpty(position.PxsId))
				{
					positionPxsName = position.PxsId;
				}
				return (subrackName + "/1/" + positionGroup + "/" + positionPxsName);
			}

			// -- NTP (20210805 SDE)

			/*
			Returns UTAC (if value)
				ELSE PositionID
				(note: for NTP, the Position ID should Line ID)
				ELSE return GA_ID
				ELSE return SeqNr

			*/
			if (position.Type == "ONTP")
			{
				if (!string.IsNullOrEmpty(position.Utac))
				{
					return (position.Utac);
				}
				if (!string.IsNullOrEmpty(position.PositionId))
				{
					return (position.PositionId);
				}
				if (!string.IsNullOrEmpty(position.LineId))
				{
					return (position.LineId);
				}

				if (position.GaId.HasValue)
				{
					return (position.GaId.ToString());
				}

				if( string.IsNullOrEmpty( position.SeqNr) == false)
				{
					return( position.SeqNr);
				}
				return (string.Empty);
			}

			/*
				If positionType is anything else so SPL with owner FK or any other type not specified above

				Take the subrack of the position and fetch the subrackName
				If Position_Id is not null return Position_Id, else if Position_id is null, then return Position_seq_nr
			*/

			string defaultSubrackName = "";
			if (!string.IsNullOrEmpty(position.Subrack.Name))
			{
				defaultSubrackName = position.Subrack.Name + "-";
			}

			if (!string.IsNullOrEmpty(position.PositionId))
			{
				defaultSubrackName = defaultSubrackName + position.PositionId;
			}

			return (defaultSubrackName);
		}

		/// <summary>
		/// Trace upwards from a SPLI towards an OLT LT position
		/// </summary>
		/// <param name="spliId"></param>
		/// <returns></returns>
		public (SrPosition, string) TraceSplInToOlt(long spliId)
		{
			using (DbCommand cmd = _connection.CreateCommand())
			{
				cmd.CommandText = @"
with tr as 
(select c.*,p1.type from_type,p2.type to_type,level trlevel from connection c inner join sr_position p1 on c.from_id = p1.id inner join sr_position p2 on c.to_id = p2.id
start with c.to_id = :resource_id
connect by nocycle prior c.from_id = c.to_id and p1.type in ('GPON','XGSP') -- stopping criterion
)
select tr.from_type olt_pos_type, tr.from_id olt_pos_id from tr where tr.trlevel = (select max(trlevel) from tr)

				";
				cmd.Parameters.Add(new OracleParameter() { ParameterName = "resource_id", DbType = DbType.Int64, Direction = ParameterDirection.Input, Value = spliId });

				DbDataReader reader = cmd.ExecuteReader();

				bool hasRecord = reader.Read();

				if (!hasRecord)
				{
					return (null, string.Empty);
				}
				long olt_pos_id = Convert.ToInt64(reader["olt_pos_id"]);
				SrPosition olt_position = _ifrfContext.SrPositions.Where(srp => srp.Id == olt_pos_id)
					.Include(srp => srp.Subrack)
					.ThenInclude(sr => sr.Pop)
					.ThenInclude(p => p.RfArea)
					.Single();
				string olt_pos_name = GetPositionName(olt_position, olt_position.Subrack.Pop.RfArea.Owner);

				return (olt_position, olt_pos_name);
			}
		}

		/// <summary date="29-09-2021, 21:53:51" author="S.Deckers">
		/// Traces the ontp to a splitter on the outgoing side, t.i. its sr_position.type = 'SPLO'
		/// </summary>
		/// <param name="ontp_id">The ontp identifier.</param>
		/// <returns></returns>
		public ResponseConnection TraceOntp2Splo( long ontp_id)
		{
			using( DbCommand cmd = _connection.CreateCommand())
			{
				cmd.CommandText = @"
with tr as
(
	select c.*
		, p1.type from_type
		, p2.type to_type
		, level trlevel 
		from connection c
			inner join sr_position p1 on c.from_id 	= p1.id 
			inner join sr_position p2 on c.to_id 	= p2.id	
	start with c.to_id = :resource_id
	connect by nocycle prior c.from_id = c.to_id
)
select 	tr.id
	,	tr.from_id
	,	tr.to_id
	, 	tr.from_type
	, 	tr.to_type 
from tr 
where 1=1
	and tr.from_type = 'SPLO'
and 2=2";

				cmd.Parameters.Add(new OracleParameter() { ParameterName = "resource_id", DbType = DbType.Int64, Direction = ParameterDirection.Input, Value = ontp_id });

				DbDataReader	reader		= cmd.ExecuteReader();
				bool			hasRecord	= reader.Read();

				if (!hasRecord)
				{
					return( null);
				}

				long spl_out_conn_id = Convert.ToInt64(reader["id"]);

				Connection	splo_conn		= _ifrfContext.Connections.Where( x => x.Id == spl_out_conn_id).Single( );
				Resource	splo_conn_res	= this._ifrfContext.RsResources.Where( x => x.Id == splo_conn.Id).Single( );

				// --- Get position SPLO-item (20210930 SDE)
				
				long spl_out_id = Convert.ToInt64(reader["from_id"]);
				SrPosition splo_pos = _ifrfContext.SrPositions.Where(srp => srp.Id == spl_out_id)
					.Include(srp => srp.Subrack)
					.ThenInclude(sr => sr.Pop)
					.ThenInclude(p => p.RfArea)
					.Single();
				//string splo_pos_name = GetPositionName( splo_pos, splo_pos.Subrack.Pop.RfArea.Owner);
				string splo_pos_name = GetPositionName( splo_pos, splo_pos?.Subrack?.Pop?.RfArea?.Owner);

				// --- Get resource (20210930 SDE)

				ResponseConnection responseConnection = ONTPPositionConnectionSearchResponse.Clone( splo_conn, splo_conn_res, splo_pos_name);

				return( responseConnection);
			}
		}

		/// <summary date="30-09-2021, 12:42:21" author="S.Deckers">
		/// Traces the SPLO upstream until OLT
		/// </summary>
		/// <param name="splo_id">The splo identifier.</param>
		/// <returns></returns>
		public ResponseSrPosition TraceSplo2Olt	( long splo_id)
		{
			using( DbCommand cmd = _connection.CreateCommand())
			{
				cmd.CommandText = @"
with tr as
(
	select c.*
		, p1.type from_type
		, p2.type to_type
		, level trlevel 
		from connection c
			inner join sr_position p1 on c.from_id 	= p1.id 
			inner join sr_position p2 on c.to_id 	= p2.id
	start with c.to_id = :resource_id
	connect by nocycle prior c.from_id = c.to_id
)
select 	tr.id
	,	tr.from_id
	,	tr.to_id
	, 	tr.from_type
	, 	tr.to_type 
from tr 
where 1=1
	and tr.from_type in  ( 'GPON', 'XGSP' )
and 2=2";

				cmd.Parameters.Add(new OracleParameter() { ParameterName = "resource_id", DbType = DbType.Int64, Direction = ParameterDirection.Input, Value = splo_id });

				DbDataReader	reader		= cmd.ExecuteReader();
				bool			hasRecord	= reader.Read();

				if (!hasRecord)
				{
					return( null);
				}

				// --- Get position SPLO-item (20210930 SDE)
				
				long olt_from_id = Convert.ToInt64(reader["from_id"]);
				SrPosition olt_pos = _ifrfContext.SrPositions.Where(srp => srp.Id == olt_from_id)
					.Include(srp => srp.Subrack)
					.ThenInclude(sr => sr.Pop)
					.ThenInclude(p => p.RfArea)
					.Single();
				//string splo_pos_name = GetPositionName( olt_pos, olt_pos.Subrack.Pop.RfArea.Owner);
				string splo_pos_name = GetPositionName( olt_pos, olt_pos?.Subrack?.Pop?.RfArea?.Owner);

				// --- Get resource (20210930 SDE)

				Resource res		= this._ifrfContext.RsResources.Where( x => x.Id == olt_from_id).Single( );
				ResponseSrPosition r = ONTPPositionConnectionSearchResponse.Clone( olt_pos, res);
				return( r);
			}
		}

		public IDictionary<long,string> TraceRfAreaSplInToOlt(long rfAreaId)
		{
			Dictionary<long, string> result = new Dictionary<long, string>();

			using (DbCommand cmd = _connection.CreateCommand())
			{
				cmd.CommandText = @"
		with splin as
		(
		select p.id 
		from sr_position p
		inner join subrack s on 
		p.subrack_id = s.id
		inner join pop on 
		s.pop_id = pop.id and pop.rf_area_id = :rf_area_id
		where 
		p.type = 'SPLI'
		),
		tr as 
		(
		select 
		CONNECT_BY_ROOT c.to_id as start_id, 
		c.to_id, 
		c.from_id, 
		p1.type as from_type,
		p2.type as to_type
		from connection c 
		inner join sr_position p1 on c.from_id = p1.id 
		inner join sr_position p2 on c.to_id = p2.id
		start with c.to_id in (select id from splin)
		connect by 
		nocycle prior c.from_id = c.to_id and p1.type in ('GPON','XGSP') -- stopping criterion
		)
		select distinct
		tr.start_id as splin_pos_id, 
		tr.from_type as olt_pos_type
		from tr 
		where 
		tr.from_type in ('GPON','XGSP')
				";
				cmd.Parameters.Add(new OracleParameter() 
				{ 
					ParameterName = "rf_area_id", 
					DbType = DbType.Int64, 
					Direction = ParameterDirection.Input, 
					Value = rfAreaId }
				);

				DbDataReader reader = cmd.ExecuteReader();

				while (reader.Read())
				{
					long splinPosId = Convert.ToInt64(reader["splin_pos_id"]);
					string oltPosType = Convert.ToString(reader["olt_pos_type"]);
					result.Add(splinPosId, oltPosType);
				}

				return result;
			}
		}

		/// <summary date="14-10-2021, 18:57:12" author="S.Deckers">
		/// Gets the name of the pop.
		/// </summary>
		/// <param name="sr_id">The sr identifier.</param>
		/// <returns></returns>
		public string GetPopName ( long sr_id)
		{
			DbConnection connection = _ifrfContext.GetOpenConnection();

			//using (DbCommand cmd = _connection.CreateCommand())
			using (DbCommand cmd = connection.CreateCommand())
			{
				cmd.CommandText = @"
select pop.name from subrack 
		inner join pop on pop.id = subrack.pop_id
where subrack.id = :sr_id";
				cmd.Parameters.Add(new OracleParameter() 
				{ 
					ParameterName	= "sr_id", 
					DbType			= DbType.Int64, 
					Direction		= ParameterDirection.Input, 
					Value			= sr_id 
				}
				);

				DbDataReader reader = cmd.ExecuteReader();

				if( reader.Read())
				{
					string popName = Convert.ToString(reader["name"]);
					return(popName);
				}

				return( string.Empty);
			}
		}
	}
}