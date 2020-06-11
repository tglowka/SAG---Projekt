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
        public Type ActorType { get; private set; }

        public RandomException(ActorPath actorPath, Type actorType) : base()
        {
            this.ActorPath = actorPath;
            this.ActorType = actorType;
        }
    }
}
