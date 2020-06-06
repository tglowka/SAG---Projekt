using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAgentBookingSystem.Exceptions
{
    public class UserActorStopException : Exception
    { 

        public Guid Id { get; private set; }

        public UserActorStopException(Guid id) : base()
        {
            this.Id = id;
        }
    }
}
