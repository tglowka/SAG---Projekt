using Akka.Actor;
using Akka.Event;
using MultiAgentBookingSystem.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAgentBookingSystem.Actors
{
    public class UserActor : ReceiveActor
    {

        public UserActor()
        {
            Receive<AddUserActorMessage>(message =>
            {
                ColorConsole.WriteLineColor($"UserActor received {message.ActorId}", ConsoleColor.Yellow);
            });
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
