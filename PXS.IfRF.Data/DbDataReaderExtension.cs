/**
 * @Name DbDataReaderExtension.cs
 * @Purpose 
 * @Date 23 February 2022, 09:19:07
 * @Author S.Deckers
 * @Description 
 */

namespace PXS.IfRF.Data
{
    #region -- Using directives --
	using System;
	using System.Data.Common;
    #endregion

	public static class DbDataReaderExtension
	{
        public static string ReadStringValue( this DbDataReader dataReader, string columnName)
        {           
            if( dataReader[ columnName] == DBNull.Value)
            { 
                return( string.Empty);
            }

            string v = System.Convert.ToString( dataReader[ columnName] );
            return( v);
        }

        public static long? ReadNullableLongValue( this DbDataReader dataReader, string columnName)
        {           
            if( dataReader[ columnName] == DBNull.Value)
            { 
                return( null);
            }

            long v = System.Convert.ToInt64( dataReader[ columnName] );
            return( v);
        }

        public static decimal? ReadNullableDecimalValue( this DbDataReader dataReader, string columnName)
        {           
            if( dataReader[ columnName] == DBNull.Value)
            { 
                return( null);
            }

            decimal v = System.Convert.ToDecimal( dataReader[ columnName] );
            return( v);
        }
	}
}