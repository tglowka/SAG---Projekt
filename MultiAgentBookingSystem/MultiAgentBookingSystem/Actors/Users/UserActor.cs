using Akka.Actor;
using Akka.Event;
using MultiAgentBookingSystem.DataResources;
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
        private Dictionary<Guid, IActorRef> brokers = new Dictionary<Guid, IActorRef>();

        private string ticketRoute;

        public UserActor(Guid id)
        {
            this.id = id;
            this.ticketRoute = TicketsHelper.GetRandomRoute();

            Context.SetReceiveTimeout(TimeSpan.FromSeconds(3));

            Become(InitialState);

            this.BookTicketByBrooker();

            LoggingConfiguration.Instance.LogActorCreation(Context.GetLogger(), this.GetType(), Self.Path);
        }

        #region private methods

        private void InitialState()
        {
            Receive<ReceiveTimeout>(message =>
            {
                this.GetAllBrokers();
                LoggingConfiguration.Instance.LogReceiveMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, message.GetType());
            });

            Receive<ReceiveAllBrokers>(message =>
            {
                this.brokers = message.brokers;
                LoggingConfiguration.Instance.LogReceiveMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, message.GetType());
            });
        }

        private void GetAllBrokers()
        {
            TicketBookingActorSystem.Instance.actorSystem.ActorSelection("/user/SystemSupervisor/BrokerCoordinator").Tell(new GetAllBrokersMessage());
        }

        private void BookTicketByBrooker()
        {
            if (brokers.Count > 0)
            {
                // Get random broker from known brokers
                IActorRef randomBroker = this.brokers.ElementAt(RandomGenerator.Instance.random.Next(0, this.brokers.Count)).Value;

                randomBroker.Tell(new BookTicketByBrokerMessage(this.id, this.ticketRoute));
            }
            else
            {
                this.GetAllBrokers();
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
