using Akka.Actor;
using Akka.Event;
using MultiAgentBookingSystem.DataResources;
using MultiAgentBookingSystem.Logger;
using MultiAgentBookingSystem.Messages;
using MultiAgentBookingSystem.Messages.Brokers;
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
        private Guid Id { get; set; }

        private string ticketRoute;

        public UserActor(Guid id)
        {
            this.Id = id;
            this.ticketRoute = TicketsHelper.GetRandomRoute();

            LoggingConfiguration.Instance.LogActorCreation(Context.GetLogger(), this.GetType(), Self.Path);
        }

        private void BookTicketByBrooker()
        {
            //TicketBookingActorSystem.Instance.actorSystem.ActorSelection("/user/SystemSupervisor/BrokerCoordinator").Tell(new BookTicketByBrokerMessage());
        }

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
