using Akka.Actor;
using Akka.Event;
using MultiAgentBookingSystem.DataResources;
using MultiAgentBookingSystem.Exceptions.Common;
using MultiAgentBookingSystem.Logger;
using MultiAgentBookingSystem.Messages.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAgentBookingSystem.Actors.Common
{
    public abstract class CoordinatorChildActor : LogActor
    {
        protected Guid Id { get; }

        protected CoordinatorChildActor(Guid id)
        {
            this.Id = id;
        }
    }
}
