using JEventBus;
using NUnit.Framework;
using TestJEventBus.Events;

namespace TestJEventBus.TestImplementation
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