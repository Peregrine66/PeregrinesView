using GalaSoft.MvvmLight.Messaging;
using System;

namespace Peregrine.WPF.ViewModel.Helpers
{
    public static class perMessageService
    {
        public static void RegisterMessageHandler<TMessage>(object recipient, Action<TMessage> action)
        {
            Messenger.Default.Register(recipient, action);
        }

        public static void SendMessage<TMessage>(TMessage message)
        {
            Messenger.Default.Send(message);
        }
    }
}
