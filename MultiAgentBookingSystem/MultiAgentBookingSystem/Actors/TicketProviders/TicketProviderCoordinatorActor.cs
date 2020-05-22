using Akka.Actor;
using Akka.Event;
using MultiAgentBookingSystem.Logger;
using MultiAgentBookingSystem.Messages;
using MultiAgentBookingSystem.Messages.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAgentBookingSystem.Actors
{
    public class TicketProviderCoordinatorActor : ReceiveActor
    {
        private Dictionary<Guid, IActorRef> childrenActors = new Dictionary<Guid, IActorRef>();

        public TicketProviderCoordinatorActor()
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
                IActorRef newChildActorRef = Context.ActorOf(Props.Create(() => new TicketProviderActor(actorId)), actorId.ToString());

                childrenActors.Add(actorId, newChildActorRef);

                ColorConsole.WriteLineColor($"TicketProviderCoordinatorActor create new child TicketProviderActor for {actorId} (Total TicketProviders: {childrenActors.Count}", ConsoleColor.Cyan);
            }
            else
            {
                ColorConsole.WriteLineColor($"ERROR - exists! TicketProviderCoordinatorActor can not create new child TicketProviderActor for {actorId} (Total TicketProviders: {childrenActors.Count}", ConsoleColor.Cyan);
            }
        }

        private void RemoveChildActor(Guid actorId)
        {
            if (childrenActors.ContainsKey(actorId))
            {
                IActorRef childActorRef = childrenActors[actorId];

                childActorRef.Tell(PoisonPill.Instance);

                childrenActors.Remove(actorId);

                ColorConsole.WriteLineColor($"TicketProviderCoordinatorActor remove child TicketProviderActor for {actorId} (Total TicketProviders: {childrenActors.Count}", ConsoleColor.Cyan);
            }
            else
            {
                ColorConsole.WriteLineColor($"ERROR - not exists! TicketProviderCoordinatorActor can not remove child TicketProviderActor for {actorId} (Total TicketProviders: {childrenActors.Count}", ConsoleColor.Cyan);
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
