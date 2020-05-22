using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAgentBookingSystem.Messages.Abstracts
{
    public  class AddActorMessage
    {
        public Guid ActorId { get; private set; }

        public AddActorMessage(Guid actorId)
        {
            this.ActorId = actorId;
        }
    }
}
