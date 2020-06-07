using Akka.Actor;
using Akka.Event;
using MultiAgentBookingSystem.DataResources;
using MultiAgentBookingSystem.Logger;
using MultiAgentBookingSystem.Messages.Abstracts;
using MultiAgentBookingSystem.Messages.Common;
using MultiAgentBookingSystem.System;
using MultiAgentBookingSystem.SystemTest.Models;
using System;

namespace MultiAgentBookingSystem.Actors
{
    public class SystemSupervisorActor : ReceiveActor
    {
        private readonly string UserCoordiatorActorName = "UserCoordinator";
        private readonly string BrokerCoordiatorActorName = "BrokerCoordinator";
        private readonly string TicketProviderCoordinatorActorName = "TicketProviderCoordinator";

        private IActorRef UserCoordinatorActor;
        private IActorRef BrokerCoordinatorActor;
        private IActorRef TicketProviderCoordinatorActor;

        public SystemSupervisorActor()
        {
            this.UserCoordinatorActor = Context.ActorOf(Props.Create(() => new UserCoordinatorActor()), this.UserCoordiatorActorName);
            this.BrokerCoordinatorActor = Context.ActorOf(Props.Create(() => new BrokerCoordinatorActor()), this.BrokerCoordiatorActorName);
            this.TicketProviderCoordinatorActor = Context.ActorOf(Props.Create(() => new TicketProviderCoordinatorActor()), this.TicketProviderCoordinatorActorName);
        }

        public SystemSupervisorActor(int userActorCount, int brokerActorCount, int ticketProviderActorCount)
        {
            this.UserCoordinatorActor = Context.ActorOf(Props.Create(() => new UserCoordinatorActor(userActorCount)), this.UserCoordiatorActorName);
            this.BrokerCoordinatorActor = Context.ActorOf(Props.Create(() => new BrokerCoordinatorActor(brokerActorCount)), this.BrokerCoordiatorActorName);
            this.TicketProviderCoordinatorActor = Context.ActorOf(Props.Create(() => new TicketProviderCoordinatorActor(ticketProviderActorCount)), this.TicketProviderCoordinatorActorName);
        }

        public SystemSupervisorActor(InputFile inputFile)
        {
            this.UserCoordinatorActor = Context.ActorOf(Props.Create(() => new UserCoordinatorActor(inputFile.InitialActorCount.UserActor)), this.UserCoordiatorActorName);
            this.BrokerCoordinatorActor = Context.ActorOf(Props.Create(() => new BrokerCoordinatorActor(inputFile.InitialActorCount.BrokerActor)), this.BrokerCoordiatorActorName);
            this.TicketProviderCoordinatorActor = Context.ActorOf(Props.Create(() => new TicketProviderCoordinatorActor(inputFile.InitialActorCount.TicketProviderActor)), this.TicketProviderCoordinatorActorName);

            this.SetupNewActorCreateScheduler(inputFile);
        }

        #region private methods

        private void SetupNewActorCreateScheduler(InputFile inputFile)
        {
            int minCount, maxCount;

            int interval = inputFile.NewActorMessageInterval.UserActor;
            if (interval > 0)
            {
                minCount = inputFile.NewActorCount.UserActor.MinCount;
                maxCount = inputFile.NewActorCount.UserActor.MaxCount;

                AddRandomCountActorMessage addUserActorMessage = new AddRandomCountActorMessage(minCount, maxCount);

                TicketBookingActorSystem.Instance.actorSystem.Scheduler.ScheduleTellRepeatedly(
                    TimeSpan.FromSeconds(0),
                    TimeSpan.FromSeconds(interval),
                    this.UserCoordinatorActor,
                    addUserActorMessage,
                    Self);
            }

            interval = inputFile.NewActorMessageInterval.BrokerActor;
            if (interval > 0)
            {
                minCount = inputFile.NewActorCount.BrokerActor.MinCount;
                maxCount = inputFile.NewActorCount.BrokerActor.MaxCount;

                AddRandomCountActorMessage addBrokerActorMessage = new AddRandomCountActorMessage(minCount, maxCount);

                TicketBookingActorSystem.Instance.actorSystem.Scheduler.ScheduleTellRepeatedly(
                    TimeSpan.FromSeconds(0),
                    TimeSpan.FromSeconds(interval),
                    this.BrokerCoordinatorActor,
                    addBrokerActorMessage,
                    Self);
            }

            interval = inputFile.NewActorMessageInterval.TicketProviderActor;
            if (interval > 0)
            {
                minCount = inputFile.NewActorCount.TicketProviderActor.MinCount;
                maxCount = inputFile.NewActorCount.TicketProviderActor.MaxCount;

                AddRandomCountActorMessage addTicketProviderActorMessage = new AddRandomCountActorMessage(minCount, maxCount);

                TicketBookingActorSystem.Instance.actorSystem.Scheduler.ScheduleTellRepeatedly(
                    TimeSpan.FromSeconds(0),
                    TimeSpan.FromSeconds(interval),
                    this.TicketProviderCoordinatorActor,
                    addTicketProviderActorMessage,
                    Self);
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
