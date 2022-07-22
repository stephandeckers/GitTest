using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PXS.IfRF.Logging
{
    public interface ILoggerManager
    {
        void Info(string message, string context="");
        void Warn(string message, string context = "");
        void Debug(string message, string context = "");
        void Error(string message, string context = "");
        void Trace(string message, string context = "");
        void LogWebRequest(Type wrapperType, HttpContext httpContext, bool hasResponse, long? responseTimeMs = null);

        public IDictionary<string, string> GetLogLevels();

        public bool IsTraceEnabled { get; }
        public bool IsDebugEnabled { get; }
        public bool IsInfoEnabled { get; }
        public bool IsWarnEnabled { get; }
        public bool IsErrorEnabled { get; }

    }
}
