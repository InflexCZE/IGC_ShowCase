using System;
using Sandbox.ModAPI.Ingame;

namespace IGC_ShowCase
{
    public sealed class ShowCase_Echo : MyGridProgram
    {
        //#script begin

        public ShowCase_Echo() //#ctor
        {
            //This is my address. Anyone conneted to same antenna relay as me can send me direct message on this addresss.
            long myIGCAddress = this.IGC.Me;

            //This is your unicast listener. You should check your mail so it doesn't go to spam ;)
            IMyUnicastListener myUnicastListener = this.IGC.UnicastListener;

            //Let's setup message callback with empty argument
            myUnicastListener.SetMessageCallback();
        }

        //Activated message callback invokes this method each time we get new message
        public void Main()
        {
            //Check if there is message waiting
            if(this.IGC.UnicastListener.HasPendingMessage == false)
                return;

            //Accept waiting message
            MyIGCMessage message = this.IGC.UnicastListener.AcceptMessage();

            //This is sender's direct address. We can use it to send direct response right back
            long senderAddress = message.Source;

            //And some data from message
            string data = message.As<string>();

            //Let's log message ...
            this.Echo($"Received message from: {senderAddress}{Environment.NewLine}{data}");

            //... and echo the data right back to sender
            this.IGC.SendUnicastMessage(data, message.Source);
        }

        //#script end
    }
}
