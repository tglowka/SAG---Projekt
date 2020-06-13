using Akka.Actor;
using Akka.Event;
using MultiAgentBookingSystem.Actors.Common;
using MultiAgentBookingSystem.DataResources;
using MultiAgentBookingSystem.Logger;
using MultiAgentBookingSystem.Messages;
using MultiAgentBookingSystem.Messages.Abstracts;
using MultiAgentBookingSystem.Messages.Brokers;
using MultiAgentBookingSystem.Messages.Common;
using MultiAgentBookingSystem.Messages.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAgentBookingSystem.Actors
{
    public class BrokerCoordinatorActor : CoordinatoActor<BrokerActor>
    {
        public BrokerCoordinatorActor()
        {
            this.Become(this.InitialState);
        }

        #region private methods

        private void InitialState()
        {
            Receive<AddActorMessage>(message =>
            {
                this.LogReceiveMessageInfo(message);
                this.CreateChildActor(message.ActorCount);
            });

            Receive<AddRandomCountActorMessage>(message =>
            {
                this.LogReceiveMessageInfo(message);
                this.CreateChildActor(message);
            });

            Receive<RemoveActorMessage>(message =>
            {
                this.LogReceiveMessageInfo(message);
                this.RemoveChildActor(message.ActorId);
            });

            Receive<GetAllBrokersMessage>(message =>
            {
                this.LogReceiveMessageInfo(message);
                this.SendAllBrokers();
            });

            Receive<LogChildernCountMessage>(message =>
            {
                this.LogReceiveMessageInfo(message);
                this.LogChildrenCount(this.GetType(), Self.Path);
            });

            Receive<RandomExceptionMessage>(message =>
            {
                this.LogReceiveMessageInfo(message);
                this.HandleRandomException(message, this.GetType());
            });
        }

        private void SendAllBrokers()
        {
            ReceiveAllBrokers receiveAllBrokersMessage = new ReceiveAllBrokers(this.childrenActors);
            Sender.Tell(receiveAllBrokersMessage);
            this.LogSendMessageInfo(receiveAllBrokersMessage, Sender.Path.ToStringWithoutAddress());
        }

        #endregion
    }
}
