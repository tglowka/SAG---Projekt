using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAgentBookingSystem.Logger
{
    /// <summary>
    ///     Class that allows to enable logging.
    /// </summary>
    public static class LoggingConfiguration
    {
        /// <summary>
        ///     Setup serilog logger to send logs to seq server.
        /// </summary>
        public static void SetupLogger()
        {
            string seqServerAddress = ConfigurationManager.AppSettings["seqserveraddress"];

            var logger = new Serilog.LoggerConfiguration()
                .WriteTo.Seq($"http://{seqServerAddress}")
                .MinimumLevel.Debug()
                .CreateLogger();

            Log.Logger = logger;
        }
    }
}
