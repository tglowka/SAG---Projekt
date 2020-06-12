﻿using Akka.Actor;
using Akka.Event;
using MultiAgentBookingSystem.DataResources;
using MultiAgentBookingSystem.Exceptions.Common;
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
        private readonly int _logChildrenCountMessageInterval = 5;

        public IActorRef UserCoordinatorActor { get; private set; }
        public IActorRef BrokerCoordinatorActor { get; private set; }
        public IActorRef TicketProviderCoordinatorActor { get; private set; }

        public SystemSupervisorActor()
        {
            this.UserCoordinatorActor = Context.ActorOf(Props.Create(() => new UserCoordinatorActor()), SystemConstants.UserCoordinatorActorName);
            this.BrokerCoordinatorActor = Context.ActorOf(Props.Create(() => new BrokerCoordinatorActor()), SystemConstants.BrokerCoordinatorActorName);
            this.TicketProviderCoordinatorActor = Context.ActorOf(Props.Create(() => new TicketProviderCoordinatorActor()), SystemConstants.TicketProviderCoordinatorActorName);

            this.SetupSchedulers();
        }

        private void SetupSchedulers()
        {
            this.SetupChildrenCountScheduler(this._logChildrenCountMessageInterval, this.UserCoordinatorActor);
            this.SetupChildrenCountScheduler(this._logChildrenCountMessageInterval, this.BrokerCoordinatorActor);
            this.SetupChildrenCountScheduler(this._logChildrenCountMessageInterval, this.TicketProviderCoordinatorActor);
        }

        private void SetupChildrenCountScheduler(int interval, IActorRef actor)
        {
            LogChildernCountMessage logChildernCountMessage = new LogChildernCountMessage();

            TicketBookingActorSystem.Instance.actorSystem.Scheduler.ScheduleTellRepeatedly(
                    TimeSpan.FromSeconds(0),
                    TimeSpan.FromSeconds(interval),
                    actor,
                    logChildernCountMessage,
                    Self);
        }

        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(
                localOnlyDecider: ex =>
                {
                    switch (ex)
                    {
                        case RandomException randomException:
                            return Directive.Resume;
                        default:
                            LoggingConfiguration.Instance.LogExceptionMessageWarning(Context.GetLogger(), this.GetType(), "Unknown actor", ex.GetType());
                            return Directive.Resume;
                    }
                });
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
