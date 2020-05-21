using System;
using System.Configuration;
using Akka.Actor;
using MultiAgentBookingSystem.Actors;
using MultiAgentBookingSystem.Messages;
using MultiAgentBookingSystem.Logger;
using Serilog;
using MultiAgentBookingSystem.System;

namespace MultiAgentBookingSystem
{

    class Program
    {
        static void Main(string[] args)
        {
            // Setup logging for the actor system
            LoggingConfiguration.Instance.SetupLogger();

            // Inititialize supervisor actor
            TicketBookingActorSystem.Instance.ActorOf(Props.Create<SystemSupervisorActor>(), "SystemSupervisor");

            // Temporary testing action
            var addBrokerMessage = new AddBrokerActorMessage("1");
            var addBrokerMessage2 = new AddBrokerActorMessage("11");

            var addTicketProviderMessage = new AddTicketProviderActorMessage("1");

            var addUserMessage1 = new AddUserActorMessage("1");


            TicketBookingActorSystem.Instance.ActorSelection("/user/SystemSupervisor/BrokerCoordinator").Tell(addBrokerMessage);
            TicketBookingActorSystem.Instance.ActorSelection("/user/SystemSupervisor/BrokerCoordinator").Tell(addBrokerMessage2);

            TicketBookingActorSystem.Instance.ActorSelection("/user/SystemSupervisor/TicketProviderCoordinator").Tell(addTicketProviderMessage);

            TicketBookingActorSystem.Instance.ActorSelection("/user/SystemSupervisor/UserCoordinator").Tell(addUserMessage1);

            Console.ReadKey();

            // Terminate actor system
            TicketBookingActorSystem.Instance.Terminate();
        }


    }
}
