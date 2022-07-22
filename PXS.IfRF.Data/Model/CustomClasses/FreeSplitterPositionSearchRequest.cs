using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using d=System.Diagnostics.Debug;

namespace PXS.IfRF.Data.Model
{
	public class FreeSplitterPositionSearchRequest
	{
		[ Required	( )]
		public string	Owner					{ get; set; }

		[ Required	( )]
		public string	PopName					{ get; set; }

		public int?		OdfTrayNr				{ get; set; }
		public string	PositionStatus			{ get; set; }
		public string	RelatedOltPositionType	{ get; set; }
		public int?		PosToRetrieve			{ get; set; }

		[ DefaultValue( "Y")]
		public string	PopStatus				{ get; set; } = "Y";

		public string	SparePositions			{ get; set; }
		public int?		InternalSubrackId		{ get; set; }

		public override string ToString( )
		{
			StringBuilder sb = new StringBuilder() ;
			sb.AppendFormat( "Owner={0}",						this.Owner);
			sb.AppendFormat( ", PopName={0}",					this.PopName);
			sb.AppendFormat( ", OdfTrayNr={0}",					this.OdfTrayNr);
			sb.AppendFormat( ", PositionStatus={0}",			this.PositionStatus);
			sb.AppendFormat( ", RelatedOltPositionType={0}",	this.RelatedOltPositionType);
			sb.AppendFormat( ", PosToRetrieve={0}",				this.PosToRetrieve);
			sb.AppendFormat( ", PopStatus={0}",					this.PopStatus);
			sb.AppendFormat( ", SparePositions={0}",			this.SparePositions);
			sb.AppendFormat( ", InternalSubrackId={0}",			this.InternalSubrackId);
			return sb.ToString( );
		}

		public void Dump( )
		{
			d.WriteLine( this.ToString());
		}
	}
}