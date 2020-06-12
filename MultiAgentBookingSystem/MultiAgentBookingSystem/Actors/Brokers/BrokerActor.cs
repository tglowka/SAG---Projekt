﻿using Akka.Actor;
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
        private Guid id;

        private IEnumerable<IActorRef> _allTicketProviders;

        public IStash Stash { get; set; }

        public BrokerActor(Guid id)
        {
            LoggingConfiguration.Instance.LogActorCreation(Context.GetLogger(), this.GetType(), Self.Path);

            this.id = id;

            Become(WaitingForTicketProvidersState);
        }

        #region private methods

        private void WaitingForTicketProvidersState()
        {
            this.GetAllTicketProviders();

            Receive<ReceiveAllTicketProvidersMessage>(message =>
            {
                LoggingConfiguration.Instance.LogReceiveMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, message.GetType(), Sender.Path.ToStringWithoutAddress());

                this._allTicketProviders = message.AllTicketProviders;
                Become(WaitingForUserActorState);
            });

            Receive<BookTicketByBrokerMessage>(message =>
            {
                LoggingConfiguration.Instance.LogReceiveMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, message.GetType(), Sender.Path.ToStringWithoutAddress());

                this.Stash.Stash();
            });

        }

        private void WaitingForUserActorState()
        {
            Stash.UnstashAll();

            Receive<ReceiveAllTicketProvidersMessage>(message =>
            {
                LoggingConfiguration.Instance.LogReceiveMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, message.GetType(), Sender.Path.ToStringWithoutAddress());

                this._allTicketProviders = message.AllTicketProviders;
            });

            Receive<BookTicketByBrokerMessage>(message =>
            {
                LoggingConfiguration.Instance.LogReceiveMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, message.GetType(), Sender.Path.ToStringWithoutAddress());

                this.NotifyRandomTicketProvider(message);
            });

            Receive<NoAvailableTicketMessage>(message =>
            {
                LoggingConfiguration.Instance.LogReceiveMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, message.GetType(), Sender.Path.ToStringWithoutAddress());

                this.ForwardNoAvailableTicketsMessage(message);
            });

            Receive<TicketProviderResponseMessage>(message =>
            {
                LoggingConfiguration.Instance.LogReceiveMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, message.GetType(), Sender.Path.ToStringWithoutAddress());

                this.BookTicket(message);
            });

            Receive<TicketProviderConfirmationMessage>(message =>
            {
                LoggingConfiguration.Instance.LogReceiveMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, message.GetType(), Sender.Path.ToStringWithoutAddress());

                this.NotifyUserAboutConfirmation(message);
            });

            Receive<RandomExceptionMessage>(message =>
            {
                this.HandleRandomException(message, this.GetType());
            });
        }

        /// <summary>
        ///     Send notification to all ticket providers that particular user want to buy a ticket on particular route.
        /// </summary>
        /// <param name="userActorId">User actor id</param>
        /// <param name="ticketRoute">Ticket route</param>
        private void NotifyRandomTicketProvider(BookTicketByBrokerMessage message)
        {
            NotifyTicketProvidersMessage notifyTicketProvidersMessage = new NotifyTicketProvidersMessage(message.UserActor, message.UserActorId, message.TicketRoute);

            IActorRef randomTicketProvider = _allTicketProviders.RandomElement(RandomGenerator.Instance.random);

            randomTicketProvider.Tell(notifyTicketProvidersMessage);

            LoggingConfiguration.Instance.LogSendMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, notifyTicketProvidersMessage.GetType(), "All ticket providers.");
        }

        /// <summary>
        ///     Send message to particular ticket provider with intention of booking the ticket.
        /// </summary>
        /// <param name="sender">Ticket provider</param>
        /// <param name="userActorId">User actor id</param>
        /// <param name="ticketRoute">Ticket route</param>
        private void BookTicket(TicketProviderResponseMessage message)
        {
            BookTicketMessage bookTicketMessage = new BookTicketMessage(message.UserActor, message.UserActorId, message.TicketRoute);

            Sender.Tell(bookTicketMessage);

            LoggingConfiguration.Instance.LogSendMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, bookTicketMessage.GetType(), Sender.Path.ToStringWithoutAddress());
        }

        /// <summary>
        ///     Notify user actor that ticket is booked. TODO Wait for user confirmation for particular time.
        /// </summary>
        private void NotifyUserAboutConfirmation(TicketProviderConfirmationMessage message)
        {
            message.UserActor.Forward(message);

            LoggingConfiguration.Instance.LogSendMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, message.GetType(), message.UserActor.Path.ToStringWithoutAddress());
        }

        private void GetAllTicketProviders()
        {
            GetAllTicketProvidersMessage getAllTicketProvidersMessage = new GetAllTicketProvidersMessage();

            TicketBookingActorSystem.Instance.actorSystem.ActorSelection(ActorPaths.TicketProviderCoordinatorActor.Path).Tell(getAllTicketProvidersMessage);

            LoggingConfiguration.Instance.LogSendMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, getAllTicketProvidersMessage.GetType(), SystemConstants.TicketProviderCoordinatorActorName);
        }

        private void ForwardNoAvailableTicketsMessage(NoAvailableTicketMessage message)
        {
            message.UserActor.Forward(message);

            LoggingConfiguration.Instance.LogSendMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, message.GetType(), message.UserActor.Path.ToStringWithoutAddress());
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
