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
        /// <param name="loggingAdapter">Logging adapter</param>
        /// <param name="actorType">Actor type</param>
        /// <param name="actorPath">Actor path</param>
        public void LogActorCreation(ILoggingAdapter loggingAdapter, Type actorType, ActorPath actorPath)
        {
            loggingAdapter.Info("ACTOR TYPE: {ActorType}, ACTOR PATH: {ActorPath} has been created", actorType.Name, actorPath);
        }

        /// <summary>
        ///     Log PreStart event for the actor.
        /// </summary>
        /// <param name="loggingAdapter">Logging adapter</param>
        /// <param name="actorPath">Actor path</param>
        public void LogActorPreStart(ILoggingAdapter loggingAdapter, ActorPath actorPath)
        {
            loggingAdapter.Debug("Prestart: {ActorPath}", actorPath);
        }

        /// <summary>
        ///     Log PostStop event for the actor.
        /// </summary>
        /// <param name="loggingAdapter">Logging adapter</param>
        /// <param name="actorPath">Actor path</param>
        public void LogActorPostStop(ILoggingAdapter loggingAdapter, ActorPath actorPath)
        {
            loggingAdapter.Debug("PostStop: {ActorPath}", actorPath);
        }

        /// <summary>
        ///     Log PreRestart event for the actor.
        /// </summary>
        /// <param name="loggingAdapter">Logging adapter</param>
        /// <param name="actorPath">Actor path</param>
        /// <param name="exception">Exception</param>
        public void LogActorPreRestart(ILoggingAdapter loggingAdapter, ActorPath actorPath, Exception exception)
        {
            loggingAdapter.Debug("PreRestart: {ActorPath}. Exception: {Exception}", actorPath, exception);
        }

        /// <summary>
        ///     Log PostRestart event for the actor.
        /// </summary>
        /// <param name="loggingAdapter">Logging adapter</param>
        /// <param name="actorPath">Actor path</param>
        /// <param name="exception">Exception</param>
        public void LogActorPostRestart(ILoggingAdapter loggingAdapter, ActorPath actorPath, Exception exception)
        {
            loggingAdapter.Debug("PostRestart: {ActorPath}. Exception: {Exception}", actorPath, exception);
        }

        /// <summary>
        ///     Log custom error message.
        /// </summary>
        /// <param name="loggingAdapter">Logging adapter</param>
        /// <param name="actorType">Actor type</param>
        /// <param name="actorPath">Actor path</param>
        /// <param name="errorMessage">Error message</param>
        public void LogCutomError(ILoggingAdapter loggingAdapter, Type actorType, ActorPath actorPath, string errorMessage)
        {
            loggingAdapter.Error("ACTOR TYPE: {ActorType}, ACTOR PATH: {ActorPath}, Error: {ErrorMessage}", actorType.Name, actorPath, errorMessage);
        }

        /// <summary>
        ///     Log custom warning message.
        /// </summary>
        /// <param name="loggingAdapter">Logging adapter</param>
        /// <param name="actorType">Actor type</param>
        /// <param name="actorPath">Actor path</param>
        /// <param name="warningMessage">Warning message</param>
        public void LogCutomWarning(ILoggingAdapter loggingAdapter, Type actorType, ActorPath actorPath, string warningMessage)
        {
            loggingAdapter.Warning("ACTOR TYPE: {ActorType}, ACTOR PATH: {ActorPath}, Warning: {WarningMessage}", actorType.Name, actorPath, warningMessage);
        }

        /// <summary>
        ///     Log custom info message.
        /// </summary>
        /// <param name="loggingAdapter">Logging adapter</param>
        /// <param name="actorType">Actor type</param>
        /// <param name="actorPath">Actor path</param>
        /// <param name="infoMessage">Info message</param>
        public void LogCutomInfo(ILoggingAdapter loggingAdapter, Type actorType, ActorPath actorPath, string infoMessage)
        {
            loggingAdapter.Info("ACTOR TYPE: {ActorType}, ACTOR PATH: {ActorPath}, Info: {InfoMessage}", actorType.Name, actorPath, infoMessage);
        }

        /// <summary>
        ///     Log custom debug message.
        /// </summary>
        /// <param name="loggingAdapter">Logging adapter</param>
        /// <param name="actorType">Actor type</param>
        /// <param name="actorPath">Actor path</param>
        /// <param name="debugMessage">Debug message</param>
        public void LogCutomDebug(ILoggingAdapter loggingAdapter, Type actorType, ActorPath actorPath, string debugMessage)
        {
            loggingAdapter.Info("ACTOR TYPE: {ActorType}, ACTOR PATH: {ActorPath}, Info: {DebugMessage}", actorType.Name, actorPath, debugMessage);
        }
    }
}
