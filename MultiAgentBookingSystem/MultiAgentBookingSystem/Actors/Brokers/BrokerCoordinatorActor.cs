using Akka.Actor;
using Akka.Event;
using MultiAgentBookingSystem.Logger;
using MultiAgentBookingSystem.Messages;
using MultiAgentBookingSystem.Messages.Brokers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAgentBookingSystem.Actors
{
    public class BrokerCoordinatorActor : ReceiveActor
    {
        private Dictionary<string, IActorRef> childrenActors = new Dictionary<string, IActorRef>();

        public BrokerCoordinatorActor()
        {
            this.Become(this.InitialState);
        }

        #region private methods

        private void InitialState()
        {
            Receive<AddBrokerActorMessage>(message =>
            {
                this.CreateChildActor(message.ActorId);
            });

            Receive<RemoveBrokerActorMessage>(message =>
            {
                this.RemoveChildActor(message.ActorId);
            });
        }

        private void CreateChildActor(string actorId)
        {
            if (!childrenActors.ContainsKey(actorId))
            {
                IActorRef newChildActorRef = Context.ActorOf(Props.Create(() => new BrokerActor()));

                childrenActors.Add(actorId, newChildActorRef);

                ColorConsole.WriteLineColor($"BrokerCoordinatorActor create new child brokerActor for {actorId} (Total Brokers: {childrenActors.Count}", ConsoleColor.Cyan);
            }
            else
            {
                ColorConsole.WriteLineColor($"ERROR - exists! BrokerCoordinatorActor can not create new child brokerActor for {actorId} (Total Brokers: {childrenActors.Count}", ConsoleColor.Cyan);
            }
        }

        private void RemoveChildActor(string actorId)
        {
            if (childrenActors.ContainsKey(actorId))
            {
                IActorRef childActorRef = childrenActors[actorId];

                childActorRef.Tell(PoisonPill.Instance);

                childrenActors.Remove(actorId);

                ColorConsole.WriteLineColor($"BrokerCoordinatorActor remove child brokerActor for {actorId} (Total Brokers: {childrenActors.Count}", ConsoleColor.Cyan);
            }
            else
            {
                ColorConsole.WriteLineColor($"ERROR - not exists! BrokerCoordinatorActor can not remove child brokerActor for {actorId} (Total Brokers: {childrenActors.Count}", ConsoleColor.Cyan);
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
