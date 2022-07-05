/**
 * @Name SettingsService.cs
 * @Purpose 
 * @Date 30 September 2021, 17:32:46
 * @Author S.Deckers
 * @Description 
 */

namespace PXS.IfRF.Services
{
	#region -- Using directives --
	using System;
	using System.Data;
	using System.Data.Common;
	using System.Text;
	using System.Collections.Generic;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.AspNet.OData;
	using Oracle.ManagedDataAccess.Client;
	using PXS.IfRF.Data.Model;
	using System.Linq;
	using PXS.IfRF.Logging;
	using d=System.Diagnostics.Debug;
    using PXS.IfRF.Data;
    using PXS.IfRF.Supporting;
	using PXS.IfRF.Enum;
	#endregion

	public class SettingsService : ISettingsService
	{
		public string GetSeqNrPrefix(SequenceNumbers item )
		{
			switch( item)
			{
				case( SequenceNumbers.Connection):
				{
					return( "CN");
				}

				case( SequenceNumbers.Position):
				{
					return( "P");
				}

				case( SequenceNumbers.Subrack):
				{
					return( "SR");
				}

				default:
				{
					throw new System.NotSupportedException( $"{ item } is not a supported value");
				}
			}
		}
	}
}