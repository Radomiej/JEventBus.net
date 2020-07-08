using System;
using System.Diagnostics;
using System.Reflection;
using Javity.EventBus;
using Javity.EventBus.Exceptions;
using Javity.EventBus.ResolverQueue;
using Javity.EventBusTest.Events;
using Javity.EventBusTest.TestImplementation;
using NUnit.Framework;

namespace Javity.EventBusTest
{
    [TestFixture]
    public class JEventBusTest
    {
        [SetUp]
        public void Setup()
        {
            JEventBus.GetDefault().ClearAll();
        }

        [Test]
        public void RegisterCustomSubscriberRaw()
        {
            var eventSubscriber = new TestEventSubscriber();
            JEventBus.GetDefault().Register(this, eventSubscriber);

            var testEvent = new TestEvent();
            for (int i = 0; i < 100; i++)
            {
                JEventBus.GetDefault().Post(testEvent);
            }

            Assert.AreEqual(100, eventSubscriber.EventCounter);
        }

        [Test]
        public void RegisterCustomSubscriber()
        {
            int counter = 0;
            var subscriber = new RawSubscriber<TestEvent>(myEvent => counter++);
            JEventBus.GetDefault().Register(this, subscriber);

            var testEvent = new TestEvent();
            for (int i = 0; i < 100; i++)
            {
                JEventBus.GetDefault().Post(testEvent);
            }

            Assert.AreEqual(100, counter);
        }

        [Test]
        public void RegisterCustomSubscriberWithPriority()
        {
            var subscriber1 = new RawSubscriber<TestEventWithParam>(myEvent =>
            {
                myEvent.Param++;
                Assert.AreEqual(1, myEvent.Param);
            });
            var subscriber2 = new RawSubscriber<TestEventWithParam>(myEvent =>
            {
                myEvent.Param++;
                Assert.AreEqual(2, myEvent.Param);
            }, 1);
            var subscriber3 = new RawSubscriber<TestEventWithParam>(myEvent =>
            {
                myEvent.Param++;
                Assert.AreEqual(3, myEvent.Param);
            }, 2);
            JEventBus.GetDefault().Register(this, subscriber3);
            JEventBus.GetDefault().Register(this, subscriber2);
            JEventBus.GetDefault().Register(this, subscriber1);

            for (int i = 0; i < 100; i++)
            {
                var testEvent = new TestEventWithParam();
                JEventBus.GetDefault().Post(testEvent);
                Assert.AreEqual(3, testEvent.Param);
            }
        }

        [Test]
        public void TestExceptionPropagation()
        {
            var subscriberAborted =
                new RawSubscriber<TestEventWithParam>(myEvent => { throw new NotSupportedException(); }, 2);
            JEventBus.GetDefault().Register(this, subscriberAborted);
            var toBeAbortedEvent = new TestEventWithParam();
            Assert.Catch<TargetInvocationException>(() =>
                JEventBus.GetDefault().Post(toBeAbortedEvent));
        }


        [Test]
        public void TestStage()
        {
            int iteration = 100;


            var subscriber1 = new RawSubscriber<TestEventWithParam>(myEvent => { myEvent.Param++; });
            var subscriber2 = new RawSubscriber<TestEventWithParam>(myEvent => { myEvent.Param++; }, 1);
            var subscriber3 = new RawSubscriber<TestEventWithParam>(myEvent => { myEvent.Param++; }, 3);
            JEventBus.GetDefault().Register(this, subscriber3);
            JEventBus.GetDefault().Register(this, subscriber2);
            JEventBus.GetDefault().BeginStage();
            JEventBus.GetDefault().Register(this, subscriber1);


            for (int i = 0; i < iteration; i++)
            {
                var testEvent = new TestEventWithParam();
                JEventBus.GetDefault().Post(testEvent);
                Assert.AreEqual(3, testEvent.Param);
            }

            JEventBus.GetDefault().CloseStage();

            for (int i = 0; i < iteration; i++)
            {
                var testEvent = new TestEventWithParam();
                JEventBus.GetDefault().Post(testEvent);
                Assert.AreEqual(2, testEvent.Param);
            }
        }

        [Test]
        public void TestInterceptors()
        {
            int iteration = 100;
            int unhandled = 0;
            int aborted = 0;


            var subscriber1 = new RawSubscriber<TestEventWithParam>(myEvent =>
            {
                myEvent.Param++;
                Assert.AreEqual(1, myEvent.Param);
            });
            var subscriber2 = new RawSubscriber<TestEventWithParam>(myEvent =>
            {
                myEvent.Param++;
                Assert.AreEqual(2, myEvent.Param);
            }, 1);
            var subscriber3 = new RawSubscriber<TestEventWithParam>(myEvent =>
            {
                myEvent.Param++;
                Assert.AreEqual(3, myEvent.Param);
            }, 3);
            JEventBus.GetDefault().Register(this, subscriber3);
            JEventBus.GetDefault().Register(this, subscriber2);
            JEventBus.GetDefault().Register(this, subscriber1);

            JEventBus.GetDefault().AddInterceptor(new RawInterceptor(o =>
            {
                if (o is TestEventWithParam eventWithParam)
                {
                    Assert.AreEqual(0, (o as TestEventWithParam).Param);
                }
            }));

            JEventBus.GetDefault()
                .AddInterceptor(new RawInterceptor(o => { Assert.AreEqual(3, (o as TestEventWithParam).Param); }),
                    JEventBus.InterceptorType.Post);

            JEventBus.GetDefault().AddInterceptor(new RawInterceptor(o =>
            {
                aborted++;
                Assert.AreEqual(2, (o as TestEventWithParam).Param);
            }), JEventBus.InterceptorType.Aborted);

            JEventBus.GetDefault().AddInterceptor(new RawInterceptor(o => { unhandled++; }),
                JEventBus.InterceptorType.Unhandled);

            var unhandledTestEvent = new TestEvent();
            for (int i = 0; i < iteration; i++)
            {
                var testEvent = new TestEventWithParam();
                JEventBus.GetDefault().Post(testEvent);
                JEventBus.GetDefault().Post(unhandledTestEvent);
                Assert.AreEqual(3, testEvent.Param);
            }

            Assert.AreEqual(iteration, unhandled);
            Assert.AreEqual(0, aborted);

            var subscriberAborted =
                new RawSubscriber<TestEventWithParam>(myEvent => { throw new StopPropagationException(); }, 2);
            JEventBus.GetDefault().Register(this, subscriberAborted);
            var toBeAbortedEvent = new TestEventWithParam();
            JEventBus.GetDefault().Post(toBeAbortedEvent);
            Assert.AreEqual(1, aborted);
            Assert.AreEqual(2, toBeAbortedEvent.Param);
        }

        [Test]
        public void RegisterClassicSubscriberWithPriority()
        {
            var subscriber1 = new TestPriority1EventHandler();
            var subscriber2 = new TestPriority2EventHandler();
            var subscriber3 = new TestPriority3EventHandler();

            JEventBus.GetDefault().Register(subscriber3);
            JEventBus.GetDefault().Register(subscriber2);
            JEventBus.GetDefault().Register(subscriber1);

            for (int i = 0; i < 100; i++)
            {
                var testEvent = new TestEventWithParam();
                JEventBus.GetDefault().Post(testEvent);
                Assert.AreEqual(3, testEvent.Param);
            }
        }

        [Test]
        public void RegisterClassicSubscriberWithPriority2()
        {
            var subscriber1 = new TestPriority1EventHandler();
            var subscriber2 = new TestPriority2EventHandler();
            var subscriber3 = new TestPriority3EventHandler();

            JEventBus.GetDefault().Register(subscriber1);
            JEventBus.GetDefault().Register(subscriber2);
            JEventBus.GetDefault().Register(subscriber3);

            for (int i = 0; i < 100; i++)
            {
                var testEvent = new TestEventWithParam();
                JEventBus.GetDefault().Post(testEvent);
                Assert.AreEqual(3, testEvent.Param);
            }
        }

        [Test]
        public void TestEventResolverQueue()
        {
            int counter = 0;
            var subscriber = new RawSubscriber<TestEvent>(myEvent => counter++);
            JEventBus.GetDefault().Register(this, subscriber);
            var resolverQueue = new ResolverQueue(JEventBus.GetDefault());
            JEventBus.GetDefault().Post(new AddEventToResolve(typeof(TestEvent)));

            var testEvent = new TestEvent();
            for (int i = 0; i < 100; i++)
            {
                JEventBus.GetDefault().Post(testEvent);
            }

            Assert.AreEqual(0, counter);
            JEventBus.GetDefault().Post(new ResolveEvent(typeof(TestEvent)));
            Assert.AreEqual(100, counter);

            for (int i = 0; i < 100; i++)
            {
                JEventBus.GetDefault().Post(testEvent);
            }

            Assert.AreEqual(200, counter);
        }

        [Test]
        public void TestNormalCase()
        {
            var subscriber = new TestEventHandler();
            JEventBus.GetDefault().Register(subscriber);

            var testEvent = new TestEvent();
            for (int i = 0; i < 100; i++)
            {
                JEventBus.GetDefault().Post(testEvent);
            }

            Assert.AreEqual(100, subscriber.EventCounter);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void PerformanceTest(bool performanceMode)
        {
            int iteration = 100000;
            var subscriber = new TestEventHandler();
            JEventBus.GetDefault().Register(subscriber);
            JEventBus.GetDefault().PerformanceMode = performanceMode;

            var testEvent = new TestEvent();
            long time = NanoTime();
            for (int i = 0; i < iteration; i++)
            {
                JEventBus.GetDefault().Post(testEvent);
            }

            long executionTime = NanoTime() - time;
            Console.WriteLine(
                $"Execution time: {executionTime / 1000000} ms | {executionTime} ns | {executionTime / iteration} ns/event & performance mode: {performanceMode}");
            Assert.AreEqual(iteration, subscriber.EventCounter);
        }
        
        [TestCase(true)]
        [TestCase(false)]
        public void PerformanceTest2(bool performanceMode)
        {
            int iteration = 100000;
            int counter = 0;
            var subscriber = new RawSubscriber<TestEvent>(myEvent => counter++);
            JEventBus.GetDefault().Register(this, subscriber);
            JEventBus.GetDefault().PerformanceMode = performanceMode;

            var testEvent = new TestEvent();
            long time = NanoTime();
            for (int i = 0; i < iteration; i++)
            {
                JEventBus.GetDefault().Post(testEvent);
            }

            long executionTime = NanoTime() - time;
            Console.WriteLine(
                $"Execution time: {executionTime / 1000000} ms | {executionTime} ns | {executionTime / iteration} ns/event & performance mode: {performanceMode}");
            Assert.AreEqual(iteration, counter);
        }
        
        [TestCase(true)]
        [TestCase(false)]
        public void PerformanceTest3(bool performanceMode)
        {
            int iteration = 1000000;
            int counter = 0;
            var subscriber = new PerformanceSubscriber<TestEvent>(incoming => counter++);
            JEventBus.GetDefault().RegisterFast(subscriber);
            JEventBus.GetDefault().PerformanceMode = performanceMode;
            
            var testEvent = new TestEvent();
            long time = NanoTime();
            for (int i = 0; i < iteration; i++)
            {
                JEventBus.GetDefault().Post(testEvent);
            }

            long executionTime = NanoTime() - time;
            Console.WriteLine(
                $"Execution time: {executionTime / 1000000} ms | {executionTime} ns | {executionTime / iteration} ns/event & performance mode: {performanceMode}");
            Assert.AreEqual(iteration, counter);
        }

        private static long NanoTime()
        {
            long nano = 10000L * Stopwatch.GetTimestamp();
            nano /= TimeSpan.TicksPerMillisecond;
            nano *= 100L;
            return nano;
        }
    }
}