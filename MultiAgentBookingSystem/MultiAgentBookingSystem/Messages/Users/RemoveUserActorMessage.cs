using MultiAgentBookingSystem.Messages.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAgentBookingSystem.Messages
{
    public class RemoveUserActorMessage : RemoveActorMessage
    {
        public RemoveUserActorMessage(string actorId) : base(actorId) { }
    }
}
