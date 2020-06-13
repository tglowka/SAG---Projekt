using Akka.Actor;
using Akka.Event;
using MultiAgentBookingSystem.Actors.Common;
using MultiAgentBookingSystem.DataResources;
using MultiAgentBookingSystem.Logger;
using MultiAgentBookingSystem.Messages.Brokers;
using MultiAgentBookingSystem.Messages.Common;
using MultiAgentBookingSystem.Messages.TicketProviders;
using MultiAgentBookingSystem.Messages.Users;
using System;
using System.Collections.Generic;
using System.Threading;

namespace MultiAgentBookingSystem.Actors
{
    public class TicketProviderActor : CoordinatorChildActor
    {
        private Dictionary<string, int> _offeredTickets;
        private Dictionary<Guid, string> _bookedTickets;

        public TicketProviderActor(Guid id) : base(id)
        {
            this.LogActorCreation();

            this._offeredTickets = TicketsHelper.GetRandomOfferedTickets();
            this._bookedTickets = new Dictionary<Guid, string>();

            this.Become(this.WaitingForBrokerState);
        }

        #region private methods

        private void WaitingForBrokerState()
        {
            this.Receive<NotifyTicketProvidersMessage>(message =>
            {
                this.LogReceiveMessageInfo(message);
                this.NotifyBrokerAboutTicketAvailability(message);
            });

            this.Receive<BookTicketMessage>(message =>
            {
                this.LogReceiveMessageInfo(message);
                this.BookTicketForUser(message);
            });

            this.Receive<LogBookedTicketCountMessage>(message =>
            {
                this.LogReceiveMessageInfo(message);
                this.LogBookedTicketCount();
            });

            this.Receive<RandomExceptionMessage>(message =>
            {
                this.LogReceiveMessageInfo(message);
                this.HandleRandomException(message, this.GetType());
            });
        }

        private void NotifyBrokerAboutTicketAvailability(NotifyTicketProvidersMessage message)
        {
            if (this._offeredTickets.ContainsKey(message.TicketRoute) && this._offeredTickets[message.TicketRoute] > 0 && !this._bookedTickets.ContainsKey(message.UserActorId))
            {
                TicketProviderResponseMessage ticketProviderResponseMessage = new TicketProviderResponseMessage(message.UserActor, message.UserActorId, message.TicketRoute, this.Id);

                Sender.Tell(ticketProviderResponseMessage);

                this.LogSendMessageInfo(ticketProviderResponseMessage, Sender.Path.ToStringWithoutAddress());

                return;
            }
            else
            {
                if (this._offeredTickets.ContainsKey(message.TicketRoute) && this._offeredTickets[message.TicketRoute] == 0)
                {
                    ++this._offeredTickets[message.TicketRoute];
                }
                if (!this._offeredTickets.ContainsKey(message.TicketRoute))
                {
                    this._offeredTickets.Add(message.TicketRoute, 1);
                }

                NoAvailableTicketMessage noAvailableTicketMessage = new NoAvailableTicketMessage(message.UserActor, message.UserActorId, message.TicketRoute);
                Sender.Tell(noAvailableTicketMessage);
                this.LogSendMessageInfo(noAvailableTicketMessage, Sender.Path.ToStringWithoutAddress());
            }
        }

        private void BookTicketForUser(BookTicketMessage message)
        {
            if (this._offeredTickets.ContainsKey(message.TicketRoute) && this._offeredTickets[message.TicketRoute] > 0 && !this._bookedTickets.ContainsKey(message.UserActorId))
            {
                this._bookedTickets.Add(message.UserActorId, message.TicketRoute);
                --this._offeredTickets[message.TicketRoute];

                TicketProviderConfirmationMessage ticketProviderConfirmationMessage = new TicketProviderConfirmationMessage(message.UserActor, message.UserActorId, message.TicketRoute, this.Id);

                LoggingConfiguration.Instance.LogTicketProviderBookingMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, message.TicketRoute, message.UserActorId);

                Sender.Tell(ticketProviderConfirmationMessage);

                this.LogSendMessageInfo(ticketProviderConfirmationMessage, Sender.Path.ToStringWithoutAddress());

                return;
            }
            else
            {
                if (this._offeredTickets.ContainsKey(message.TicketRoute) && this._offeredTickets[message.TicketRoute] == 0)
                {
                    ++this._offeredTickets[message.TicketRoute];
                }
                if (!this._offeredTickets.ContainsKey(message.TicketRoute))
                {
                    this._offeredTickets.Add(message.TicketRoute, 1);
                }

                NoAvailableTicketMessage noAvailableTicketMessage = new NoAvailableTicketMessage(message.UserActor, message.UserActorId, message.TicketRoute);

                Sender.Tell(noAvailableTicketMessage);

                this.LogSendMessageInfo(noAvailableTicketMessage, Sender.Path.ToStringWithoutAddress());
            }

        }

        private void LogBookedTicketCount()
        {
            LoggingConfiguration.Instance.LogTicketProviderBookedTicketCountMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, this._bookedTickets.Count);
        }

        #endregion
    }
}
