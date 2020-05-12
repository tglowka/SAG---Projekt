using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAgentBookingSystem.Messages
{
    public class RemoveUserActorMessage
    {
        public int UserId { get; private set; }

        public RemoveUserActorMessage(int userId)
        {
            this.UserId = userId;
        }
    }
}
