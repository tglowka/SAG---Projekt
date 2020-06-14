using MultiAgentBookingSystem.SystemTest.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SAG.SystemTest.Models
{
    public class SystemDelays
    {
        public MinMaxCount Coordinators { get; set; }
        public MinMaxCount CoordinatorsChildren { get; set; }
        public double CoordinatorsProbability { get; set; }
        public double CoordinatorsChildrenProbability { get; set; }
    }
}
