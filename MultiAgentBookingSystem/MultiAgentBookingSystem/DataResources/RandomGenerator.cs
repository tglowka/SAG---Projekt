using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAgentBookingSystem.DataResources
{
    /// <summary>
    ///     Singleton that returns random generaton instance.
    /// </summary>
    public sealed class RandomGenerator
    {
        private static readonly Lazy<RandomGenerator> instance = new Lazy<RandomGenerator>(() => new RandomGenerator(new Random()));

        public readonly Random random;
        private RandomGenerator(Random random)
        {
            this.random = random;
        }

        public static RandomGenerator Instance
        {
            get
            {
                return instance.Value;
            }
        }
    }
}
