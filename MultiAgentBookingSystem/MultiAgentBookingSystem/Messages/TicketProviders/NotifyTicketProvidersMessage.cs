using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAgentBookingSystem.Messages.TicketProviders
{
    public class NotifyTicketProvidersMessage
    {
        public Guid UserActorId { get; private set; }
        public string TicketRoute { get; private set; }

        public NotifyTicketProvidersMessage(Guid userActorId, string ticketRoute)
        {
            this.UserActorId = userActorId;
            this.TicketRoute = ticketRoute;
        }
    }
}
