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
using System.Threading;
using System.Threading.Tasks;

namespace MultiAgentBookingSystem.Actors.Common
{
    public class LogActor : ReceiveActor
    {
        private int _messageCounter = 0;

        protected virtual void HandleRandomException(RandomExceptionMessage message, Type actorType)
        {
            double randomDouble = RandomGenerator.NextDouble() * 100;

            if (message.ExceptionProbability > randomDouble)
            {
                RandomException randomException = new RandomException(Self.Path, actorType);
                LoggingConfiguration.Instance.LogExceptionMessageWarning(Context.GetLogger(), actorType, Self.Path.ToStringWithoutAddress(), typeof(RandomException), randomException.Message);
                throw randomException;
            }
        }

        protected void LogReceiveMessageInfo(Object message)
        {
            ++this._messageCounter;
            LoggingConfiguration.Instance.LogReceiveMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, message.GetType(), Sender.Path.ToStringWithoutAddress(), this._messageCounter);
        }

        protected void LogSendMessageInfo(Object message, string to)
        {
            ++this._messageCounter;
            LoggingConfiguration.Instance.LogSendMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, message.GetType(), to, this._messageCounter);
        }

        protected void LogActorCreation()
        {
            LoggingConfiguration.Instance.LogActorCreation(Context.GetLogger(), this.GetType(), Self.Path);
        }

        protected void DelayMessageProcessing(int minMilliseconds, int maxMilliseconds, double probability)
        {
            if (minMilliseconds > 0 && maxMilliseconds > 0)
            {
                double randomDouble = RandomGenerator.NextDouble() * 100;

                if (probability > randomDouble)
                {
                    int delay = RandomGenerator.Next(minMilliseconds, maxMilliseconds + 1);
                    Thread.Sleep(delay);
                }
            }
        }
    }
}
