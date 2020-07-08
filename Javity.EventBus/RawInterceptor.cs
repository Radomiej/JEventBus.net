﻿using System;

 namespace Javity.EventBus
{
    public class RawInterceptor : IRawInterceptor
    {
        private readonly Action<object> _handler;
        private readonly int _priority;

        public RawInterceptor(Action<object> handler, int priority = 0)
        {
            _handler = handler;
            this._priority = priority;
        }

        public void SubscribeRaw(object incomingEvent)
        {
            _handler(incomingEvent);
        }
        public int GetPriority()
        {
            return _priority;
        }
    }
}