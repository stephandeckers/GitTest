/**
 * @Name DataDemo.cs
 * @Purpose DataDemo
 * @Since 28 July 2003
 * @Author S.Deckers
 * @Modified
 * o 2 February 2005, Changed to DataDump
 * o 25 maart 2005, renamed main to something else so andere klassen kunnen eenvoudig gebruik
 *   maken van dumping door deze class op te nemen in het project 'Whoua'
 */

namespace PXS.IfRF
{
	#region -- Using directives --
	using System;
	using System.Text;
	using System.Data;
	using System.Data.Common;
	using d = System.Diagnostics.Debug;
	#endregion

	/// <summary>
	/// Een datadumper
	/// </summary>
	public class DataDumper
	{
		/// <summary>
		/// Use this to keep a readable DataReader
		/// </summary>
		/// <param name="command">The command.</param>
		public static void Dump( System.Data.Common.DbCommand command )
		{
			DbDataReader reader	= command.ExecuteReader( );
			Dump( reader);
		}

		private static void Dump( System.Data.Common.DbDataReader r)
		{
			d.WriteLine( string.Format( "DUMPING DbDataReader"));

			int rowCount = 1;

			while( r.Read())
			{
				StringBuilder sb = new StringBuilder( );

				sb.AppendFormat( "{0}",		rowCount++);

				for( int i = 0; i< r.FieldCount; i++)
				{
					string fieldName	= r.GetName( i).ToString( );
					string theValue		= string.Empty;
										
					if( r[ i] == DBNull.Value)
					{ 
						theValue = "null";
					}
					else
					{
						theValue = r[ i].ToString( );
					}
					sb.AppendFormat( ":{0}={1}",	fieldName, theValue);					
				}

				d.WriteLine( sb.ToString( ));
			}
		}
	}
}