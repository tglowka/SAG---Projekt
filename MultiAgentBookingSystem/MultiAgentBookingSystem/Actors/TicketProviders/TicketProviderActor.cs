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
        private Guid id;

        private Dictionary<string, int> offeredTickets;
        private Dictionary<Guid, string> bookedTickets;

        public TicketProviderActor(Guid id)
        {
            LoggingConfiguration.Instance.LogActorCreation(Context.GetLogger(), this.GetType(), Self.Path);

            this.id = id;
            this.offeredTickets = TicketsHelper.GetRandomOfferedTickets();
            this.bookedTickets = new Dictionary<Guid, string>();

            Become(WaitingForBrokerState);
        }

        #region private methods

        private void WaitingForBrokerState()
        {
            Receive<NotifyTicketProvidersMessage>(message =>
            {
                LoggingConfiguration.Instance.LogReceiveMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, message.GetType(), Sender.Path.ToStringWithoutAddress());

                this.NotifyBrokerAboutTicketAvailability(message);
            });

            Receive<BookTicketMessage>(message =>
            {
                LoggingConfiguration.Instance.LogReceiveMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, message.GetType(), Sender.Path.ToStringWithoutAddress());

                this.BookTicketForUser(message);
            });

            Receive<LogBookedTicketCountMessage>(message =>
            {
                LoggingConfiguration.Instance.LogReceiveMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, message.GetType(), Sender.Path.ToStringWithoutAddress());

                LoggingConfiguration.Instance.LogTicketProviderBookedTicketCountMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, this.bookedTickets.Count);
            });

            Receive<RandomExceptionMessage>(message =>
            {
                this.HandleRandomException(message, this.GetType());
            });
        }

        /// <summary>
        ///     Notify the broker about ticket availability. If there is no such a ticket, it will be added.
        /// </summary>
        /// <param name="userActorId">User actor id</param>
        /// <param name="ticketRoute">Desired ticket route</param>
        private void NotifyBrokerAboutTicketAvailability(NotifyTicketProvidersMessage message)
        {
            if (this.offeredTickets.ContainsKey(message.TicketRoute) && this.offeredTickets[message.TicketRoute] > 0 && !this.bookedTickets.ContainsKey(message.UserActorId))
            {
                TicketProviderResponseMessage ticketProviderResponseMessage = new TicketProviderResponseMessage(message.UserActor, message.UserActorId, message.TicketRoute, this.id);

                Sender.Tell(ticketProviderResponseMessage);

                LoggingConfiguration.Instance.LogSendMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, ticketProviderResponseMessage.GetType(), Sender.Path.ToStringWithoutAddress());
            }
            else if (this.offeredTickets.ContainsKey(message.TicketRoute) && this.offeredTickets[message.TicketRoute] == 0)
            {
                ++this.offeredTickets[message.TicketRoute];

                NoAvailableTicketMessage noAvailableTicketMessage = new NoAvailableTicketMessage(message.UserActor, message.UserActorId, message.TicketRoute);

                Sender.Tell(noAvailableTicketMessage);

                LoggingConfiguration.Instance.LogSendMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, noAvailableTicketMessage.GetType(), Sender.Path.ToStringWithoutAddress());
            }
            else if (!this.offeredTickets.ContainsKey(message.TicketRoute))
            {
                this.offeredTickets.Add(message.TicketRoute, 1);

                NoAvailableTicketMessage noAvailableTicketMessage = new NoAvailableTicketMessage(message.UserActor, message.UserActorId, message.TicketRoute);

                Sender.Tell(noAvailableTicketMessage);

                LoggingConfiguration.Instance.LogSendMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, noAvailableTicketMessage.GetType(), Sender.Path.ToStringWithoutAddress());
            }
        }

        /// <summary>
        ///     Book ticket for particular user.
        /// </summary>
        /// <param name="userActorId">User actor id</param>
        /// <param name="ticketRoute">Desired ticket route</param>
        private void BookTicketForUser(BookTicketMessage message)
        {
            if (this.offeredTickets.ContainsKey(message.TicketRoute) && this.offeredTickets[message.TicketRoute] > 0 && !this.bookedTickets.ContainsKey(message.UserActorId))
            {
                this.bookedTickets.Add(message.UserActorId, message.TicketRoute);
                --this.offeredTickets[message.TicketRoute];

                TicketProviderConfirmationMessage ticketProviderConfirmationMessage = new TicketProviderConfirmationMessage(message.UserActor, message.UserActorId, message.TicketRoute, this.id);

                LoggingConfiguration.Instance.LogTicketProviderBookingMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, message.TicketRoute, message.UserActorId);

                Sender.Tell(ticketProviderConfirmationMessage);

                LoggingConfiguration.Instance.LogSendMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, ticketProviderConfirmationMessage.GetType(), Sender.Path.ToStringWithoutAddress());

            }
            else if (this.offeredTickets.ContainsKey(message.TicketRoute) && this.offeredTickets[message.TicketRoute] == 0)
            {
                ++this.offeredTickets[message.TicketRoute];

                NoAvailableTicketMessage noAvailableTicketMessage = new NoAvailableTicketMessage(message.UserActor, message.UserActorId, message.TicketRoute);

                Sender.Tell(noAvailableTicketMessage);

                LoggingConfiguration.Instance.LogSendMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, noAvailableTicketMessage.GetType(), Sender.Path.ToStringWithoutAddress());

            }
            else if (!this.offeredTickets.ContainsKey(message.TicketRoute))
            {
                this.offeredTickets.Add(message.TicketRoute, 1);

                NoAvailableTicketMessage noAvailableTicketMessage = new NoAvailableTicketMessage(message.UserActor, message.UserActorId, message.TicketRoute);

                Sender.Tell(noAvailableTicketMessage);

                LoggingConfiguration.Instance.LogSendMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, noAvailableTicketMessage.GetType(), Sender.Path.ToStringWithoutAddress());

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
