/**
 * @Name UniFiberCsvConnection.cs
 * @Purpose 
 * @Date 11 March 2022, 11:51:33
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
	using PXS.IfRF.Data;
	using PXS.IfRF.BusinessLogic;
	using PXS.IfRF.Logging;
	using PXS.IfRF.Data.Model;
	using PXS.IfRF.Supporting;
	using d=System.Diagnostics.Debug;
	#endregion

	public class UniFiberCsvConnection : GeneralCsvConnection
	{
		public	override int		NrColumns => 14;

		public string CustomerReference	{ get; set; }

		public UniFiberCsvConnection
		(
			int					LineNr
		,	string				csvLine
		,	ModelContext		ifrfContext
		,	IConnectionService	connectionService
		,	ILoggerManager		logger
		,	ISharedMethods		sharedMethods
		):base( LineNr, csvLine, "JV2", ifrfContext, connectionService, logger, sharedMethods)
		{ }

		internal override void ConstructFromCols( string [] cols)
		{
			short p = 0;

			this.ConnectionNr		= cols[p++] ?? cols[p++];
			this.Type				= cols[p++] ?? cols[p++];
			this.FromPopName		= cols[p++] ?? cols[p++];
			this.FromFrameId		= cols[p++] ?? cols[p++];
			this.FromSubrackId		= cols[p++] ?? cols[p++];
			this.FromPositionId		= cols[p++] ?? cols[p++];
			this.ToPopName			= cols[p++] ?? cols[p++];
			this.ToFrameId			= cols[p++] ?? cols[p++];
			this.ToSubrackId		= cols[p++] ?? cols[p++];
			this.ToPositionId		= cols[p++] ?? cols[p++];
			this.CustomerReference	= cols[p++] ?? cols[p++];
			this.LineId				= cols[p++] ?? cols[p++];
			this.Length				= cols[p++] ?? cols[p++];
			this.Route				= cols[p++] ?? cols[p++];
		}			

		public override string ToString( )
		{
			StringBuilder sb = new StringBuilder( );
			sb.AppendFormat( "Owner={0}",				this.Owner);
			sb.AppendFormat( ", LineNr={0}",			this.LineNr);
			sb.AppendFormat( ", ConnectionNr={0}",		this.ConnectionNr );
			sb.AppendFormat( ", Type={0}",				this.Type );
			sb.AppendFormat( ", FromPopName={0}",		this.FromPopName );
			sb.AppendFormat( ", FromFrameId={0}",		this.FromFrameId );
			sb.AppendFormat( ", FromPositionId={0}",	this.FromPositionId );
			sb.AppendFormat( ", ToPopName={0}",			this.ToPopName );
			sb.AppendFormat( ", ToFrameId={0}",			this.ToFrameId );
			sb.AppendFormat( ", ToFrameId={0}",			this.ToFrameId );
			sb.AppendFormat( ", ToSubrackId={0}",		this.ToSubrackId );
			sb.AppendFormat( ", ToPositionId={0}",		this.ToPositionId );
			sb.AppendFormat( ", CustomerReference={0}",	this.CustomerReference); // --- UniFiber only (20220311 SDE)
			sb.AppendFormat( ", LineId={0}",			this.LineId);
			sb.AppendFormat( ", Length={0}",			this.Length);
			sb.AppendFormat( ", Route={0}",				this.Route);
			return( sb.ToString( ));
		}

		internal override void ValidateTo( )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty ) );

			base.ValidateTo( );
			if( this.InvalidReasons.Count() > 0) return;

			// --- Validate To for FD-C types for UniFiber (20220610 SDE)

			if( this.Type != "FD-C")
			{
				return;
			}			

			if( this.SqlOperation != SqlOperation.Insert)
			{
				return;
			}

			// --- Inserting -> Search for SPLIN(20220316 SDE)

			string sql = 
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
		left outer join connection 	on connection.to_id		= sr_position.id
where 1=1
	and sr_position.type = 'SPLI'
	and rf_area.owner 	= :theOwner
	and pop.name 		= :thePopName
	and connection.id is null
and 2=2 fetch first row only";

			DbConnection	connection = this._modelContext.GetOpenConnection();
			DbCommand		cmd			= connection.CreateCommand();
			cmd.Parameters.Add( new OracleParameter() { ParameterName = "theOwner",			DbType = DbType.String, Direction = ParameterDirection.Input,	Value = this.Owner });
			cmd.Parameters.Add( new OracleParameter() { ParameterName = "thePopName",		DbType = DbType.String, Direction = ParameterDirection.Input,	Value = this.ToPopName });

			cmd.CommandText = sql;

			DbDataReader reader	= cmd.ExecuteAndLogReader( _logger);

			if( ! reader.Read())
			{
				StringBuilder sb = new StringBuilder( );
				sb.AppendFormat( "No Free SPLIN Position found");
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
