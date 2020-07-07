﻿using System;

 namespace JEventBus.ResolverQueue
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