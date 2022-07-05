using PXS.IfRF.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace PXS.IfRF.Data
{
    public static class ExecuteReaderExtension
    {
        public static DbDataReader ExecuteAndLogReader(this DbCommand command, ILoggerManager logger)
        {
            QueryCommandInterceptor.LogCommand(command, logger);
            DbDataReader reader = command.ExecuteReader();
            return reader;
        }
    }
}
