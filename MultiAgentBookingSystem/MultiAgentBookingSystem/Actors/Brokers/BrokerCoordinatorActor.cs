using Akka.Actor;
using Akka.Event;
using MultiAgentBookingSystem.DataResources;
using MultiAgentBookingSystem.Logger;
using MultiAgentBookingSystem.Messages;
using MultiAgentBookingSystem.Messages.Abstracts;
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
        private Dictionary<Guid, IActorRef> childrenActors = new Dictionary<Guid, IActorRef>();

        public BrokerCoordinatorActor()
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

            Receive<BookTicketByBrokerMessage>(message =>
            {
                this.BookTicketByBroker(message);
            });
        }

        private void CreateChildActor(Guid actorId)
        {
            if (!childrenActors.ContainsKey(actorId))
            {
                IActorRef newChildActorRef = Context.ActorOf(Props.Create(() => new BrokerActor(actorId)), actorId.ToString());

                childrenActors.Add(actorId, newChildActorRef);

                ColorConsole.WriteLineColor($"BrokerCoordinatorActor create new child brokerActor for {actorId} (Total Brokers: {childrenActors.Count}", ConsoleColor.Cyan);
            }
            else
            {
                ColorConsole.WriteLineColor($"ERROR - exists! BrokerCoordinatorActor can not create new child brokerActor for {actorId} (Total Brokers: {childrenActors.Count}", ConsoleColor.Cyan);
            }
        }

        private void RemoveChildActor(Guid actorId)
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

        private void BookTicketByBroker(BookTicketByBrokerMessage message)
        {
            // Get random broker
            IActorRef randomBroker = this.childrenActors.ElementAt(RandomGenerator.Instance.random.Next(0, this.childrenActors.Count)).Value;

            // Forward message to random broker
            randomBroker.Forward(message);
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
