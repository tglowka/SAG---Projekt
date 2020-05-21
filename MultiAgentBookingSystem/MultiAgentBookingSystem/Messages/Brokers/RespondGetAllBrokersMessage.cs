using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAgentBookingSystem.Messages.Brokers
{

    public class RespondGetAllBrokersMessage
    {
        public Dictionary<string, IActorRef> brokers { get; private set; }

        public RespondGetAllBrokersMessage(Dictionary<string, IActorRef> brokers)
        {
            this.brokers = brokers;
        }
    }
}
