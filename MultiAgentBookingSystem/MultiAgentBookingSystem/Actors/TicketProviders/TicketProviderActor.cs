using Akka.Actor;
using Akka.Event;
using MultiAgentBookingSystem.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAgentBookingSystem.Actors
{
    public class TicketProviderActor : ReceiveActor
    {
        private Guid Id { get; set; }

        public TicketProviderActor(Guid id)
        {
            this.Id = id;

            LoggingConfiguration.Instance.LogActorCreation(Context.GetLogger(), this.GetType(), Self.Path);
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
