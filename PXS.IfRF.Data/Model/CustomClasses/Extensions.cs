/**
 * @Name Extensions.cs
 * @Purpose 
 * @Date 05 August 2021, 12:13:12
 * @Author S.Deckers
 * @Description 
 */

namespace PXS.IfRF.Data.Model
{
	#region -- Using directives --
	using System;
	using System.Collections.Generic;
	using System.Text;
	using d=System.Diagnostics.Debug;
	#endregion

	public static class Extensions
	{		
		public static void Dump( this SrPosition item)
		{
			StringBuilder sb = new StringBuilder( );
			sb.AppendFormat( "Id={0}",				item.Id);
			sb.AppendFormat( ", Comments={0}",		item.Comments);
			sb.AppendFormat( ", Optics={0}",		item.ConnectorType == null ? "null" : item.ConnectorType.ToString( ));
			sb.AppendFormat( ", GaId={0}",			item.GaId == null ? "null" : item.GaId.ToString( ));
			sb.AppendFormat( ", LineId={0}",		item.LineId);
			sb.AppendFormat( ", PosGroup={0}",		item.PosGroup);
			sb.AppendFormat( ", PositionId={0}",	item.PositionId);
			sb.AppendFormat( ", PositionName={0}",	item.PositionName);			
			sb.AppendFormat( ", PxsId={0}",			item.PxsId);
			sb.AppendFormat( ", SeqNr={0}",			item.SeqNr == null ? "null" : item.SeqNr.ToString( ));
			sb.AppendFormat( ", SpecId={0}",		item.SpecId == null ? "null" : item.SpecId.ToString( ));
			sb.AppendFormat( ", SubrackId={0}",		item.SubrackId);
			sb.AppendFormat( ", Type={0}",			item.Type);
			sb.AppendFormat( ", Utac={0}",			item.Utac);
		}

		public static void Dump( this MetaSpecSrPosition item)
		{
			StringBuilder sb = new StringBuilder( );
			sb.AppendFormat( "Id={0}",					item.Id);
			sb.AppendFormat( ", SpecCode={0}",			item.SpecCode);
			sb.AppendFormat( ", Type={0}",				item.Type);
			sb.AppendFormat( ", NrPositions={0}",		item.NrPositions);
			sb.AppendFormat( ", PositionListSr={0}",	item.PositionListSr);
			sb.AppendFormat( ", PxsPositionListSr={0}",	item.PxsPositionListSr);
			d.WriteLine( sb.ToString( ) );
		}

		public static void Dump( this Connection item)
		{
			StringBuilder sb = new StringBuilder( );
			sb.AppendFormat( "Id={0}",				item.Id);
			sb.AppendFormat( ", FromId={0}",		item.FromId);
			sb.AppendFormat( ", ToId={0}",			item.ToId);			
			sb.AppendFormat( ", FromPopName={0}",	item.FromPopName);
			sb.AppendFormat( ", ToPopName={0}",		item.ToPopName);
			sb.AppendFormat( ", FromSrName={0}",	item.FromSrName);
			sb.AppendFormat( ", ToSrName={0}",		item.ToSrName);
			sb.AppendFormat( ", Type={0}",			item.Type);
			sb.AppendFormat( ", Attentuation={0}",	item.Attentuation);
			sb.AppendFormat( ", LineId={0}",		item.LineId);
			sb.AppendFormat( ", Nr={0}",			item.Nr);
			sb.AppendFormat( ", LineId={0}",		item.LineId);
			sb.AppendFormat( ", Comments={0}",		item.Comments);

			d.WriteLine( sb.ToString( ) );
		}

		public static void Dump( this Subrack item)
		{
			StringBuilder sb = new StringBuilder( );
			sb.AppendFormat( "Id={0}",					item.Id);
			sb.AppendFormat( ", SeqNr={0}",				item.SeqNr == null ? "null" : item.SeqNr.ToString( ));
			sb.AppendFormat( ", SubrackId={0}",			item.SubrackId);
			sb.AppendFormat( ", Type={0}",				item.Type);
			sb.AppendFormat( ", Comments={0}",			item.Comments);
			sb.AppendFormat( ", PxsId={0}",				item.PxsId);
			sb.AppendFormat( ", AsbMaterialCode={0}",	item.AsbMaterialCode);
			sb.AppendFormat( ", Name={0}",				item.Name);
			sb.AppendFormat( ", PopId={0}",				item.PopId == null ? "null" : item.PopId.ToString( ));
			sb.AppendFormat( ", SpecId={0}",			item.SpecId == null ? "null" : item.SpecId.ToString( ));
			sb.AppendFormat( ", RackSpaceId={0}",		item.RackSpaceId == null ? "null" : item.RackSpaceId.ToString( ));
			sb.AppendFormat( ", FromHu={0}",			item.FromHu == null ? "null" : item.FromHu.ToString( ));
			sb.AppendFormat( ", ToHu={0}",				item.ToHu == null ? "null" : item.ToHu.ToString( ));

			d.WriteLine( sb.ToString( ) );
		}

		public static void Dump( this Pop item)
		{
			StringBuilder sb = new StringBuilder( );
			sb.AppendFormat( "ApCpLength={0}",		item.ApCpLength);
			sb.AppendFormat( ", CentralPopId={0}",	item.CentralPopId);
			sb.AppendFormat( ", Comments={0}",		item.Comments);
			sb.AppendFormat( ", CoorX={0}",			item.CoorX);
			sb.AppendFormat( ", CoorY={0}",			item.CoorY);
			sb.AppendFormat( ", GaId={0}",			item.GaId);
			sb.AppendFormat( ", LocationName={0}",	item.LocationName);
			sb.AppendFormat( ", Model={0}",			item.Model);
			sb.AppendFormat( ", Municipality={0}",	item.Municipality);
			sb.AppendFormat( ", Name={0}",			item.Name);
			sb.AppendFormat( ", Node={0}",			item.Node);
			sb.AppendFormat( ", NrOfLines={0}",		item.NrOfLines);
			sb.AppendFormat( ", PopAssemblyId={0}", item.PopAssemblyId);
			sb.AppendFormat( ", Postcode={0}",		item.Postcode);
			sb.AppendFormat( ", RfAreaId={0}",		item.RfAreaId);
			sb.AppendFormat( ", Status={0}",		item.Status);
			sb.AppendFormat( ", Street={0}",		item.Street);
			sb.AppendFormat( ", Subaddress={0}",	item.Subaddress);
			sb.AppendFormat( ", Suffix={0}",		item.Suffix);
			sb.AppendFormat( ", Type={0}",			item.Type);

			d.WriteLine( sb.ToString( ) );
		}

		public static void Dump( this RefPlPopType item)
		{
			StringBuilder sb = new StringBuilder( );
			sb.AppendFormat( "Key={0}",				item.Key);
			sb.AppendFormat( ", Owner={0}",			item.Owner);
			sb.AppendFormat( ", Description={0}",	item.Description);
			sb.AppendFormat( ", DisplayName={0}",	item.DisplayName);
			sb.AppendFormat( ", ExternalRef={0}",	item.ExternalRef);
			d.WriteLine( sb.ToString( ) );
		}

		public static void Dump( this RackSpace item)
		{
			StringBuilder sb = new StringBuilder( );
			sb.AppendFormat( "Id={0}",					item.Id);
			sb.AppendFormat( ", FrameId={0}",			item.FrameId);
			sb.AppendFormat( ", FrameHuB={0}",			item.FrameHuB == null ? "null" : item.FrameHuB.ToString( ));
			sb.AppendFormat( ", FrameHuT={0}",			item.FrameHuT == null ? "null" : item.FrameHuT.ToString( ));
			sb.AppendFormat( ", FrameType={0}",			item.FrameType);
			sb.AppendFormat( ", LoudnessNominal={0}",	item.LoudnessNominal == null ? "null" : item.LoudnessNominal.ToString( ));
			sb.AppendFormat( ", LoudnessPeak={0}",		item.LoudnessPeak == null ? "null" : item.LoudnessPeak.ToString( ));
			sb.AppendFormat( ", NrHu={0}",				item.NrHu == null ? "null" : item.NrHu.ToString( ));
			sb.AppendFormat( ", PopId={0}",				item.PopId);
			sb.AppendFormat( ", PowerNominal={0}",		item.PowerNominal == null ? "null" : item.PowerNominal.ToString( ));
			sb.AppendFormat( ", PowerPeak={0}",			item.PowerPeak == null ? "null" : item.PowerPeak.ToString( ));
			d.WriteLine( sb.ToString( ) );
		}

		public static void Dump( this AvailabilityResponse item)
		{
			/*
			item.SplitterPosition.Dump( );
			item.OLTPosition.Dump( );
			*/

			// --- sorting check (20210805 SDE)

			StringBuilder sb = new StringBuilder( );
			sb.AppendFormat( "Id={0}",	item.SplitterPosition.Id);
			sb.AppendFormat( ", Subracktype={0}",	item.SplitterPosition.Type);
			sb.AppendFormat( ", PopName={0}",		item.SplitterPosition.Subrack?.Pop?.Name);
			sb.AppendFormat( ", FrameId={0}",		item.SplitterPosition.Subrack?.RackSpace?.FrameId);
			sb.AppendFormat( ", SubrackId={0}",		item.OLTPosition.Subrack?.Id);
			sb.AppendFormat( ", PosGroup={0}",		item.OLTPosition.PosGroup);
			sb.AppendFormat( ", PositionType={0}",	item.OLTPosition.Type);
			sb.AppendFormat( ", PositionId={0}",	item.OLTPosition.Id);
			d.WriteLine( sb.ToString( ) );			
		}

		public static void Dump( this List<AvailabilityResponse> items)
		{
			foreach( AvailabilityResponse item in items)
			{
				item.Dump( );
			}
		}
	}
}
