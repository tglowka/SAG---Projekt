using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAgentBookingSystem.Actors
{
    public class UserActor : ReceiveActor
    {
        public int UserId { get; private set; }

        public UserActor(int userId)
        {
            this.UserId = userId;
        }

        #region Lifecycle hooks

        protected override void PreStart()
        {
            ColorConsole.WriteLineColor("UserActor Prestart", ConsoleColor.Yellow);
        }

        protected override void PostStop()
        {
            ColorConsole.WriteLineColor("UserActor PostSTop", ConsoleColor.Yellow);
        }

        protected override void PreRestart(Exception reason, object message)
        {
            ColorConsole.WriteLineColor("UserActor PreRestart because: " + reason, ConsoleColor.Yellow);

            base.PreRestart(reason, message);
        }

        protected override void PostRestart(Exception reason)
        {
            ColorConsole.WriteLineColor("UserActor PostRestart because: " + reason, ConsoleColor.Yellow);

            base.PostRestart(reason);
        }

        #endregion

    }
}
