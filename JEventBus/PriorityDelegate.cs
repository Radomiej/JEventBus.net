﻿using System;

 namespace JEventBus
{
    public class PriorityDelegate : IComparable
    {
        public readonly int Priority;
        public readonly Delegate Handler;
        public readonly bool PerformanceMode = false;
        
        public PriorityDelegate(int priority, Delegate handler, bool performanceMode = false)
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
    }
}