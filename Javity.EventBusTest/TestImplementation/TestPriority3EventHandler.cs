using Javity.EventBus;
using Javity.EventBusTest.Events;
using NUnit.Framework;

namespace Javity.EventBusTest.TestImplementation
{
    public class TestPriority3EventHandler
    {
        private readonly int AssertPriority = 1;

        [Subscribe(3)]
        public void TestEventListener(TestEventWithParam testEvent)
        {
            testEvent.Param++;
            Assert.AreEqual(AssertPriority, testEvent.Param);
        }
    }
}