using Akka.Actor;
using System;

namespace MultiAgentBookingSystem.Actors
{
    public class SystemSupervisorActor : ReceiveActor
    {
        private const string UserCoordiatorActorName = "UserCoordinator";
        private const string StatisticsActorActorName = "StatiscticsCoordinator";

        public SystemSupervisorActor()
        {
            Context.ActorOf(Props.Create<UserCoordinatorActor>(), SystemSupervisorActor.UserCoordiatorActorName);
            Context.ActorOf(Props.Create<StatisticsActor>(), SystemSupervisorActor.StatisticsActorActorName);
        }

        #region Lifecycle hooks

        protected override void PreStart()
        {
            ColorConsole.WriteLineColor("SystemSupervisorActor Prestart", ConsoleColor.Green);
        }

        protected override void PostStop()
        {
            ColorConsole.WriteLineColor("SystemSupervisorActor PostSTop", ConsoleColor.Green);
        }

        protected override void PreRestart(Exception reason, object message)
        {
            ColorConsole.WriteLineColor("SystemSupervisorActor PreRestart because: " + reason, ConsoleColor.Green);

            base.PreRestart(reason, message);
        }

        protected override void PostRestart(Exception reason)
        {
            ColorConsole.WriteLineColor("SystemSupervisorActor PostRestart because: " + reason, ConsoleColor.Green);

            base.PostRestart(reason);
        }

        #endregion
    }
}
