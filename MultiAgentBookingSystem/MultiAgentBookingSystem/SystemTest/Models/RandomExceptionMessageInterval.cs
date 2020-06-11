using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAgentBookingSystem.SystemTest.Models
{
    public class RandomExceptionMessageInterval
    {
        public int UserCoordinatorActor { get; set; }
        public int BrokerCoordinatorActor { get; set; }
        public int TicketProviderCoordinatorActor { get; set; }
        public int UserActor { get; set; }
        public int BrokerActor { get; set; }
        public int TicketProviderActor { get; set; }
    }
}
