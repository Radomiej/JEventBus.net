﻿using System;

 namespace Javity.EventBus
{
    public class PriorityDelegate : IComparable
    {
        protected readonly int Priority;
        public readonly Delegate Handler;
        public readonly bool PerformanceMode;

        public PriorityDelegate(int priority, Delegate handler) : this(priority, handler, false)
        {
        }

        public PriorityDelegate(int priority, Delegate handler, bool performanceMode)
        {
            Priority = priority;
            Handler = handler;
            PerformanceMode = performanceMode;
        }


        public int CompareTo(object obj)
        {
            if(obj is PriorityDelegate other)
            {
                return Priority - other.Priority ;
            }

            throw new NotSupportedException("Cannot compare PriorityDelegate with other type");
        }

        public Delegate GetHandler()
        {
            return Handler;
        }
    }
}