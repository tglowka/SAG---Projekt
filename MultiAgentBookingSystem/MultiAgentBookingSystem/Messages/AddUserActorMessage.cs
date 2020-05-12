using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAgentBookingSystem.Messages
{
    public class AddUserActorMessage
    {
        public int UserId { get; private set; }

        public AddUserActorMessage(int userId)
        {
            this.UserId = userId;
        }
    }
}
