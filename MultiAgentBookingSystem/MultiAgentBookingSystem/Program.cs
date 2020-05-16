using System;
using Akka.Actor;
using MultiAgentBookingSystem.Actors;
using MultiAgentBookingSystem.Messages;

namespace MultiAgentBookingSystem
{
    class Program
    {
        private static ActorSystem TicketBookingActorSystem;
        static void Main(string[] args)
        {
            Console.WriteLine("Creating TicketBookingActorSystem");
            TicketBookingActorSystem = ActorSystem.Create("TicketBookingActorSystem");

            Console.WriteLine("Creating actor supervisory hierarchy");
            TicketBookingActorSystem.ActorOf(Props.Create<SystemSupervisorActor>(), "SystemSupervisor");
            //TicketBookingActorSystem.ActorOf(Props.Create<SystemSupervisorActor>(), "SystemSupervisor2");

            var addUserMessage = new AddUserActorMessage("1");
            TicketBookingActorSystem.ActorSelection("/user/SystemSupervisor/UserCoordinator").Tell(addUserMessage);          
            var addBrokerMessage = new AddBrokerActorMessage("1");
            TicketBookingActorSystem.ActorSelection("/user/SystemSupervisor/BrokerCoordinator").Tell(addBrokerMessage); 
            var addTicketProviderMessage = new AddTicketProviderActorMessage("1");
            TicketBookingActorSystem.ActorSelection("/user/SystemSupervisor/TicketProviderCoordinator").Tell(addTicketProviderMessage);

            var removeUserMessage = new RemoveUserActorMessage("1");
            TicketBookingActorSystem.ActorSelection("/user/SystemSupervisor/UserCoordinator").Tell(removeUserMessage); 
            var removeBrokerMessage = new RemoveBrokerActorMessage("1");
            TicketBookingActorSystem.ActorSelection("/user/SystemSupervisor/BrokerCoordinator").Tell(removeBrokerMessage);
            var removeTicketProviderMessage = new RemoveTicketProviderActorMessage("1");
            TicketBookingActorSystem.ActorSelection("/user/SystemSupervisor/TicketProviderCoordinator").Tell(removeTicketProviderMessage);

            Console.WriteLine("Hello World");
            Console.ReadKey();

            TicketBookingActorSystem.Terminate();
        }
    }
}
