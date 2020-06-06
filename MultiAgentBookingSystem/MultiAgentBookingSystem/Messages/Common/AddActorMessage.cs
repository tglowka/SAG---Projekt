using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAgentBookingSystem.Messages.Abstracts
{
    public class AddActorMessage
    {
        public int ActorCount { get; private set; }

        public AddActorMessage(int actorCount)
        {
            this.ActorCount = actorCount;
        }
    }
}
