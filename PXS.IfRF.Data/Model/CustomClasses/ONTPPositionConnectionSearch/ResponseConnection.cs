/**
 * @Name ResponseConnection.cs
 * @Purpose 
 * @Date 30 September 2021, 13:19:47
 * @Author S.Deckers
 * @Description 
 */

namespace PXS.IfRF.Data.Model.ONTPPositionConnectionSearch
{
	public class ResponseConnection : Connection
	{
		public string	PositionName		{ get; set; }
		public string	LifecycleStatus		{ get; set; }

		public string	OperationalStatus	{ get; set; }
		public long?	InternalOrderId		{ get; set; }
		public string	OrderId				{ get; set; }
	}
}