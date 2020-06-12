using Akka.Actor;
using Akka.Event;
using MultiAgentBookingSystem.Actors.Common;
using MultiAgentBookingSystem.DataResources;
using MultiAgentBookingSystem.Logger;
using MultiAgentBookingSystem.Messages;
using MultiAgentBookingSystem.Messages.Abstracts;
using MultiAgentBookingSystem.Messages.Brokers;
using MultiAgentBookingSystem.Messages.Common;
using MultiAgentBookingSystem.Messages.TicketProviders;
using MultiAgentBookingSystem.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAgentBookingSystem.Actors
{
    public class TicketProviderCoordinatorActor : CoordinatoActor<TicketProviderActor>
    {
        public TicketProviderCoordinatorActor()
        {
            this.SetupBookedTicketsCountScheduler(5);

            this.Become(this.InitialState);
        }

        public TicketProviderCoordinatorActor(int childCount)
        {
            this.CreateChildActor(childCount);

            this.Become(this.InitialState);
        }

        #region private methods

        private void InitialState()
        {
            Receive<AddActorMessage>(message =>
            {
                LoggingConfiguration.Instance.LogReceiveMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, message.GetType(), Sender.Path.ToStringWithoutAddress());

                this.CreateChildActor(message.ActorCount);
                this.SendAllTicketProvidersToAllBrokers();
            });

            Receive<AddRandomCountActorMessage>(message =>
            {
                LoggingConfiguration.Instance.LogReceiveMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, message.GetType(), Sender.Path.ToStringWithoutAddress());

                this.CreateChildActor(message.MinActorCount, message.MaxActorCount);
                this.SendAllTicketProvidersToAllBrokers();
            });

            Receive<RemoveActorMessage>(message =>
            {
                LoggingConfiguration.Instance.LogReceiveMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, message.GetType(), Sender.Path.ToStringWithoutAddress());

                this.RemoveChildActor(message.ActorId);
            });

            Receive<GetAllTicketProvidersMessage>(message =>
           {
               LoggingConfiguration.Instance.LogReceiveMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, message.GetType(), Sender.Path.ToStringWithoutAddress());

               this.SendAllTicketProviders();
           });

            Receive<LogChildernCountMessage>(message =>
            {
                this.LogChildrenCount(this.GetType(), Self.Path);
            });

            Receive<RandomExceptionMessage>(message =>
            {
                this.HandleRandomException(message, this.GetType());
            });
        }

        private void SendAllTicketProviders()
        {
            ReceiveAllTicketProvidersMessage receiveAllTicketProvidersMessage = new ReceiveAllTicketProvidersMessage(Context.GetChildren());

            Sender.Tell(receiveAllTicketProvidersMessage);
        }

        private void SendAllTicketProvidersToAllBrokers()
        {
            ReceiveAllTicketProvidersMessage receiveAllTicketProvidersMessage = new ReceiveAllTicketProvidersMessage(Context.GetChildren());

            TicketBookingActorSystem.Instance.actorSystem.ActorSelection(ActorPaths.BrokerActors.Path).Tell(receiveAllTicketProvidersMessage);
        }

        private void SetupBookedTicketsCountScheduler(int interval)
        {
            LogBookedTicketCountMessage logBookedTicketCountMessage = new LogBookedTicketCountMessage();

            TicketBookingActorSystem.Instance.actorSystem.Scheduler.ScheduleTellRepeatedly(
                    TimeSpan.FromSeconds(0),
                    TimeSpan.FromSeconds(interval),
                    TicketBookingActorSystem.Instance.actorSystem.ActorSelection(ActorPaths.TickerProviderActors.Path),
                    logBookedTicketCountMessage,
                    Self);
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
