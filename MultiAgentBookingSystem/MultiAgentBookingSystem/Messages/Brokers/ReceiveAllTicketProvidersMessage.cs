using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAgentBookingSystem.Messages.Brokers
{
    public class ReceiveAllTicketProvidersMessage
    {
        public IEnumerable<IActorRef> AllTicketProviders { get; private set; }

        public ReceiveAllTicketProvidersMessage(IEnumerable<IActorRef> allTicketProviders)
        {
            this.AllTicketProviders = allTicketProviders;
        }
    }
}
