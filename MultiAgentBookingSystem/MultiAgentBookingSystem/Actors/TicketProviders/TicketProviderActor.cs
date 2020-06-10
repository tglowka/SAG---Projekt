using Akka.Actor;
using Akka.Event;
using MultiAgentBookingSystem.Actors.Common;
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
                try
                {
                    LoggingConfiguration.Instance.LogReceiveMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, message.GetType(), Sender.Path.ToStringWithoutAddress());

                    this.NotifyBrokerAboutTicketAvailability(message.UserActorId, message.TicketRoute, Sender);
                }
                catch (Exception exception)
                {
                    throw exception;
                }
            });

            Receive<BookTicketMessage>(message =>
            {
                try
                {
                    LoggingConfiguration.Instance.LogReceiveMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, message.GetType(), Sender.Path.ToStringWithoutAddress());

                    this.BookTicketForUser(Sender, message.UserActorId, message.TicketRoute);
                }
                catch (Exception exception)
                {
                    throw exception;
                }
            });
        }

        /// <summary>
        ///     Notify the broker about ticket availability. If there is no such a ticket, it will be added.
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
            else if (this.offeredTickets.ContainsKey(ticketRoute) && this.offeredTickets[ticketRoute] == 0)
            {
                ++this.offeredTickets[ticketRoute];
            }
            else if (!this.offeredTickets.ContainsKey(ticketRoute))
            {
                this.offeredTickets.Add(ticketRoute, 1);
            }
        }

        /// <summary>
        ///     Book ticket for particular user.
        /// </summary>
        /// <param name="userActorId">User actor id</param>
        /// <param name="ticketRoute">Desired ticket route</param>
        private void BookTicketForUser(IActorRef broker, Guid userActorId, string ticketRoute)
        {
            if (this.offeredTickets.ContainsKey(ticketRoute) && this.offeredTickets[ticketRoute] > 0)
            {
                this.bookedTickets.Add(userActorId, ticketRoute);
                --this.offeredTickets[ticketRoute];
                TicketProviderConfirmationMessage ticketProviderConfirmationMessage = new TicketProviderConfirmationMessage();

                broker.Tell(ticketProviderConfirmationMessage);

                LoggingConfiguration.Instance.LogSendMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, ticketProviderConfirmationMessage.GetType(), broker.Path.ToStringWithoutAddress());
            }
            else if (this.offeredTickets.ContainsKey(ticketRoute) && this.offeredTickets[ticketRoute] == 0)
            {
                ++this.offeredTickets[ticketRoute];
            }
            else if (!this.offeredTickets.ContainsKey(ticketRoute))
            {
                this.offeredTickets.Add(ticketRoute, 1);
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
