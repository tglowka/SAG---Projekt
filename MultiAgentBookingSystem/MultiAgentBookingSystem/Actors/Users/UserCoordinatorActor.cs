using Akka.Actor;
using Akka.Event;
using MultiAgentBookingSystem.Logger;
using MultiAgentBookingSystem.Messages;
using MultiAgentBookingSystem.Messages.Abstracts;
using System;
using System.Collections.Generic;
using System.Threading;

namespace MultiAgentBookingSystem.Actors
{
    public class UserCoordinatorActor : ReceiveActor
    {
        private Dictionary<Guid, IActorRef> childrenActors = new Dictionary<Guid, IActorRef>();

        public UserCoordinatorActor()
        {
            this.Become(this.InitialState);
        }

        #region private methods

        private void InitialState()
        {
            Receive<AddActorMessage>(message =>
            {
                this.CreateChildActor(message.ActorId);
            });

            Receive<RemoveActorMessage>(message =>
            {
                this.RemoveChildActor(message.ActorId);
            });
        }

        private void CreateChildActor(Guid actorId)
        {
            if (!childrenActors.ContainsKey(actorId))
            {
                IActorRef newChildActorRef = Context.ActorOf(Props.Create(() => new UserActor(actorId)), actorId.ToString());

                childrenActors.Add(actorId, newChildActorRef);
            }
            else
            {
            }
        }

        private void RemoveChildActor(Guid actorId)
        {
            if (childrenActors.ContainsKey(actorId))
            {
                IActorRef childActorRef = childrenActors[actorId];

                childActorRef.Tell(PoisonPill.Instance);

                childrenActors.Remove(actorId);

                ColorConsole.WriteLineColor($"UserCoordinatorActor remove child userActor for {actorId} (Total Users: {childrenActors.Count}", ConsoleColor.Cyan);
            }
            else
            {
                ColorConsole.WriteLineColor($"ERROR - not exists! UserCoordinatorActor can not remove child userActor for {actorId} (Total Users: {childrenActors.Count}", ConsoleColor.Cyan);
            }
        }

        #endregion

        #region Lifecycle hooks

        protected override void PreStart()
        {
            LoggingConfiguration.Instance.LogActorPreStart(Context.GetLogger(), Self.Path);
        }

        protected override void PostStop()
        {
            LoggingConfiguration.Instance.LogActorPostStop(Context.GetLogger(), Self.Path);
        }

        protected override void PreRestart(Exception reason, object message)
        {
            LoggingConfiguration.Instance.LogActorPreRestart(Context.GetLogger(), Self.Path, reason);
            base.PreRestart(reason, message);
        }

        protected override void PostRestart(Exception reason)
        {
            LoggingConfiguration.Instance.LogActorPostRestart(Context.GetLogger(), Self.Path, reason);
            base.PostRestart(reason);
        }

        #endregion

    }
}
