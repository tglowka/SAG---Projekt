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
        private enum CustomTypes
        {
            Creation,
            Stop
        };

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
            //loggingAdapter.Info("ACTOR TYPE: {ActorType}, ACTOR PATH: {ActorPath}, EVENT: {CustomEvent} ", actorType.Name, actorPath.ToStringWithoutAddress(), CustomTypes.Creation.ToString("g"));
        }

        /// <summary>
        ///     Create actor stop info log.
        /// </summary>
        /// <param name="loggingAdapter">Logging adapter</param>
        /// <param name="actorType">Actor type</param>
        /// <param name="actorPath">Actor path</param>
        public void LogActorStop(ILoggingAdapter loggingAdapter, Type actorType, ActorPath actorPath)
        {
            //loggingAdapter.Info("ACTOR TYPE: {ActorType}, ACTOR PATH: {ActorPath}, EVENT: {CustomEvent} ", actorType.Name, actorPath.ToStringWithoutAddress(), CustomTypes.Stop.ToString("g"));
        }

        /// <summary>
        ///     Create active actor count info log.
        /// </summary>
        /// <param name="loggingAdapter">Logging adapter</param>
        /// <param name="actorType">Actor type</param>
        /// <param name="actorPath">Actor path</param>
        /// <param name="childActorCount">Active child actor count</param>
        public void LogActiveActorCount(ILoggingAdapter loggingAdapter, Type actorType, ActorPath actorPath, int childActorCount)
        {
            loggingAdapter.Info("ACTOR TYPE: {ActorType}, ACTOR PATH: {ActorPath},  CHILD ACTOR COUNT: {ChildActorCount} ", actorType.Name, actorPath.ToStringWithoutAddress(), childActorCount);
        }

        /// <summary>
        ///     Create all actors count info log.
        /// </summary>
        /// <param name="loggingAdapter">Logging adapter</param>
        /// <param name="actorType">Actor type</param>
        /// <param name="actorPath">Actor path</param>
        /// <param name="allChildActorCount">All child actor count</param>
        public void LogAllActorCount(ILoggingAdapter loggingAdapter, Type actorType, ActorPath actorPath, int allChildActorCount)
        {
            loggingAdapter.Info("ACTOR TYPE: {ActorType}, ACTOR PATH: {ActorPath},  ALL CHILD ACTOR COUNT: {AllChildActorCount}", actorType.Name, actorPath.ToStringWithoutAddress(), allChildActorCount);
        }

        /// <summary>
        ///     Log PreStart event for the actor.
        /// </summary>
        /// <param name="loggingAdapter">Logging adapter</param>
        /// <param name="actorPath">Actor path</param>
        public void LogActorPreStart(ILoggingAdapter loggingAdapter, ActorPath actorPath)
        {
            //    loggingAdapter.Debug("Prestart: {ActorPath}", actorPath.ToStringWithoutAddress());
        }

        /// <summary>
        ///     Log PostStop event for the actor.
        /// </summary>
        /// <param name="loggingAdapter">Logging adapter</param>
        /// <param name="actorPath">Actor path</param>
        public void LogActorPostStop(ILoggingAdapter loggingAdapter, ActorPath actorPath)
        {
            //      loggingAdapter.Debug("PostStop: {ActorPath}", actorPath.ToStringWithoutAddress());
        }

        /// <summary>
        ///     Log PreRestart event for the actor.
        /// </summary>
        /// <param name="loggingAdapter">Logging adapter</param>
        /// <param name="actorPath">Actor path</param>
        /// <param name="exception">Exception</param>
        public void LogActorPreRestart(ILoggingAdapter loggingAdapter, ActorPath actorPath, Exception exception)
        {
            //loggingAdapter.Debug("PreRestart: {ActorPath}. Exception: {Exception}", actorPath.ToStringWithoutAddress(), exception);
        }

        /// <summary>
        ///     Log PostRestart event for the actor.
        /// </summary>
        /// <param name="loggingAdapter">Logging adapter</param>
        /// <param name="actorPath">Actor path</param>
        /// <param name="exception">Exception</param>
        public void LogActorPostRestart(ILoggingAdapter loggingAdapter, ActorPath actorPath, Exception exception)
        {
            //  loggingAdapter.Debug("PostRestart: {ActorPath}. Exception: {Exception}", actorPath.ToStringWithoutAddress(), exception);
        }

        /// <summary>
        ///     Log custom error message.
        /// </summary>
        /// <param name="loggingAdapter">Logging adapter</param>
        /// <param name="actorType">Actor type</param>
        /// <param name="actorPath">Actor path</param>
        /// <param name="errorMessage">Error message</param>
        public void LogCustomError(ILoggingAdapter loggingAdapter, Type actorType, ActorPath actorPath, string errorMessage)
        {
            loggingAdapter.Error("ACTOR TYPE: {ActorType}, ACTOR PATH: {ActorPath}, Error: {ErrorMessage}", actorType.Name, actorPath.ToStringWithoutAddress(), errorMessage);
        }

        /// <summary>
        ///     Log custom warning message.
        /// </summary>
        /// <param name="loggingAdapter">Logging adapter</param>
        /// <param name="actorType">Actor type</param>
        /// <param name="actorPath">Actor path</param>
        /// <param name="warningMessage">Warning message</param>
        public void LogCustomWarning(ILoggingAdapter loggingAdapter, Type actorType, ActorPath actorPath, string warningMessage)
        {
            loggingAdapter.Warning("ACTOR TYPE: {ActorType}, ACTOR PATH: {ActorPath}, Warning: {WarningMessage}", actorType.Name, actorPath.ToStringWithoutAddress(), warningMessage);
        }

        /// <summary>
        ///     Log custom info message.
        /// </summary>
        /// <param name="loggingAdapter">Logging adapter</param>
        /// <param name="actorType">Actor type</param>
        /// <param name="actorPath">Actor path</param>
        /// <param name="infoMessage">Info message</param>
        public void LogCustomInfo(ILoggingAdapter loggingAdapter, Type actorType, ActorPath actorPath, string infoMessage)
        {
            loggingAdapter.Info("ACTOR TYPE: {ActorType}, ACTOR PATH: {ActorPath}, Info: {InfoMessage}", actorType.Name, actorPath.ToStringWithoutAddress(), infoMessage);
        }

        /// <summary>
        ///     Log custom debug message.
        /// </summary>
        /// <param name="loggingAdapter">Logging adapter</param>
        /// <param name="actorType">Actor type</param>
        /// <param name="actorPath">Actor path</param>
        /// <param name="debugMessage">Debug message</param>
        public void LogCustomDebug(ILoggingAdapter loggingAdapter, Type actorType, ActorPath actorPath, string debugMessage)
        {
            loggingAdapter.Info("ACTOR TYPE: {ActorType}, ACTOR PATH: {ActorPath}, Debug: {DebugMessage}", actorType.Name, actorPath.ToStringWithoutAddress(), debugMessage);
        }

        /// <summary>
        ///     Log message receiving event.
        /// </summary>
        /// <param name="loggingAdapter">Logging adapter</param>
        /// <param name="actorType">Actor type</param>
        /// <param name="actorPath">Actor path</param>
        /// <param name="messageType">Message type</param>
        /// <param name="from">Message sender</param>
        public void LogReceiveMessageInfo(ILoggingAdapter loggingAdapter, Type actorType, ActorPath actorPath, Type messageType, string from)
        {
            //loggingAdapter.Info("ACTOR TYPE: {ActorType}, ACTOR PATH: {ActorPath}, Received: {MessageType} From: {From}", actorType.Name, actorPath.ToStringWithoutAddress(), messageType.Name, from);
        }

        /// <summary>
        ///     Log message sending event.
        /// </summary>
        /// <param name="loggingAdapter">Logging adapter</param>
        /// <param name="actorType">Actor type</param>
        /// <param name="actorPath">Actor path</param>
        /// <param name="messageType">Message type</param>
        /// <param name="to">Message recipient</param>
        public void LogSendMessageInfo(ILoggingAdapter loggingAdapter, Type actorType, ActorPath actorPath, Type messageType, string to)
        {
            //loggingAdapter.Info("ACTOR TYPE: {ActorType}, ACTOR PATH: {ActorPath}, Send: {MessageType}, To: {To}", actorType.Name, actorPath.ToStringWithoutAddress(), messageType.Name, to);
        }

        /// <summary>
        ///     Log ticket booking event.
        /// </summary>
        /// <param name="loggingAdapter">Logging adapter</param>
        /// <param name="actorType">Actor type</param>
        /// <param name="actorPath">ACtor path</param>
        /// <param name="bookedTicketRoute">Booked ticket's route</param>
        /// <param name="userActorId">User actor id</param>
        public void LogTicketProviderBookingMessageInfo(ILoggingAdapter loggingAdapter, Type actorType, ActorPath actorPath, string bookedTicketRoute, Guid userActorId)
        {
            //loggingAdapter.Info("ACTOR TYPE: {ActorType}, ACTOR PATH: {ActorPath}, Booked: {BookedTicketRoute}, For: {BookedTicketOwner}", actorType.Name, actorPath.ToStringWithoutAddress(), bookedTicketRoute, userActorId);
        }

        /// <summary>
        ///     Log ticket booking event.
        /// </summary>
        /// <param name="loggingAdapter">Logging adapter</param>
        /// <param name="actorType">Actor type</param>
        /// <param name="actorPath">ACtor path</param>
        /// <param name="bookedTicketCount">Booked ticket count</param>
        public void LogTicketProviderBookedTicketCountMessageInfo(ILoggingAdapter loggingAdapter, Type actorType, ActorPath actorPath, int bookedTicketCount)
        {
            loggingAdapter.Info("ACTOR TYPE: {ActorType}, ACTOR PATH: {ActorPath}, Booked: {BookedTicketCount}", actorType.Name, actorPath.ToStringWithoutAddress(), bookedTicketCount);
        }

        /// <summary>
        ///     Log ticket booking event.
        /// </summary>
        /// <param name="loggingAdapter">Logging adapter</param>
        /// <param name="actorType">Actor type</param>
        /// <param name="actorPath">ACtor path</param>
        /// <param name="bookedTicketRoute">Booked ticket's route</param>
        /// <param name="userActorId">User actor id</param>
        public void LogExceptionMessageWarning(ILoggingAdapter loggingAdapter, Type actorType, string actorPath, Type exception)
        {
            loggingAdapter.Warning("ACTOR TYPE: {ActorType}, ACTOR PATH: {ActorPath}, Exception: {ExceptionType}", actorType.Name, actorPath, exception.Name);
        }


    }
}
