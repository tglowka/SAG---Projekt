using Akka.Actor;
using Akka.Event;
using MultiAgentBookingSystem.DataResources;
using MultiAgentBookingSystem.Exceptions;
using MultiAgentBookingSystem.Logger;
using MultiAgentBookingSystem.Messages;
using MultiAgentBookingSystem.Messages.Abstracts;
using MultiAgentBookingSystem.Messages.Common;
using MultiAgentBookingSystem.System;
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

        public UserCoordinatorActor(int childCount)
        {
            this.CreateChildActor(childCount);

            this.Become(this.InitialState);
        }

        #region private methods

        private void InitialState()
        {
            Receive<AddActorMessage>(message =>
            {
                LoggingConfiguration.Instance.LogReceiveMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, message.GetType(), Sender.Path.ToStringWithoutAddress());

                this.CreateChildActor(message.ActorCount);
            });

            Receive<AddRandomCountActorMessage>(message =>
            {
                LoggingConfiguration.Instance.LogReceiveMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, message.GetType(), Sender.Path.ToStringWithoutAddress());

                this.CreateChildActor(message.MinActorCount, message.MaxActorCount);
            });

            Receive<RemoveActorMessage>(message =>
            {
                LoggingConfiguration.Instance.LogReceiveMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, message.GetType(), Sender.Path.ToStringWithoutAddress());

                this.RemoveChildActor(message.ActorId);
            });
        }

        private void CreateChildActor(int actorCount = 1)
        {
            for (int i = 0; i < actorCount; i++)
            {
                Guid newActorId = Guid.NewGuid();

                IActorRef newChildActorRef = Context.ActorOf(Props.Create(() => new UserActor(newActorId)), newActorId.ToString());
                this.childrenActors.Add(newActorId, newChildActorRef);
            }
        }

        private void CreateChildActor(int minCount, int maxCount)
        {
            int actorCount = RandomGenerator.Instance.random.Next(minCount, maxCount + 1);

            for (int i = 0; i < actorCount; i++)
            {
                Guid newActorId = Guid.NewGuid();

                IActorRef newChildActorRef = Context.ActorOf(Props.Create(() => new UserActor(newActorId)), newActorId.ToString());
                this.childrenActors.Add(newActorId, newChildActorRef);
            }
        }

        private void RemoveChildActor(Guid actorId)
        {
            if (this.childrenActors.ContainsKey(actorId))
            {
                IActorRef childActorRef = this.childrenActors[actorId];

                childActorRef.Tell(PoisonPill.Instance);

                this.childrenActors.Remove(actorId);

                ColorConsole.WriteLineColor($"UserCoordinatorActor remove child userActor for {actorId} (Total Users: {this.childrenActors.Count}", ConsoleColor.Cyan);
            }
            else
            {
                ColorConsole.WriteLineColor($"ERROR - not exists! UserCoordinatorActor can not remove child userActor for {actorId} (Total Users: {this.childrenActors.Count}", ConsoleColor.Cyan);
            }
        }

        #endregion

        #region protected methods
        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(
                null,
                null,
                localOnlyDecider: ex =>
                {
                    switch (ex)
                    {
                        case ArithmeticException ae:
                            return Directive.Resume;
                        case NullReferenceException nre:
                            return Directive.Restart;
                        case ArgumentException are:
                            Console.WriteLine("EEEEEEEERRRRRRRRRRROOOOOOOOORRRRRRRRRRRRRRRRRRR");
                            UserActorStopException userActorStopException = ex as UserActorStopException;
                            this.childrenActors.Remove(userActorStopException.Id);
                            return Directive.Stop;
                        default:
                            return Directive.Escalate;
                    }

                    return Akka.Actor.SupervisorStrategy.DefaultStrategy.Decider.Decide(ex);
                });
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
