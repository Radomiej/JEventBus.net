﻿using System;

 namespace Javity.EventBus.ResolverQueue
{
    public class AddEventToResolve
    {
        public readonly Type EventTypeToResolve;

        public AddEventToResolve(Type eventTypeToResolve)
        {
            this.EventTypeToResolve = eventTypeToResolve;
        }
    }
}