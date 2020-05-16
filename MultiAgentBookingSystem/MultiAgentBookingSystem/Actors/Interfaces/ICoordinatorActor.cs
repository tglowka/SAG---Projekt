using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAgentBookingSystem.Interfaces
{
    public interface ICoordinatorActor
    {
        /// <summary>
        /// Dictionary that stores all actor's children references.
        /// </summary>
        Dictionary<string, IActorRef> childrenActors { get; set; }

        /// <summary>
        /// Create the child actor.
        /// </summary>
        /// <param name="actorId">Child actor identifier.</param>
        void CreateChildActor(string actorId);

        /// <summary>
        /// Remove the child actor.
        /// </summary>
        /// <param name="actorId">Child actor identifier.</param>
        void RemoveChildActor(string actorId);
    }
}
