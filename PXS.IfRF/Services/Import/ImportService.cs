/**
 * @Name ImportService.cs
 * @Purpose 
 * @Date 31 January 2022, 11:38:24
 * @Author S.Deckers
 * @Description 
 */

namespace PXS.IfRF.Services
{
	#region -- Using directives --
	using System;
	using System.IO;
	using System.Text;
	using PXS.IfRF.Logging;	
	using System.Collections.Generic;
	using PXS.IfRF.BusinessLogic;
	using Microsoft.AspNet.OData;
	using PXS.IfRF.Data.Model;
	using System.Linq;
	using PXS.IfRF.Services.Import;

	using PXS.IfRF.Data;
	using Microsoft.AspNetCore.Http;

	using PXS.IfRF.Data.Model.Import;
	using PXS.IfRF.Services.Import.Extensions;

	#endregion

	public class ImportService : IImportService
	{
		private readonly ModelContext		_modelContext;
		private readonly IConnectionService _connectionService;
		private readonly ILoggerManager		_logger;
		private readonly ISharedMethods		_sharedMethods;

		public ImportService
		( 
			ModelContext		modelContext
		,	IConnectionService	connectionService
		,	ILoggerManager		logger
		,	ISharedMethods		sharedMethods
		)
		{
			this._modelContext		= modelContext;
			this._connectionService	= connectionService;
			this._logger			= logger;
			this._sharedMethods		= sharedMethods;
		}

		/// <summary>
		/// Imports the specified request.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <param name="file">The file.</param>
		/// <returns></returns>
		public ImportResponse Import
		( 
			ImportRequest	request
		,	IFormFile		file
		)
		{
			return( importFile( request, file));
		}

		/// <summary>
		/// Imports the file.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <param name="file">The file.</param>
		/// <returns></returns>
		private ImportResponse importFile
		( 
			ImportRequest	request
		,	IFormFile		file
		)
		{
			ImportResponse importResult	= new ImportResponse( );

			if( request.Owner != "JV1" && request.Owner != "JV2")
			{
				importResult.Messages.Add( string.Format( "'{0}' is not a supported JointVenture", request.Owner));
				return( importResult);
			}

			if (file == null)
			{
				importResult.Messages.Add( "inputfile is required" );
				return (importResult);
			}

			// --- Read files (20220310 SDE)

			List<GeneralCsvConnection> items = null;

			items = readConnections( request.Owner, file.OpenReadStream() );

			items.Import( );
			importResult.Messages = items.ConstructReport( );

			return( importResult);
		}

		/// <summary date="11-03-2022, 11:44:33" author="S.Deckers">
		/// Reads the connections.
		/// </summary>
		/// <param name="jv">The jv.</param>
		/// <param name="stream">The stream.</param>
		/// <returns></returns>
		/// <exception cref="System.NotImplementedException"></exception>
		private List<GeneralCsvConnection> readConnections( string jv, System.IO.Stream stream)
		{
			List<GeneralCsvConnection> items = new List<GeneralCsvConnection>( );

			using( StreamReader reader = new StreamReader( stream))
			{
				int currentLine = -1;

				while( reader.Peek() >= 0)
				{
					string theLine  = reader.ReadLine();

					currentLine++;
					if( currentLine == 0)
					{
						continue;
					}

					if( string.IsNullOrEmpty( theLine))
					{
						continue;
					}

					if( theLine[ 0] == '#')
					{
						continue;
					}

					if( jv == "JV1") 
					{ 
						FiberKlaarCsvConnection item = new FiberKlaarCsvConnection( currentLine, theLine, this._modelContext, this._connectionService, this._logger, this._sharedMethods);
						items.Add( item);
					};

					if( jv == "JV2") 
					{ 
						UniFiberCsvConnection item = new UniFiberCsvConnection( currentLine, theLine, this._modelContext, this._connectionService, this._logger, this._sharedMethods);
						items.Add( item);
					};
				}
			}
			return( items);
		}
	}
}