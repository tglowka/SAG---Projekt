using MultiAgentBookingSystem.DataResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAgentBookingSystem.Helpers
{
    public static class Helpers
    {
        public static T RandomElement<T>(this IEnumerable<T> source)
        {
            T current = default(T);
            int count = 0;
            foreach (T element in source)
            {
                count++;
                if (RandomGenerator.Next(count) == 0)
                {
                    current = element;
                }
            }
            if (count == 0)
            {
                throw new InvalidOperationException("Sequence was empty");
            }
            return current;
        }
    }
}
