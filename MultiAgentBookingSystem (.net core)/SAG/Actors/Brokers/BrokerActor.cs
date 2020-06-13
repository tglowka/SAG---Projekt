using Akka.Actor;
using Akka.Event;
using MultiAgentBookingSystem.Actors.Common;
using MultiAgentBookingSystem.DataResources;
using MultiAgentBookingSystem.Helpers;
using MultiAgentBookingSystem.Logger;
using MultiAgentBookingSystem.Messages.Brokers;
using MultiAgentBookingSystem.Messages.Common;
using MultiAgentBookingSystem.Messages.TicketProviders;
using MultiAgentBookingSystem.Messages.Users;
using MultiAgentBookingSystem.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAgentBookingSystem.Actors
{
    public class BrokerActor : CoordinatorChildActor, IWithUnboundedStash
    {
        private IEnumerable<IActorRef> _allTicketProviders;

        public IStash Stash { get; set; }

        public BrokerActor(Guid id) : base(id)
        {
            this.LogActorCreation();
            this.Become(this.WaitingForTicketProvidersState);
        }

        #region private methods

        private void WaitingForTicketProvidersState()
        {
            this.GetAllTicketProviders();

            this.Receive<ReceiveAllTicketProvidersMessage>(message =>
            {
                this.LogReceiveMessageInfo(message);
                this._allTicketProviders = message.AllTicketProviders;
                if (this._allTicketProviders.Count() > 0)
                    this.Become(this.WaitingForUserActorState);
            });

            this.Receive<BookTicketByBrokerMessage>(message =>
            {
                this.LogReceiveMessageInfo(message);
                this.Stash.Stash();
            });

        }

        private void WaitingForUserActorState()
        {
            this.Stash.UnstashAll();

            this.Receive<ReceiveAllTicketProvidersMessage>(message =>
            {
                this.LogReceiveMessageInfo(message);
                this._allTicketProviders = message.AllTicketProviders;
            });

            this.Receive<BookTicketByBrokerMessage>(message =>
            {
                this.LogReceiveMessageInfo(message);
                this.NotifyRandomTicketProvider(message);
            });

            this.Receive<NoAvailableTicketMessage>(message =>
            {
                this.LogReceiveMessageInfo(message);
                this.ForwardNoAvailableTicketsMessage(message);
            });

            this.Receive<TicketProviderResponseMessage>(message =>
            {
                this.LogReceiveMessageInfo(message);
                this.BookTicket(message);
            });

            this.Receive<TicketProviderConfirmationMessage>(message =>
            {
                this.LogReceiveMessageInfo(message);
                this.NotifyUserAboutConfirmation(message);
            });

            this.Receive<RandomExceptionMessage>(message =>
            {
                this.LogReceiveMessageInfo(message);
                this.HandleRandomException(message, this.GetType());
            });
        }

        private void NotifyRandomTicketProvider(BookTicketByBrokerMessage message)
        {
            NotifyTicketProvidersMessage notifyTicketProvidersMessage = new NotifyTicketProvidersMessage(message.UserActor, message.UserActorId, message.TicketRoute);

            IActorRef randomTicketProvider = _allTicketProviders.RandomElement();

            randomTicketProvider.Tell(notifyTicketProvidersMessage);

            this.LogSendMessageInfo(notifyTicketProvidersMessage, randomTicketProvider.Path.ToStringWithoutAddress());
        }

        private void BookTicket(TicketProviderResponseMessage message)
        {
            BookTicketMessage bookTicketMessage = new BookTicketMessage(message.UserActor, message.UserActorId, message.TicketRoute);

            Sender.Tell(bookTicketMessage);

            this.LogSendMessageInfo(bookTicketMessage, Sender.Path.ToStringWithoutAddress());
        }

        private void NotifyUserAboutConfirmation(TicketProviderConfirmationMessage message)
        {
            message.UserActor.Forward(message);

            this.LogSendMessageInfo(message, message.UserActor.Path.ToStringWithoutAddress());
        }

        private void GetAllTicketProviders()
        {
            GetAllTicketProvidersMessage getAllTicketProvidersMessage = new GetAllTicketProvidersMessage();

            TicketBookingActorSystem.Instance.actorSystem.ActorSelection(ActorPaths.TicketProviderCoordinatorActor.Path).Tell(getAllTicketProvidersMessage);

            this.LogSendMessageInfo(getAllTicketProvidersMessage, SystemConstants.TicketProviderCoordinatorActorName);
        }

        private void ForwardNoAvailableTicketsMessage(NoAvailableTicketMessage message)
        {
            message.UserActor.Forward(message);
            this.LogSendMessageInfo(message, message.UserActor.Path.ToStringWithoutAddress());
        }

        #endregion
    }
}
