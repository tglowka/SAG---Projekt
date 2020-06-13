using Akka.Actor;
using MultiAgentBookingSystem.Actors;
using MultiAgentBookingSystem.Logger;
using MultiAgentBookingSystem.System;
using MultiAgentBookingSystem.SystemTest;
using System;

namespace SAG
{
    class Program
    {
        static void Main(string[] args)
        {
            // Setup logging for the actor system
            LoggingConfiguration.Instance.SetupLogger();

            // Inititialize supervisor actor
            TicketBookingActorSystem.Instance.actorSystem.ActorOf(Props.Create<SystemSupervisorActor>(), "SystemSupervisor");

            TicketBookingActorSystem.Instance.actorSystem.ActorOf(Props.Create<TestsSupervisorActor>(@"\SystemTest\TestInputFiles\", "1.json"), "TestsSupervisor");

            Console.ReadKey();

            // Terminate actor system
            TicketBookingActorSystem.Instance.actorSystem.Terminate();
        }
    }
}
