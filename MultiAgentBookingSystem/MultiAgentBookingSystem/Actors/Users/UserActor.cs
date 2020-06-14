using Akka.Actor;
using Akka.Event;
using MultiAgentBookingSystem.Actors.Common;
using MultiAgentBookingSystem.DataResources;
using MultiAgentBookingSystem.Logger;
using MultiAgentBookingSystem.Messages;
using MultiAgentBookingSystem.Messages.Brokers;
using MultiAgentBookingSystem.Messages.Common;
using MultiAgentBookingSystem.Messages.Users;
using MultiAgentBookingSystem.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAgentBookingSystem.Actors
{
    public class UserActor : CoordinatorChildActor
    {
        private Dictionary<Guid, IActorRef> _brokers;

        private readonly string _ticketRoute;

        public UserActor(Guid id) : base(id)
        {
            this.LogActorCreation();
            this._ticketRoute = TicketsHelper.GetRandomRoute();
            this.Become(this.LookingForBrokersState);
        }

        #region private methods

        private void LookingForBrokersState()
        {
            Context.SetReceiveTimeout(TimeSpan.FromSeconds(10));

            this.GetAllBrokers();

            this.Receive<ReceiveTimeout>(message =>
            {
                this.LogReceiveMessageInfo(message);
                this.GetAllBrokers();
            });

            this.Receive<ReceiveAllBrokers>(message =>
            {
                this.LogReceiveMessageInfo(message);
                this.ReceiveAllBrokersMessageHandler(message);

            });

            this.Receive<RandomExceptionMessage>(message =>
            {
                this.LogReceiveMessageInfo(message);
                this.HandleRandomException(message, this.GetType());
            });
        }

        private void BookingTicketState()
        {
            Context.SetReceiveTimeout(null);

            this.BookTicketByBroker();

            Receive<TicketProviderConfirmationMessage>(message =>
            {
                this.LogReceiveMessageInfo(message);
                this.ReceiveTicketAndStop();
            });

            Receive<NoAvailableTicketMessage>(message =>
            {
                this.LogReceiveMessageInfo(message);
                this.KeepLookingForTicket();
            });

            Receive<RandomExceptionMessage>(message =>
            {
                this.LogReceiveMessageInfo(message);
                this.HandleRandomException(message, this.GetType());
            });
        }
        private void ReceiveAllBrokersMessageHandler(ReceiveAllBrokers message)
        {
            this._brokers = new Dictionary<Guid, IActorRef>(message.brokers);

            if (this._brokers.Count > 0)
            {
                this.Become(this.BookingTicketState);
            }
        }

        private void GetAllBrokers()
        {
            GetAllBrokersMessage getAllBrokersMessage = new GetAllBrokersMessage();
            ActorSelection brokerCoordinator = TicketBookingActorSystem.Instance.actorSystem.ActorSelection(ActorPaths.BrokerCoordinatorActor.Path);

            brokerCoordinator.Tell(getAllBrokersMessage);

            this.LogSendMessageInfo(getAllBrokersMessage, ActorPaths.BrokerCoordinatorActor.Path);
        }

        private void BookTicketByBroker()
        {
            // Get random broker from known brokers and remove him from known brokers.
            Guid randomBroker = this._brokers.ElementAt(RandomGenerator.Instance.random.Next(0, this._brokers.Count)).Key;
            IActorRef randomBrokerActor = this._brokers[randomBroker];
            this._brokers.Remove(randomBroker);

            BookTicketByBrokerMessage bookTicketByBrokerMessage = new BookTicketByBrokerMessage(Self, this.Id, this._ticketRoute);
            randomBrokerActor.Tell(bookTicketByBrokerMessage);

            this.LogSendMessageInfo(bookTicketByBrokerMessage, randomBrokerActor.Path.ToStringWithoutAddress());
        }

        private void KeepLookingForTicket()
        {
            if (this._brokers?.Count > 0)
                this.BookTicketByBroker();
            else
                this.Become(this.LookingForBrokersState);
        }

        private void ReceiveTicketAndStop()
        {
            LoggingConfiguration.Instance.LogTicketProviderBookingMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, this._ticketRoute, this.Id);

            Context.Stop(Self);
        }

        #endregion
    }
}
