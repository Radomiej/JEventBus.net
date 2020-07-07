using JEventBus;
using TestJEventBus.Events;

namespace TestJEventBus.TestImplementation
{
    public class TestEventHandler
    {
        public int EventCounter = 0;
        
        [Subscribe]
        public void TestEventListener(TestEvent testEvent)
        {
            EventCounter++;
        }
    }
}