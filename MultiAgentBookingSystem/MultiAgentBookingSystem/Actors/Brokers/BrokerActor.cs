using Akka.Actor;
using Akka.Event;
using MultiAgentBookingSystem.Actors.Common;
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
    public class BrokerActor : CoordinatorChildActor
    {
        private Guid id;

        public BrokerActor(Guid id)
        {
            LoggingConfiguration.Instance.LogActorCreation(Context.GetLogger(), this.GetType(), Self.Path);

            this.id = id;

            Become(WaitingForUserActorState);
        }

        #region private methods

        private void WaitingForUserActorState()
        {
            Receive<BookTicketByBrokerMessage>(message =>
            {
                try
                {
                    LoggingConfiguration.Instance.LogReceiveMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, message.GetType(), Sender.Path.ToStringWithoutAddress());

                    this.NotifyTicketProviders(message);
                }
                catch (Exception exception)
                {
                    throw exception;
                }
            });

            Receive<TicketProviderResponseMessage>(message =>
            {
                try
                {
                    LoggingConfiguration.Instance.LogReceiveMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, message.GetType(), Sender.Path.ToStringWithoutAddress());

                    this.BookTicket(Sender, message);
                }
                catch (Exception exception)
                {
                    throw exception;
                }
            });

            Receive<TicketProviderConfirmationMessage>(message =>
            {
                try
                {
                    LoggingConfiguration.Instance.LogReceiveMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, message.GetType(), Sender.Path.ToStringWithoutAddress());

                    this.NotifyUserAboutConfirmation(message);
                }
                catch (Exception exception)
                {
                    throw exception;
                }
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
        private void NotifyTicketProviders(BookTicketByBrokerMessage message)
        {
            NotifyTicketProvidersMessage notifyTicketProvidersMessage = new NotifyTicketProvidersMessage(message.UserActor, message.UserActorId, message.TicketRoute);
            string allTicketProviders = $"{ActorPaths.TicketProviderCoordinatorActor.Path}/*";

            TicketBookingActorSystem.Instance.actorSystem.ActorSelection(allTicketProviders).Tell(notifyTicketProvidersMessage);

            LoggingConfiguration.Instance.LogSendMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, notifyTicketProvidersMessage.GetType(), "All ticket providers.");
        }

        /// <summary>
        ///     Send message to particular ticket provider with intention of booking the ticket.
        /// </summary>
        /// <param name="sender">Ticket provider</param>
        /// <param name="userActorId">User actor id</param>
        /// <param name="ticketRoute">Ticket route</param>
        private void BookTicket(IActorRef sender, TicketProviderResponseMessage message)
        {
            BookTicketMessage bookTicketMessage = new BookTicketMessage(message.UserActor, message.UserActorId, message.TicketRoute);

            sender.Tell(bookTicketMessage);

            LoggingConfiguration.Instance.LogSendMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, bookTicketMessage.GetType(), sender.Path.ToStringWithoutAddress());
        }

        /// <summary>
        ///     Notify user actor that ticket is booked. TODO Wait for user confirmation for particular time.
        /// </summary>
        private void NotifyUserAboutConfirmation(TicketProviderConfirmationMessage message)
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
