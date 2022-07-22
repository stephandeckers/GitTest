/**
 * @Name FiberKlaarCsvConnection.cs
 * @Purpose 
 * @Date 11 March 2022, 11:50:30
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

	public class FiberKlaarCsvConnection : GeneralCsvConnection
	{
		public	override int		NrColumns => 13;

		public FiberKlaarCsvConnection
		(
			int					LineNr
		,	string				csvLine
		,	ModelContext		ifrfContext
		,	IConnectionService	connectionService
		,	ILoggerManager		logger
		,	ISharedMethods		sharedMethods
		):base( LineNr, csvLine, "JV1", ifrfContext, connectionService, logger, sharedMethods)
		{ }

		internal override void ConstructFromCols( string [] cols)
		{
			short p = 0;

			this.ConnectionNr	= cols[p++] ?? cols[p++];
			this.Type			= cols[p++] ?? cols[p++];
			this.FromPopName	= cols[p++] ?? cols[p++];
			this.FromFrameId	= cols[p++] ?? cols[p++];
			this.FromSubrackId	= cols[p++] ?? cols[p++];
			this.FromPositionId = cols[p++] ?? cols[p++];
			this.ToPopName		= cols[p++] ?? cols[p++];
			this.ToFrameId		= cols[p++] ?? cols[p++];
			this.ToSubrackId	= cols[p++] ?? cols[p++];
			this.ToPositionId	= cols[p++] ?? cols[p++];
			this.LineId			= cols[p++] ?? cols[p++];
			this.Length			= cols[p++] ?? cols[p++];
			this.Route			= cols[p++] ?? cols[p++];
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
			sb.AppendFormat( ", LineId={0}",			this.LineId);
			sb.AppendFormat( ", Length={0}",			this.Length);
			sb.AppendFormat( ", Route={0}",				this.Route);
			return( sb.ToString( ));
		}
	}
}
