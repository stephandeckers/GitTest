/**
 * @Name ImportRequest.cs
 * @Purpose 
 * @Date 01 February 2022, 07:41:35
 * @Author S.Deckers
 * @Description 
 */

namespace PXS.IfRF.Data.Model.Import
{
	#region -- Using directives
	using System.ComponentModel;
	using System.ComponentModel.DataAnnotations;
	#endregion

	public class ImportRequest
	{
		[ Required( )]
		public string Owner		{ get; set; }
	}
}
