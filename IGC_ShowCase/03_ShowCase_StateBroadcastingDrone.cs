using System;

using VRage;
using VRageMath;
using Sandbox.ModAPI.Ingame;

namespace IGC_ShowCase
{
    public class ShowCase_StateBroadcastingDrone : MyGridProgram
    {
        //#script begin

        public const string DRONE_NAME = "Autonomous drone 1";
        public const string DRONE_STATE_BROADCAST_TAG = "DRONE_STATE";

        public readonly Random Random = new Random();
        public readonly Vector3D GridSize = new Vector3D(-100, 100, 100);
        public readonly Vector3D BaseGridPosition = new Vector3D(-100, 10, 50);

        public IMyRemoteControl RemoteBlock { get; }
        public IMyIntergridCommunicationSystem IGC { get; }

        public Vector3D LastSpeed;

        public ShowCase_StateBroadcastingDrone() //#ctor
        {
            this.RemoteBlock = this.GetFirstBlockOfType<IMyRemoteControl>();
        }

        public void Main()
        {
            Vector3D currentSpeed = this.RemoteBlock.GetShipVelocities().LinearVelocity;
            Vector3D speedDifference = this.LastSpeed - currentSpeed;
            Vector3D acceleration = speedDifference * this.Runtime.TimeSinceLastRun.TotalSeconds;
            this.LastSpeed = currentSpeed;

            //Please notice that there is no serialization going on!
            // You can directly pass your data in and let the communication system do the dirt work for you
            var message = MyTuple.Create
            (
                DRONE_NAME,
                currentSpeed,
                acceleration
            );
            this.IGC.SendBroadcastMessage(message, DRONE_STATE_BROADCAST_TAG);

            if(this.RemoteBlock.IsAutoPilotEnabled == false)
            {
                SetRandomRCTarget();
            }
        }

        public void SetRandomRCTarget()
        {
            this.RemoteBlock.ClearWaypoints();

            var rnd = this.Random;
            var baseOffset = this.GridSize * new Vector3D(rnd.NextDouble(), rnd.NextDouble(), rnd.NextDouble());
            this.RemoteBlock.AddWaypoint(this.BaseGridPosition + baseOffset, "Target");
            this.RemoteBlock.SetAutoPilotEnabled(true);
        }

        //#include<Misc/Utils.cs>
        //#script end
    }

}