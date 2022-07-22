/**
 * @Name Request.cs
 * @Purpose 
 * @Date 30 September 2021, 13:15:10
 * @Author S.Deckers
 * @Description 
 */

namespace PXS.IfRF.Data.Model.ONTPPositionConnectionSearch
{	
	#region -- Using directives
	using System.ComponentModel;
	using System.ComponentModel.DataAnnotations;
	#endregion

	public class ONTPPositionConnectionSearchRequest
	{
		[ Required( )]
		public string PositionType		{ get; set; }

		[ DefaultValue("false")]
		public bool		FreeOnly		{ get; set; } = false;
		public long?	GA				{ get; set; }
		public string	Utac			{ get; set; }
		public string	PositionId		{ get; set; }
		public string	OrderId			{ get; set; }
	}
}
