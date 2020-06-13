using Akka.Actor;
using Akka.Event;
using MultiAgentBookingSystem.Actors.Common;
using MultiAgentBookingSystem.DataResources;
using MultiAgentBookingSystem.Exceptions.Common;
using MultiAgentBookingSystem.Logger;
using MultiAgentBookingSystem.Messages;
using MultiAgentBookingSystem.Messages.Abstracts;
using MultiAgentBookingSystem.Messages.Common;
using MultiAgentBookingSystem.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MultiAgentBookingSystem.Actors
{
    public class UserCoordinatorActor : CoordinatoActor<UserActor>
    {
        public UserCoordinatorActor()
        {
            this.Become(this.InitialState);
        }

        #region private methods

        private void InitialState()
        {
            Receive<AddActorMessage>(message =>
            {
                this.LogReceiveMessageInfo(message);
                this.CreateChildActor(message.ActorCount);
            });

            Receive<AddRandomCountActorMessage>(message =>
            {
                this.LogReceiveMessageInfo(message);
                this.CreateChildActor(message);
            });

            Receive<RemoveActorMessage>(message =>
            {
                this.LogReceiveMessageInfo(message);
                this.RemoveChildActor(message.ActorId);
            });

            Receive<LogChildernCountMessage>(message =>
            {
                this.LogReceiveMessageInfo(message);
                this.LogChildrenCount(this.GetType(), Self.Path);
            });

            Receive<RandomExceptionMessage>(message =>
            {
                this.LogReceiveMessageInfo(message);
                this.HandleRandomException(message, this.GetType());
            });
        }

        #endregion

    }
}
