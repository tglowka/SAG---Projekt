using MultiAgentBookingSystem.Messages.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAgentBookingSystem.Messages
{
    public class AddTicketProviderActorMessage : AddActorMessage
    {
        public AddTicketProviderActorMessage(string actorId) : base(actorId) { }
    }
}
