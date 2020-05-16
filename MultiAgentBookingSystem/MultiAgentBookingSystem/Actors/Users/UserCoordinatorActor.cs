
using Akka.Actor;
using MultiAgentBookingSystem.Interfaces;
using MultiAgentBookingSystem.Messages;
using System;
using System.Collections.Generic;
using System.Threading;

namespace MultiAgentBookingSystem.Actors
{
    public class UserCoordinatorActor : ReceiveActor, ICoordinatorActor
    {
        public Dictionary<string, IActorRef> childrenActors { get; set; }

        public UserCoordinatorActor()
        {
            childrenActors = new Dictionary<string, IActorRef>();

            Receive<AddUserActorMessage>(message =>
            {
                this.CreateChildActor(message.ActorId);
            });

            Receive<RemoveUserActorMessage>(message =>
            {
                this.RemoveChildActor(message.ActorId);
            });
        }

        public void CreateChildActor(string actorId)
        {
            if (!childrenActors.ContainsKey(actorId))
            {
                IActorRef newChildActorRef = Context.ActorOf(Props.Create(() => new UserActor()), "User" + actorId);

                childrenActors.Add(actorId, newChildActorRef);

                ColorConsole.WriteLineColor($"UserCoordinatorActor create new child userActor for {actorId} (Total Users: {childrenActors.Count}", ConsoleColor.Cyan);
            }
            else
            {
                ColorConsole.WriteLineColor($"ERROR - exists! UserCoordinatorActor can not create new child userActor for {actorId} (Total Users: {childrenActors.Count}", ConsoleColor.Cyan);
            }
        }

        public void RemoveChildActor(string actorId)
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

        #region Lifecycle hooks

        protected override void PreStart()
        {
            ColorConsole.WriteLineColor("UserCoordinatorActor Prestart", ConsoleColor.Cyan);
        }

        protected override void PostStop()
        {
            ColorConsole.WriteLineColor("UserCoordinatorActor PostSTop", ConsoleColor.Cyan);
        }

        protected override void PreRestart(Exception reason, object message)
        {
            ColorConsole.WriteLineColor("UserCoordinatorActor PreRestart because: " + reason, ConsoleColor.Cyan);

            base.PreRestart(reason, message);
        }

        protected override void PostRestart(Exception reason)
        {
            ColorConsole.WriteLineColor("UserCoordinatorActor PostRestart because: " + reason, ConsoleColor.Cyan);

            base.PostRestart(reason);
        }



        #endregion

    }
}
