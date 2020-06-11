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
    public abstract class CoordinatorChildActor : ReceiveActor
    {
        protected virtual void HandleRandomException(RandomExceptionMessage message, Type actorType)
        {
            double randomDouble = RandomGenerator.Instance.random.NextDouble() * 100;

            if (message.ExceptionProbability > randomDouble)
            {
                LoggingConfiguration.Instance.LogExceptionMessageWarning(Context.GetLogger(), actorType, Self.Path.ToStringWithoutAddress(), typeof(RandomException));
                throw new RandomException(Self.Path, actorType);
            }
        }
    }
}
