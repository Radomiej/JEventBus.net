using System;
using JEventBus;
using TestJEventBus.Events;

namespace TestJEventBus.TestImplementation
{
    public class TestEventSubscriber : IRawSubscriber
    {
        public int EventCounter = 0;
        public void SubscribeRaw(object incomingEvent)
        {
           EventCounter++;
        }

        public Type GetEventType()
        {
            return typeof(TestEvent);
        }

        public int GetPriority()
        {
            return 0;
        }
    }
}