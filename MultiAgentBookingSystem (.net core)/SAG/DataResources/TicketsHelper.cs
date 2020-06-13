using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAgentBookingSystem.DataResources
{
    public static class TicketsHelper
    {
        /// <summary>
        ///     Max ticket count on particular route.
        /// </summary>
        public static int singleRouteCount = 0;

        /// <summary>
        ///     List of the possible routes that user actor is able to book.
        /// </summary>
        private static List<string> possibleRoutes = new List<string>()
        {
            "A-B",
            "A1-B10",
            "A2-B9",
            "A3-B8",
            "A4-B7",
            "A5-B6",
            "A6-B5",
            "A7-B4",
            "A8-B3",
            "A9-B2",
            "A10-B1"
        };

        /// <summary>
        ///     Get random route from the list of the possible routes.
        /// </summary>
        /// <returns>Random route from possible routes.</returns>
        public static string GetRandomRoute()
        {
            return possibleRoutes[RandomGenerator.Instance.random.Next(possibleRoutes.Count)];
        }

        /// <summary>
        ///     Get random tickets (route name, number of tickets for that route) that particular ticket provider offers to the customers (user actors).
        /// </summary>
        /// <returns>Dictionary of random routes and number of available tickets for that route.</returns>
        public static Dictionary<string, int> GetRandomOfferedTickets()
        {
            Dictionary<string, int> offeredTickets = new Dictionary<string, int>();

            for (int i = 0; i < possibleRoutes.Count; ++i)
            {
                offeredTickets.Add(possibleRoutes[i], singleRouteCount);
            }

            return offeredTickets;
        }
    }
}
