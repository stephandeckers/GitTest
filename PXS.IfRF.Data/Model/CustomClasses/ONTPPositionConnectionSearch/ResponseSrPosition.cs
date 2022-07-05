/**
 * @Name ResponseSrPosition.cs
 * @Purpose 
 * @Date 30 September 2021, 13:20:04
 * @Author S.Deckers
 * @Description 
 */

namespace PXS.IfRF.Data.Model.ONTPPositionConnectionSearch
{
	public class SubrackInfo
	{
		public long?	Id						{ get; set; }
		public string	SubrackId				{ get; set; }
		public string	FrameId					{ get; set; }
		public string	Name					{ get; set; }
		public string	SubrackType				{ get; set; }
	}

	public class ResponseSrPosition : SrPosition
	{
		public string		OperationalStatus	{ get; set; }
		public string		LifecycleStatus		{ get; set; }
		public long?		InternalOrderId		{ get; set; }
		public string		OrderId				{ get; set; }
		public SubrackInfo	SubrackInfo			{ get; set; }
	}
}