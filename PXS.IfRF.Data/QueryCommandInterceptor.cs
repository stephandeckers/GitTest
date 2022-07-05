using Microsoft.EntityFrameworkCore.Diagnostics;
using Oracle.ManagedDataAccess.Client;
using PXS.IfRF.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PXS.IfRF.Data
{
    /// <summary>
    /// Used to log the SQL statements
    /// </summary>
    public class QueryCommandInterceptor : DbCommandInterceptor
    {
        private readonly ILoggerManager _logger;

        public QueryCommandInterceptor(ILoggerManager logger)
        {
            _logger = logger;
        }
        public override InterceptionResult<DbDataReader> ReaderExecuting(
            DbCommand command,
            CommandEventData eventData,
            InterceptionResult<DbDataReader> result)
        {
            LogCommand(command);
            //ManipulateCommand(command);

            return result;
        }

        public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
            DbCommand command,
            CommandEventData eventData,
            InterceptionResult<DbDataReader> result,
            CancellationToken cancellationToken = default)
        {
            LogCommand(command);
            //ManipulateCommand(command);

            return new ValueTask<InterceptionResult<DbDataReader>>(result);
        }

        private void LogCommand(DbCommand command)
        {
            LogCommand(command, _logger);
        }

        public static void LogCommand(DbCommand command, ILoggerManager logger)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var aParam in command.Parameters)
            {
                sb.Append(((OracleParameter)aParam).ParameterName + ": " + ((OracleParameter)aParam).Value.ToString());
            }
            string valueToLog = string.Concat(
                "Query:",
                Environment.NewLine,
                command.CommandText,
                Environment.NewLine,
                "Parameters:",
                Environment.NewLine,
                sb.ToString());
            logger.Trace(valueToLog, "DB");
        }
    }
}
