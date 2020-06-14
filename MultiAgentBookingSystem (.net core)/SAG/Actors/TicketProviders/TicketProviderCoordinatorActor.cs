using Akka.Actor;
using Akka.Event;
using MultiAgentBookingSystem.Actors.Common;
using MultiAgentBookingSystem.DataResources;
using MultiAgentBookingSystem.Logger;
using MultiAgentBookingSystem.Messages;
using MultiAgentBookingSystem.Messages.Abstracts;
using MultiAgentBookingSystem.Messages.Brokers;
using MultiAgentBookingSystem.Messages.Common;
using MultiAgentBookingSystem.Messages.TicketProviders;
using MultiAgentBookingSystem.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAgentBookingSystem.Actors
{
    public class TicketProviderCoordinatorActor : CoordinatoActor<TicketProviderActor>
    {
        private readonly int _schedulerInterval = 5;

        public TicketProviderCoordinatorActor()
        {
            this.SetupBookedTicketsCountScheduler(this._schedulerInterval);
            this.Become(this.InitialState);
        }

        #region private methods

        private void InitialState()
        {
            this.Receive<AddActorMessage>(message =>
            {
                this.Delay();
                this.LogReceiveMessageInfo(message);
                this.CreateChildActor(message.ActorCount);
                this.SendAllTicketProvidersToAllBrokers();
            });

            this.Receive<AddRandomCountActorMessage>(message =>
            {
                this.Delay();
                this.LogReceiveMessageInfo(message);
                this.CreateChildActor(message);
                this.SendAllTicketProvidersToAllBrokers();
            });

            this.Receive<RemoveActorMessage>(message =>
            {
                this.Delay();
                this.LogReceiveMessageInfo(message);
                this.RemoveChildActor(message.ActorId);
            });

            this.Receive<GetAllTicketProvidersMessage>(message =>
           {
               this.Delay();
               this.LogReceiveMessageInfo(message);
               this.SendAllTicketProviders();
           });

            this.Receive<LogChildernCountMessage>(message =>
            {
                this.Delay();
                this.LogReceiveMessageInfo(message);
                this.LogChildrenCount(this.GetType(), Self.Path);
            });

            this.Receive<RandomExceptionMessage>(message =>
            {
                this.Delay();
                this.LogReceiveMessageInfo(message);
                this.HandleRandomException(message, this.GetType());
            });
        }

        private void SendAllTicketProviders()
        {
            ReceiveAllTicketProvidersMessage receiveAllTicketProvidersMessage = new ReceiveAllTicketProvidersMessage(Context.GetChildren());

            Sender.Tell(receiveAllTicketProvidersMessage);

            this.LogSendMessageInfo(receiveAllTicketProvidersMessage, Sender.Path.ToStringWithoutAddress());
        }

        private void SendAllTicketProvidersToAllBrokers()
        {
            ReceiveAllTicketProvidersMessage receiveAllTicketProvidersMessage = new ReceiveAllTicketProvidersMessage(Context.GetChildren());

            TicketBookingActorSystem.Instance.actorSystem.ActorSelection(ActorPaths.BrokerActors.Path).Tell(receiveAllTicketProvidersMessage);

            this.LogSendMessageInfo(receiveAllTicketProvidersMessage, ActorPaths.BrokerActors.Path);
        }

        private void SetupBookedTicketsCountScheduler(int interval)
        {
            LogBookedTicketCountMessage logBookedTicketCountMessage = new LogBookedTicketCountMessage();

            TicketBookingActorSystem.Instance.actorSystem.Scheduler.ScheduleTellRepeatedly(
                    TimeSpan.FromSeconds(0),
                    TimeSpan.FromSeconds(interval),
                    TicketBookingActorSystem.Instance.actorSystem.ActorSelection(ActorPaths.TickerProviderActors.Path),
                    logBookedTicketCountMessage,
                    Self);
        }

        #endregion
    }
}
