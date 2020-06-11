using Akka.Actor;
using MultiAgentBookingSystem.Actors;
using MultiAgentBookingSystem.Messages.Abstracts;
using MultiAgentBookingSystem.Messages.Common;
using MultiAgentBookingSystem.System;
using MultiAgentBookingSystem.SystemTest.Models;
using MultiAgentBookingSystem.SystemTest.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAgentBookingSystem.SystemTest
{
    public class TestsSupervisorActor : ReceiveActor
    {
        private readonly string userCoordinatorActorPath = ActorPaths.UserCoordinatorActor.Path;
        private readonly string brokerCoordinatorActorPath = ActorPaths.BrokerCoordinatorActor.Path;
        private readonly string ticketProviderCoordinatorActorPath = ActorPaths.TicketProviderCoordinatorActor.Path;

        private readonly string InputFilesDirectory;

        private readonly string InputFileName;

        private readonly InputFile InputFile;

        private SystemTestsService SystemTestsService;

        public TestsSupervisorActor(string inputFilesDirectory, string inputFileName)
        {
            this.SystemTestsService = new SystemTestsService();

            this.InputFilesDirectory = inputFilesDirectory;
            this.InputFileName = inputFileName;

            this.InputFile = this.SystemTestsService.GetInputFIle(this.InputFilesDirectory, this.InputFileName);

            this.StartSimulation();
        }

        public void StartSimulation()
        {
            this.SetupNewActorCreateScheduler();
            this.SetupInitialActorCount();
        }

        #region private methods

        private void SetupInitialActorCount()
        {
            int initialUserActorCount = this.InputFile.InitialActorCount.UserActor;
            int initialBrokerActorCount = this.InputFile.InitialActorCount.BrokerActor;
            int initialTicketProviderActorCount = this.InputFile.InitialActorCount.TicketProviderActor;

            AddActorMessage addUserActorMessage = new AddActorMessage(initialUserActorCount);
            TicketBookingActorSystem.Instance.actorSystem.ActorSelection(this.userCoordinatorActorPath).Tell(addUserActorMessage);

            AddActorMessage addBrokerActorMessage = new AddActorMessage(initialBrokerActorCount);
            TicketBookingActorSystem.Instance.actorSystem.ActorSelection(this.brokerCoordinatorActorPath).Tell(addBrokerActorMessage);

            AddActorMessage addTicketProviderActorMessage = new AddActorMessage(initialTicketProviderActorCount);
            TicketBookingActorSystem.Instance.actorSystem.ActorSelection(this.ticketProviderCoordinatorActorPath).Tell(addTicketProviderActorMessage);
        }

        private void SetupNewActorCreateScheduler()
        {
            int minCount, maxCount;

            int interval = this.InputFile.NewActorMessageInterval.UserActor;
            if (interval > 0)
            {
                minCount = this.InputFile.NewActorCount.UserActor.MinCount;
                maxCount = this.InputFile.NewActorCount.UserActor.MaxCount;

                AddRandomCountActorMessage addUserActorMessage = new AddRandomCountActorMessage(minCount, maxCount);

                TicketBookingActorSystem.Instance.actorSystem.Scheduler.ScheduleTellRepeatedly(
                    TimeSpan.FromSeconds(0),
                    TimeSpan.FromSeconds(interval),
                    TicketBookingActorSystem.Instance.actorSystem.ActorSelection(this.userCoordinatorActorPath),
                    addUserActorMessage,
                    Self);
            }

            interval = this.InputFile.NewActorMessageInterval.BrokerActor;
            if (interval > 0)
            {
                minCount = this.InputFile.NewActorCount.BrokerActor.MinCount;
                maxCount = this.InputFile.NewActorCount.BrokerActor.MaxCount;

                AddRandomCountActorMessage addBrokerActorMessage = new AddRandomCountActorMessage(minCount, maxCount);

                TicketBookingActorSystem.Instance.actorSystem.Scheduler.ScheduleTellRepeatedly(
                    TimeSpan.FromSeconds(0),
                    TimeSpan.FromSeconds(interval),
                    TicketBookingActorSystem.Instance.actorSystem.ActorSelection(this.brokerCoordinatorActorPath),
                    addBrokerActorMessage,
                    Self);
            }

            interval = this.InputFile.NewActorMessageInterval.TicketProviderActor;
            if (interval > 0)
            {
                minCount = this.InputFile.NewActorCount.TicketProviderActor.MinCount;
                maxCount = this.InputFile.NewActorCount.TicketProviderActor.MaxCount;

                AddRandomCountActorMessage addTicketProviderActorMessage = new AddRandomCountActorMessage(minCount, maxCount);

                TicketBookingActorSystem.Instance.actorSystem.Scheduler.ScheduleTellRepeatedly(
                    TimeSpan.FromSeconds(0),
                    TimeSpan.FromSeconds(interval),
                    TicketBookingActorSystem.Instance.actorSystem.ActorSelection(this.ticketProviderCoordinatorActorPath),
                    addTicketProviderActorMessage,
                    Self);
            }
        }

        #endregion

    }
}
