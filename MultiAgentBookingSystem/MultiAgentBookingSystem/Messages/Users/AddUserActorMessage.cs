using MultiAgentBookingSystem.Messages.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAgentBookingSystem.Messages
{
    public class AddUserActorMessage : AddActorMessage
    {
        public AddUserActorMessage(string actorId) : base(actorId) { }
    }
}
