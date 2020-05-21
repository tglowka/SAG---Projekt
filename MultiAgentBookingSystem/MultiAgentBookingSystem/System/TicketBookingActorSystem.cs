using Akka.Actor;
using MultiAgentBookingSystem.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAgentBookingSystem.System
{
    /// <summary>
    ///     Singleton that returns ticket booking actor system.
    /// </summary>
    public sealed class TicketBookingActorSystem
    {
        private const string actorSystemName = "TicketBookingActorSystem";

        private static readonly Lazy<ActorSystem> instance = new Lazy<ActorSystem>(() => ActorSystem.Create(actorSystemName));
        private TicketBookingActorSystem() { }

        public static ActorSystem Instance
        {
            get
            {
                return instance.Value;
            }
        }
    }
}
