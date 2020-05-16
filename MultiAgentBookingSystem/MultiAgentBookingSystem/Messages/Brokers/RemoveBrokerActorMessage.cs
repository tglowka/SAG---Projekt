using MultiAgentBookingSystem.Messages.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAgentBookingSystem.Messages
{
    public class RemoveBrokerActorMessage : RemoveActorMessage
    {
        public RemoveBrokerActorMessage(string actorId) : base(actorId) { }
    }
}
