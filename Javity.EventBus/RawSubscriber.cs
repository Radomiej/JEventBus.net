﻿using System;

 namespace Javity.EventBus
{
    public class RawSubscriber<T> : IRawSubscriber where T : class
    {
        private readonly Action<T> _handler;
        private readonly int _priority;
        
        public RawSubscriber(Action<T> handler, int priority = 0)
        {
            _handler = handler;
            this._priority = priority;
        }
        public void SubscribeRaw(object incomingEvent)
        {
            Subscribe(incomingEvent as T);
        }

        public void Subscribe(T incomingEvent)
        {
            _handler(incomingEvent);
        }

        public Type GetEventType()
        {
            return GetType().GetGenericArguments()[0];
        }

        public int GetPriority()
        {
            return _priority;
        }
    }
}