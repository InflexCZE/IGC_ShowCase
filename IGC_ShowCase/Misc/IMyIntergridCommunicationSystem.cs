using System;
using System.Collections.Generic;

namespace Sandbox.ModAPI.Ingame
{
    /// <summary>
    /// This is the entry point for all communication operations.
    /// </summary>
    public interface IMyIntergridCommunicationSystem
    {
        /// <summary>
        /// Gets communication endpoint for current programmable block.
        /// </summary>
        long Me { get; }

        /// <summary>
        /// Gets unicast listener for current programmable block.
        /// </summary>
        IMyUnicastListener UnicastListener { get; }

        /// <summary>
        /// Determines if given endpoint is currently reachable. Similar to sending ICMP message.
        /// </summary>
        bool IsEndpointReachable(long address);

        /// <summary>
        /// Registers broadcast listener with given tag for current programmable block. 
        /// In case there is already another active broadcast lister with same tag it return this one.
        /// </summary>
        /// <param name="tag">String tag broadcast listener should listen for.</param>
        /// <returns>Active broadcast listener for given tag.</returns>
        IMyBroadcastListener RegisterBroadcastListener(string tag);

        /// <summary>
        /// Disables given broadcast listener. In case given broadcast listener is not active nothing happens.
        /// Instance of this broadcast listener remains valid and all pending messages may be accepted as normal.
        /// Disabling broadcast listener also disables it's message callback if activated.
        /// </summary>
        /// <param name="broadcastListener">Broadcast listener which should be deactivated.</param>
        void DisableBroadcastListener(IMyBroadcastListener broadcastListener);

        /// <summary>
        /// Retrieves list of all active broadcast listeners and listeners with pending messages registered by current programmable block.
        /// Returned list is snapshot of current state and is not updated by future operations.
        /// </summary>
        /// <returns>List or all active broadcast listeners and listeners with pending messages.</returns>
        void GetBroadcastListeners(List<IMyBroadcastListener> broadcastListeners, Func<IMyBroadcastListener, bool> collect = null);

        /// <summary>
        /// Sends broadcast message with given content and tag. 
        /// This is fire and forget operation and cannot fail.
        /// Only broadcast listeners listening to this tag will accept this message.
        /// Important: Message will be delivered only to currently reachable IGC endpoints.
        /// </summary>
        /// <param name="data">Message data to be send.</param>
        /// <param name="tag">Tag of broadcast listeners this message should be accepted by.</param>
        void SendBroadcastMessage<TData>(TData data, string tag);

        /// <summary>
        /// Sends unicast message with given content to the given IGC endpoint.
        /// This operation may fail in case the given IGC endpoint is currently unreachable.
        /// </summary>
        /// <param name="data">Message data to be send.</param>
        /// <param name="addressee">IGC endpoint to send this message to.</param>
        /// <returns></returns>
        bool SendUnicastMessage<TData>(TData data, long addressee);
    }

    /// <summary>
    /// Each received message is provided to script as instance of object implementing this interface.
    /// </summary>
    public struct MyIGCMessage
    {
        /// <summary>
        /// Gets the data received in message.
        /// </summary>
        public readonly object Data;

        /// <summary>
        /// Gets source/author of this message.
        /// </summary>
        public readonly long Source;

        public MyIGCMessage(object data, long source)
        {
            this.Data = data;
            this.Source = source;
        }

        public TData As<TData>()
        {
            return (TData) this.Data;
        }
    }

    /// <summary>
    /// Base interface for all message providers.
    /// </summary>
    public interface IMyMessageProvider
    {
        /// <summary>
        /// Determines whether there is a message waiting to be accepted by this message provider or not.
        /// </summary>
        bool IsMessageWaiting { get; }

        /// <summary>
        /// Indicates number of max messages waiting in queue before the oldest one will be dropped to make space for new one.
        /// </summary>
        int MaxWaitingMessages { get; }

        /// <summary>
        /// Accepts first message from pending message queue for this message provider.
        /// </summary>
        /// <returns>First message from pending message queue, default(<see cref="MyIGCMessage"/>) if there are no messages awaiting to be accepted.</returns>
        MyIGCMessage AcceptMessage();

        /// <summary>
        /// Disables registered message callback.
        /// </summary>
        void DisableMessageCallback();

        /// <summary>
        /// Whenever given message provider obtains new message respective programmable block gets called with provided argument.
        /// Each raised callback argument will be called only once per simulation tick!
        /// </summary>
        /// <param name="argument"></param>
        void SetMessageCallback(string argument = "");
    }

    /// <summary>
    /// Broadcast listeners scan the network for broadcasted messages with specific tag.
    /// </summary>
    public interface IMyBroadcastListener : IMyMessageProvider
    {
        /// <summary>
        /// Gets the tag this broadcast listener is listening for.
        /// </summary>
        string Tag { get; }

        /// <summary>
        /// Gets a value that indicates whether the broadcast listener is active.
        /// </summary>
        bool IsActive { get; }
    }

    /// <summary>
    /// Unicast listener hooks up all messages addressed directly to this endpoint.
    /// </summary>
    public interface IMyUnicastListener : IMyMessageProvider
    { }
}