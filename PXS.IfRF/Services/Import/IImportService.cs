/**
 * @Name IImportService.cs
 * @Purpose 
 * @Date 11 March 2022, 12:09:20
 * @Author S.Deckers
 * @Description 
 */

namespace PXS.IfRF.Services
{
	#region -- Using directives --
	using PXS.IfRF.Data.Model.Import;
	using Microsoft.AspNetCore.Http;
	#endregion

	public interface IImportService
	{
		ImportResponse	Import	( ImportRequest	request, IFormFile file);
	}
}

