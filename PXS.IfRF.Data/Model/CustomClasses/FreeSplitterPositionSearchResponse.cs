namespace PXS.IfRF.Data.Model
{
	#region -- Using directives --
	using System;
	using System.Text;
	using d=System.Diagnostics.Debug;
	#endregion

	public class FreeSplitterPositionSearchResponse
	{
		public SrPosition SplitterPosition	{ get; set; }
		public SrPosition PPPosition		{ get; set; }
		public SrPosition OLTPosition		{ get; set; }

		public void Dump()
		{
			this.SplitterPosition.Dump			( );
			this.SplitterPosition?.Subrack.Dump	( );
			this.OLTPosition.Dump				( );
		}
	}
}
