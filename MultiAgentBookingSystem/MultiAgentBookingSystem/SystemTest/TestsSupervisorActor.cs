using Akka.Actor;
using MultiAgentBookingSystem.Actors;
using MultiAgentBookingSystem.DataResources;
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
        private readonly string _userCoordinatorActorPath = ActorPaths.UserCoordinatorActor.Path;
        private readonly string _brokerCoordinatorActorPath = ActorPaths.BrokerCoordinatorActor.Path;
        private readonly string _ticketProviderCoordinatorActorPath = ActorPaths.TicketProviderCoordinatorActor.Path;
        private readonly string _userActors = ActorPaths.UserActors.Path;
        private readonly string _brokerActors = ActorPaths.BrokerActors.Path;
        private readonly string _ticketProviderActors = ActorPaths.TickerProviderActors.Path;


        private readonly InputFile InputFile;

        private SystemTestsService SystemTestsService;

        public TestsSupervisorActor(string inputFilesDirectory, string inputFileName)
        {
            this.SystemTestsService = new SystemTestsService();

            this.InputFile = this.SystemTestsService.GetInputFIle(inputFilesDirectory, inputFileName);

            this.StartSimulation();
        }

        private void StartSimulation()
        {
            this.SetupSingleRouteCount();
            this.SetupNewActorMessageSchedulers();
            this.SetupInitialActorCount();
            this.SetupRandomExceptionSchedulers();
        }

        #region private methods

        private void SetupInitialActorCount()
        {
            int initialUserActorCount = this.InputFile.InitialActorCount.UserActor;
            int initialBrokerActorCount = this.InputFile.InitialActorCount.BrokerActor;
            int initialTicketProviderActorCount = this.InputFile.InitialActorCount.TicketProviderActor;

            AddActorMessage addUserActorMessage = new AddActorMessage(initialUserActorCount);
            TicketBookingActorSystem.Instance.actorSystem.ActorSelection(this._userCoordinatorActorPath).Tell(addUserActorMessage);

            AddActorMessage addBrokerActorMessage = new AddActorMessage(initialBrokerActorCount);
            TicketBookingActorSystem.Instance.actorSystem.ActorSelection(this._brokerCoordinatorActorPath).Tell(addBrokerActorMessage);

            AddActorMessage addTicketProviderActorMessage = new AddActorMessage(initialTicketProviderActorCount);
            TicketBookingActorSystem.Instance.actorSystem.ActorSelection(this._ticketProviderCoordinatorActorPath).Tell(addTicketProviderActorMessage);
        }

        private void SetupNewActorMessageSchedulers()
        {
            this.SetupNewActorMessageScheduler(
                                                this.InputFile.NewActorCount.UserActor.MinCount,
                                                this.InputFile.NewActorCount.UserActor.MaxCount,
                                                this.InputFile.NewActorMessageInterval.UserActor,
                                                this._userCoordinatorActorPath
                                                );

            this.SetupNewActorMessageScheduler(
                                                this.InputFile.NewActorCount.BrokerActor.MinCount,
                                                this.InputFile.NewActorCount.BrokerActor.MaxCount,
                                                this.InputFile.NewActorMessageInterval.BrokerActor,
                                                this._brokerCoordinatorActorPath
                                                );

            this.SetupNewActorMessageScheduler(
                                                this.InputFile.NewActorCount.TicketProviderActor.MinCount,
                                                this.InputFile.NewActorCount.TicketProviderActor.MaxCount,
                                                this.InputFile.NewActorMessageInterval.TicketProviderActor,
                                                this._ticketProviderCoordinatorActorPath
                                                );
        }

        private void SetupNewActorMessageScheduler(int minCount, int maxCount, int interval, string coordinatorActorPath)
        {
            if (interval > 0)
            {
                AddRandomCountActorMessage addRandomCountActorMessage = new AddRandomCountActorMessage(minCount, maxCount);

                TicketBookingActorSystem.Instance.actorSystem.Scheduler.ScheduleTellRepeatedly(
                    TimeSpan.FromSeconds(0),
                    TimeSpan.FromSeconds(interval),
                    TicketBookingActorSystem.Instance.actorSystem.ActorSelection(coordinatorActorPath),
                    addRandomCountActorMessage,
                    Self);
            }
        }

        private void SetupRandomExceptionSchedulers()
        {
            this.SetupRandomExceptionScheduler(
                                                this.InputFile.RandomExceptionMessageProbability.UserCoordinatorActor,
                                                this.InputFile.RandomExceptionMessageInterval.UserCoordinatorActor,
                                                this._userCoordinatorActorPath
                                                );

            this.SetupRandomExceptionScheduler(
                                               this.InputFile.RandomExceptionMessageProbability.BrokerCoordinatorActor,
                                               this.InputFile.RandomExceptionMessageInterval.BrokerCoordinatorActor,
                                               this._brokerCoordinatorActorPath
                                               );

            this.SetupRandomExceptionScheduler(
                                               this.InputFile.RandomExceptionMessageProbability.TicketProviderCoordinatorActor,
                                               this.InputFile.RandomExceptionMessageInterval.TicketProviderCoordinatorActor,
                                               this._ticketProviderCoordinatorActorPath
                                               );

            this.SetupRandomExceptionScheduler(
                                               this.InputFile.RandomExceptionMessageProbability.UserActor,
                                               this.InputFile.RandomExceptionMessageInterval.UserActor,
                                               this._userActors
                                               );

            this.SetupRandomExceptionScheduler(
                                               this.InputFile.RandomExceptionMessageProbability.BrokerActor,
                                               this.InputFile.RandomExceptionMessageInterval.BrokerActor,
                                               this._brokerActors
                                               );

            this.SetupRandomExceptionScheduler(
                                               this.InputFile.RandomExceptionMessageProbability.TicketProviderActor,
                                               this.InputFile.RandomExceptionMessageInterval.TicketProviderActor,
                                               this._ticketProviderActors
                                               );
        }

        private void SetupRandomExceptionScheduler(double exceptionProbability, int interval, string coordinatorActorPath)
        {
            if (interval > 0)
            {
                RandomExceptionMessage randomExceptionMessage = new RandomExceptionMessage(exceptionProbability);

                TicketBookingActorSystem.Instance.actorSystem.Scheduler.ScheduleTellRepeatedly(
                    TimeSpan.FromSeconds(0),
                    TimeSpan.FromSeconds(interval),
                    TicketBookingActorSystem.Instance.actorSystem.ActorSelection(coordinatorActorPath),
                    randomExceptionMessage,
                    Self);
            }
        }

        private void SetupSingleRouteCount()
        {
            TicketsHelper.singleRouteCount = this.InputFile.InitiazlSingleRouteTicketsCount;
        }

        #endregion

    }
}
