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

namespace MultiAgentBookingSystem
{

    class Program
    {
        static void Main(string[] args)
        {
            // Setup logging for the actor system
            LoggingConfiguration.Instance.SetupLogger();

            // Inititialize supervisor actor
            TicketBookingActorSystem.Instance.actorSystem.ActorOf(Props.Create<SystemSupervisorActor>(), "SystemSupervisor");

            Thread.Sleep(2000);

            // Temporary testing action
            var addBrokerMessage = new AddActorMessage(Guid.NewGuid());
            var addBrokerMessage2 = new AddActorMessage(Guid.NewGuid());

            var addTicketProviderMessage = new AddActorMessage(Guid.NewGuid());
            var addTicketProviderMessage2 = new AddActorMessage(Guid.NewGuid());
            var addTicketProviderMessage3 = new AddActorMessage(Guid.NewGuid());

            var addUserMessage1 = new AddActorMessage(Guid.NewGuid());
            var addUserMessage2 = new AddActorMessage(Guid.NewGuid());
            var addUserMessage3 = new AddActorMessage(Guid.NewGuid());


            TicketBookingActorSystem.Instance.actorSystem.ActorSelection("/user/SystemSupervisor/BrokerCoordinator").Tell(addBrokerMessage);
            TicketBookingActorSystem.Instance.actorSystem.ActorSelection("/user/SystemSupervisor/BrokerCoordinator").Tell(addBrokerMessage2);

            TicketBookingActorSystem.Instance.actorSystem.ActorSelection("/user/SystemSupervisor/TicketProviderCoordinator").Tell(addTicketProviderMessage);
            TicketBookingActorSystem.Instance.actorSystem.ActorSelection("/user/SystemSupervisor/TicketProviderCoordinator").Tell(addTicketProviderMessage3);

            TicketBookingActorSystem.Instance.actorSystem.ActorSelection("/user/SystemSupervisor/UserCoordinator").Tell(addUserMessage1);
            TicketBookingActorSystem.Instance.actorSystem.ActorSelection("/user/SystemSupervisor/UserCoordinator").Tell(addUserMessage2);
            TicketBookingActorSystem.Instance.actorSystem.ActorSelection("/user/SystemSupervisor/UserCoordinator").Tell(addUserMessage3);

            Console.ReadKey();

            // Terminate actor system
            TicketBookingActorSystem.Instance.actorSystem.Terminate();
        }


    }
}
