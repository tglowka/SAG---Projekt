using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAgentBookingSystem.Actors
{
    public class TicketProviderActor : ReceiveActor
    {
        public TicketProviderActor()
        {
        }

        #region Lifecycle hooks

        protected override void PreStart()
        {
            ColorConsole.WriteLineColor($"{this.GetType().Name} Prestart", ConsoleColor.DarkYellow);
        }

        protected override void PostStop()
        {
            ColorConsole.WriteLineColor($"{this.GetType().Name} PostSTop", ConsoleColor.DarkYellow);
        }

        protected override void PreRestart(Exception reason, object message)
        {
            ColorConsole.WriteLineColor($"{this.GetType().Name} PreRestart because: " + reason, ConsoleColor.DarkYellow);

            base.PreRestart(reason, message);
        }

        protected override void PostRestart(Exception reason)
        {
            ColorConsole.WriteLineColor($"{this.GetType().Name} PostRestart because: " + reason, ConsoleColor.DarkYellow);

            base.PostRestart(reason);
        }

        #endregion

    }
}
