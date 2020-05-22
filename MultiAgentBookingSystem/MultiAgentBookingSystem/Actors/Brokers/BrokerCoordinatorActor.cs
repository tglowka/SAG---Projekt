using Akka.Actor;
using Akka.Event;
using MultiAgentBookingSystem.DataResources;
using MultiAgentBookingSystem.Logger;
using MultiAgentBookingSystem.Messages;
using MultiAgentBookingSystem.Messages.Abstracts;
using MultiAgentBookingSystem.Messages.Brokers;
using MultiAgentBookingSystem.Messages.Users;
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
                LoggingConfiguration.Instance.LogReceiveMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, message.GetType(), Sender.Path.ToString());

                this.CreateChildActor(message.ActorId);
            });

            Receive<RemoveActorMessage>(message =>
            {
                LoggingConfiguration.Instance.LogReceiveMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, message.GetType(), Sender.Path.ToString());

                this.RemoveChildActor(message.ActorId);
            });

            Receive<GetAllBrokersMessage>(message =>
            {
                LoggingConfiguration.Instance.LogReceiveMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, message.GetType(), Sender.Path.ToString());

                this.SendAllBrokers(Sender);
            });
        }

        private void CreateChildActor(Guid actorId)
        {
            if (!childrenActors.ContainsKey(actorId))
            {
                IActorRef newChildActorRef = Context.ActorOf(Props.Create(() => new BrokerActor(actorId)), actorId.ToString());
                childrenActors.Add(actorId, newChildActorRef);
            }
        }

        private void RemoveChildActor(Guid actorId)
        {
            if (childrenActors.ContainsKey(actorId))
            {
                IActorRef childActorRef = childrenActors[actorId];
                childActorRef.Tell(PoisonPill.Instance);
                childrenActors.Remove(actorId);
            }
        }

        private void SendAllBrokers(IActorRef sender)
        {
            ReceiveAllBrokers receiveAllBrokersMessage = new ReceiveAllBrokers(this.childrenActors);

            sender.Tell(receiveAllBrokersMessage);
            LoggingConfiguration.Instance.LogSendMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, receiveAllBrokersMessage.GetType(), sender.Path.ToString());
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
