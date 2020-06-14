using Akka.Actor;
using MultiAgentBookingSystem.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAgentBookingSystem.System
{
    public sealed class TicketBookingActorSystem
    {
        private const string actorSystemName = "TicketBookingActorSystem";

        public readonly ActorSystem actorSystem;

        private static readonly Lazy<TicketBookingActorSystem> instance = new Lazy<TicketBookingActorSystem>(() => new TicketBookingActorSystem(ActorSystem.Create(actorSystemName)));
        private TicketBookingActorSystem(ActorSystem actorSystem)
        {
            this.actorSystem = actorSystem;
        }

        public static TicketBookingActorSystem Instance
        {
            get
            {
                return instance.Value;
            }
        }
    }
}
