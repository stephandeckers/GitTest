/**
 * @Name ImportController.cs
 * @Purpose 
 * @Date 18 January 2022, 11:54:31
 * @Author S.Deckers
 * @Description 
 */

namespace PXS.IfRF.Controllers
{
	#region -- Using directives --
	using System;
	using System.Text;
	using System.IO;
	using System.Collections.Generic;
	using Microsoft.AspNetCore.Http;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Authorization;
	using Swashbuckle.AspNetCore.Annotations;
	using PXS.IfRF.Services;
	using PXS.IfRF.Data.Model;
	using PXS.IfRF.Logging;
	using PXS.IfRF.Data.Model.Import;
	using d = System.Diagnostics.Debug;
	#endregion

	public partial class ImportController : IfRFBaseController
    {
        private const string swaggerControllerDescription = "Import";

		private readonly IImportService _importService;

		public ImportController
		(
			ILoggerManager	logger
		,	IImportService	importService
		,	IRefdataService refdataService
		)
			: base( logger, refdataService )
		{ 
			this._importService = importService;
		}

		[ HttpPost			( )]
		[ Authorize			( AuthenticationSchemes = AUTHSCHEME, Policy = EDITORROLE_lvl_2)]
		[ SwaggerOperation	( Tags = new[] { swaggerControllerDescription }, Summary = "Import connections" )]
		public FileContentResult Import
		( 
			[ FromForm()] 
			ImportRequest	request
		,	IFormFile		file
		)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			ImportResponse res = _importService.Import( request, file);

			// --- Write issues to file and return (20220309 SDE)

			string		theFile = writeResultToFile( res.Messages);
			FileStream	stream	= System.IO.File.OpenRead( theFile);

			MemoryStream ms = new MemoryStream( );
			stream.CopyTo( ms);
			var bytes = ms.ToArray( );
			ms.Dispose( );

			string importFile = file.FileName;

			DateTime theDate = System.DateTime.Now;
			string fname = $"Import_{importFile}_{theDate.Day:00}{theDate.Month:00}{theDate.Year:00}  {theDate.Hour:00}{theDate.Minute:00}{theDate.Second:00}.txt";

			string mimeType = "application/octet-stream";
			return new FileContentResult(bytes, mimeType)
			{
				FileDownloadName=fname
			};
		}

		/// <summary date="09-03-2022, 13:55:56" author="S.Deckers">
		/// Writes the result to file.
		/// </summary>
		/// <param name="issues">The issues.</param>
		/// <returns></returns>
		private string writeResultToFile( List<string> issues )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty ) );

			string theFile = Path.GetTempFileName( );

			StringBuilder sb = new StringBuilder( );

			foreach( string issue in issues)
			{
				if( sb.Length == 0)
				{
					sb.AppendFormat( "{0}", issue);
					continue;
				}
				sb.AppendLine( );
				sb.AppendFormat( "{0}", issue);
			}

			string theErrors = sb.ToString( );
			System.IO.File.WriteAllText( theFile, theErrors);

			return (theFile);
		}
	}
}