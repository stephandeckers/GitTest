/**
 * @Name InfoService.cs
 * @Purpose 
 * @Date 01 August 2021, 10:26:13
 * @Author S.Deckers
 * @Description 
 */

namespace PXS.IfRF.Services
{
	#region -- Using directives --
	using System;
	using System.Linq;
	using System.Data.Common;
	using System.Collections.Generic;
	using Microsoft.Extensions.Configuration;
	using Microsoft.EntityFrameworkCore;
	using PXS.IfRF.Data.Model;
	using PXS.IfRF.Logging;
    using PXS.IfRF.Data;
    using Microsoft.AspNetCore.Http;
    using PXS.IfRF.Supporting;
    #endregion

    public class InfoService : IInfoService
    {
        private readonly ModelContext   _ifrfContext;
        private readonly ILoggerManager _logger;
		private readonly HttpContext	_httpContext;
		private readonly IConfiguration	_configuration;

        public InfoService
		(
			ModelContext			ifrfContext
		,	ILoggerManager			logger
		,	IHttpContextAccessor	httpContextAccessor
		,	IConfiguration			configuration
		)
        {
            _ifrfContext	= ifrfContext;
			_logger         = logger;
			_httpContext	= httpContextAccessor.HttpContext;
			_configuration	= configuration;
        }

		public InfoResponse Get ()
        {
			return GetBuildVersionAndDate();
        }

		public List<string> GetGroups( string role)
		{
			return( getDefinedGroups( role));
		}

		private List<string> getDefinedGroups( string theRole)
		{
			IDictionary<string, IList<string>> roleGroupMappings = new Dictionary<string, IList<string>>();

            var rgMaps = _configuration.GetSection("RoleGroupMappings").GetChildren();
            foreach(IConfigurationSection anRgMap in rgMaps)
            {
                var children = anRgMap.GetChildren();
                string role = children.Single(c => c.Key.Equals("Role")).Value;
                var groups = children.Single(c => c.Key.Equals("Groups")).GetChildren().Select(gr => gr.Value).ToList();
				roleGroupMappings.Add(role, groups);
            }

			var item = roleGroupMappings.Where( x => x.Key == theRole);
			if( item.FirstOrDefault().Key == null)
			{
				return( new List<string>());
			}
			List<string> items = item.FirstOrDefault().Value.ToList();
			return( items);
		}

		/// <summary>
		/// Gets the build version and date.
		/// </summary>
		/// <returns></returns>
		private InfoResponse GetBuildVersionAndDate()
		{
			string		assemblyVersion	= GetType().Assembly.GetName().Version.ToString();
			string		assemblyPath	= GetType().Assembly.Location;
			DateTime	buildDate		= System.IO.File.GetLastWriteTime(assemblyPath);

			//DateTime lastDdlTime;
			//string dbVersion;
			//(lastDdlTime, dbVersion) = GetDbInfo();

			IDictionary<string, string> loglevels = _logger.GetLogLevels();

			InfoResponse version = new InfoResponse()
			{
				AssemblyVersion		= assemblyVersion
			,	BuildDate			= buildDate.ToString("dd/MM/yyyy HH:mm:ss")
			//,	LastDdlTime			= lastDdlTime.ToString("dd/MM/yyyy HH:mm:ss")
			//,	DbVersion			= dbVersion
			,	ConsoleLogLevel		= loglevels["consoleJson"]
			,	FileLogLevel		= loglevels["logfile"]
			,	UserId				= _httpContext.Request.Headers.getUsername()
			,	UserIvGroups		= _httpContext.Request.Headers[ "iv-groups"]
			};
			return version;
		}

		private (DateTime, string) GetDbInfo()
		{
			using var command = _ifrfContext.GetOpenConnection().CreateCommand();

			command.CommandText =
				@"SELECT 
				(select max(last_ddl_time) from all_objects where owner = 'IFRF_SCHEMA') AS LAST_DDL_TIME, 
				(SELECT GLOBAL_NAME FROM global_name) as DB_NAME
				FROM dual";

			//_ifrfContext.Database.OpenConnection();
			DateTime lastDdlTime = DateTime.Now;
			string dbName = string.Empty;
			using (DbDataReader result = command.ExecuteAndLogReader(_logger))
			{
				result.Read();
				lastDdlTime = Convert.ToDateTime(result["LAST_DDL_TIME"]);
				dbName = Convert.ToString(result["DB_NAME"]);
			}

			return (lastDdlTime, dbName);
		}
    }
}