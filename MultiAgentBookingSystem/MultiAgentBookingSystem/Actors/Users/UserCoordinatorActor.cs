using Akka.Actor;
using Akka.Event;
using MultiAgentBookingSystem.Logger;
using MultiAgentBookingSystem.Messages;
using System;
using System.Collections.Generic;
using System.Threading;

namespace MultiAgentBookingSystem.Actors
{
    public class UserCoordinatorActor : ReceiveActor
    {
        private Dictionary<string, IActorRef> childrenActors = new Dictionary<string, IActorRef>();

        public UserCoordinatorActor()
        {
            this.Become(this.InitialState);
        }

        #region private methods

        private void InitialState()
        {
            Receive<AddUserActorMessage>(message =>
            {
                this.CreateChildActor(message.ActorId);
            });

            Receive<RemoveUserActorMessage>(message =>
            {
                this.RemoveChildActor(message.ActorId);
            });
        }

        private void CreateChildActor(string actorId)
        {
            if (!childrenActors.ContainsKey(actorId))
            {
                IActorRef newChildActorRef = Context.ActorOf(Props.Create(() => new UserActor()));

                childrenActors.Add(actorId, newChildActorRef);

                ColorConsole.WriteLineColor($"UserCoordinatorActor create new child userActor for {actorId} (Total Users: {childrenActors.Count}", ConsoleColor.Cyan);
            }
            else
            {
                ColorConsole.WriteLineColor($"ERROR - exists! UserCoordinatorActor can not create new child userActor for {actorId} (Total Users: {childrenActors.Count}", ConsoleColor.Cyan);
            }
        }

        private void RemoveChildActor(string actorId)
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
