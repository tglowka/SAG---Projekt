using Akka.Actor;
using Akka.Event;
using MultiAgentBookingSystem.DataResources;
using MultiAgentBookingSystem.Exceptions;
using MultiAgentBookingSystem.Logger;
using MultiAgentBookingSystem.Messages;
using MultiAgentBookingSystem.Messages.Brokers;
using MultiAgentBookingSystem.Messages.Users;
using MultiAgentBookingSystem.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAgentBookingSystem.Actors
{
    public class UserActor : ReceiveActor
    {
        private Guid id;
        private Dictionary<Guid, IActorRef> brokers;

        private string ticketRoute;

        public UserActor(Guid id)
        {
            LoggingConfiguration.Instance.LogActorCreation(Context.GetLogger(), this.GetType(), Self.Path);

            this.id = id;
            this.ticketRoute = TicketsHelper.GetRandomRoute();

            //Console.WriteLine("ROUTE: {0}", ticketRoute);

            Become(LookingForBrokersState);
        }

        #region private methods

        private void LookingForBrokersState()
        {
            Context.SetReceiveTimeout(TimeSpan.FromSeconds(10));

            this.GetAllBrokers();

            Receive<ReceiveTimeout>(message =>
            {
                LoggingConfiguration.Instance.LogReceiveMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, message.GetType(), Sender.Path.ToStringWithoutAddress());

                this.GetAllBrokers();
            });

            Receive<ReceiveAllBrokers>(message =>
            {
                LoggingConfiguration.Instance.LogReceiveMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, message.GetType(), Sender.Path.ToStringWithoutAddress());

                this.brokers = new Dictionary<Guid, IActorRef>(message.brokers);

                Become(BookingTicketState);
            });
        }

        private void BookingTicketState()
        {
            Context.SetReceiveTimeout(TimeSpan.FromSeconds(20));

            this.BookTicketByBroker();

            Receive<ReceiveTimeout>(message =>
            {
                LoggingConfiguration.Instance.LogReceiveMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, message.GetType(), Sender.Path.ToStringWithoutAddress());

                if (this.brokers?.Count > 0)
                    this.BookTicketByBroker();
                else
                    Become(LookingForBrokersState);
            });

            Receive<TicketProviderConfirmationMessage>(message =>
            {
                LoggingConfiguration.Instance.LogReceiveMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, message.GetType(), Sender.Path.ToStringWithoutAddress());

                //Console.WriteLine($"User: {this.id} has booked ticket {this.ticketRoute}.");

                Context.Stop(Self);
            });
        }

        /// <summary>
        ///     Send message to BrokerCoordinator actor in order to get all brokers.
        /// </summary>
        private void GetAllBrokers()
        {
            GetAllBrokersMessage getAllBrokersMessage = new GetAllBrokersMessage();
            ActorSelection brokerCoordinator = TicketBookingActorSystem.Instance.actorSystem.ActorSelection("/user/SystemSupervisor/BrokerCoordinator");

            brokerCoordinator.Tell(getAllBrokersMessage);

            LoggingConfiguration.Instance.LogSendMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, getAllBrokersMessage.GetType(), "/user/SystemSupervisor/BrokerCoordinator");
        }

        /// <summary>
        ///     Send BookTicketByBrokerMessage message to random known broker. If there is no known broker, send message to BrokerCoordinator to get them.
        /// </summary>
        private void BookTicketByBroker()
        {
            // Get random broker from known brokers and remove him from known brokers.
            Guid randomBroker = this.brokers.ElementAt(RandomGenerator.Instance.random.Next(0, this.brokers.Count)).Key;
            IActorRef randomBrokerActor = this.brokers[randomBroker];
            this.brokers.Remove(randomBroker);

            BookTicketByBrokerMessage bookTicketByBrokerMessage = new BookTicketByBrokerMessage(this.id, this.ticketRoute);
            randomBrokerActor.Tell(bookTicketByBrokerMessage);

            LoggingConfiguration.Instance.LogSendMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, bookTicketByBrokerMessage.GetType(), randomBrokerActor.Path.ToStringWithoutAddress());
        }

        #endregion

        #region Lifecycle hooks

        protected override void PreStart()
        {
            LoggingConfiguration.Instance.LogActorPreStart(Context.GetLogger(), Self.Path);
        }

        protected override void PostStop()
        {
            //LoggingConfiguration.Instance.LogActorPostStop(Context.GetLogger(), Self.Path);
            LoggingConfiguration.Instance.LogActorStop(Context.GetLogger(), this.GetType(), Self.Path);
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
