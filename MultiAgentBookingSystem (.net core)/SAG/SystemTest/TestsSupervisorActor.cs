﻿using Akka.Actor;
using MultiAgentBookingSystem.Actors;
using MultiAgentBookingSystem.DataResources;
using MultiAgentBookingSystem.Logger;
using MultiAgentBookingSystem.Messages.Abstracts;
using MultiAgentBookingSystem.Messages.Common;
using MultiAgentBookingSystem.System;
using MultiAgentBookingSystem.SystemTest.Models;
using MultiAgentBookingSystem.SystemTest.Services;
using SAG.SystemTest.Models;
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

        private readonly InputFile _inputFile;

        private SystemTestsService _systemTestsService;

        public TestsSupervisorActor(InputFile inputFile)
        {

            this._systemTestsService = new SystemTestsService();
            this._inputFile = inputFile;

            this.StartSimulation();
        }

        private void StartSimulation()
        {
            this.SetupNewActorMessageSchedulers();
            this.SetupInitialActorCount();
            this.SetupRandomExceptionSchedulers();
        }

        #region private methods

        private void SetupInitialActorCount()
        {
            int initialUserActorCount = this._inputFile.InitialActorCount.UserActor;
            int initialBrokerActorCount = this._inputFile.InitialActorCount.BrokerActor;
            int initialTicketProviderActorCount = this._inputFile.InitialActorCount.TicketProviderActor;

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
                                                this._inputFile.NewActorCount.UserActor.MinCount,
                                                this._inputFile.NewActorCount.UserActor.MaxCount,
                                                this._inputFile.NewActorMessageInterval.UserActor,
                                                this._userCoordinatorActorPath
                                                );

            this.SetupNewActorMessageScheduler(
                                                this._inputFile.NewActorCount.BrokerActor.MinCount,
                                                this._inputFile.NewActorCount.BrokerActor.MaxCount,
                                                this._inputFile.NewActorMessageInterval.BrokerActor,
                                                this._brokerCoordinatorActorPath
                                                );

            this.SetupNewActorMessageScheduler(
                                                this._inputFile.NewActorCount.TicketProviderActor.MinCount,
                                                this._inputFile.NewActorCount.TicketProviderActor.MaxCount,
                                                this._inputFile.NewActorMessageInterval.TicketProviderActor,
                                                this._ticketProviderCoordinatorActorPath
                                                );
        }

        private void SetupNewActorMessageScheduler(int minCount, int maxCount, int interval, string coordinatorActorPath)
        {
            if (interval > 0)
            {
                AddRandomCountActorMessage addRandomCountActorMessage = new AddRandomCountActorMessage(minCount, maxCount);

                TicketBookingActorSystem.Instance.actorSystem.Scheduler.ScheduleTellRepeatedly(
                    TimeSpan.FromSeconds(interval),
                    TimeSpan.FromSeconds(interval),
                    TicketBookingActorSystem.Instance.actorSystem.ActorSelection(coordinatorActorPath),
                    addRandomCountActorMessage,
                    Self);
            }
        }

        private void SetupRandomExceptionSchedulers()
        {
            this.SetupRandomExceptionScheduler(
                                                this._inputFile.RandomExceptionMessageProbability.UserCoordinatorActor,
                                                this._inputFile.RandomExceptionMessageInterval.UserCoordinatorActor,
                                                this._userCoordinatorActorPath
                                                );

            this.SetupRandomExceptionScheduler(
                                               this._inputFile.RandomExceptionMessageProbability.BrokerCoordinatorActor,
                                               this._inputFile.RandomExceptionMessageInterval.BrokerCoordinatorActor,
                                               this._brokerCoordinatorActorPath
                                               );

            this.SetupRandomExceptionScheduler(
                                               this._inputFile.RandomExceptionMessageProbability.TicketProviderCoordinatorActor,
                                               this._inputFile.RandomExceptionMessageInterval.TicketProviderCoordinatorActor,
                                               this._ticketProviderCoordinatorActorPath
                                               );

            this.SetupRandomExceptionScheduler(
                                               this._inputFile.RandomExceptionMessageProbability.UserActor,
                                               this._inputFile.RandomExceptionMessageInterval.UserActor,
                                               this._userActors
                                               );

            this.SetupRandomExceptionScheduler(
                                               this._inputFile.RandomExceptionMessageProbability.BrokerActor,
                                               this._inputFile.RandomExceptionMessageInterval.BrokerActor,
                                               this._brokerActors
                                               );

            this.SetupRandomExceptionScheduler(
                                               this._inputFile.RandomExceptionMessageProbability.TicketProviderActor,
                                               this._inputFile.RandomExceptionMessageInterval.TicketProviderActor,
                                               this._ticketProviderActors
                                               );
        }

        private void SetupRandomExceptionScheduler(double exceptionProbability, int interval, string coordinatorActorPath)
        {
            if (interval > 0)
            {
                RandomExceptionMessage randomExceptionMessage = new RandomExceptionMessage(exceptionProbability);

                TicketBookingActorSystem.Instance.actorSystem.Scheduler.ScheduleTellRepeatedly(
                    TimeSpan.FromSeconds(interval),
                    TimeSpan.FromSeconds(interval),
                    TicketBookingActorSystem.Instance.actorSystem.ActorSelection(coordinatorActorPath),
                    randomExceptionMessage,
                    Self);
            }
        }

        #endregion

    }
}
