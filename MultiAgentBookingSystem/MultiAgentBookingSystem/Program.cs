using System;
using System.Configuration;
using Akka.Actor;
using MultiAgentBookingSystem.Actors;
using MultiAgentBookingSystem.Messages;
using MultiAgentBookingSystem.Logger;
using Serilog;
using MultiAgentBookingSystem.System;
using MultiAgentBookingSystem.Messages.Abstracts;
using System.Threading;
using System.Text;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using MultiAgentBookingSystem.SystemTest.Models;
using MultiAgentBookingSystem.SystemTest.Services;

namespace MultiAgentBookingSystem
{

    class Program
    {
        static void Main(string[] args)
        {

            SystemTestsService systemTestsService = new SystemTestsService();
            InputFile inputFile = systemTestsService.GetInputFIle("1.json");

            // Setup logging for the actor system
            LoggingConfiguration.Instance.SetupLogger();

            // Inititialize supervisor actor
            TicketBookingActorSystem.Instance.actorSystem.ActorOf(Props.Create<SystemSupervisorActor>(inputFile), "SystemSupervisor");

            Console.ReadKey();

            // Terminate actor system
            TicketBookingActorSystem.Instance.actorSystem.Terminate();
        }
    }
}
