/**
 * @Name ONTPPositionConnectionSearchService.cs
 * @Purpose 
 * @Date 27 September 2021, 21:19:21
 * @Author S.Deckers
 * @Description 
 */

namespace PXS.IfRF.Services
{
	#region -- Using directives --
	using System;
	using System.Data;
	using System.Text;
	using System.Data.Common;
	using Oracle.ManagedDataAccess.Client;
	using System.Linq;
	using System.Collections.Generic;
	using Microsoft.EntityFrameworkCore;
	using PXS.IfRF.Logging;
	using PXS.IfRF.Data.Model;
	using PXS.IfRF.Data.Model.ONTPPositionConnectionSearch;
	using PXS.IfRF.BusinessLogic;
	using PXS.IfRF.Supporting;
	using PXS.IfRF.Data;
	using d=System.Diagnostics.Debug;	
	#endregion

	public class ONTPPositionConnectionSearchService : IONTPPositionConnectionSearchService
	{
		private readonly ModelContext	_ifrfContext	= null;
		private readonly ILoggerManager _logger			= null;
		private readonly ISharedMethods _sharedMethods	= null;

		public ONTPPositionConnectionSearchService
		(
			ModelContext	ifrfContext
		,	ISharedMethods	sharedMethods
		,	ILoggerManager	logger
		)
		{
			_ifrfContext	= ifrfContext;
			_sharedMethods	= sharedMethods;
			_logger			= logger;
		}

		/// <summary date="06-10-2021, 11:58:52" author="S.Deckers">
		/// Executes the specified ONTPPositionConnectionSearchRequest
		/// </summary>
		/// <param name="r">The r.</param>
		/// <returns></returns>
		public (List<ONTPPositionConnectionSearchResponse>, string) Execute( ONTPPositionConnectionSearchRequest r)
		{
			if( r.PositionType != "ONTP")
			{
				return( null, "Only 'ONTP' is supported as PositionType");
			}

			(List<ONTPPositionConnectionSearchResponse> allItems, string errorMessage) = getFreeONTPPositions ( r.GA, r.Utac, r.PositionId, r.OrderId, r.FreeOnly);

			return( allItems, errorMessage);
		}

		/// <summary date="06-10-2021, 11:58:35" author="S.Deckers">
		/// Gets the free ontp positions.
		/// </summary>
		/// <param name="ga">The ga.</param>
		/// <param name="utac">The utac.</param>
		/// <param name="positionId">The position identifier.</param>
		/// <param name="orderId">The order identifier.</param>
		/// <param name="freeOnly">if set to <c>true</c> [free only].</param>
		/// <returns></returns>
		private (List<ONTPPositionConnectionSearchResponse>, string) getFreeONTPPositions
		( 
			long?	ga
		,	string	utac
		,	string	positionId
		,	string	orderId
		,	bool	freeOnly
		)
		{
			List<ONTPPositionConnectionSearchResponse> items = new List<ONTPPositionConnectionSearchResponse>( );

			var result = this._ifrfContext.SrPositions.Where( x => x.Type == "ONTP");

			if( ga.HasValue)							result = result.Where( x => x.GaId == ga.Value);
			if( ! string.IsNullOrEmpty( utac))			result = result.Where( x => x.Utac == utac);
			if( ! string.IsNullOrEmpty( positionId))	result = result.Where( x => x.PositionId == positionId);

			if( ! string.IsNullOrEmpty( orderId))
			{
				result = result.Include( x => x.Resource).ThenInclude( x => x.ResourceOrder).Where( x => x.Resource.ResourceOrder.OrderId == orderId);
			}
			else
			{
				result = result.Include( x => x.Resource).ThenInclude( x => x.ResourceOrder);
			}

			/* --- We need Linq to generate inner joins, but this statement generates left outer joins creating 
			 *     invalid results. Because of this, below an item is traced if searching for free only (20211217 SDE)
			 *     
			if( freeOnly)
			{
				result = result.Include( x => x.ConnectionFroms.Where( x => x.FromId == null && x.ToId == null) );
				result = result.Include( x => x.ConnectionTos.Where( x => x.FromId == null && x.ToId == null ) );
			}
			*/

			foreach ( var srPosition in result)
			{
				Resource resource = this._ifrfContext.RsResources.Where( x => x.Id == srPosition.Id).FirstOrDefault( );

				if (freeOnly)
				{	
					ResponseConnection rc = this._sharedMethods.TraceOntp2Splo( srPosition.Id);

					if( rc != null)
					{
						continue;
					}
				}

				if ( resource == null)
				{
					continue;
				}

				ONTPPositionConnectionSearchResponse response = new ONTPPositionConnectionSearchResponse( );

				response.ONTPPosition = ONTPPositionConnectionSearchResponse.Clone( srPosition, resource);

				// --- Get JV-name

				SrPosition ont_position = _ifrfContext.SrPositions.Where(srp => srp.Id == srPosition.Id)
					.Include(srp => srp.Subrack)
					.ThenInclude(sr => sr.Pop)
					.ThenInclude(p => p.RfArea)
					.Single();

				string jv = ont_position?.Subrack?.Pop?.RfArea?.Owner;	

				response.ONTPPosition.SubrackInfo.Id			= ont_position?.Subrack?.Id;
				response.ONTPPosition.SubrackInfo.SubrackId		= ont_position?.Subrack?.SubrackId;
				response.ONTPPosition.SubrackInfo.FrameId		= ont_position?.Subrack?.RackSpace?.FrameId;
				response.ONTPPosition.SubrackInfo.SubrackType	= ont_position?.Subrack?.Type;
				response.ONTPPosition.PositionName				= this._sharedMethods.GetPositionName( ont_position, jv);

				items.Add( response);
			}

			// --- If freeOnly is set to true, do not get info about connections (20211217 SDE)

			if( freeOnly)
			{
				return( items, string.Empty);
			}

			// --- Get info about connected Splitter outport (SPLO) and OLT connected to the splitter (20210930 SDE)

			foreach( var item in items)
			{
				ResponseConnection rc = this._sharedMethods.TraceOntp2Splo( item.ONTPPosition.Id);

				if( rc == null)
				{
					continue;
				}

				item.ConnectionInfo = rc;
				
				Resource resource = this._ifrfContext.RsResources.Where( x => x.Id == item.ConnectionInfo.FromId)
						.Include( x => x.ResourceOrder)
						.FirstOrDefault( );

				SrPosition splo_position = _ifrfContext.SrPositions.Where( srp => srp.Id == item.ConnectionInfo.FromId )
					.Include( srp => srp.Subrack )
					.ThenInclude( s1 => s1.RackSpace)
					.ThenInclude( sr => sr.Pop )
					.ThenInclude( p => p.RfArea )
					.Single( );

				string jv = splo_position?.Subrack?.Pop?.RfArea?.Owner;

				// --- FromSrName (20211014 SDE)

				SrPosition oltItem = new SrPosition();
				oltItem = _ifrfContext.SrPositions.Where(x => x.Id == rc.FromId).FirstOrDefault();
				item.ConnectionInfo.FromSrName	= _sharedMethods.GetSubrackName(subrack: oltItem.Subrack, owner: jv);
				item.ConnectionInfo.FromPopName = _sharedMethods.GetPopName( oltItem.Subrack.Id);

				// --- Subrack (20211014 SDE)

				if( splo_position.SubrackId != null)
				{
					Subrack		subrack			= this._ifrfContext.Subracks.Where( x => x.Id == splo_position.SubrackId).FirstOrDefault( );
					splo_position.Subrack.Name	= _sharedMethods.GetSubrackName(subrack: subrack, owner: jv);
					splo_position.Subrack = subrack;
				}

				item.SPLOPosition		= ONTPPositionConnectionSearchResponse.Clone( splo_position, resource);

				item.SPLOPosition.SubrackInfo.Id			= splo_position?.SubrackId;
				item.SPLOPosition.SubrackInfo.SubrackId		= splo_position?.Subrack?.SubrackId;
				item.SPLOPosition.SubrackInfo.FrameId		= splo_position?.Subrack?.RackSpace?.FrameId;
				item.SPLOPosition.SubrackInfo.SubrackType	= splo_position?.Subrack?.Type;
				item.SPLOPosition.SubrackInfo.Name			= this._sharedMethods.GetSubrackName(subrack: splo_position.Subrack, owner: jv);
				item.SPLOPosition.PositionName				= this._sharedMethods.GetPositionName( splo_position, jv );
			}

			// --- For each item, trace upstream until the OLT (20210930 SDE)

			foreach( var item in items)
			{
				if( item.ConnectionInfo == null)
				{
					continue;
				}

				if( item.ConnectionInfo?.FromId.HasValue == false)
				{
					continue;
				}

				SrPosition	oltPosition = this._sharedMethods.TraceSplo2Olt( item.ConnectionInfo.FromId.Value);

				if( oltPosition == null)
				{
					continue;
				}

				oltPosition = _ifrfContext.SrPositions.Where( srp => srp.Id == oltPosition.Id )
					.Include( srp => srp.Subrack )
					.ThenInclude( sr => sr.Pop )
					.ThenInclude( p => p.RfArea )
					.Single( );

				string		jv			= oltPosition?.Subrack?.Pop?.RfArea?.Owner;

				Resource resource = this._ifrfContext.RsResources.Where( x => x.Id == item.ConnectionInfo.FromId)
						.Include( x => x.ResourceOrder)
						.Single( );

				item.OLTPPosition		= ONTPPositionConnectionSearchResponse.Clone( oltPosition, resource);

				var frameId = _ifrfContext.SrPositions.Where( srp => srp.Id == oltPosition.Id )
					.Include	( srp => srp.Subrack )
					.ThenInclude( i2 => i2.RackSpace)
					.Single		( );

				item.OLTPPosition.SubrackInfo.Id			= oltPosition?.SubrackId;
				item.OLTPPosition.SubrackInfo.SubrackId		= oltPosition?.Subrack?.SubrackId;
				item.OLTPPosition.SubrackInfo.FrameId		= frameId?.Subrack?.RackSpace?.FrameId;
				item.OLTPPosition.SubrackInfo.SubrackType	= oltPosition?.Subrack?.Type;
				item.OLTPPosition.PositionName				= this._sharedMethods.GetPositionName( oltPosition, jv );

				// --- FeedingConnectionInfo (20220225 SDE)

				long splo_id = item.SPLOPosition.Id;
				long? thePP = 0;
				long? theConnection = 0;

				( theConnection, thePP) = getFeedingConnection( splo_id:splo_id);

				if( theConnection == null)
				{
					continue;
				}

				SrPosition	p			= this._ifrfContext.SrPositions.Where( x => x.Id == thePP).Include( x => x.Subrack ).ThenInclude( x => x.RackSpace).Single( );
				Resource	pResource	= this._ifrfContext.RsResources.Where( x => x.Id == thePP).Single( );

				var connections = this._ifrfContext.Connections.Where( x => x.Id == theConnection).Include( rs => rs.Resource).ToList();
				p.ConnectionFroms = connections;

				p.ConnectionFroms.Single().FromSrName	= _sharedMethods.GetSubrackName( subrack: p.Subrack, owner: jv);
				p.ConnectionFroms.Single().FromPopName	= _sharedMethods.GetPopName( p.Subrack.Id);

				p.PositionName = this._sharedMethods.GetPositionName( p, jv );
				item.PPPosition = ONTPPositionConnectionSearchResponse.Clone( p, pResource);
			}

			return( items, string.Empty);
		}

		/// <summary date="08-03-2022, 14:22:49" author="S.Deckers">
		/// Gets the feeding connection.
		/// </summary>
		/// <param name="splo_id">The splo identifier.</param>
		/// <returns></returns>
		private (long?, long?) getFeedingConnection( long splo_id)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			string sql = 
@"select c2.id connection_id
	,	c2.from_id	pp_id
	from connection c1
		inner join connection c2 on c2.to_id = c1.from_id and c2.type ='FD-C' 
where 1=1
	and c1.type = 'INT'
	and c1.to_id = :splitter_out_pos
and 2=2";

			DbConnection connection = _ifrfContext.GetOpenConnection();

			try
			{
				DbCommand cmd = connection.CreateCommand();
				cmd.CommandText = sql;
				cmd.Parameters.Add( new OracleParameter() { ParameterName = "splitter_out_pos",	DbType = DbType.Int64, Direction = ParameterDirection.Input, Value = splo_id });

				DbDataReader reader	= cmd.ExecuteAndLogReader( _logger);

				if( ! reader.Read())
				{
					return( null, null);
				}

				long theConnection	= System.Convert.ToInt64( reader[ "connection_id"]);
				long thePP			= System.Convert.ToInt64( reader[ "pp_id"]);

				return( theConnection, thePP);
			}
			finally
			{
				if( connection.State == ConnectionState.Open)
				{
					connection.Close();
				}
			}
		}

		/// <returns></returns>
		//private SrPosition constructPPPosition( long id, string owner)
		//{
		//	d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

		//	// --- Construct the PPPosition (20220222 SDE)

		//	SrPosition p = this._ifrfContext.SrPositions.Where	( x => x.Id == id).FirstOrDefault( );

		//	var connections = this._ifrfContext.Connections.Where( x => x.FromId == id).Include( rs => rs.Resource).ToList();

		//	p.ConnectionFroms = connections;

		//	return( p);
		//}
	}
}