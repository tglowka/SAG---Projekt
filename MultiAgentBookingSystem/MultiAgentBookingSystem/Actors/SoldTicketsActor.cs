using Akka.Actor;
using MultiAgentBookingSystem.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAgentBookingSystem.Actors
{
    public class SoldTicketsActor : ReceiveActor
    {
        private Dictionary<string, int> soldTickets;
        public SoldTicketsActor()
        {
            this.soldTickets = new Dictionary<string, int>();

            Receive<IncrementSoldTicketsAmount>(message => HandeIncrementMessage(message));
        }

        private void HandeIncrementMessage(IncrementSoldTicketsAmount message)
        {
            if (this.soldTickets.ContainsKey(message.Route))
            {
                ++this.soldTickets[message.Route];
            }
            else
            {
                this.soldTickets.Add(message.Route, 1);
            }

            ColorConsole.WriteLineColor($"SoldTicketsActor {message.Route} has been sold {this.soldTickets[message.Route]} times", ConsoleColor.Magenta);
        }



        #region Lifecycle hooks

        protected override void PreStart()
        {
            ColorConsole.WriteLineColor($"{this.GetType().Name} Prestart", ConsoleColor.White);
        }

        protected override void PostStop()
        {
            ColorConsole.WriteLineColor($"{this.GetType().Name} PostSTop", ConsoleColor.White);
        }

        protected override void PreRestart(Exception reason, object message)
        {
            ColorConsole.WriteLineColor($"{this.GetType().Name} PreRestart because: " + reason, ConsoleColor.White);

            base.PreRestart(reason, message);
        }

        protected override void PostRestart(Exception reason)
        {
            ColorConsole.WriteLineColor($"{this.GetType().Name} PostRestart because: " + reason, ConsoleColor.White);

            base.PostRestart(reason);
        }

        #endregion

    }
}
