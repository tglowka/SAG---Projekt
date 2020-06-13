using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAgentBookingSystem.Messages.Users
{
    public class ReceiveAllBrokers
    {
        public Dictionary<Guid, IActorRef> brokers { get; private set; }
        public ReceiveAllBrokers(Dictionary<Guid, IActorRef> brokers)
        {
            this.brokers = new Dictionary<Guid, IActorRef>(brokers);
        }
    }
}
