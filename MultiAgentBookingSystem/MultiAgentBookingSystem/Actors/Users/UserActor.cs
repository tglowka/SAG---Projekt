using Akka.Actor;
using Akka.Event;
using MultiAgentBookingSystem.DataResources;
using MultiAgentBookingSystem.Messages;
using MultiAgentBookingSystem.Messages.Brokers;
using MultiAgentBookingSystem.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAgentBookingSystem.Actors
{
    public class UserActor : ReceiveActor
    {
        private ILoggingAdapter loggingAdapter = Context.GetLogger();

        public UserActor()
        {
            loggingAdapter.Info("User actor {User} has been created", Self.Path);
            loggingAdapter.Debug("User actor {User} has been created Debug message", Self.Path);
            loggingAdapter.Error(new Exception("MY CUSTOM USER ACTOR EXCEPTION"), "User actor {User} has thrown an error", Self.Path);
        }

        #region Lifecycle hooks

        protected override void PreStart()
        {
            ColorConsole.WriteLineColor($"{this.GetType().Name} Prestart", ConsoleColor.Yellow);
        }

        protected override void PostStop()
        {
            ColorConsole.WriteLineColor($"{this.GetType().Name} PostSTop", ConsoleColor.Yellow);
        }

        protected override void PreRestart(Exception reason, object message)
        {
            ColorConsole.WriteLineColor($"{this.GetType().Name} PreRestart because: " + reason, ConsoleColor.Yellow);

            base.PreRestart(reason, message);
        }

        protected override void PostRestart(Exception reason)
        {
            ColorConsole.WriteLineColor($"{this.GetType().Name} PostRestart because: " + reason, ConsoleColor.Yellow);

            base.PostRestart(reason);
        }

        #endregion

    }
}
