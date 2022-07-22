using Microsoft.AspNetCore.Http;
using NLog;
using NLog.Config;
using NLog.Fluent;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PXS.IfRF.Logging
{
    public class LoggerManager : ILoggerManager
    {
        const string LOGGERNAME = "CustomLogger";

        private readonly ILogger logger;

        // Use this methdod only for unit testing
        public LoggerManager() : this(fileLogLevel: "Info", consoleLogLevel: "Debug")
        { }

        public LoggerManager(string fileLogLevel, string consoleLogLevel)
        {
            var baseFolder = AppDomain.CurrentDomain.BaseDirectory;
            LogManager.LoadConfiguration(string.Concat(baseFolder, "/nlog.config"));
            LogManager.Configuration.Variables["FileLogLevel"] = fileLogLevel;
            LogManager.Configuration.Variables["ConsoleLogLevel"] = consoleLogLevel;

            logger = LogManager.GetCurrentClassLogger();
        }
        public bool IsTraceEnabled { get => logger.IsTraceEnabled; }
        public bool IsDebugEnabled { get => logger.IsDebugEnabled; }
        public bool IsInfoEnabled { get => logger.IsInfoEnabled; }
        public bool IsWarnEnabled { get => logger.IsWarnEnabled; }
        public bool IsErrorEnabled { get => logger.IsErrorEnabled; }

        public IDictionary<string,string> GetLogLevels()
        {
            IDictionary<string, string> loglevels = new Dictionary<string, string>();

            foreach (LoggingRule loggingRule in LogManager.Configuration.LoggingRules)
            {
                loglevels.Add(loggingRule.Targets[0].Name, LogLevel.FromOrdinal(loggingRule.Levels.Min(l => l.Ordinal)).Name);
            }
            return loglevels;
        }

        public void Trace(string message, string context = "")
        {
            LogWrapped(LogLevel.Trace, message, context);
        }

        public void Debug(string message, string context = "")
        {
            LogWrapped(LogLevel.Debug, message, context);
        }

        public void Info(string message, string context="")
        {
            LogWrapped(LogLevel.Info, message, context);
        }

        public void Warn(string message, string context = "")
        {
            LogWrapped(LogLevel.Warn, message, context);
        }

        public void Error(string message, string context = "")
        {
            LogWrapped(LogLevel.Error, message, context);
        }

        private void LogWrapped(LogLevel logLevel, string message, string context = "")
        {
            LogEventInfo li = new LogEventInfo(logLevel, logger.Name, message);
            li.Properties.Add("Context", context);

            logger.Log(typeof(LoggerManager), li);
        }

        public void LogWebRequest(
            Type wrapperType, 
            HttpContext httpContext, 
            bool hasResponse, 
            long? responseTimeMs = null)
        {
            LogEventInfo li = new LogEventInfo(LogLevel.Info, logger.Name, "");
            li.Properties.Add("Context", "APIRequest");
            li.Properties.Add("RemoteIPAddress", httpContext.Connection.RemoteIpAddress.ToString());

            li.Properties.Add("RequestMethod", httpContext.Request.Method.ToString());
            li.Properties.Add("RequestUri", httpContext.Request.Path + httpContext.Request.QueryString);

            string theUser = httpContext.Request.Headers[ "iv-user"].ToString();

            if(string.IsNullOrEmpty( theUser))
            {
                theUser = "-";
            }
            //li.Properties.Add("UserId", httpContext.Items["iv-user"]);
            li.Properties.Add("UserId", theUser);
            string message;
            if (hasResponse)
            {
                message = "Request";
                li.Properties.Add("ResponseCode", httpContext.Response.StatusCode.ToString());
                li.Properties.Add("ResponseTimeMs", responseTimeMs.ToString());
            }
            else
            {
                message = "Response";
            }
            li.Message = message;

            logger.Log(wrapperType, li);
        }
    }
}
