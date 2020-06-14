using Akka.Actor;
using Akka.Event;
using MultiAgentBookingSystem.DataResources;
using MultiAgentBookingSystem.Exceptions.Common;
using MultiAgentBookingSystem.Logger;
using MultiAgentBookingSystem.Messages.Common;
using MultiAgentBookingSystem.SystemTest;
using SAG.SystemTest;
using SAG.SystemTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiAgentBookingSystem.Actors.Common
{
    public abstract class CoordinatorChildActor : LogActor
    {
        protected Guid Id { get; }

        protected CoordinatorChildActor(Guid id)
        {
            this.Id = id;
        }

        protected void Delay()
        {
            this.DelayMessageProcessing(
                InputFileAdditionalOptions.SystemDelays.CoordinatorsChildren.MinCount,
                InputFileAdditionalOptions.SystemDelays.CoordinatorsChildren.MaxCount,
                InputFileAdditionalOptions.SystemDelays.CoordinatorsChildrenProbability
                );
        }
    }
}
