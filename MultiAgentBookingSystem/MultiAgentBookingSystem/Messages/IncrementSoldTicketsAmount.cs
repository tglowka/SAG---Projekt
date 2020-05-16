using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAgentBookingSystem.Messages
{
    public class IncrementSoldTicketsAmount
    {
        public string Route { get; private set; }

        public IncrementSoldTicketsAmount(string route)
        {
            this.Route = route;
        }
    }
}
