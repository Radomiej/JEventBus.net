using System;
using System.Collections.Generic;
using System.Text;

namespace Rad.EventBus.Tests.Events.Subscribers
{
    class SubscriptionCounter
    {
        public int fakeOneCount = 0;

        public void counterFakeOneEvents(FakeOneEvent e)
        {
            fakeOneCount++;
        }
    }
}
