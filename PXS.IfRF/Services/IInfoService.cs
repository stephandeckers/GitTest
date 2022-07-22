/**
 * @Name IInfoService.cs
 * @Purpose 
 * @Date 01 August 2021, 10:22:54
 * @Author S.Deckers
 * @Description 
 */

namespace PXS.IfRF.Services
{
	#region -- Using directives --
	using System;
	using System.Collections.Generic;
    using PXS.IfRF.Data.Model;
	#endregion

    public interface IInfoService
    {
		InfoResponse Get ();
		List<string> GetGroups( string role);
    }
}

