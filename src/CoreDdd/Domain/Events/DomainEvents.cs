﻿using CoreIoC;
using CoreUtils.Extensions;

namespace CoreDdd.Domain.Events
{
    public static class DomainEvents
    {
        public static void RaiseEvent<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : IDomainEvent
        {
            var domainEventHandlers = IoC.ResolveAll<IDomainEventHandler<TDomainEvent>>();
            if (domainEventHandlers.IsEmpty()) throw new MissingDomainEventHandlerException("No domain event handler for " + domainEvent);
            domainEventHandlers.Each(e => e.Handle(domainEvent));
        }
    }
}
