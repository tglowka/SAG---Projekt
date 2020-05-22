using Akka.Actor;
using Akka.Event;
using MultiAgentBookingSystem.Logger;
using System;

namespace MultiAgentBookingSystem.Actors
{
    public class SystemSupervisorActor : ReceiveActor
    {
        private readonly string UserCoordiatorActorName = "UserCoordinator";
        private readonly string BrokerCoordiatorActorName = "BrokerCoordinator";
        private readonly string TicketProviderCoordinatorActorName = "TicketProviderCoordinator";

        public SystemSupervisorActor()
        {
            Context.ActorOf(Props.Create<UserCoordinatorActor>(), this.UserCoordiatorActorName);
            Context.ActorOf(Props.Create<BrokerCoordinatorActor>(), this.BrokerCoordiatorActorName);
            Context.ActorOf(Props.Create<TicketProviderCoordinatorActor>(), this.TicketProviderCoordinatorActorName);
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
