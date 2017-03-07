using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMagnetApp
{
    public class Logger
    {
        private readonly ILog log4NetAdapter;

        public Logger(Type type)
        {
            this.log4NetAdapter = LogManager.GetLogger(type);
        }

        #region Public Methods Decleration

        /// <summary>
        /// Debug Function having single message argumanet
        /// </summary>
        ///

        public void LogDebug(string message)
        {
            this.log4NetAdapter.Debug(message);
        }

        /// <summary>
        /// Debugg Function having message and exception type argumanet
        /// </summary>
        ///

        public void LogDebug(string message, Exception exception)
        {
            this.log4NetAdapter.Debug(message, exception);
        }

        /// <summary>
        /// Logg Error Function having single message argumanet
        /// </summary>
        ///

        public void LogError(string message)
        {
            this.log4NetAdapter.Error(message);
        }

        /// <summary>
        /// Log Error Function having message and exception type argumanet
        /// </summary>
        ///

        public void LogError(string message, Exception exception)
        {
            this.log4NetAdapter.Error(message, exception);
        }

        /// <summary>
        /// Fatal Function having single message argumanet
        /// </summary>
        ///

        public void LogFatal(string message)
        {
            this.log4NetAdapter.Fatal(message);
        }

        /// <summary>
        /// Fatal Function having message and exception type argumanet
        /// </summary>
        ///

        public void LogFatal(string message, Exception exception)
        {
            this.log4NetAdapter.Fatal(message, exception);
        }

        /// <summary>
        /// LogInfo Function having single message argumanet
        /// </summary>
        ///

        public void LogInfo(string message)
        {
            this.log4NetAdapter.Info(message);
        }

        /// <summary>
        /// Informative Function having message and exception type argumanet
        /// </summary>
        ///

        public void LogInfo(string message, Exception exception)
        {
            this.log4NetAdapter.Info(message, exception);
        }

        /// <summary>
        /// Warning Function having single message argumanet
        /// </summary>
        ///

        public void LogWarning(string message)
        {
            this.log4NetAdapter.Warn(message);
        }

        /// <summary>
        /// Warning Function having message and exception type argumanet
        /// </summary>
        ///

        public void LogWarning(string message, Exception exception)
        {
            this.log4NetAdapter.Warn(message, exception);
        }
        #endregion
    }
}

