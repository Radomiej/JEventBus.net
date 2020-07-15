using Javity.EventBus;
using Javity.EventBusTest.Events;
using NUnit.Framework;

namespace Javity.EventBusTest.TestImplementation
{
    public class TestPriority2EventHandler
    {
        private readonly int AssertPriority = 2;

        [Subscribe(2)]
        public void TestEventListener(TestEventWithParam testEvent)
        {
            testEvent.Param++;
            Assert.AreEqual(AssertPriority, testEvent.Param);
        }
    }
}