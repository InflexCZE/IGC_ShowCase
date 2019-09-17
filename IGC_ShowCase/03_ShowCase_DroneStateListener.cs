using System;
using System.Text;
using System.Collections.Generic;

using VRageMath;
using Sandbox.ModAPI.Ingame;
using VRage;

namespace IGC_ShowCase
{
    public sealed class ShowCase_DroneStateListener : MyGridProgram
    {
        //#script begin

        public const string STATE_BROADCAST_TAG = "DRONE_STATE";

        public IMyTextPanel TextPanel { get; }
        public IMyBroadcastListener BroadcastListener { get; }
        public Dictionary<string, DroneInfo> Drones { get; } = new Dictionary<string, DroneInfo>();

        private StringBuilder Buffer = new StringBuilder();

        public ShowCase_DroneStateListener() //#ctor
        {
            this.TextPanel = this.GetClosestBlockOfType<IMyTextPanel>();

            this.BroadcastListener = this.IGC.RegisterBroadcastListener(STATE_BROADCAST_TAG);
            this.BroadcastListener.SetMessageCallback();
        }

        public void Main()
        {
            while(this.BroadcastListener.HasPendingMessage)
            {
                var message = this.BroadcastListener.AcceptMessage();

                //Notice no deserializetion is needed.
                // You can get your data easy like this:
                var data = message.As<MyTuple<string, Vector3D, Vector3D>>();

                var droneInfo = new DroneInfo
                {
                    Name = data.Item1,
                    Velocity = data.Item2,
                    Acceleration = data.Item3
                };

                this.Drones[droneInfo.Name] = droneInfo;
            }

            foreach(var x in this.Drones.Values)
            {
                this.Buffer .AppendLine(x.Name)
                            .AppendLine("Velocity: ").Append(x.Velocity.ToString("F2")).AppendLine(" m/s")
                            .Append("Acceleration: ").Append(x.Acceleration.ToString("F2")).AppendLine(" m/s2");

                this.Buffer.AppendLine();
            }

            this.TextPanel.WritePublicText(this.Buffer);
            this.Buffer.Clear();
        }

        public struct DroneInfo
        {
            public string Name { get; set; }
            public Vector3D Velocity { get; set; }
            public Vector3D Acceleration { get; set; }

            public override string ToString()
            {
                return this.Name + Environment.NewLine +
                       $"Velocity: {this.Velocity.ToString("F2")} m/s" + Environment.NewLine +
                       $"Acceleration: {this.Acceleration.ToString("F2")} m/s2" + Environment.NewLine;
            }
        }

        //#include<Misc/Utils.cs>
        //#script end
    }

}
