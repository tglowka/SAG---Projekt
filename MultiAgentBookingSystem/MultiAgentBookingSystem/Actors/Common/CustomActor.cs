﻿using Akka.Actor;
using Akka.Event;
using MultiAgentBookingSystem.DataResources;
using MultiAgentBookingSystem.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAgentBookingSystem.Actors.Common
{
    public abstract class CustomActor<TParentOf> : ReceiveActor where TParentOf : class
    {
        protected Dictionary<Guid, IActorRef> childrenActors = new Dictionary<Guid, IActorRef>();

        protected void CreateChildActor(int actorCount = 1)
        {
            for (int i = 0; i < actorCount; i++)
            {
                Guid newActorId = Guid.NewGuid();

                IActorRef newChildActorRef = Context.ActorOf(Props.Create(typeof(TParentOf), newActorId), newActorId.ToString());
                this.childrenActors.Add(newActorId, newChildActorRef);
            }
        }

        protected void CreateChildActor(int minCount, int maxCount)
        {
            int actorCount = RandomGenerator.Instance.random.Next(minCount, maxCount + 1);

            for (int i = 0; i < actorCount; i++)
            {
                Guid newActorId = Guid.NewGuid();

                IActorRef newChildActorRef = Context.ActorOf(Props.Create(typeof(TParentOf), newActorId), newActorId.ToString());
                this.childrenActors.Add(newActorId, newChildActorRef);
            }
        }

        protected void RemoveChildActor(Guid actorId)
        {
            if (this.childrenActors.ContainsKey(actorId))
            {
                IActorRef childActorRef = this.childrenActors[actorId];

                childActorRef.Tell(PoisonPill.Instance);

                this.childrenActors.Remove(actorId);
            }
        }

        protected void LogChildrenCount(Type actorType, ActorPath actorPath)
        {
            LoggingConfiguration.Instance.LogActiveActorCount(Context.GetLogger(), actorType, actorPath, Context.GetChildren().Count());
            LoggingConfiguration.Instance.LogAllActorCount(Context.GetLogger(), actorType, actorPath, this.childrenActors.Count);
        }
    }
}
