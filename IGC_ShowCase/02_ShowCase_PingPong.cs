using System;
using System.Text;
using System.Collections.Generic;

using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;

namespace IGC_ShowCase
{
    public sealed class ShowCase_PingPong : MyGridProgram
    {
        //#script begin

        public const string MY_TAG = "PING";
        //private const string MY_TAG = "PONG";

        private long OtherEndpointAddress { get; set; } = -1;

        public const string MESSAGE_CALLBACK = "NEW_MESSAGE";

        public LCDLogger Log { get; }
        public IMyTimerBlock Timer { get; }
        public IMyLightingBlock Light { get; }

        public ShowCase_PingPong() //#ctor
        {
            this.Timer = this.GetFirstBlockOfType<IMyTimerBlock>();
            this.Light = this.GetFirstBlockOfType<IMyLightingBlock>();
            this.Log = new LCDLogger(this.GetFirstBlockOfType<IMyTextPanel>());

            if(MY_TAG == "PING")
            {
                if(string.IsNullOrEmpty(this.Storage))
                {
                    //This is first run. Lets run timer to kick in fist callback
                    this.Timer.ApplyAction("Start");
                    this.Light.ApplyAction("OnOff_On");

                    this.Storage = "Initialized dummy";
                }
            }

            this.Log.WriteLine($"My address is: {this.IGC.Me}");
            this.IGC.UnicastListener.SetMessageCallback(MESSAGE_CALLBACK);
        }

        public void Main(string arg)
        {
            if(arg == MESSAGE_CALLBACK)
            {//I was called from message callback

                var message = this.IGC.UnicastListener.AcceptMessage();

                if(message.Data == null)
                {
                    //Should NEVER happen
                    throw new Exception("Message was null while notified by message callback!");
                }

                //Get data from message
                this.OtherEndpointAddress = message.Source;
                this.Log.WriteLine($"{message.Source} -> ME: {message.As<string>()}");

                //Schedule timer callback
                this.Timer.ApplyAction("Start");
                this.Light.ApplyAction("OnOff_On");
            }
            else
            {//I was called from timer block

                if(this.OtherEndpointAddress == -1)
                    throw new Exception("Please set address of other endpoint!");


                var succeeded = this.IGC.SendUnicastMessage(MY_TAG, this.OtherEndpointAddress);
                if(succeeded)
                {
                    //Message was successfully sent
                    this.Log.WriteLine($"ME -> {this.OtherEndpointAddress}: {MY_TAG}");
                    this.Light.ApplyAction("OnOff_Off");
                }
                else
                {
                    //Other side is currently unavailable. Let's try it later
                    this.Log.WriteLine($"ME -> {this.OtherEndpointAddress}: FAILED! Scheduling retry...");
                    this.Timer.ApplyAction("Start");
                }
            }
        }

        //#include<Misc/Utils.cs>
        //#script end
    }
}