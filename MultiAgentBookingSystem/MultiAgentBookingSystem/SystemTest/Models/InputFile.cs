using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAgentBookingSystem.SystemTest.Models
{
    public class InputFile
    {
        public InitialActorCount InitialActorCount { get; set; }
        public NewActorMessageInterval NewActorMessageInterval { get; set; }
        public NewActorCount NewActorCount { get; set; }
        public RandomExceptionMessageProbability RandomExceptionMessageProbability { get; set; }
        public RandomExceptionMessageInterval RandomExceptionMessageInterval { get; set; }
        public int InitiazlSingleRouteTicketsCount { get; set; }
    }
}
