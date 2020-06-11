using Akka.Actor;
using MultiAgentBookingSystem.DataResources;
using MultiAgentBookingSystem.Exceptions.Common;
using MultiAgentBookingSystem.Messages.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAgentBookingSystem.Actors.Common
{
    public abstract class CoordinatorChildActor : ReceiveActor
    {
        protected virtual void HandleRandomException(RandomExceptionMessage message)
        {
            double randomDouble = RandomGenerator.Instance.random.NextDouble() * 100;

            if (message.ExceptionProbability > randomDouble)
            {
                throw new RandomException(Self.Path);
            }
        }
    }
}
