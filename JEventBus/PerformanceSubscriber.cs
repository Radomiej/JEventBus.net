using System;

namespace JEventBus
{
    public class PerformanceSubscriber<T> : PriorityDelegate, IPerformanceSubscriber
    {
        private readonly Type _subscribeType;
        private readonly Action<T> _handler;

        public PerformanceSubscriber(Action<T> handler, int priority = 0) : base(priority, null, true)
        {
            _handler = handler;
            _subscribeType = typeof(T);
        }

        public Type GetEventType()
        {
            return _subscribeType;
        }

        public void Subscribe(T incomingEvent)
        {
            _handler(incomingEvent);
        }

        public void SubscribeRaw(object eventObject)
        {
            Subscribe((T) eventObject);
        }

        public int GetPriority()
        {
            return Priority;
        }
    }
}