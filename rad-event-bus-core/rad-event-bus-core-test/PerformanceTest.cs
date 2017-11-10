using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rad.EventBus.Tests.Events.Subscribers;
using System;

namespace Rad.EventBus.Tests.Events
{
    [TestClass]
    public class PerformanceTest
    {
        [TestMethod]
        public void MassiveEventSenderTest()
        {
            SubscriptionCounter subscriptionCounter = new SubscriptionCounter();
            RadBus bus = RadBus.GetDefault();


            long milliseconds = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            bus.Register(subscriptionCounter);
            FakeOneEvent fakeOneEvent = new FakeOneEvent();
            for (int i = 0; i < 100000; i++)
            {
                bus.Post(fakeOneEvent);
            }
            bool validTest = subscriptionCounter.fakeOneCount == 100000 ? true : false;
            long executingTime = DateTimeOffset.Now.ToUnixTimeMilliseconds() - milliseconds;
            Console.WriteLine("execute time: " + executingTime);
        }

        [TestMethod]
        public void MassiveEventSenderWithMassiveSubscriberTest()
        {
            RadBus bus = RadBus.GetDefault();

            long milliseconds = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            for (int i = 0; i < 100000; i++)
            {
                bus.Register(new SubscriptionCounter());
            }
            FakeOneEvent fakeOneEvent = new FakeOneEvent();
            for (int i = 0; i < 100000; i++)
            {
                bus.Post(fakeOneEvent);
            }
            long executingTime = DateTimeOffset.Now.ToUnixTimeMilliseconds() - milliseconds;
            Console.WriteLine("execute time: " + executingTime);
        }
    }
}
