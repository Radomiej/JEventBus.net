using Javity.EventBus;
using Javity.EventBusTest.Events;
using NUnit.Framework;

namespace Javity.EventBusTest.TestImplementation
{
    public class TestPriority1EventHandler
    {
        public readonly int AssertPriority = 1;

        [Subscribe(1)]
        public void TestEventListener(TestEventWithParam testEvent)
        {
            testEvent.Param++;
            Assert.AreEqual(AssertPriority, testEvent.Param);
        }
    }
}