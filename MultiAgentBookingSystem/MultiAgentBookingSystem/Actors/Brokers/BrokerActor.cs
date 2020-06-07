using Akka.Actor;
using Akka.Event;
using MultiAgentBookingSystem.Logger;
using MultiAgentBookingSystem.Messages.Brokers;
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
    public class BrokerActor : ReceiveActor, IWithUnboundedStash
    {
        private Guid id;

        private Guid currentSupportedUserActorId;
        private string currentSupportedUserActorTicketRoute;
        private IActorRef currentSupportedUserActorRef;

        private Guid currentSupportedTicketProviderId = Guid.Empty;

        public IStash Stash { get; set; }

        public BrokerActor(Guid id)
        {
            LoggingConfiguration.Instance.LogActorCreation(Context.GetLogger(), this.GetType(), Self.Path);

            this.id = id;

            Become(WaitingForUserActorState);
        }

        #region private methods

        private void WaitingForUserActorState()
        {
            Context.SetReceiveTimeout(null);

            this.ClearCurrentContext();

            Receive<BookTicketByBrokerMessage>(message =>
            {
                LoggingConfiguration.Instance.LogReceiveMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, message.GetType(), Sender.Path.ToStringWithoutAddress());

                this.SetCurrentContext(message.UserActorId, message.TicketRoute, Sender, Guid.Empty);
                this.NotifyTicketProviders(message.UserActorId, message.TicketRoute);

                Become(BookingState);
            });
        }

        private void BookingState()
        {
            Context.SetReceiveTimeout(TimeSpan.FromSeconds(2));

            Receive<ReceiveTimeout>(message =>
            {
                LoggingConfiguration.Instance.LogReceiveMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, message.GetType(), Sender.Path.ToStringWithoutAddress());
                // TODO 
                Stash.UnstashAll();
                Become(WaitingForUserActorState);
            });

            Receive<BookTicketByBrokerMessage>(message =>
            {
                LoggingConfiguration.Instance.LogReceiveMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, message.GetType(), Sender.Path.ToStringWithoutAddress());

                Stash.Stash();
            });

            Receive<TicketProviderResponseMessage>(message =>
            {
                LoggingConfiguration.Instance.LogReceiveMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, message.GetType(), Sender.Path.ToStringWithoutAddress());

                this.SetCurrentContext(this.currentSupportedUserActorId, this.currentSupportedUserActorTicketRoute, this.currentSupportedUserActorRef, message.TicketProviderId);

                if (this.CheckWithCurrentContext(message.UserActorId, message.TicketRoute, message.TicketProviderId))
                    this.BookTicket(Sender, message.UserActorId, message.TicketRoute);

            });

            Receive<TicketProviderConfirmationMessage>(message =>
            {
                LoggingConfiguration.Instance.LogReceiveMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, message.GetType(), Sender.Path.ToStringWithoutAddress());

                this.NotifyUserAboutConfirmation(message);
                
                Stash.UnstashAll();
                Become(WaitingForUserActorState);
            });
        }

        /// <summary>
        ///     Set broker's current context.
        /// </summary>
        /// <param name="userActorId">User actor id</param>
        /// <param name="ticketRoute">Desired ticket route for user actor</param>
        private void SetCurrentContext(Guid userActorId, string ticketRoute, IActorRef userActorRef, Guid ticketProviderId)
        {
            this.currentSupportedUserActorId = userActorId;
            this.currentSupportedUserActorTicketRoute = ticketRoute;
            this.currentSupportedUserActorRef = userActorRef;

            if (this.currentSupportedTicketProviderId == Guid.Empty)
                this.currentSupportedTicketProviderId = ticketProviderId;
        }

        /// <summary>
        ///     Clear broker's current context.
        /// </summary>
        private void ClearCurrentContext()
        {
            this.currentSupportedUserActorId = Guid.Empty;
            this.currentSupportedUserActorTicketRoute = String.Empty;
            this.currentSupportedUserActorRef = null;
            this.currentSupportedTicketProviderId = Guid.Empty;
        }

        /// <summary>
        ///     Check whether user actor id and ticket route match with current context.
        /// </summary>
        /// <param name="userId">User actor id</param>
        /// <param name="ticketRoute">Ticket route</param>
        /// <param name="ticketProviderId">Ticket provider id</param>
        /// <returns></returns>
        private bool CheckWithCurrentContext(Guid userId, string ticketRoute, Guid ticketProviderId)
        {
            return this.currentSupportedUserActorId == userId
                && this.currentSupportedUserActorTicketRoute == ticketRoute
                && this.currentSupportedTicketProviderId == ticketProviderId;
        }

        /// <summary>
        ///     Send notification to all ticket providers that particular user want to buy a ticket on particular route.
        /// </summary>
        /// <param name="userActorId">User actor id</param>
        /// <param name="ticketRoute">Ticket route</param>
        private void NotifyTicketProviders(Guid userActorId, string ticketRoute)
        {
            NotifyTicketProvidersMessage notifyTicketProvidersMessage = new NotifyTicketProvidersMessage(userActorId, ticketRoute);

            TicketBookingActorSystem.Instance.actorSystem.ActorSelection("/user/SystemSupervisor/TicketProviderCoordinator/*").Tell(notifyTicketProvidersMessage);

            LoggingConfiguration.Instance.LogSendMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, notifyTicketProvidersMessage.GetType(), "All ticket providers.");
        }

        /// <summary>
        ///     Send message to particular ticket provider with intention of booking the ticket.
        /// </summary>
        /// <param name="sender">Ticket provider</param>
        /// <param name="userActorId">User actor id</param>
        /// <param name="ticketRoute">Ticket route</param>
        private void BookTicket(IActorRef sender, Guid userActorId, string ticketRoute)
        {
            BookTicketMessage bookTicketMessage = new BookTicketMessage(userActorId, ticketRoute);

            sender.Tell(bookTicketMessage);

            LoggingConfiguration.Instance.LogSendMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, bookTicketMessage.GetType(), sender.Path.ToStringWithoutAddress());
        }

        /// <summary>
        ///     Notify user actor that ticket is booked. TODO Wait for user confirmation for particular time.
        /// </summary>
        private void NotifyUserAboutConfirmation(TicketProviderConfirmationMessage message)
        {
            this.currentSupportedUserActorRef.Forward(message);

            LoggingConfiguration.Instance.LogSendMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, message.GetType(), this.currentSupportedUserActorRef.Path.ToStringWithoutAddress());
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
