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
    public class LogActor : ReceiveActor
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

        protected void LogReceiveMessageInfo(Object message)
        {
            LoggingConfiguration.Instance.LogReceiveMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, message.GetType(), Sender.Path.ToStringWithoutAddress());
        }

        protected void LogSendMessageInfo(Object message, string to)
        {
            LoggingConfiguration.Instance.LogSendMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, message.GetType(), to);
        }

        protected void LogActorCreation()
        {
            LoggingConfiguration.Instance.LogActorCreation(Context.GetLogger(), this.GetType(), Self.Path);
        }
    }
}
