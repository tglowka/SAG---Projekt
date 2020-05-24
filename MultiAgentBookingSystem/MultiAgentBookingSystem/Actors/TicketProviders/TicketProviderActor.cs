using Akka.Actor;
using Akka.Event;
using MultiAgentBookingSystem.DataResources;
using MultiAgentBookingSystem.Logger;
using MultiAgentBookingSystem.Messages.Brokers;
using MultiAgentBookingSystem.Messages.TicketProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAgentBookingSystem.Actors
{
    public class TicketProviderActor : ReceiveActor
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

            foreach (KeyValuePair<string, int> kvp in offeredTickets)
            {
                Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
            }

            Become(WaitingForBrokerState);
        }

        #region private methods

        private void WaitingForBrokerState()
        {
            Context.SetReceiveTimeout(null);

            Receive<NotifyTicketProvidersMessage>(message =>
            {
                LoggingConfiguration.Instance.LogReceiveMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, message.GetType(), Sender.Path.ToStringWithoutAddress());

                this.NotifyBrokerAboutTicketAvailability(message.UserActorId, message.TicketRoute, Sender);
            });

            Receive<BookTicketMessage>(message =>
            {
                LoggingConfiguration.Instance.LogReceiveMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, message.GetType(), Sender.Path.ToStringWithoutAddress());

                this.BookTicketForUser(message.UserActorId, message.TicketRoute);
                this.SendConfirmationMessageToTheBroker(Sender);
            });
        }

        /// <summary>
        ///     Notify the broker about ticket availability.
        /// </summary>
        /// <param name="userActorId">User actor id</param>
        /// <param name="ticketRoute">Desired ticket route</param>
        private void NotifyBrokerAboutTicketAvailability(Guid userActorId, string ticketRoute, IActorRef broker)
        {
            if (this.offeredTickets.ContainsKey(ticketRoute) && this.offeredTickets[ticketRoute] > 0)
            {
                TicketProviderResponseMessage ticketProviderResponseMessage = new TicketProviderResponseMessage(userActorId, ticketRoute, this.id);

                broker.Tell(ticketProviderResponseMessage);

                LoggingConfiguration.Instance.LogSendMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, ticketProviderResponseMessage.GetType(), broker.Path.ToStringWithoutAddress());
            }
        }

        /// <summary>
        ///     Book ticket for particular user.
        /// </summary>
        /// <param name="userActorId">User actor id</param>
        /// <param name="ticketRoute">Desired ticket route</param>
        private void BookTicketForUser(Guid userActorId, string ticketRoute)
        {
            if (this.offeredTickets.ContainsKey(ticketRoute) && this.offeredTickets[ticketRoute] > 0)
            {
                this.bookedTickets.Add(userActorId, ticketRoute);
                --this.offeredTickets[ticketRoute];

                LoggingConfiguration.Instance.LogTicketProviderBookingMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, ticketRoute, userActorId);
            }
        }

        /// <summary>
        ///     Send booking confirmation to the broker.
        /// </summary>
        /// <param name="broker">Broker</param>
        private void SendConfirmationMessageToTheBroker(IActorRef broker)
        {
            TicketProviderConfirmationMessage ticketProviderConfirmationMessage = new TicketProviderConfirmationMessage();

            broker.Tell(ticketProviderConfirmationMessage);

            LoggingConfiguration.Instance.LogSendMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, ticketProviderConfirmationMessage.GetType(), broker.Path.ToStringWithoutAddress());
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
