/**
 * @Name ImportExtensions.cs
 * @Purpose 
 * @Date 11 March 2022, 10:36:12
 * @Author S.Deckers
 * @Description 
 */

namespace PXS.IfRF.Services.Import.Extensions
{
    #region -- Using directives --
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using System.Data.Common;
	using PXS.IfRF.Services.Import;
    #endregion

	public static class ImportExtensions
	{
		public static void Import ( this List<GeneralCsvConnection> items)
		{
			foreach( GeneralCsvConnection item in items)
			{
				item.Import( );
			}
		}

		public static List<string> ConstructReport ( this List<GeneralCsvConnection> items)
		{
			List<string> theReport = new List<string>( );

			int succesCount = items.Where( x => x.IsValid == true).Count( );
			int errrorCount = items.Where( x => x.IsValid == false).Count( );
			int totalCount	= items.Count( );

			string firstValidPop = items.Where( x => x.IsValid == true)?.FirstOrDefault()?.FromPopName;

			theReport.Add( string.Format( "{0}/{1} connections have been successfully imported, first valid pop [{2}]", succesCount, totalCount, firstValidPop));

			// --- Add invalid items (20220311 SDE)

			foreach( GeneralCsvConnection item in items)
			{
				if( item.IsValid)
				{
					continue;
				}

				foreach( string theReason in item.InvalidReasons)
				{
					theReport.Add( string.Format( "Row {0}, {1}", item.LineNr + 1,theReason));
				}
			}

			return( theReport);
		}
	}
}