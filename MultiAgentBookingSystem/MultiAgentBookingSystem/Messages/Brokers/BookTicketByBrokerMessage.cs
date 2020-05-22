using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAgentBookingSystem.Messages.Brokers
{
    public class BookTicketByBrokerMessage
    {
        public Guid UserActorId { get; private set; }
        public string TicketRoute { get; private set; }

        public BookTicketByBrokerMessage(Guid userActorId, string ticketRoute)
        {
            this.UserActorId = userActorId;
            this.TicketRoute = ticketRoute;
        }
    }
}
