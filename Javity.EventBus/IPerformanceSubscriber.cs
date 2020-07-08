﻿namespace Javity.EventBus
{
    public interface IPerformanceSubscriber
    {
        void SubscribeRaw(object incomingEvent);
    }
}