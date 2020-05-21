using Akka.Actor;
using Akka.Event;
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
    ///     Singleton class that allows to enable logging.
    /// </summary>
    public sealed class LoggingConfiguration
    {
        private static readonly Lazy<LoggingConfiguration> instance = new Lazy<LoggingConfiguration>(() => new LoggingConfiguration());

        private LoggingConfiguration() { }

        public static LoggingConfiguration Instance
        {
            get
            {
                return instance.Value;
            }

        }
        /// <summary>
        ///     Setup serilog logger to send logs to seq server.
        /// </summary>
        public void SetupLogger()
        {
            string seqServerAddress = ConfigurationManager.AppSettings["seqserveraddress"];

            var logger = new Serilog.LoggerConfiguration()
                .WriteTo.Seq($"http://{seqServerAddress}")
                .MinimumLevel.Debug()
                .CreateLogger();

            Log.Logger = logger;
        }

        /// <summary>
        ///     Create actor creation info log.
        /// </summary>
        /// <param name="loggingAdapter">Logging adapter.</param>
        /// <param name="actorType">Actor type.</param>
        /// <param name="actorPath">Actor path.</param>
        public void LogActorCreation(ILoggingAdapter loggingAdapter, Type actorType, ActorPath actorPath)
        {
            loggingAdapter.Info("{ActorType} {ActorPath} has been created", actorType.Name, actorPath);
        }

        public void LogActorPreStart(ILoggingAdapter loggingAdapter, ActorPath actorPath)
        {
            loggingAdapter.Debug("Prestart: {ActorPath}", actorPath);
        }

        public void LogActorPostStop(ILoggingAdapter loggingAdapter, ActorPath actorPath)
        {
            loggingAdapter.Debug("PostStop: {ActorPath}", actorPath);
        }

        public void LogActorPreRestart(ILoggingAdapter loggingAdapter, ActorPath actorPath, Exception exception)
        {
            loggingAdapter.Debug("PreRestart: {ActorPath}. Exception: {Exception}", actorPath, exception);
        }

        public void LogActorPostRestart(ILoggingAdapter loggingAdapter, ActorPath actorPath, Exception exception)
        {
            loggingAdapter.Debug("PostRestart: {ActorPath}. Exception: {Exception}", actorPath, exception);
        }
    }
}
