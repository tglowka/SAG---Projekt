using MultiAgentBookingSystem.Messages.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAgentBookingSystem.Messages
{
    public class RemoveTicketProviderActorMessage : RemoveActorMessage
    {
        public RemoveTicketProviderActorMessage(string actorId) : base(actorId) { }
    }
}
