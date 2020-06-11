using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAgentBookingSystem.Exceptions.Common
{
    public class RandomException : Exception
    {
        public ActorPath ActorPath { get; private set; }

        public RandomException(ActorPath actorPath) : base()
        {
            this.ActorPath = actorPath;
        }
    }
}
