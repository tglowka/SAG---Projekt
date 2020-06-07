using Akka.Actor;
using Akka.Event;
using MultiAgentBookingSystem.Actors.Common;
using MultiAgentBookingSystem.DataResources;
using MultiAgentBookingSystem.Logger;
using MultiAgentBookingSystem.Messages;
using MultiAgentBookingSystem.Messages.Abstracts;
using MultiAgentBookingSystem.Messages.Common;
using MultiAgentBookingSystem.System;
using System;
using System.Collections.Generic;
using System.Threading;

namespace MultiAgentBookingSystem.Actors
{
    public class UserCoordinatorActor : CustomActor<UserActor>
    {
        public UserCoordinatorActor()
        {
            this.Become(this.InitialState);
        }

        public UserCoordinatorActor(int childCount)
        {
            this.CreateChildActor(childCount);

            this.Become(this.InitialState);
        }

        #region private methods

        private void InitialState()
        {
            Receive<AddActorMessage>(message =>
            {
                LoggingConfiguration.Instance.LogReceiveMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, message.GetType(), Sender.Path.ToStringWithoutAddress());

                this.CreateChildActor(message.ActorCount);
            });

            Receive<AddRandomCountActorMessage>(message =>
            {
                LoggingConfiguration.Instance.LogReceiveMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, message.GetType(), Sender.Path.ToStringWithoutAddress());

                this.CreateChildActor(message.MinActorCount, message.MaxActorCount);
            });

            Receive<RemoveActorMessage>(message =>
            {
                LoggingConfiguration.Instance.LogReceiveMessageInfo(Context.GetLogger(), this.GetType(), Self.Path, message.GetType(), Sender.Path.ToStringWithoutAddress());

                this.RemoveChildActor(message.ActorId);
            });
        }

        #endregion

        #region protected methods
        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(
                null,
                null,
                localOnlyDecider: ex =>
                {
                    //                  switch (ex)
                    //                {
                    //case ArithmeticException ae:
                    //    return Directive.Resume;
                    //case NullReferenceException nre:
                    //    return Directive.Restart;
                    //case ArgumentException are:
                    //    Console.WriteLine("EEEEEEEERRRRRRRRRRROOOOOOOOORRRRRRRRRRRRRRRRRRR");
                    //    UserActorStopException userActorStopException = ex as UserActorStopException;
                    //    this.childrenActors.Remove(userActorStopException.Id);
                    //    return Directive.Stop;
                    //default:
                    //    return Directive.Escalate;
                    return Directive.Resume;
                    //                    }
                });
        }

        #endregion

        #region Lifecycle hooks

        protected override void PreStart()
        {
            LoggingConfiguration.Instance.LogActorPreStart(Context.GetLogger(), Self.Path);
        }

        protected override void PostStop()
        {
            LoggingConfiguration.Instance.LogActorPostStop(Context.GetLogger(), Self.Path);
        }

        protected override void PreRestart(Exception reason, object message)
        {
            LoggingConfiguration.Instance.LogActorPreRestart(Context.GetLogger(), Self.Path, reason);
            base.PreRestart(reason, message);
        }

        protected override void PostRestart(Exception reason)
        {
            LoggingConfiguration.Instance.LogActorPostRestart(Context.GetLogger(), Self.Path, reason);
            base.PostRestart(reason);
        }

        #endregion

    }
}
