using Javity.EventBus;
using Javity.EventBusTest.Events;

namespace Javity.EventBusTest.TestImplementation
{
    public class TestEventHandler
    {
        public int EventCounter;
        
        [Subscribe]
        public void TestEventListener(TestEvent testEvent)
        {
            EventCounter++;
        }
    }
}