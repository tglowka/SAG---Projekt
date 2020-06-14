using Akka.Actor;
using MultiAgentBookingSystem.Actors;
using MultiAgentBookingSystem.Logger;
using MultiAgentBookingSystem.System;
using MultiAgentBookingSystem.SystemTest;
using SAG.SystemTest;
using System;

namespace SAG
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Select Test Input File: ");
            
            string filename = Console.ReadLine();
                     
            TestsSupervisor testsSupervisor = new TestsSupervisor(@"/SystemTest/TestInputFiles/", filename);

            // Setup logging for the actor system
            LoggingConfiguration.Instance.SetupLogger();

            // Inititialize supervisor actor
            TicketBookingActorSystem.Instance.actorSystem.ActorOf(Props.Create<SystemSupervisorActor>(), "SystemSupervisor");

            TicketBookingActorSystem.Instance.actorSystem.ActorOf(Props.Create<TestsSupervisorActor>(testsSupervisor.InputFile), "TestsSupervisor");

            Console.ReadKey();

            // Terminate actor system
            TicketBookingActorSystem.Instance.actorSystem.Terminate();
        }
    }
}
