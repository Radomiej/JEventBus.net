using System;
using Javity.EventBus;
using Javity.EventBusTest.Events;

namespace Javity.EventBusTest.TestImplementation
{
    public class TestEventSubscriber : IRawSubscriber
    {
        public int EventCounter;
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