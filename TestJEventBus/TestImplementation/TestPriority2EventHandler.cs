using JEventBus;
using NUnit.Framework;
using TestJEventBus.Events;

namespace TestJEventBus.TestImplementation
{
    public class TestPriority2EventHandler
    {
        public readonly int AssertPriority = 2;

        [Subscribe(2)]
        public void TestEventListener(TestEventWithParam testEvent)
        {
            testEvent.Param++;
            Assert.AreEqual(AssertPriority, testEvent.Param);
        }
    }
}