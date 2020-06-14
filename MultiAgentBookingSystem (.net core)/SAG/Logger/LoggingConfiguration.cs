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

        public bool DeepLogging { get; set; } = true;

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

        public void LogActorCreation(ILoggingAdapter loggingAdapter, Type actorType, ActorPath actorPath)
        {
            if (this.DeepLogging)
                loggingAdapter.Info("ACTOR TYPE: {ActorType}, ACTOR PATH: {ActorPath}, EVENT: {CustomEvent} ", actorType.Name, actorPath.ToStringWithoutAddress(), CustomTypes.Creation.ToString("g"));
        }

        public void LogReceiveMessageInfo(ILoggingAdapter loggingAdapter, Type actorType, ActorPath actorPath, Type messageType, string from, int messageCounter)
        {
            if (this.DeepLogging)
                loggingAdapter.Info("COUNTER: {MessageCounter} ACTOR TYPE: {ActorType}, ACTOR PATH: {ActorPath}, Received: {MessageType} From: {From}", messageCounter, actorType.Name, actorPath.ToStringWithoutAddress(), messageType.Name, from);
        }

        public void LogSendMessageInfo(ILoggingAdapter loggingAdapter, Type actorType, ActorPath actorPath, Type messageType, string to, int messageCounter)
        {
            if (this.DeepLogging)
                loggingAdapter.Info("COUNTER: {MessageCounter} ACTOR TYPE: {ActorType}, ACTOR PATH: {ActorPath}, Send: {MessageType}, To: {To}", messageCounter, actorType.Name, actorPath.ToStringWithoutAddress(), messageType.Name, to);
        }

        public void LogTicketProviderBookingMessageInfo(ILoggingAdapter loggingAdapter, Type actorType, ActorPath actorPath, string bookedTicketRoute, Guid userActorId)
        {
            if (this.DeepLogging)
                loggingAdapter.Info("ACTOR TYPE: {ActorType}, ACTOR PATH: {ActorPath}, Booked: {BookedTicketRoute}, For: {BookedTicketOwner}", actorType.Name, actorPath.ToStringWithoutAddress(), bookedTicketRoute, userActorId);
        }
        public void LogExceptionMessageWarning(ILoggingAdapter loggingAdapter, Type actorType, string actorPath, Type exceptionType, string exceptionMessage)
        {
            loggingAdapter.Warning("ACTOR TYPE: {ActorType}, ACTOR PATH: {ActorPath}, ExceptionType: {ExceptionType}, ExceptionMessage: {ExceptionMessage}", actorType.Name, actorPath, exceptionType.Name, exceptionMessage);
        }

        public void LogTicketProviderBookedTicketCountMessageInfo(ILoggingAdapter loggingAdapter, Type actorType, ActorPath actorPath, int bookedTicketCount)
        {
            loggingAdapter.Info("ACTOR TYPE: {ActorType}, ACTOR PATH: {ActorPath}, Booked: {BookedTicketCount}", actorType.Name, actorPath.ToStringWithoutAddress(), bookedTicketCount);
        }

        public void LogActiveActorCount(ILoggingAdapter loggingAdapter, Type actorType, ActorPath actorPath, int childActorCount)
        {
            loggingAdapter.Info("ACTOR TYPE: {ActorType}, ACTOR PATH: {ActorPath},  CHILD ACTOR COUNT: {ChildActorCount} ", actorType.Name, actorPath.ToStringWithoutAddress(), childActorCount);
        }

        public void LogAllActorCount(ILoggingAdapter loggingAdapter, Type actorType, ActorPath actorPath, int allChildActorCount)
        {
            loggingAdapter.Info("ACTOR TYPE: {ActorType}, ACTOR PATH: {ActorPath},  ALL CHILD ACTOR COUNT: {AllChildActorCount}", actorType.Name, actorPath.ToStringWithoutAddress(), allChildActorCount);
        }
    }
}
