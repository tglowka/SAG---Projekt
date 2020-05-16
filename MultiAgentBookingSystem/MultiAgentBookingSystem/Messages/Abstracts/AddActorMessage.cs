using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAgentBookingSystem.Messages.Abstracts
{
    public abstract class AddActorMessage
    {
        public string ActorId { get; private set; }

        public AddActorMessage(string actorId)
        {
            this.ActorId = actorId;
        }
    }
}
