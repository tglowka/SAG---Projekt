
using Akka.Actor;
using MultiAgentBookingSystem.Messages;
using System;
using System.Collections.Generic;

namespace MultiAgentBookingSystem.Actors
{
    public class UserCoordinatorActor : ReceiveActor
    {
        private readonly Dictionary<int, IActorRef> users;
        public UserCoordinatorActor()
        {
            users = new Dictionary<int, IActorRef>();

            Receive<AddUserActorMessage>(message =>
            {
                this.CreateChildUser(message.UserId);
                IActorRef childActorRef = users[message.UserId];
                childActorRef.Tell(message);
            });

            Receive<RemoveUserActorMessage>(message =>
            {
                IActorRef childActorRef = users[message.UserId];
                childActorRef.Tell(PoisonPill.Instance);
                this.RemoveChildUser(message.UserId);
            });
        }

        private void CreateChildUser(int userId)
        {
            if (!users.ContainsKey(userId))
            {
                IActorRef newChildActorRef = Context.ActorOf(Props.Create(() => new UserActor(userId)), "User" + userId);

                users.Add(userId, newChildActorRef);

                ColorConsole.WriteLineColor($"UserCoordinatorActor create new child userActor for {userId} (Total Users: {users.Count}", ConsoleColor.Cyan);
            }
            else
            {
                ColorConsole.WriteLineColor($"ERROR - exists! UserCoordinatorActor can not create new child userActor for {userId} (Total Users: {users.Count}", ConsoleColor.Cyan);
            }
        }

        private void RemoveChildUser(int userId)
        {
            if (users.ContainsKey(userId))
            {
                users.Remove(userId);

                ColorConsole.WriteLineColor($"UserCoordinatorActor remove child userActor for {userId} (Total Users: {users.Count}", ConsoleColor.Cyan);
            }
            else
            {
                ColorConsole.WriteLineColor($"ERROR - not exists! UserCoordinatorActor can not remove child userActor for {userId} (Total Users: {users.Count}", ConsoleColor.Cyan);
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
