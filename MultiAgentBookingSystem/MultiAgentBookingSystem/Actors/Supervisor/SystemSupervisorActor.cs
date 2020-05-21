using Akka.Actor;
using System;

namespace MultiAgentBookingSystem.Actors
{
    public class SystemSupervisorActor : ReceiveActor
    {
        private const string UserCoordiatorActorName = "UserCoordinator";
        private const string BrokerCoordiatorActorName = "BrokerCoordinator";
        private const string TIcketProviderCoordinatorActorName = "TicketProviderCoordinator";

        public SystemSupervisorActor()
        {
            Context.ActorOf(Props.Create<UserCoordinatorActor>(), SystemSupervisorActor.UserCoordiatorActorName);
            Context.ActorOf(Props.Create<BrokerCoordinatorActor>(), SystemSupervisorActor.BrokerCoordiatorActorName);
            Context.ActorOf(Props.Create<TicketProviderCoordinatorActor>(), SystemSupervisorActor.TIcketProviderCoordinatorActorName);
        }

        #region Lifecycle hooks

        protected override void PreStart()
        {
            ColorConsole.WriteLineColor($"{this.GetType().Name} Prestart", ConsoleColor.Green);
        }

        protected override void PostStop()
        {
            ColorConsole.WriteLineColor($"{this.GetType().Name} PostSTop", ConsoleColor.Green);
        }

        protected override void PreRestart(Exception reason, object message)
        {
            ColorConsole.WriteLineColor($"{this.GetType().Name} PreRestart because: " + reason, ConsoleColor.Green);

            base.PreRestart(reason, message);
        }

        protected override void PostRestart(Exception reason)
        {
            ColorConsole.WriteLineColor($"{this.GetType().Name} PostRestart because: " + reason, ConsoleColor.Green);

            base.PostRestart(reason);
        }

        #endregion
    }
}
