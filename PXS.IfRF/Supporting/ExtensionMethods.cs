/**
 * @Name ExtensionMethods.cs
 * @Purpose 
 * @Date 29 June 2022, 12:42:23
 * @Author S.Deckers
 * @Description 
 */

namespace PXS.IfRF.Supporting
{
	#region -- Using directives --
	using System.Collections;
	using System.Collections.Generic;
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Http;
	using Microsoft.EntityFrameworkCore.Diagnostics;
	using Microsoft.Extensions.DependencyInjection;
	using d=System.Diagnostics.Debug;
	#endregion

	public static class ExtensionMethods
	{
		public static void Dump(this HttpContext context)
		{
			d.WriteLine( $"Dumping '{context.ToString()}'");

			if( context.Request == null)	 d.WriteLine( $" null Request");

			if( context?.Request?.Headers == null)	 d.WriteLine( $" null Request.Headers");

			if( context?.Request?.Headers?.Count == null)	 d.WriteLine( $" {context?.Request?.Headers?.Count} Request.Headers");
		}

		public static void Dump(this Dictionary<string, string> dictionary, string name)
		{
			d.WriteLine( $"Dumping '{name}'");
			foreach( KeyValuePair<string, string> item in dictionary)
			{
				d.WriteLine( $"key={item.Key},value={item.Value}");
			}
		}

		internal static string getUsername( this Microsoft.AspNetCore.Http.IHeaderDictionary headers)
		{
			string user = headers[ "iv-user"].ToString();

			if( string.IsNullOrEmpty( user))
			{
				return( "-");
			}

			return( user);
		}

		public static void Dump( this Microsoft.AspNetCore.Http.IHeaderDictionary headers)
		{
			d.WriteLine( $"{headers.Count} headers");

			foreach(var item in headers)
			{
				d.WriteLine( $"{item.Key}={item.Value}");
			}
		}
	}
}
