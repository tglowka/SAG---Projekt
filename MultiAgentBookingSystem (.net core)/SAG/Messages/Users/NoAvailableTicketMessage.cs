using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAgentBookingSystem.Messages.Users
{
    public class NoAvailableTicketMessage
    {
        public IActorRef UserActor { get; private set; }
        public Guid UserActorId { get; private set; }
        public string TicketRoute { get; private set; }

        public NoAvailableTicketMessage(IActorRef userActor, Guid userActorId, string ticketRoute)
        {
            this.UserActor = userActor;
            this.UserActorId = userActorId;
            this.TicketRoute = ticketRoute;
        }
    }
}
