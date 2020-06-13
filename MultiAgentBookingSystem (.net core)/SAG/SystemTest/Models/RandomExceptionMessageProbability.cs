using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAgentBookingSystem.SystemTest.Models
{
    public class RandomExceptionMessageProbability
    {
        public double UserCoordinatorActor { get; set; }
        public double BrokerCoordinatorActor { get; set; }
        public double TicketProviderCoordinatorActor { get; set; }
        public double UserActor { get; set; }
        public double BrokerActor { get; set; }
        public double TicketProviderActor { get; set; }
    }
}
