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

            var message = new AddUserActorMessage(1);
            TicketBookingActorSystem.ActorSelection("/user/SystemSupervisor/UserCoordinator").Tell(message);

            var message2 = new RemoveUserActorMessage(1);
            TicketBookingActorSystem.ActorSelection("/user/SystemSupervisor/UserCoordinator").Tell(message2);

            Console.WriteLine("Hello World");
            Console.ReadKey();

            TicketBookingActorSystem.Terminate();
        }
    }
}
