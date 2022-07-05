/**
 * @Name CustomPositionCreationRequest.cs
 * @Purpose 
 * @Date 25 March 2022, 11:24:29
 * @Author S.Deckers
 * @Description 
 */

namespace PXS.IfRF.Data.Model
{
	#region -- Using directives --
	using System.ComponentModel.DataAnnotations;
	#endregion

	public class CustomPositionRequest
	{
		[ Required	( )]
		public string Type				{ get; set; }

		[ Required	( )]
		public long SubrackId			{ get; set; }

		[ Required	( )]
		public long PositionSpecId		{ get; set; }

		public string Group				{ get; set; }

		[ Required	( )]
		public string OperationalStatus	{ get; set; }
	}
}