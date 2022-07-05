/**
 * @Name ISettingsService.cs
 * @Purpose 
 * @Date 30 September 2021, 17:28:51
 * @Author S.Deckers
 * @Description 
 */

namespace PXS.IfRF.Services
{
	#region -- Using directives --
	using PXS.IfRF.Enum;
	#endregion

	public interface ISettingsService
	{
		public string	GetSeqNrPrefix	( SequenceNumbers item);
	}
}