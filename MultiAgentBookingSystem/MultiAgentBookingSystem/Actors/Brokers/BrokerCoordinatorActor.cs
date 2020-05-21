using Akka.Actor;
using MultiAgentBookingSystem.Interfaces;
using MultiAgentBookingSystem.Messages;
using MultiAgentBookingSystem.Messages.Brokers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAgentBookingSystem.Actors
{
    public class BrokerCoordinatorActor : ReceiveActor, ICoordinatorActor
    {
        public Dictionary<string, IActorRef> childrenActors { get; set; }

        public BrokerCoordinatorActor()
        {
            childrenActors = new Dictionary<string, IActorRef>();

            //Receive<AddBrokerActorMessage>(message =>
            //{
            //    this.CreateChildActor(message.ActorId);
            //});

            Receive<RemoveBrokerActorMessage>(message =>
            {
                this.RemoveChildActor(message.ActorId);
            });
        }

        public void CreateChildActor(string actorId)
        {
            if (!childrenActors.ContainsKey(actorId))
            {
                IActorRef newChildActorRef = Context.ActorOf(Props.Create(() => new BrokerActor()), "Broker" + actorId);

                childrenActors.Add(actorId, newChildActorRef);

                ColorConsole.WriteLineColor($"BrokerCoordinatorActor create new child brokerActor for {actorId} (Total Brokers: {childrenActors.Count}", ConsoleColor.Cyan);
            }
            else
            {
                ColorConsole.WriteLineColor($"ERROR - exists! BrokerCoordinatorActor can not create new child brokerActor for {actorId} (Total Brokers: {childrenActors.Count}", ConsoleColor.Cyan);
            }
        }

        public void RemoveChildActor(string actorId)
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

        #region Lifecycle hooks

        protected override void PreStart()
        {
            ColorConsole.WriteLineColor($"{this.GetType().Name} Prestart", ConsoleColor.DarkRed);
        }

        protected override void PostStop()
        {
            ColorConsole.WriteLineColor($"{this.GetType().Name} PostSTop", ConsoleColor.DarkRed);
        }

        protected override void PreRestart(Exception reason, object message)
        {
            ColorConsole.WriteLineColor($"{this.GetType().Name} PreRestart because: " + reason, ConsoleColor.DarkRed);

            base.PreRestart(reason, message);
        }

        protected override void PostRestart(Exception reason)
        {
            ColorConsole.WriteLineColor($"{this.GetType().Name} PostRestart because: " + reason, ConsoleColor.DarkRed);

            base.PostRestart(reason);
        }
        #endregion

    }
}
