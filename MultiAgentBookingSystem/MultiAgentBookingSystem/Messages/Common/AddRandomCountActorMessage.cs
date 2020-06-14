using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAgentBookingSystem.Messages.Common
{
    public class AddRandomCountActorMessage
    {
        public int MinActorCount { get; private set; }
        public int MaxActorCount { get; private set; }

        public AddRandomCountActorMessage(int minActorCount, int maxActorCount)
        {
            this.MinActorCount = minActorCount;
            this.MaxActorCount = maxActorCount;
        }
    }
}
