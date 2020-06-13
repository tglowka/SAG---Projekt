﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAgentBookingSystem.Messages.Common
{
    public class RandomExceptionMessage
    {
        public double ExceptionProbability { get; private set; }

        public RandomExceptionMessage(double exceptionProbability)
        {
            this.ExceptionProbability = exceptionProbability;
        }
    }
}
