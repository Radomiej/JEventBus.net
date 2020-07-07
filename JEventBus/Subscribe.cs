﻿using System;

 namespace JEventBus
{
    [AttributeUsage(AttributeTargets.Method)]
    public class Subscribe : Attribute
    {
        private readonly int _priority;

        public Subscribe() : this(0)
        {
        }

        public Subscribe(int priority)
        {
            this._priority = priority;
        }

        public int priority => _priority;
    }
}