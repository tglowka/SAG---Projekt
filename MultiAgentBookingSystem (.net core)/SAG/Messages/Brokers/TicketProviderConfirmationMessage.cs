using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAgentBookingSystem.Messages.Brokers
{
    public class TicketProviderConfirmationMessage
    {
        public IActorRef UserActor { get; private set; }
        public Guid UserActorId { get; private set; }
        public string TicketRoute { get; private set; }
        public Guid TicketProviderId { get; private set; }

        public TicketProviderConfirmationMessage(IActorRef userActor, Guid userActorId, string ticketRoute, Guid ticketProviderId)
        {
            this.UserActor = userActor;
            this.UserActorId = userActorId;
            this.TicketRoute = ticketRoute;
            this.TicketProviderId = ticketProviderId;
        }
    }
}
