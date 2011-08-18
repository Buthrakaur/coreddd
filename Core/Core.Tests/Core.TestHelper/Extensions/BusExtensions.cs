﻿using System.Collections.Generic;
using System.Linq;
using NServiceBus;
using Rhino.Mocks;

namespace Core.TestHelper.Extensions
{
    public static class BusExtensions
    {
        public static TMessage MessageShouldHaveBeenSent<TMessage>(this IBus bus) where TMessage : class, IMessage, new()
        {
            bus.AssertWasCalled(a => a.Send(Arg<IMessage[]>.Is.Anything));
            return ((IMessage[])bus.GetArgumentsForCallsMadeOn(a => a.Send(Arg<IMessage[]>.Is.Anything))[0][0])[0] as TMessage;
        }

        public static IEnumerable<TMessage> MessagesShouldHaveBeenSentLocally<TMessage>(this IBus bus) where TMessage : class, IMessage, new()
        {
            var allMessages = bus.GetArgumentsForCallsMadeOn(a => a.SendLocal(Arg<IMessage[]>.Is.Anything));
            var flatAllMessages = allMessages.SelectMany(x => x).SelectMany(x => x as IMessage[]);
            return flatAllMessages.Where(p => p is TMessage).Select(s => s as TMessage);
        }
    }
}
