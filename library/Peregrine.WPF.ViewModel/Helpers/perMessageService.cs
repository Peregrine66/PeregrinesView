using GalaSoft.MvvmLight.Messaging;
using System;

namespace Peregrine.WPF.ViewModel.Helpers
{
    /// <summary>
    /// A common facade over the designated messaging service - provides a single point of reference
    /// </summary>
    public static class perMessageService
    {
        /// <summary>
        /// Register the recipient object as a receiver for the specified message type,
        /// with an action to perform when a message is received.
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="recipient"></param>
        /// <param name="action"></param>
        public static void RegisterMessageHandler<TMessage>(object recipient, Action<TMessage> action)
        {
            Messenger.Default.Register(recipient, action);
        }

        /// <summary>
        /// Send a message to all registered receivers for the specified message type.
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="message"></param>
        public static void SendMessage<TMessage>(TMessage message)
        {
            Messenger.Default.Send(message);
        }
    }
}
