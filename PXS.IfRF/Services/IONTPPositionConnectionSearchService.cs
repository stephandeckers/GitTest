/**
 * @Name IONTPPositionConnectionSearchService.cs
 * @Purpose 
 * @Date 28 September 2021, 07:09:05
 * @Author S.Deckers
 * @Description 
 */

namespace PXS.IfRF.Services
{
	#region -- Using directives --
	using System;
	using PXS.IfRF.Data.Model;
	using PXS.IfRF.Data.Model.ONTPPositionConnectionSearch;
	using System.Collections.Generic;
	#endregion

	public interface IONTPPositionConnectionSearchService
	{
		(List<ONTPPositionConnectionSearchResponse>, string) Execute (ONTPPositionConnectionSearchRequest request);
	}
}