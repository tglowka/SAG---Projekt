using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAgentBookingSystem.System
{
    public static class ActorPaths
    {
        public static readonly ActorMetaData SystemSupervisorActor = new ActorMetaData(SystemConstants.SystemSupervisorActorName);
        public static readonly ActorMetaData UserCoordinatorActor = new ActorMetaData(SystemConstants.UserCoordinatorActorName, ActorPaths.SystemSupervisorActor);
        public static readonly ActorMetaData BrokerCoordinatorActor = new ActorMetaData(SystemConstants.BrokerCoordinatorActorName, ActorPaths.SystemSupervisorActor);
        public static readonly ActorMetaData TicketProviderCoordinatorActor = new ActorMetaData(SystemConstants.TicketProviderCoordinatorActorName, ActorPaths.SystemSupervisorActor);
    }
}
