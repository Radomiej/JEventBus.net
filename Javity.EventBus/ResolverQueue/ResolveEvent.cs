﻿using System;

 namespace Javity.EventBus.ResolverQueue
{
    public class ResolveEvent
    {
        public readonly Type EventTypeToResolve;

        public ResolveEvent(Type eventTypeToResolve)
        {
            this.EventTypeToResolve = eventTypeToResolve;
        }
    }
}