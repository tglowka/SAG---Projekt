using System;
using Akka.Actor;

namespace MultiAgentBookingSystem
{
    class Program
    {

        private static ActorSystem TicketBookingActorSystem;
        static void Main(string[] args)
        {
            TicketBookingActorSystem = ActorSystem.Create("TicketBookingActorSystem");

            Console.WriteLine("Hello World");
            Console.ReadKey();

            TicketBookingActorSystem.Terminate();
        }
    }
}
