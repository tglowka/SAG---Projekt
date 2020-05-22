using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAgentBookingSystem.Messages.Brokers
{
    public class BookTicketByBrokerMessage
    {
        private Guid userActorId;

        public BookTicketByBrokerMessage(Guid userActorId)
        {
            this.userActorId = userActorId;
        }
    }
}
